<template>
  <div class="flex items-center gap-2 rounded-xl border border-white/10 bg-slate-950/80 p-2 text-white shadow-lg">
    <label class="sr-only" for="app-language">{{ tr('Language', 'Gjuha') }}</label>
    <select id="app-language" v-model="preferences.language.value" class="rounded-lg border border-slate-700 bg-slate-800 px-2 py-2 text-sm text-white">
      <option value="en">EN</option>
      <option value="sq">SQ</option>
    </select>
    <label class="sr-only" for="app-theme">{{ tr('App theme', 'Pamja e aplikacionit') }}</label>
    <select id="app-theme" v-model="preferences.theme.value" class="max-w-32 rounded-lg border border-slate-700 bg-slate-800 px-2 py-2 text-sm text-white">
      <option value="emerald">{{ tr('Emerald', 'Jeshile') }}</option>
      <option value="midnight">{{ tr('Midnight', 'Mesnatë') }}</option>
      <option value="terracotta">{{ tr('Terracotta', 'Tullë') }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { preferences, tr } from '../store/preferences'
</script>
