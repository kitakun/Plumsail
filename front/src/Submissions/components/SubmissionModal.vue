<template>
  <Modal 
    title="Submit Files" 
    :close-button-disabled="submitting"
    :prevent-overlay-close="submitting"
    max-width="800px"
    @close="close"
  >
    <div v-for="(file, index) in files" :key="index" class="file-item">
      <div class="file-header">
        <span class="file-name">{{ file.file.name }}</span>
        <button class="remove-button" @click="removeFile(index)" :disabled="submitting">Remove</button>
      </div>

      <div class="file-properties">
        <div class="form-group">
          <label>Description</label>
          <input
            v-model="file.description"
            type="text"
            placeholder="Enter file description..."
            class="form-input"
            :disabled="submitting"
          />
        </div>

        <div class="form-group">
          <label>Payload</label>
          <input
            v-model="file.payload"
            type="text"
            placeholder="Enter value to add not existing data to the model..."
            class="form-input"
            :disabled="submitting"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Status</label>
            <select v-model="file.status" class="form-select" :disabled="submitting">
              <option :value="undefined">Select status...</option>
              <option value="Pending">Pending</option>
              <option value="InReview">In Review</option>
              <option value="Approved">Approved</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>

          <div class="form-group">
            <label>Priority</label>
            <select v-model="file.priority" class="form-select" :disabled="submitting">
              <option :value="undefined">Select priority...</option>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label class="checkbox-label">
            <input
              v-model="file.isPublic"
              type="checkbox"
              class="checkbox-input"
              :disabled="submitting"
            />
            <span>Is Public</span>
          </label>
        </div>
      </div>
    </div>

    <div class="file-upload-area" :class="{ 'disabled': submitting }" @click="triggerFileInput" @dragover.prevent @drop.prevent="handleDrop">
      <input
        ref="fileInput"
        type="file"
        multiple
        class="file-input"
        @change="handleFileSelect"
        :disabled="submitting"
      />
      <div class="upload-placeholder">
        <p>Click or drag files here to upload</p>
        <p class="upload-hint">You can upload multiple files</p>
      </div>
    </div>

    <div v-if="submitError" class="error-message">{{ submitError }}</div>
    <div v-if="submitSuccess" class="success-message">Files submitted successfully!</div>

    <template #footer>
      <button class="cancel-button" @click="close" :disabled="submitting">Cancel</button>
      <button
        class="submit-button"
        @click="submit"
        :disabled="files.length === 0 || submitting"
      >
        {{ submitting ? 'Submitting...' : 'Submit' }}
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { submissionApi } from '../services/submission.api';
import type { FileSubmission } from '../submissions.types';
import Modal from '../../shared/Modal.vue';

const emit = defineEmits<{
  close: [];
  submit: [];
}>();

const fileInput = ref<HTMLInputElement | null>(null);
const files = ref<FileSubmission[]>([]);
const submitting = ref(false);
const submitError = ref<string | null>(null);
const submitSuccess = ref(false);

const triggerFileInput = () => {
  if (submitting.value) return;
  fileInput.value?.click();
};

const handleFileSelect = (event: Event) => {
  if (submitting.value) return;
  const target = event.target as HTMLInputElement;
  if (target.files) {
    addFiles(Array.from(target.files));
  }
};

const handleDrop = (event: DragEvent) => {
  if (submitting.value) return;
  event.preventDefault();
  if (event.dataTransfer?.files) {
    addFiles(Array.from(event.dataTransfer.files));
  }
};

const addFiles = (newFiles: File[]) => {
  const fileSubmissions: FileSubmission[] = newFiles.map(file => ({
    file,
    description: '',
    payload: '',
    status: undefined,
    priority: undefined,
    isPublic: false,
  }));
  files.value.push(...fileSubmissions);
};

const removeFile = (index: number) => {
  files.value.splice(index, 1);
};

const close = () => {
  if (submitting.value) return;
  files.value = [];
  submitError.value = null;
  submitSuccess.value = false;
  emit('close');
};

