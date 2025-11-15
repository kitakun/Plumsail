<template>
  <div class="signup-form-table">
    <div class="table-header">
      <div class="filter-section">
        <input
          v-model="searchTerm"
          type="text"
          placeholder="Search by name..."
          class="search-input"
          @input="handleSearch"
        />
      </div>
      <button class="submit-button" @click="openModal">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="plus-icon">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Add Entry
      </button>
    </div>

    <div v-if="loading" class="loading">Loading...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    <div v-else>
      <table class="data-table">
        <thead>
          <tr>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Birth Date</th>
            <th>Gender</th>
            <th>Tags</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in filteredRecords" :key="item.id">
            <td>{{ item.firstName }}</td>
            <td>{{ item.lastName }}</td>
            <td>{{ formatDate(item.birthDate) }}</td>
            <td>{{ item.gender }}</td>
            <td>
              <div class="tags-cell">
                <span
                  v-for="(tag, index) in item.tags"
                  :key="index"
                  class="tag-badge-small"
                >
                  {{ tag }}
                </span>
                <span v-if="item.tags.length === 0" class="no-tags">No tags</span>
              </div>
            </td>
            <td>
              <button 
                class="preview-button" 
                @click="handlePreview(item)"
              >
                Preview
              </button>
            </td>
          </tr>
          <tr v-if="filteredRecords.length === 0">
            <td colspan="6" class="no-data">No entries found</td>
          </tr>
        </tbody>
      </table>
      <Pagination
        :current-page="currentPage"
        :total-pages="totalPages"
        :items-count="filteredRecords.length"
        :total-count="totalCount"
        @page-change="goToPage"
        @previous="goToPreviousPage"
        @next="goToNextPage"
      />
    </div>

    <SignUpFormModal
      v-if="showModal"
      @close="closeModal"
      @submit="handleSubmit"
    />

    <SignUpFormPreviewModal
      v-if="previewRecord"
      :record="previewRecord"
      @close="closePreview"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { signupApi } from '../services/signup.api';
import type { FileRecord } from '../../Submissions/submissions.types';
import type { SignUpRecord, SignUpFormData, AvailableTag } from '../signup.types';
import { AVAILABLE_TAGS } from '../signup.types';
import SignUpFormModal from './SignUpFormModal.vue';
import SignUpFormPreviewModal from './SignUpFormPreviewModal.vue';
import Pagination from '../../shared/Pagination.vue';

const records = ref<SignUpRecord[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);
const searchTerm = ref('');
const showModal = ref(false);
const previewRecord = ref<SignUpRecord | null>(null);
const currentPage = ref(1);
const totalCount = ref(0);
const pageSize = 5;
let searchTimeout: ReturnType<typeof setTimeout> | null = null;

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize));

const filteredRecords = computed(() => {
  return records.value;
});

const mapFileRecordToSignUpRecord = (fileRecord: FileRecord): SignUpRecord | null => {
  try {
    const payload = fileRecord.payload;
    
    if (typeof payload !== 'object' || payload === null) return null;

    const hasNonSignUpFields = 
      'Description' in payload || 
      'Status' in payload || 
      'Priority' in payload || 
      'IsPublic' in payload ||
      'File' in payload ||
      'FileName' in payload;
    
    if (hasNonSignUpFields) return null;

    if (!('FirstName' in payload) || !('LastName' in payload) || 
        !('BirthDate' in payload) || !('Gender' in payload)) {
      return null;
    }

    const firstName = payload.FirstName;
    if (typeof firstName !== 'string' || firstName.trim() === '') return null;

    const lastName = payload.LastName;
    if (typeof lastName !== 'string' || lastName.trim() === '') return null;

    const birthDate = payload.BirthDate;
    if (typeof birthDate !== 'string' || birthDate.trim() === '') return null;
    // Validate date format (YYYY-MM-DD)
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateRegex.test(birthDate)) return null;
    const parsedDate = new Date(birthDate);
    if (isNaN(parsedDate.getTime())) return null;

    const gender = payload.Gender;
    const validGenders = ['Male', 'Female', 'Other', 'Prefer not to say'] as const;
    if (typeof gender !== 'string' || !(validGenders as readonly string[]).includes(gender)) return null;

    let tags: AvailableTag[] = [];
    if ('Tags' in payload && payload.Tags !== null && payload.Tags !== undefined) {
      if (Array.isArray(payload.Tags)) {
        // Filter to only include valid AvailableTag values
        tags = payload.Tags
          .filter((tag): tag is string => typeof tag === 'string')
          .filter((tag): tag is AvailableTag => 
            AVAILABLE_TAGS.includes(tag as AvailableTag)
          );
      }
    }

    return {
      id: fileRecord.id,
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      birthDate: birthDate.trim(),
      gender: gender as 'Male' | 'Female' | 'Other' | 'Prefer not to say',
      tags,
    };
  } catch {
    return null;
  }
};

