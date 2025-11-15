<template>
  <div class="app">
    <header>
      <img alt="Vue logo" class="logo" src="/vite.svg" width="125" height="125" />
      <h1>Vue 3 + TypeScript + Vite</h1>
    </header>
    <main>
      <p>Welcome to your Vue 3 application with TypeScript and Vite!</p>
      <p>Edit <code>src/App.vue</code> to get started.</p>
    </main>
  </div>
</template>

<script setup lang="ts">
// This is a Vue 3 Composition API component with TypeScript
</script>

<style scoped>
.app {
  text-align: center;
  padding: 2rem;
}

header {
  margin-bottom: 2rem;
}

.logo {
  display: block;
  margin: 0 auto 1rem;
}

h1 {
  font-size: 2.5rem;
  color: #42b983;
  margin-bottom: 1rem;
}

main {
  max-width: 800px;
  margin: 0 auto;
}

p {
  font-size: 1.2rem;
  line-height: 1.6;
  margin-bottom: 1rem;
}

code {
  background-color: #f4f4f4;
  padding: 0.2rem 0.4rem;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  color: #e83e8c;
}
</style>

