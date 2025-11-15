<template>
  <Modal title="File Preview" @close="close">
    <div class="preview-section">
      <div v-if="isImage" class="image-preview">
        <img v-if="!imageError" :src="file.preSign" :alt="file.name" @error="handleImageError" />
        <div v-else class="image-error">
          <svg xmlns="http://www.w3.org/2000/svg" width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="#ef4444" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
          <p class="error-message-text">Failed to load image</p>
        </div>
      </div>
      <div v-else class="file-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
          <polyline points="14 2 14 8 20 8"></polyline>
          <line x1="16" y1="13" x2="8" y2="13"></line>
          <line x1="16" y1="17" x2="8" y2="17"></line>
          <polyline points="10 9 9 9 8 9"></polyline>
        </svg>
        <p class="file-type">{{ file.type || 'Unknown' }}</p>
      </div>
    </div>

    <div class="properties-section">
      <h3>Properties</h3>
      <div class="properties-grid">
        <div class="property-item">
          <label>Name</label>
          <div class="property-value">{{ file.name }}</div>
        </div>

        <div class="property-item">
          <label>Size</label>
          <div class="property-value">{{ formatFileSize(file.size) }}</div>
        </div>

        <div class="property-item">
          <label>Type</label>
          <div class="property-value">{{ file.type }}</div>
        </div>

        <div class="property-item">
          <label>ID</label>
          <div class="property-value id-value">{{ file.id }}</div>
        </div>

        <div v-if="file.description" class="property-item full-width">
          <label>Description</label>
          <div class="property-value">{{ file.description }}</div>
        </div>

        <div v-if="file.status" class="property-item">
          <label>Status</label>
          <div class="property-value status-badge" :class="getStatusClass(file.status)">
            {{ getStatusLabel(file.status) }}
          </div>
        </div>

        <div v-if="file.priority" class="property-item">
          <label>Priority</label>
          <div class="property-value priority-badge" :class="getPriorityClass(file.priority)">
            {{ getPriorityLabel(file.priority) }}
          </div>
        </div>

        <div v-if="file.createdDate" class="property-item">
          <label>Created Date</label>
          <div class="property-value">{{ formatDate(file.createdDate) }}</div>
        </div>

        <div v-if="file.isPublic !== undefined" class="property-item">
          <label>Is Public</label>
          <div class="property-value">
            <span :class="file.isPublic ? 'public-badge' : 'private-badge'">
              {{ file.isPublic ? 'Yes' : 'No' }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <button class="close-button-footer" @click="close">Close</button>
      <button v-if="file.preSign" class="open-button" @click="openInNewTab">
        Open in New Tab
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { FileRecord } from '../submissions.types';
import { getStatusLabel, getPriorityLabel } from '../utils/enumFormatters';
import Modal from '../../shared/Modal.vue';

const props = defineProps<{
  file: FileRecord;
}>();

const emit = defineEmits<{
  close: [];
}>();

const imageError = ref(false);

const isImage = computed(() => {
  if (!props.file.type) return false;
  return props.file.type.startsWith('image/');
});

const handleImageError = () => {
  imageError.value = true;
};

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
};

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  } catch {
    return dateString;
  }
};

const getStatusClass = (status: string): string => {
  const statusMap: Record<string, string> = {
    'Pending': 'status-pending',
    'InReview': 'status-inreview',
    'Approved': 'status-approved',
    'Rejected': 'status-rejected'
  };
  return statusMap[status] || '';
};

const getPriorityClass = (priority: string): string => {
  const priorityMap: Record<string, string> = {
    'Low': 'priority-low',
    'Medium': 'priority-medium',
    'High': 'priority-high',
    'Critical': 'priority-critical'
  };
  return priorityMap[priority] || '';
};

const close = () => {
  emit('close');
};

const openInNewTab = () => {
  if (props.file.preSign) {
    window.open(props.file.preSign, '_blank');
  }
};
</script>

<style scoped>
.preview-section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 300px;
  background-color: #2a2a2a;
  border-radius: 8px;
  margin-bottom: 2rem;
  padding: 2rem;
}

.image-preview {
  max-width: 100%;
  max-height: 500px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.image-preview img {
  max-width: 100%;
  max-height: 500px;
  object-fit: contain;
  border-radius: 4px;
}

.image-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.error-message-text {
  color: #ef4444;
  font-size: 1rem;
  margin: 0;
  font-weight: 500;
}

.file-icon {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  color: #666;
}

.file-type {
  margin: 0;
  font-size: 0.9rem;
  color: #999;
}

.properties-section h3 {
  margin: 0 0 1rem 0;
  color: rgba(255, 255, 255, 0.87);
  font-size: 1.2rem;
}

.properties-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
}

.property-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.property-item.full-width {
  grid-column: 1 / -1;
}

.property-item label {
  font-size: 0.85rem;
  font-weight: 500;
  color: #999;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.property-value {
  color: rgba(255, 255, 255, 0.87);
  font-size: 1rem;
}

.id-value {
  font-family: monospace;
  font-size: 0.9rem;
  color: #999;
}

.status-badge,
.priority-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.85rem;
  font-weight: 500;
  text-transform: capitalize;
}

.status-pending {
  background-color: #fbbf24;
  color: #78350f;
}

.status-inreview {
  background-color: #3b82f6;
  color: #1e3a8a;
}

.status-approved {
  background-color: #10b981;
  color: #064e3b;
}

.status-rejected {
  background-color: #ef4444;
  color: #7f1d1d;
}

.priority-low {
  background-color: #6b7280;
  color: white;
}

.priority-medium {
  background-color: #3b82f6;
  color: white;
}

.priority-high {
  background-color: #f59e0b;
  color: white;
}

.priority-critical {
  background-color: #ef4444;
  color: white;
}

.public-badge {
  color: #10b981;
  font-weight: 500;
}

.private-badge {
  color: #999;
}

.close-button-footer,
.open-button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.3s;
}

.close-button-footer {
  background-color: #444;
  color: rgba(255, 255, 255, 0.87);
}

.close-button-footer:hover {
  background-color: #555;
}

.open-button {
  background-color: #42b983;
  color: white;
}

.open-button:hover {
  background-color: #35a372;
}

@media (prefers-color-scheme: light) {
  .preview-section {
    background-color: #f9f9f9;
  }

  .properties-section h3 {
    color: #213547;
  }

  .property-item label {
    color: #666;
  }

  .property-value {
    color: #213547;
  }

  .id-value {
    color: #666;
  }

  .file-icon {
    color: #999;
  }

  .file-type {
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

@media (max-width: 768px) {
  .properties-grid {
    grid-template-columns: 1fr;
  }
}
</style>

