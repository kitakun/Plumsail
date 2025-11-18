<template>
  <div class="modal-overlay" @click.self="handleOverlayClick">
    <div class="modal-content" :style="{ maxWidth: maxWidth }">
      <div class="modal-header">
        <h2>{{ title }}</h2>
        <button class="close-button" @click="handleClose" :disabled="closeButtonDisabled">
          &times;
        </button>
      </div>

      <div class="modal-body">
        <slot></slot>
      </div>

      <div v-if="$slots.footer" class="modal-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue';

const props = withDefaults(
  defineProps<{
    title: string;
    closeButtonDisabled?: boolean;
    maxWidth?: string;
    preventOverlayClose?: boolean;
  }>(),
  {
    closeButtonDisabled: false,
    maxWidth: '900px',
    preventOverlayClose: false,
  }
);

const emit = defineEmits<{
  close: [];
}>();

const handleClose = () => {
  if (!props.closeButtonDisabled) {
    emit('close');
  }
};

const handleOverlayClick = () => {
  if (!props.preventOverlayClose && !props.closeButtonDisabled) {
    emit('close');
  }
};

onMounted(() => {
  document.body.style.overflow = 'hidden';
});

onUnmounted(() => {
  document.body.style.overflow = '';
});
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: #1e1e1e;
  border-radius: 8px;
  width: 90%;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #333;
}

.modal-header h2 {
  margin: 0;
  color: rgba(255, 255, 255, 0.87);
}

.close-button {
  background: none;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  color: #999;
  line-height: 1;
  padding: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-button:hover:not(:disabled) {
  color: rgba(255, 255, 255, 0.87);
}

.close-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding: 1.5rem;
  border-top: 1px solid #333;
}

@media (prefers-color-scheme: light) {
  .modal-content {
    background: white;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }

  .modal-header {
    border-bottom: 1px solid #eee;
  }

  .modal-header h2 {
    color: #213547;
  }

  .close-button {
    color: #999;
  }

  .close-button:hover:not(:disabled) {
    color: #333;
  }

  .modal-footer {
    border-top: 1px solid #eee;
  }
}
</style>
