<template>
  <div class="app-container">
    <HeaderNavigation :current-path="currentPath" @navigate="handleNavigate" />
    <SubmissionsPage v-if="currentPath === '/submissions'" />
    <SignUpFormPage v-else-if="currentPath === '/signup'" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import HeaderNavigation from './shared/HeaderNavigation.vue';
import SubmissionsPage from './Submissions/Submissions.Page.vue';
import SignUpFormPage from './SignUpForm/SignUpForm.Page.vue';

const currentPath = ref('/submissions');

const handleNavigate = (path: string) => {
  currentPath.value = path;
};
</script>

<style>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
</style>

