<template>
  <div class="app">
    <main>
      <SubmissionTable />
    </main>
  </div>
</template>

<script setup lang="ts">
import SubmissionTable from './components/SubmissionTable.vue';
</script>

<style scoped>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

header {
  background-color: #42b983;
  color: white;
  padding: 1.5rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

header h1 {
  margin: 0;
  font-size: 1.8rem;
}

main {
  flex: 1;
  padding: 0;
}
</style>

