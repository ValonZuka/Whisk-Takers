<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50 flex items-center justify-center px-4">
    <div class="w-full max-w-md rounded-2xl border border-yellow-500/40 bg-emerald-900/60 p-6">
      <h1 class="text-2xl font-bold text-yellow-300">Sign in</h1>
      <p class="text-sm text-emerald-200">Role: <span class="font-semibold">{{ role }}</span></p>

      <form class="mt-6 space-y-4" @submit.prevent="login">
        <div>
          <label class="block text-sm mb-1">Email</label>
          <input v-model="email" type="email" class="w-full rounded bg-emerald-950 border border-emerald-700 px-3 py-2" placeholder="email@example.com" />
          <p v-if="emailError" class="text-sm text-red-300 mt-1">{{ emailError }}</p>
        </div>
        <div>
          <label class="block text-sm mb-1">Password</label>
          <input v-model="password" type="password" class="w-full rounded bg-emerald-950 border border-emerald-700 px-3 py-2" placeholder="Your password" />
          <p v-if="passwordError" class="text-sm text-red-300 mt-1">{{ passwordError }}</p>
        </div>
        <div>
          <label class="block text-sm mb-1">Name</label>
          <input v-model="name" class="w-full rounded bg-emerald-950 border border-emerald-700 px-3 py-2" placeholder="Your name" />
        </div>
        <button class="w-full rounded bg-yellow-500 text-emerald-950 font-bold py-2">Continue</button>
        <p v-if="authError" class="text-sm text-red-300 text-center">{{ authError }}</p>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { encodeToken, setToken } from '../store/auth'
import type { UserRole } from '../store/auth'

const route = useRoute()
const router = useRouter()
const role = (route.query.role as UserRole) || 'client'
const name = ref('')
const email = ref('')
const password = ref('')
const emailError = ref('')
const passwordError = ref('')
const authError = ref('')

async function login() {
  emailError.value = ''
  passwordError.value = ''
  authError.value = ''

  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    emailError.value = 'Enter a valid email.'
  }
  if (!password.value || password.value.length < 6) {
    passwordError.value = 'Password must be at least 6 characters.'
  }
  if (emailError.value || passwordError.value) return

  const result = await fakeSignIn(email.value, password.value, role)
  if (!result) {
    authError.value = 'Incorrect credentials.'
    return
  }
  
  // Store the real JWT token from backend
  setToken(result.token)
  router.replace(`/${result.user.role}`)
}

async function fakeSignIn(email: string, password: string, role: UserRole) {
  try {
    const response = await fetch('http://localhost:5058/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password })
    })
    
    if (!response.ok) return false
    
    const data = await response.json()
    return { token: data.token, user: data.user }
  } catch {
    return false
  }
}
</script>


