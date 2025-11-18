<template>
  <Modal title="Sign Up Form Preview" @close="close">
    <div class="preview-container">
      <table class="preview-table">
        <tbody>
          <tr>
            <th>ID</th>
            <td class="id-value">{{ props.record.id }}</td>
          </tr>
          <tr>
            <th>First Name</th>
            <td>{{ props.record.firstName }}</td>
          </tr>
          <tr>
            <th>Last Name</th>
            <td>{{ props.record.lastName }}</td>
          </tr>
          <tr>
            <th>Birth Date</th>
            <td>{{ formatDate(props.record.birthDate) }}</td>
          </tr>
          <tr>
            <th>Gender</th>
            <td>{{ props.record.gender }}</td>
          </tr>
          <tr>
            <th>Tags</th>
            <td>
              <div v-if="props.record.tags.length > 0" class="tags-preview">
                <span
                  v-for="(tag, index) in props.record.tags"
                  :key="index"
                  class="tag-badge-preview"
                >
                  {{ tag }}
                </span>
              </div>
              <span v-else class="no-tags">No tags selected</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <template #footer>
      <button class="close-button-footer" @click="close">Close</button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import type { SignUpRecord } from '../signup.types';
import Modal from '../../shared/Modal.vue';

const props = defineProps<{
  record: SignUpRecord;
}>();

const emit = defineEmits<{
  close: [];
}>();

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  } catch {
    return dateString;
  }
};

const close = () => {
  emit('close');
};
</script>

<style scoped>
.preview-container {
  padding: 1rem 0;
}

.preview-table {
  width: 100%;
  border-collapse: collapse;
}

.preview-table th {
  text-align: left;
  padding: 0.75rem 1rem;
  font-weight: 600;
  color: #999;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  border-bottom: 1px solid #333;
  width: 30%;
}

.preview-table td {
  padding: 0.75rem 1rem;
  color: rgba(255, 255, 255, 0.87);
  border-bottom: 1px solid #333;
}

.preview-table tbody tr:last-child th,
.preview-table tbody tr:last-child td {
  border-bottom: none;
}

.id-value {
  font-family: monospace;
  font-size: 0.9rem;
  color: #999;
}

.tags-preview {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag-badge-preview {
  display: inline-block;
  padding: 0.5rem 0.75rem;
  background-color: #42b983;
  color: white;
  border-radius: 4px;
  font-size: 0.9rem;
}

.no-tags {
  color: #999;
  font-style: italic;
}

.close-button-footer {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  background-color: #444;
  color: rgba(255, 255, 255, 0.87);
  transition: background-color 0.3s;
}

.close-button-footer:hover {
  background-color: #555;
}

@media (prefers-color-scheme: light) {
  .preview-table th {
    color: #666;
    border-bottom: 1px solid #eee;
  }

  .preview-table td {
    color: #213547;
    border-bottom: 1px solid #eee;
  }

  .id-value {
    color: #666;
  }

  .no-tags {
    color: #666;
  }

  .close-button-footer {
    background-color: #f5f5f5;
    color: #333;
  }

  .close-button-footer:hover {
    background-color: #e5e5e5;
  }
}
</style>
