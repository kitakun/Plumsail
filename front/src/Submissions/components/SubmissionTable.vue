<template>
  <div class="submission-table">
    <div class="table-header">
      <div class="filter-section">
        <input
          v-model="searchTerm"
          type="text"
          placeholder="Search by name or description..."
          class="search-input"
          @input="handleSearch"
        />
      </div>
      <button class="submit-button" @click="openModal">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="plus-icon">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Add file
      </button>
    </div>

    <div v-if="loading" class="loading">Loading...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    <div v-else>
      <table class="data-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Size</th>
            <th>Type</th>
            <th>ID</th>
            <th>Preview</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in submissions" :key="item.id">
            <td>{{ item.name }}</td>
            <td>{{ formatFileSize(item.size) }}</td>
            <td>{{ item.type }}</td>
            <td class="id-cell">{{ item.id }}</td>
            <td>
              <button 
                class="preview-button" 
                @click="handlePreview(item)"
              >
                Preview
              </button>
            </td>
          </tr>
          <tr v-if="submissions.length === 0">
            <td colspan="5" class="no-data">No submissions found</td>
          </tr>
        </tbody>
      </table>
      <Pagination
        :current-page="currentPage"
        :total-pages="totalPages"
        :items-count="submissions.length"
        :total-count="totalCount"
        @page-change="goToPage"
        @previous="goToPreviousPage"
        @next="goToNextPage"
      />
    </div>

    <SubmissionModal
      v-if="showModal"
      @close="closeModal"
      @submit="handleSubmit"
    />

    <SubmissionPreviewModal
      v-if="previewFile"
      :file="previewFile"
      @close="closePreview"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { submissionApi } from '../services/submission.api';
import type { FileRecord } from '../submissions.types';
import SubmissionModal from './SubmissionModal.vue';
import SubmissionPreviewModal from './SubmissionPreviewModal.vue';
import Pagination from '../../shared/Pagination.vue';

const submissions = ref<FileRecord[]>([]);
const totalCount = ref(0);
const loading = ref(false);
const error = ref<string | null>(null);
const searchTerm = ref('');
const showModal = ref(false);
const previewFile = ref<FileRecord | null>(null);
const currentPage = ref(1);
const pageSize = 5;
let searchTimeout: ReturnType<typeof setTimeout> | null = null;

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize));

const loadSubmissions = async () => {
  loading.value = true;
  error.value = null;
  try {
    const offset = (currentPage.value - 1) * pageSize;
    const response = await submissionApi.getSubmissions(offset, pageSize);
    if (response.isSuccess && response.result) {
      submissions.value = response.result.items;
      totalCount.value = response.result.totalCount;
    } else {
      error.value = response.exception || 'Failed to load submissions';
    }
  } catch (err) {
    error.value = 'An error occurred while loading submissions';
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
        const response = await submissionApi.searchSubmissions(searchTerm.value.trim(), offset, pageSize);
        if (response.isSuccess && response.result) {
          submissions.value = response.result.items;
          totalCount.value = response.result.totalCount;
        } else {
          error.value = response.exception || 'Failed to search submissions';
        }
      } catch (err) {
        error.value = 'An error occurred while searching';
        console.error(err);
      } finally {
        loading.value = false;
      }
    } else {
      await loadSubmissions();
    }
  }, 300);
};

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
};

const openModal = () => {
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
};

const handleSubmit = async () => {
  currentPage.value = 1;
  await loadSubmissions();
  closeModal();
};

const goToPage = async (page: number) => {
  if (page < 1 || page > totalPages.value) return;
  currentPage.value = page;
  if (searchTerm.value.trim()) {
    loading.value = true;
    error.value = null;
    try {
      const offset = (currentPage.value - 1) * pageSize;
      const response = await submissionApi.searchSubmissions(searchTerm.value.trim(), offset, pageSize);
      if (response.isSuccess && response.result) {
        submissions.value = response.result.items;
        totalCount.value = response.result.totalCount;
      } else {
        error.value = response.exception || 'Failed to search submissions';
      }
    } catch (err) {
      error.value = 'An error occurred while searching';
      console.error(err);
    } finally {
      loading.value = false;
    }
  } else {
    await loadSubmissions();
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

const handlePreview = (file: FileRecord) => {
  previewFile.value = file;
};

const closePreview = () => {
  previewFile.value = null;
};

onMounted(() => {
  loadSubmissions();
});
</script>

<style scoped>
.submission-table {
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

@media (prefers-color-scheme: light) {
  .search-input {
    border: 2px solid #ddd;
    background-color: white;
    color: #213547;
  }

  .search-input::placeholder {
    color: #999;
  }
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

.submit-button:active {
  transform: scale(0.98);
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

@media (prefers-color-scheme: light) {
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
}

.id-cell {
  font-family: monospace;
  font-size: 0.9rem;
  color: #999;
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
  .id-cell {
    color: #666;
  }

  .loading {
    color: #213547;
  }
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

.preview-button:active {
  transform: scale(0.98);
}

.no-preview {
  color: #666;
  font-style: italic;
}
</style>

