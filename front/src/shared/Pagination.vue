<template>
  <div class="pagination">
    <div class="pagination-info">
      Showing {{ itemsCount }} of {{ totalCount }} submissions
    </div>
    <div class="pagination-controls">
      <button 
        class="pagination-button" 
        @click="$emit('previous')"
        :disabled="currentPage === 1"
        title="Previous page"
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="15 18 9 12 15 6"></polyline>
        </svg>
      </button>
      
      <button
        v-for="page in visiblePages"
        :key="page"
        class="pagination-button"
        :class="{ active: page === currentPage }"
        @click="$emit('page-change', page)"
      >
        {{ page }}
      </button>
      
      <button 
        class="pagination-button" 
        @click="$emit('next')"
        :disabled="currentPage === totalPages"
        title="Next page"
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="9 18 15 12 9 6"></polyline>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  currentPage: number;
  totalPages: number;
  itemsCount: number;
  totalCount: number;
}>();

defineEmits<{
  'page-change': [page: number];
  'previous': [];
  'next': [];
}>();

const visiblePages = computed(() => {
  const pages: number[] = [];
  const maxVisible = 5;
  let start = Math.max(1, props.currentPage - Math.floor(maxVisible / 2));
  let end = Math.min(props.totalPages, start + maxVisible - 1);
  
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1);
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  
  return pages;
});
</script>

<style scoped>
.pagination {
  margin-top: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
}

.pagination-info {
  color: #999;
  font-size: 0.9rem;
}

.pagination-controls {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.pagination-button {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 2.5rem;
  height: 2.5rem;
  padding: 0.5rem;
  border: 1px solid #444;
  border-radius: 4px;
  background-color: #2a2a2a;
  color: rgba(255, 255, 255, 0.87);
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.3s, border-color 0.3s;
}

.pagination-button:hover:not(:disabled) {
  background-color: #353535;
  border-color: #555;
}

.pagination-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-button.active {
  background-color: #42b983;
  border-color: #42b983;
  color: white;
}

.pagination-button.active:hover {
  background-color: #35a372;
  border-color: #35a372;
}

@media (prefers-color-scheme: light) {
  .pagination-info {
    color: #666;
  }
}
</style>

