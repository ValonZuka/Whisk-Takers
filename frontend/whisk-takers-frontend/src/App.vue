<template>
  <router-view />
  <div v-if="welcomeName" class="fixed left-1/2 top-5 z-[60] -translate-x-1/2 rounded-xl border border-emerald-400/40 bg-slate-950/95 px-5 py-3 text-center text-white shadow-xl" role="status">Mirë se u ktheve, <strong>{{ welcomeName }}</strong> 👋</div>
  <div v-if="!['Waiter', 'Chef', 'Admin', 'Client'].includes(String(route.name))" class="fixed bottom-4 right-4 z-50 flex items-center gap-2 rounded-xl border border-white/15 bg-slate-950/90 p-2 text-white shadow-xl backdrop-blur">
    <span class="text-xs text-slate-300">{{ tr('Language', 'Gjuha') }}</span>
    <button class="rounded-lg px-2 py-1 text-sm" :class="preferences.language.value === 'en' ? 'bg-emerald-500' : 'bg-slate-700'" @click="preferences.language.value = 'en'">EN</button>
    <button class="rounded-lg px-2 py-1 text-sm" :class="preferences.language.value === 'sq' ? 'bg-emerald-500' : 'bg-slate-700'" @click="preferences.language.value = 'sq'">SQ</button>
    <select v-model="preferences.theme.value" :aria-label="tr('App theme', 'Pamja e aplikacionit')" class="max-w-28 rounded-lg border-0 bg-slate-800 px-2 py-1 text-sm text-white">
      <option value="emerald">{{ tr('Emerald', 'Jeshile') }}</option><option value="midnight">{{ tr('Midnight', 'Mesnatë') }}</option><option value="terracotta">{{ tr('Terracotta', 'Tullë') }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { onUnmounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { preferences, tr } from './store/preferences'
const route = useRoute(), welcomeName = ref('')
let welcomeTimer = 0
watch(() => route.name, (name) => {
  if (welcomeTimer) window.clearTimeout(welcomeTimer)
  welcomeName.value = ''
  if (['Waiter', 'Chef', 'Admin'].includes(String(name))) {
    welcomeName.value = sessionStorage.getItem('whisk-welcome-name') || ''
    sessionStorage.removeItem('whisk-welcome-name')
    if (welcomeName.value) welcomeTimer = window.setTimeout(() => { welcomeName.value = '' }, 4500)
  }
}, { immediate: true })
onUnmounted(() => window.clearTimeout(welcomeTimer))
</script>
