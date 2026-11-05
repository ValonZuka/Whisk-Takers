<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-5xl mx-auto px-4 py-16 text-center">
      <h1 class="text-4xl font-extrabold text-yellow-300">Whisk Takers</h1>
      <div class="mt-4 flex justify-center"><PreferencesControls /></div>
      <p class="mt-4 text-emerald-100">{{ tr('Select your role to continue', 'Zgjidh rolin për të vazhduar') }}</p>

      <div class="mt-10 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <button v-for="r in roles" :key="r.path" class="rounded-xl border border-yellow-500/40 bg-emerald-900/60 p-6 hover:bg-emerald-900 transition text-left" @click="goLogin(r.role)">
          <p class="text-xl font-bold text-yellow-200">{{ tr(r.title, r.sqTitle) }}</p>
          <p class="mt-1 text-emerald-100">{{ tr(r.description, r.sqDescription) }}</p>
        </button>
      </div>

      <div class="mt-8 text-center">
        <p class="text-emerald-200 mb-4">{{ tr('New to our team?', 'I ri në ekipin tonë?') }}</p>
        <router-link to="/signup" class="inline-block bg-yellow-500 hover:bg-yellow-600 text-emerald-950 font-semibold px-6 py-3 rounded-lg transition-colors">
          {{ tr('Join as Waiter or Chef', 'Regjistrohu si kamarier ose kuzhinier') }}
        </router-link>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { UserRole } from '../store/auth'
import PreferencesControls from '../components/PreferencesControls.vue'
import { tr } from '../store/preferences'

const roles: Array<{ title: string; sqTitle: string; role: UserRole; description: string; sqDescription: string; path: string }> = [
  { title: 'Client', sqTitle: 'Klient', role: 'client', description: 'Browse and order dishes', sqDescription: 'Shfleto menunë dhe porosit', path: '/client' },
  { title: 'Waiter', sqTitle: 'Kamarier', role: 'waiter', description: 'Create and manage orders', sqDescription: 'Krijo dhe menaxho porositë', path: '/waiter' },
  { title: 'Chef', sqTitle: 'Kuzhinier', role: 'chef', description: 'Prepare and update statuses', sqDescription: 'Përgatit porositë dhe përditëso statuset', path: '/chef' },
  { title: 'Admin', sqTitle: 'Administrator', role: 'admin', description: 'Analytics and inventory', sqDescription: 'Analitika dhe menaxhimi i menusë', path: '/admin' },
]

const router = useRouter()
function goLogin(role: UserRole) {
  if (role === 'client') { router.push('/client'); return }
  router.push({ name: 'Login', query: { role } })
}
</script>

