<template>
  <Modal
    title="Sign Up Form"
    :close-button-disabled="submitting"
    :prevent-overlay-close="submitting"
    max-width="600px"
    @close="close"
  >
    <div class="form-container">
      <div class="form-row">
        <div class="form-group">
          <label>First Name *</label>
          <input
            v-model="formData.firstName"
            type="text"
            placeholder="Enter first name..."
            class="form-input"
            :disabled="submitting"
            required
          />
        </div>

        <div class="form-group">
          <label>Last Name *</label>
          <input
            v-model="formData.lastName"
            type="text"
            placeholder="Enter last name..."
            class="form-input"
            :disabled="submitting"
            required
          />
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label>Birth Date *</label>
          <input
            v-model="formData.birthDate"
            type="date"
            class="form-input"
            :disabled="submitting"
            required
          />
        </div>

        <div class="form-group">
          <label>Gender *</label>
          <select v-model="formData.gender" class="form-select" :disabled="submitting" required>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
            <option value="Other">Other</option>
            <option value="Prefer not to say">Prefer not to say</option>
          </select>
        </div>
      </div>

      <div class="form-group">
        <label>Tags</label>
        <div ref="typeaheadContainer" class="typeahead-container">
          <input
            ref="tagInput"
            v-model="tagSearch"
            type="text"
            placeholder="Search and select tags..."
            class="form-input"
            :disabled="submitting"
            @input="handleTagSearch"
            @focus="handleInputFocus"
            @blur="handleInputBlur"
          />
          <Teleport to="body">
            <div
              v-if="showTagSuggestions && filteredTags.length > 0"
              ref="tagSuggestions"
              class="tag-suggestions"
              :style="suggestionsStyle"
            >
              <div
                v-for="tag in filteredTags"
                :key="tag"
                class="tag-suggestion"
                @mousedown.prevent="selectTag(tag)"
              >
                {{ tag }}
              </div>
            </div>
          </Teleport>
        </div>
        <div v-if="formData.tags.length > 0" class="selected-tags">
          <span v-for="(tag, index) in formData.tags" :key="index" class="tag-badge">
            {{ tag }}
            <button type="button" class="tag-remove" @click="removeTag(tag)" :disabled="submitting">
              ×
            </button>
          </span>
        </div>
      </div>
    </div>

    <div v-if="submitError" class="error-message">{{ submitError }}</div>
    <div v-if="submitSuccess" class="success-message">Form submitted successfully!</div>

    <template #footer>
      <button class="cancel-button" @click="close" :disabled="submitting">Cancel</button>
      <button class="submit-button" @click="submit" :disabled="!isFormValid || submitting">
        {{ submitting ? 'Submitting...' : 'Submit' }}
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue';
import type { SignUpFormData, AvailableTag } from '../signup.types';
import { AVAILABLE_TAGS } from '../signup.types';
import Modal from '../../shared/Modal.vue';

const emit = defineEmits<{
  close: [];
  submit: [data: SignUpFormData];
}>();

const formData = ref<SignUpFormData>({
  firstName: '',
  lastName: '',
  birthDate: '',
  gender: 'Prefer not to say',
  tags: [],
});

const tagSearch = ref('');
const showTagSuggestions = ref(false);
const submitting = ref(false);
const submitError = ref<string | null>(null);
const submitSuccess = ref(false);
const typeaheadContainer = ref<HTMLElement | null>(null);
const tagInput = ref<HTMLInputElement | null>(null);
const tagSuggestions = ref<HTMLElement | null>(null);
const suggestionsStyle = ref<{ top: string; left: string; width: string }>({
  top: '0px',
  left: '0px',
  width: '0px',
});

const filteredTags = computed(() => {
  if (!tagSearch.value.trim()) {
    return AVAILABLE_TAGS.filter(tag => !formData.value.tags.includes(tag));
  }
  const searchLower = tagSearch.value.toLowerCase();
  return AVAILABLE_TAGS.filter(
    tag => !formData.value.tags.includes(tag) && tag.toLowerCase().includes(searchLower)
  );
});

const isFormValid = computed(() => {
  return (
    formData.value.firstName.trim() !== '' &&
    formData.value.lastName.trim() !== '' &&
    formData.value.birthDate !== ''
  );
});

