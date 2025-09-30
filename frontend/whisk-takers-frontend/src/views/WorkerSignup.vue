<template>
  <div class="min-h-screen bg-emerald-950 flex items-center justify-center px-4">
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-yellow-300 mb-2">Whisk Takers</h1>
        <p class="text-emerald-200">Join Our Team</p>
      </div>

      <div class="bg-emerald-900/60 rounded-lg border border-yellow-500/30 p-8 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
        <h2 class="text-2xl font-bold text-yellow-300 mb-6 text-center">New Worker Signup</h2>
        
        <form @submit.prevent="signup" class="space-y-4">
          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">Full Name</label>
            <input 
              v-model="form.name" 
              type="text" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
              placeholder="Enter your full name"
            />
          </div>

          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">Email</label>
            <input 
              v-model="form.email" 
              type="email" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
              placeholder="Enter your email"
            />
          </div>

          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">Password</label>
            <input 
              v-model="form.password" 
              type="password" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
              placeholder="Enter your password"
            />
          </div>

          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">Role</label>
            <select 
              v-model="form.role" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
            >
              <option value="">Select your role</option>
              <option value="waiter">Waiter</option>
              <option value="chef">Chef</option>
            </select>
          </div>

          <div v-if="error" class="bg-red-900/50 border border-red-500/50 rounded-lg p-3 text-red-200 text-sm">
            {{ error }}
          </div>

          <div v-if="success" class="bg-green-900/50 border border-green-500/50 rounded-lg p-3 text-green-200 text-sm">
            {{ success }}
          </div>

          <button 
            type="submit" 
            :disabled="loading"
            class="w-full bg-yellow-500 hover:bg-yellow-600 disabled:bg-yellow-500/50 text-emerald-950 font-semibold py-3 px-4 rounded-lg transition-colors"
          >
            {{ loading ? 'Creating Account...' : 'Create Account' }}
          </button>
        </form>

        <div class="mt-6 text-center">
          <p class="text-emerald-200 text-sm">
            Already have an account? 
            <router-link :to="{ name: 'Login', query: { role: form.role } }" class="text-yellow-300 hover:text-yellow-200 font-medium">
              Sign in here
            </router-link>
          </p>
        </div>

        <div class="mt-4 text-center">
          <router-link to="/" class="text-emerald-300 hover:text-emerald-200 text-sm">
            ← Back to Home
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios'

const router = useRouter()
const loading = ref(false)
const error = ref('')
const success = ref('')

const form = reactive({
  name: '',
  email: '',
  password: '',
  role: ''
})

async function signup() {
  loading.value = true
  error.value = ''
  success.value = ''

  try {
    const response = await api.post('/auth/register', {
      name: form.name,
      email: form.email,
      password: form.password,
      role: form.role
    })

    if (response.data.success) {
      success.value = 'Account created successfully! You can now sign in.'
      
      // Store the role before clearing the form
      const selectedRole = form.role
      
      // Clear form
      form.name = ''
      form.email = ''
      form.password = ''
      form.role = ''
      
      // Redirect to login with the selected role after 2 seconds
      setTimeout(() => {
        router.push({ name: 'Login', query: { role: selectedRole } })
      }, 2000)
    }
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Failed to create account. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
</style>