const submit = async () => {
  if (files.value.length === 0) return;

  submitting.value = true;
  submitError.value = null;
  submitSuccess.value = false;

  try {
    const response = await submissionApi.submitFiles(files.value);
    if (response.isSuccess) {
      submitSuccess.value = true;
      setTimeout(() => {
        emit('submit');
      }, 1000);
    } else {
      submitError.value = response.exception || 'Failed to submit files';
    }
  } catch (error) {
    submitError.value = 'An error occurred while submitting files';
    console.error(error);
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.file-item {
  margin-bottom: 1.5rem;
  padding: 1rem;
  border: 1px solid #333;
  border-radius: 4px;
  background-color: #2a2a2a;
}

.file-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.file-name {
  font-weight: 600;
  color: rgba(255, 255, 255, 0.87);
}

.remove-button {
  background-color: #e74c3c;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
}

.remove-button:hover:not(:disabled) {
  background-color: #c0392b;
}

.remove-button:disabled {
  background-color: #666;
  cursor: not-allowed;
  opacity: 0.6;
}

.file-properties {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

label {
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #999;
  font-size: 0.9rem;
}

.form-input,
.form-select {
  padding: 0.75rem;
  border: 1px solid #444;
  border-radius: 4px;
  font-size: 1rem;
  background-color: #2a2a2a;
  color: rgba(255, 255, 255, 0.87);
  transition: border-color 0.3s;
}

.form-input:focus:not(:disabled),
.form-select:focus:not(:disabled) {
  outline: none;
  border-color: #42b983;
}

.form-input:disabled,
.form-select:disabled,
.checkbox-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.form-input::placeholder {
  color: #888;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.checkbox-input {
  width: 1.2rem;
  height: 1.2rem;
  cursor: pointer;
}

.file-upload-area {
  border: 2px dashed #444;
  border-radius: 4px;
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  transition: border-color 0.3s, background-color 0.3s;
  margin-top: 1rem;
  background-color: #2a2a2a;
}

.file-upload-area:hover:not(.disabled) {
  border-color: #42b983;
  background-color: #353535;
}

.file-upload-area.disabled {
  opacity: 0.6;
  cursor: not-allowed;
  pointer-events: none;
}

.file-input {
  display: none;
}

.upload-placeholder p {
  margin: 0.5rem 0;
  color: rgba(255, 255, 255, 0.87);
}

.upload-hint {
  font-size: 0.9rem;
  color: #999;
}

.error-message {
  background-color: #4a1f1f;
  color: #ff6b6b;
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
  border: 1px solid #6b2a2a;
}

.success-message {
  background-color: #1f4a1f;
  color: #6bff6b;
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
  border: 1px solid #2a6b2a;
}

.cancel-button,
.submit-button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
}

.cancel-button {
  background-color: #444;
  color: rgba(255, 255, 255, 0.87);
}

.cancel-button:hover:not(:disabled) {
  background-color: #555;
}

.cancel-button:disabled {
  background-color: #666;
  cursor: not-allowed;
  opacity: 0.6;
}

.submit-button {
  background-color: #42b983;
  color: white;
}

.submit-button:hover:not(:disabled) {
  background-color: #35a372;
}

.submit-button:disabled {
  background-color: #666;
  cursor: not-allowed;
}

@media (prefers-color-scheme: light) {
  .file-item {
    border: 1px solid #ddd;
    background-color: #f9f9f9;
  }

  .file-name {
    color: #333;
  }

  label {
    color: #555;
  }

  .form-input,
  .form-select {
    border: 1px solid #ddd;
    background-color: white;
    color: #213547;
  }

  .form-input::placeholder {
    color: #999;
  }

  .file-upload-area {
    border: 2px dashed #ddd;
    background-color: white;
  }

  .file-upload-area:hover:not(.disabled) {
    border-color: #42b983;
    background-color: #f0fdf4;
  }

  .file-upload-area.disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  .upload-placeholder p {
    color: #666;
  }

  .cancel-button {
    background-color: #f5f5f5;
    color: #333;
  }

  .cancel-button:hover {
    background-color: #e5e5e5;
  }

  .submit-button:disabled {
    background-color: #ccc;
  }

  .error-message {
    background-color: #fee;
    color: #c33;
    border: 1px solid #fcc;
  }

  .success-message {
    background-color: #efe;
    color: #3c3;
    border: 1px solid #cfc;
  }
}
</style>

