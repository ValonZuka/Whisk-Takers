import { computed, ref, watch } from 'vue'

export type Language = 'sq' | 'en'
export type Theme = 'emerald' | 'midnight' | 'terracotta'

const language = ref<Language>((localStorage.getItem('whisk-language') as Language) || 'en')
const theme = ref<Theme>((localStorage.getItem('whisk-theme') as Theme) || 'emerald')
watch(language, value => localStorage.setItem('whisk-language', value))
watch(theme, value => {
  localStorage.setItem('whisk-theme', value)
  document.documentElement.dataset.theme = value
}, { immediate: true })

export const preferences = { language, theme }
export const isAlbanian = computed(() => language.value === 'sq')
export function tr(en: string, sq: string) { return language.value === 'sq' ? sq : en }

