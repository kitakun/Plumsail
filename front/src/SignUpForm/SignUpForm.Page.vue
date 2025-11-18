<template>
  <div class="signup-form-page">
    <main>
      <SignUpFormTable />
    </main>
  </div>
</template>

<script setup lang="ts">
import SignUpFormTable from './components/SignUpFormTable.vue';
</script>

<style scoped>
.signup-form-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
  padding: 0;
}
</style>
