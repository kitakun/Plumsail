<template>
  <header class="header-navigation">
    <nav class="nav-container">
      <h1 class="logo">Plumsail</h1>
      <div class="nav-links">
        <button
          v-for="link in links"
          :key="link.path"
          :class="['nav-link', { active: currentPath === link.path }]"
          @click="navigate(link.path)"
        >
          {{ link.label }}
        </button>
      </div>
    </nav>
  </header>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const props = defineProps<{
  currentPath?: string;
}>();

const emit = defineEmits<{
  navigate: [path: string];
}>();

const links = [
  { path: '/submissions', label: 'Submissions' },
  { path: '/signup', label: 'Sign Up Form' },
];

const currentPath = ref(props.currentPath || '/submissions');

const navigate = (path: string) => {
  currentPath.value = path;
  emit('navigate', path);
};

onMounted(() => {
  // Set initial path based on current route if needed
});
</script>

<style scoped>
.header-navigation {
  background-color: #42b983;
  color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  margin: 0;
  font-size: 1.8rem;
  font-weight: 600;
}

.nav-links {
  display: flex;
  gap: 1rem;
}

.nav-link {
  padding: 0.5rem 1.5rem;
  background: transparent;
  border: 2px solid transparent;
  border-radius: 4px;
  color: white;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
}

.nav-link.active {
  background-color: rgba(255, 255, 255, 0.2);
  border-color: white;
}

@media (prefers-color-scheme: light) {
  .header-navigation {
    background-color: #42b983;
  }
}

@media (max-width: 768px) {
  .nav-container {
    flex-direction: column;
    gap: 1rem;
  }

  .nav-links {
    width: 100%;
    justify-content: center;
  }
}
</style>