const loadRecords = async () => {
  loading.value = true;
  error.value = null;
  try {
    const offset = (currentPage.value - 1) * pageSize;
    const response = await signupApi.getSubmissions(offset, pageSize);
    if (response.isSuccess && response.result) {
      const mappedRecords = response.result.items
        .map(mapFileRecordToSignUpRecord)
        .filter((record): record is SignUpRecord => record !== null);
      records.value = mappedRecords;
      totalCount.value = response.result.totalCount;
    } else {
      error.value = response.exception || 'Failed to load records';
    }
  } catch (err) {
    error.value = 'An error occurred while loading records';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const handleSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout);
  }
  
  searchTimeout = setTimeout(async () => {
    currentPage.value = 1;
    if (searchTerm.value.trim()) {
      loading.value = true;
      error.value = null;
      try {
        const offset = (currentPage.value - 1) * pageSize;
        const response = await signupApi.searchSubmissions(searchTerm.value.trim(), offset, pageSize);
        if (response.isSuccess && response.result) {
          const mappedRecords = response.result.items
            .map(mapFileRecordToSignUpRecord)
            .filter((record): record is SignUpRecord => record !== null);
          records.value = mappedRecords;
          totalCount.value = response.result.totalCount;
        } else {
          error.value = response.exception || 'Failed to search records';
        }
      } catch (err) {
        error.value = 'An error occurred while searching';
        console.error(err);
      } finally {
        loading.value = false;
      }
    } else {
      await loadRecords();
    }
  }, 300);
};

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString();
  } catch {
    return dateString;
  }
};

const openModal = () => {
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
};

const handleSubmit = async (data: SignUpFormData) => {
  loading.value = true;
  error.value = null;
  try {
    const formData = new FormData();
    
    // Add form fields directly
    formData.append('[0].FirstName', data.firstName);
    formData.append('[0].LastName', data.lastName);
    formData.append('[0].BirthDate', data.birthDate);
    formData.append('[0].Gender', data.gender);
    
    // Add tags as array
    data.tags.forEach((tag, index) => {
      formData.append(`[0].Tags[${index}]`, tag);
    });

    const response = await signupApi.submitSignUpForm(formData);
    if (response.isSuccess) {
      closeModal();
      currentPage.value = 1;
      await loadRecords();
    } else {
      error.value = response.exception || 'Failed to submit form';
    }
  } catch (err) {
    error.value = 'An error occurred while submitting the form';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const handlePreview = (record: SignUpRecord) => {
  previewRecord.value = record;
};

const closePreview = () => {
  previewRecord.value = null;
};

const goToPage = async (page: number) => {
  if (page < 1 || page > totalPages.value) return;
  currentPage.value = page;
  if (searchTerm.value.trim()) {
    await handleSearch();
  } else {
    await loadRecords();
  }
};

const goToPreviousPage = () => {
  if (currentPage.value > 1) {
    goToPage(currentPage.value - 1);
  }
};

const goToNextPage = () => {
  if (currentPage.value < totalPages.value) {
    goToPage(currentPage.value + 1);
  }
};

onMounted(() => {
  loadRecords();
});
</script>

<style scoped>
.signup-form-table {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.table-header {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  margin-bottom: 1.5rem;
  gap: 1rem;
}

.filter-section {
  flex: 1;
  max-width: none;
}

.search-input {
  box-sizing: border-box;
  width: 100%;
  max-width: none;
  padding: 0.75rem;
  font-size: 1rem;
  border: 2px solid #444;
  border-radius: 4px;
  background-color: #2a2a2a;
  color: rgba(255, 255, 255, 0.87);
  transition: border-color 0.3s;
}

.search-input:focus {
  outline: none;
  border-color: #42b983;
}

.search-input::placeholder {
  color: #888;
}

.submit-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.3s;
}

.plus-icon {
  flex-shrink: 0;
}

.submit-button:hover {
  background-color: #35a372;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background-color: #1e1e1e;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  border-radius: 8px;
  overflow: hidden;
}

.data-table thead {
  background-color: #42b983;
  color: white;
}

.data-table th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
}

.data-table td {
  padding: 1rem;
  border-bottom: 1px solid #333;
  color: rgba(255, 255, 255, 0.87);
}

.data-table tbody tr {
  background-color: #2a2a2a;
}

.data-table tbody tr:hover {
  background-color: #353535;
}

.data-table tbody tr:last-child td {
  border-bottom: none;
}

.tags-cell {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.tag-badge-small {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background-color: #42b983;
  color: white;
  border-radius: 4px;
  font-size: 0.85rem;
}

.no-tags {
  color: #999;
  font-style: italic;
  font-size: 0.9rem;
}

.preview-button {
  padding: 0.5rem 1rem;
  font-size: 0.9rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.3s;
}

.preview-button:hover {
  background-color: #35a372;
}

.no-data {
  text-align: center;
  color: #999;
  font-style: italic;
  padding: 2rem;
}

.loading,
.error {
  text-align: center;
  padding: 2rem;
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.87);
}

.error {
  color: #e74c3c;
}

@media (prefers-color-scheme: light) {
  .search-input {
    border: 2px solid #ddd;
    background-color: white;
    color: #213547;
  }

  .data-table {
    background-color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .data-table td {
    border-bottom: 1px solid #eee;
    color: #213547;
  }

  .data-table tbody tr {
    background-color: white;
  }

  .data-table tbody tr:hover {
    background-color: #f5f5f5;
  }

  .no-tags {
    color: #666;
  }

  .loading {
    color: #213547;
  }
}
</style>

