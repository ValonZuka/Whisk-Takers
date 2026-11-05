<template>
  <div class="min-h-screen bg-emerald-950 flex items-center justify-center px-4">
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-yellow-300 mb-2">Whisk Takers</h1>
        <p class="text-emerald-200">{{ tr('Join our team', 'Bëhu pjesë e ekipit') }}</p>
      </div>

      <div class="bg-emerald-900/60 rounded-lg border border-yellow-500/30 p-8 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
        <h2 class="text-2xl font-bold text-yellow-300 mb-6 text-center">{{ tr('New staff account', 'Llogari e re e stafit') }}</h2>
        
        <form @submit.prevent="signup" class="space-y-4">
          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">{{ tr('Full name', 'Emri i plotë') }}</label>
            <input 
              v-model="form.name" 
              type="text" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
              :placeholder="tr('Enter your full name', 'Shkruaj emrin e plotë')"
            />
          </div>

          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">{{ tr('Password', 'Fjalëkalimi') }}</label>
            <input 
              v-model="form.password" 
              type="password" 
              minlength="6"
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
              :placeholder="tr('Enter a password', 'Shkruaj fjalëkalimin')"
            />
          </div>

          <div>
            <label class="block text-emerald-200 text-sm font-medium mb-2">{{ tr('Role', 'Roli') }}</label>
            <select 
              v-model="form.role" 
              required 
              class="w-full rounded-lg bg-emerald-950 border border-emerald-700 px-4 py-3 text-emerald-100 focus:border-yellow-500 focus:outline-none"
            >
              <option value="">{{ tr('Select your role', 'Zgjidh rolin') }}</option>
              <option value="waiter">{{ tr('Waiter', 'Kamarier') }}</option>
              <option value="chef">{{ tr('Chef', 'Kuzhinier') }}</option>
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
            {{ loading ? tr('Creating account…', 'Duke krijuar llogarinë…') : tr('Create account', 'Krijo llogari') }}
          </button>
        </form>

        <div class="mt-6 text-center">
          <p class="text-emerald-200 text-sm">
            {{ tr('Already have an account?', 'Ke tashmë llogari?') }}
            <router-link :to="{ name: 'Login', query: { role: form.role } }" class="text-yellow-300 hover:text-yellow-200 font-medium">
              {{ tr('Sign in here', 'Hyr këtu') }}
            </router-link>
          </p>
        </div>

        <div class="mt-4 text-center">
          <router-link to="/" class="text-emerald-300 hover:text-emerald-200 text-sm">
            ← {{ tr('Back to home', 'Kthehu në fillim') }}
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
import { tr } from '../store/preferences'

const router = useRouter()
const loading = ref(false)
const error = ref('')
const success = ref('')

const form = reactive({
  name: '',
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
      password: form.password,
      role: form.role
    })

    if (response.data.success) {
      success.value = tr('Account created successfully! You can now sign in.', 'Llogaria u krijua. Tani mund të hysh.')
      
      // Store the role before clearing the form
      const selectedRole = form.role
      
      // Clear form
      form.name = ''
      form.password = ''
      form.role = ''
      
      // Redirect to login with the selected role after 2 seconds
      setTimeout(() => {
        router.push({ name: 'Login', query: { role: selectedRole } })
      }, 2000)
    }
  } catch (err: any) {
    error.value = err.response?.data?.message || tr('Could not create account. Please try again.', 'Llogaria nuk u krijua. Provo përsëri.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
</style>
