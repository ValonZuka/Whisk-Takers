<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-5xl mx-auto px-4 py-16 text-center">
      <h1 class="text-4xl font-extrabold text-yellow-300">Whisk Takers</h1>
      <p class="mt-2 text-emerald-100">Select your role to continue</p>

      <div class="mt-10 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <button v-for="r in roles" :key="r.path" class="rounded-xl border border-yellow-500/40 bg-emerald-900/60 p-6 hover:bg-emerald-900 transition text-left" @click="goLogin(r.role)">
          <p class="text-xl font-bold text-yellow-200">{{ r.title }}</p>
          <p class="mt-1 text-emerald-100">{{ r.description }}</p>
        </button>
      </div>

      <div class="mt-8 text-center">
        <p class="text-emerald-200 mb-4">New to our team?</p>
        <router-link to="/signup" class="inline-block bg-yellow-500 hover:bg-yellow-600 text-emerald-950 font-semibold px-6 py-3 rounded-lg transition-colors">
          Join as Waiter or Chef
        </router-link>
      </div>

      <div class="mt-8 text-center">
        <p class="text-emerald-200 mb-4">View our facilities and staff</p>
        <div class="flex gap-4 justify-center">
          <router-link to="/fabrika" class="inline-block bg-emerald-600 hover:bg-emerald-700 text-white font-semibold px-6 py-3 rounded-lg transition-colors">
            View Fabrika
          </router-link>
          <router-link to="/puntori" class="inline-block bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-3 rounded-lg transition-colors">
            View Puntori
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { UserRole } from '../store/auth'

const roles = [
  { title: 'Client', role: 'client', description: 'Browse and order dishes', path: '/client' },
  { title: 'Waiter', role: 'waiter', description: 'Create and manage orders', path: '/waiter' },
  { title: 'Chef', role: 'chef', description: 'Prepare and update statuses', path: '/chef' },
  { title: 'Admin', role: 'admin', description: 'Analytics and inventory', path: '/admin' },
]

const router = useRouter()
function goLogin(role: UserRole) {
  router.push({ name: 'Login', query: { role } })
}
</script>


