<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-3xl mx-auto px-4 py-12">
      <div class="mb-6 flex justify-end"><PreferencesControls /></div>
      <h1 class="text-4xl font-extrabold text-yellow-300">{{ tr('About Whisk Takers', 'Rreth Whisk Takers') }}</h1>
      <p class="mt-6 leading-7 text-emerald-100">{{ tr('Whisk Takers brings together carefully prepared coffee, fresh bakery favorites, and desserts made for sharing. We care about good ingredients, a warm welcome, and making every visit feel easy.', 'Whisk Takers bashkon kafenë e përgatitur me kujdes, produktet e freskëta të furrës dhe ëmbëlsirat për t’u ndarë me të tjerët. Na interesojnë përbërësit cilësorë, mikpritja dhe një përvojë e këndshme në çdo vizitë.') }}</p>
      <RouterLink to="/client" class="inline-block mt-8 text-yellow-300 underline hover:text-yellow-200">{{ tr('Back to menu', 'Kthehu te menuja') }}</RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import PreferencesControls from '../components/PreferencesControls.vue'
import { tr } from '../store/preferences'
</script>

<style scoped>
</style>


