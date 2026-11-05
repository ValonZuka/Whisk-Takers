<template>
  <main class="flex min-h-screen items-center justify-center bg-emerald-950 px-4 text-emerald-50">
    <section class="w-full max-w-md rounded-2xl border border-yellow-500/40 bg-emerald-900/60 p-7">
      <h1 class="text-2xl font-bold text-yellow-300">{{ tr('Sign in', 'Hyr') }}</h1>
      <p class="mt-1 text-sm text-emerald-200">{{ tr('Role', 'Roli') }}: {{ roleLabel }}</p>
      <form class="mt-6 space-y-4" @submit.prevent="login">
        <label class="block text-sm">{{ tr('Password', 'Fjalëkalimi') }}
          <input v-model="password" required minlength="6" type="password" autocomplete="current-password" autofocus class="mt-1 w-full rounded border border-emerald-700 bg-emerald-950 px-3 py-3" :placeholder="tr('Enter your password', 'Shkruaj fjalëkalimin')" />
        </label>
        <p v-if="authError" class="text-sm text-red-300">{{ authError }}</p>
        <button :disabled="loading" class="w-full rounded bg-yellow-500 py-3 font-bold text-emerald-950 disabled:opacity-50">{{ loading ? tr('Signing in…', 'Duke hyrë…') : tr('Continue', 'Vazhdo') }}</button>
      </form>
      <p class="mt-5 text-center text-sm text-emerald-200"><RouterLink to="/" class="underline">{{ tr('Back', 'Kthehu') }}</RouterLink></p>
    </section>
  </main>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import api from '../api/axios'
import { setToken, type UserRole } from '../store/auth'
import { tr } from '../store/preferences'

const route = useRoute(), router = useRouter()
const roles: UserRole[] = ['admin', 'waiter', 'chef']
const requestedRole = route.query.role as UserRole
const role = roles.includes(requestedRole) ? requestedRole : 'waiter'
const roleLabel = computed(() => ({ admin: tr('Admin', 'Administratori'), waiter: tr('Waiter', 'Kamarieri'), chef: tr('Chef', 'Kuzhinieri'), client: 'Client' })[role])
const password = ref(''), authError = ref(''), loading = ref(false)
async function login() {
  loading.value = true; authError.value = ''
  try {
    const { data } = await api.post('/auth/login', { role, password: password.value })
    setToken(data.token)
    sessionStorage.setItem('whisk-welcome-name', data.user.name)
    await router.replace(`/${data.user.role}`)
  } catch (error: any) {
    authError.value = error.response?.data?.message || tr('Incorrect password.', 'Fjalëkalimi nuk është i saktë.')
  } finally { loading.value = false }
}
</script>