const updateSuggestionsPosition = async () => {
  if (!tagInput.value) return;

  await nextTick();
  const rect = tagInput.value.getBoundingClientRect();
  suggestionsStyle.value = {
    top: `${rect.bottom + window.scrollY}px`,
    left: `${rect.left + window.scrollX}px`,
    width: `${rect.width}px`,
  };
};

const handleTagSearch = async () => {
  showTagSuggestions.value = true;
  await updateSuggestionsPosition();
};

const handleInputFocus = async () => {
  showTagSuggestions.value = true;
  await updateSuggestionsPosition();
};

const handleInputBlur = () => {
  // Delay hiding to allow click on suggestion
  setTimeout(() => {
    const activeElement = document.activeElement;
    if (
      !tagSuggestions.value?.contains(activeElement) &&
      !tagInput.value?.contains(activeElement)
    ) {
      showTagSuggestions.value = false;
    }
  }, 200);
};

const selectTag = (tag: AvailableTag) => {
  if (!formData.value.tags.includes(tag)) {
    formData.value.tags.push(tag);
  }
  tagSearch.value = '';
  showTagSuggestions.value = false;
  tagInput.value?.blur();
};

const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement;
  if (
    typeaheadContainer.value &&
    !typeaheadContainer.value.contains(target) &&
    tagSuggestions.value &&
    !tagSuggestions.value.contains(target)
  ) {
    showTagSuggestions.value = false;
  }
};

const handleScroll = () => {
  if (showTagSuggestions.value) {
    updateSuggestionsPosition();
  }
};

const handleResize = () => {
  if (showTagSuggestions.value) {
    updateSuggestionsPosition();
  }
};

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
  window.addEventListener('scroll', handleScroll, true);
  window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
  window.removeEventListener('scroll', handleScroll, true);
  window.removeEventListener('resize', handleResize);
});

const removeTag = (tag: AvailableTag) => {
  const index = formData.value.tags.indexOf(tag);
  if (index > -1) {
    formData.value.tags.splice(index, 1);
  }
};

const close = () => {
  if (submitting.value) return;
  formData.value = {
    firstName: '',
    lastName: '',
    birthDate: '',
    gender: 'Prefer not to say',
    tags: [],
  };
  tagSearch.value = '';
  submitError.value = null;
  submitSuccess.value = false;
  showTagSuggestions.value = false;
  emit('close');
};

const submit = async () => {
  if (!isFormValid.value) return;

  submitting.value = true;
  submitError.value = null;
  submitSuccess.value = false;

  try {
    emit('submit', { ...formData.value });
  } catch (error) {
    submitError.value = 'An error occurred while submitting the form';
    console.error(error);
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.form-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
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
.form-select:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.typeahead-container {
  position: relative;
}

.tag-suggestions {
  position: fixed;
  max-height: 200px;
  overflow-y: auto;
  background-color: #2a2a2a;
  border: 1px solid #444;
  border-radius: 4px;
  margin-top: 0.25rem;
  z-index: 10000;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
}

.tag-suggestion {
  padding: 0.75rem;
  cursor: pointer;
  color: rgba(255, 255, 255, 0.87);
  transition: background-color 0.2s;
}

.tag-suggestion:hover {
  background-color: #353535;
}

.selected-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.tag-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  background-color: #42b983;
  color: white;
  border-radius: 4px;
  font-size: 0.9rem;
}

.tag-remove {
  background: none;
  border: none;
  color: white;
  font-size: 1.2rem;
  line-height: 1;
  cursor: pointer;
  padding: 0;
  width: 1.2rem;
  height: 1.2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.tag-remove:hover:not(:disabled) {
  background-color: rgba(255, 255, 255, 0.2);
}

.tag-remove:disabled {
  opacity: 0.6;
  cursor: not-allowed;
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
  label {
    color: #555;
  }

  .form-input,
  .form-select {
    border: 1px solid #ddd;
    background-color: white;
    color: #213547;
  }

  .tag-suggestions {
    background-color: white;
    border: 1px solid #ddd;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }

  .tag-suggestion {
    color: #213547;
  }

  .tag-suggestion:hover {
    background-color: #f5f5f5;
  }

  .cancel-button {
    background-color: #f5f5f5;
    color: #333;
  }

  .cancel-button:hover {
    background-color: #e5e5e5;
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

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}
</style>
