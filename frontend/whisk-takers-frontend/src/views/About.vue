<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-3xl mx-auto px-4 py-12">
      <h1 class="text-4xl font-extrabold text-yellow-300">About Whisk Takers</h1>
      <p class="mt-6 leading-7 text-emerald-100">
        Born from the dream of two visionary chefs, Whisk Takers blends tradition with innovation.
        Our founders believed cuisine could be both bold and elegant—like emerald and gold.
      </p>
      <p class="mt-4 leading-7 text-emerald-100">
        From humble beginnings to a renowned dining experience, our mission remains the same:
        craft unforgettable dishes and serve them with royal hospitality.
      </p>
      <RouterLink to="/client" class="inline-block mt-8 text-yellow-300 underline hover:text-yellow-200">Back to Menu</RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<style scoped>
</style>


