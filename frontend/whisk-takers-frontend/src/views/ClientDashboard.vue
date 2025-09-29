<template>
  <div class="min-h-screen bg-gradient-to-b from-emerald-950 via-emerald-900 to-emerald-950 text-emerald-50">
    <section class="max-w-7xl mx-auto px-4 py-12">
      <div class="flex items-center justify-between mb-6">
        <div class="text-yellow-300 font-bold">Client</div>
        <button @click="logout" class="rounded bg-yellow-500 text-emerald-950 font-semibold px-3 py-1">Sign out</button>
      </div>
      <div class="text-center mb-10">
        <h1 class="text-4xl font-extrabold text-emerald-300">Welcome to Whisk Takers</h1>
        <p class="mt-3 text-emerald-100">Culinary excellence with a touch of royalty</p>
      </div>

      <div class="relative overflow-hidden rounded-2xl border border-yellow-500/40 shadow-[0_0_50px_rgba(234,179,8,0.1)] mb-12">
        <div class="absolute inset-0 bg-gradient-to-r from-yellow-500/10 via-transparent to-yellow-500/10 animate-pulse"></div>
        <img src="/placeholder.svg" alt="Hero" class="w-full h-64 object-cover opacity-70" />
        <div class="absolute inset-0 flex flex-col items-center justify-center">
          <h2 class="text-3xl md:text-5xl font-bold text-yellow-300 drop-shadow">A Royal Feast Awaits</h2>
          <p class="mt-2 text-emerald-50">Explore our curated dishes below</p>
        </div>
      </div>

      <div class="flex items-center justify-between mb-6">
        <h3 class="text-2xl font-semibold text-yellow-300">Menu</h3>
        <div class="flex items-center gap-3">
          <select v-model="filter" class="rounded bg-emerald-950 border border-emerald-700 px-3 py-2 text-emerald-100">
            <option value="all">All</option>
            <option value="Starters">Appetisers</option>
            <option value="Mains">Main Course</option>
            <option value="Desserts">Desserts</option>
          </select>
          <RouterLink to="/about" class="text-yellow-300 hover:text-yellow-200 underline">About Us</RouterLink>
        </div>
      </div>

      <div class="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="p in fullMenu" :key="p.id" class="rounded-xl bg-emerald-950/70 border border-yellow-500/30 hover:border-yellow-400/50 transition shadow-[0_0_30px_rgba(16,185,129,0.1)] overflow-hidden">
          <img :src="p.image || '/public/placeholder.svg'" alt="Dish image" class="w-full h-40 object-cover opacity-90" />
          <div class="p-5">
            <div class="flex items-center justify-between">
              <h4 class="text-xl font-bold text-yellow-200">{{ p.name }}</h4>
              <span class="text-sm px-2 py-1 rounded-full border border-yellow-500/40 text-yellow-200" v-if="p.badge">{{ p.badge }}</span>
            </div>
            <p class="mt-1 text-emerald-200">{{ p.categoryName }}</p>
            <p class="mt-3 text-yellow-300 font-semibold">$ {{ p.price.toFixed(2) }}</p>
            <p class="mt-3 text-emerald-100">{{ p.description || lorem }}</p>
          </div>
        </div>
      </div>
    </section>
  </div>
  
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { useRouter } from 'vue-router'
import api from '../api/axios'
import { signOut } from '../store/auth'
interface ProductDto {
  id: number
  name: string
  price: number
  categoryName: string
  image?: string
  description?: string
}

const products = ref<ProductDto[]>([])
const router = useRouter()

// Signature curated dishes to enrich the menu visually
const signatureDishes = ref<Array<ProductDto & { description: string; badge?: string }>>([
  { id: 1001, name: 'Emerald Herb Risotto', price: 19.5, categoryName: 'Mains', description: lorem, badge: 'Signature', image: '/public/placeholder.svg' },
  { id: 1002, name: 'Golden Truffle Pasta', price: 24, categoryName: 'Mains', description: lorem, badge: 'Chef\'s Pick', image: '/public/placeholder.svg' },
  { id: 1003, name: 'Royal Garden Salad', price: 12, categoryName: 'Starters', description: lorem, image: '/public/placeholder.svg' },
  { id: 1004, name: 'Saffron Seafood Paella', price: 28, categoryName: 'Mains', description: lorem, image: '/public/placeholder.svg' },
  { id: 1005, name: 'Matcha Cheesecake', price: 9.5, categoryName: 'Desserts', description: lorem, image: '/public/placeholder.svg' },
  { id: 1006, name: 'Citrus Herb Chicken', price: 17, categoryName: 'Mains', description: lorem, image: '/public/placeholder.svg' },
  { id: 1007, name: 'Velvet Mushroom Soup', price: 10, categoryName: 'Starters', description: lorem, image: '/public/placeholder.svg' },
  { id: 1008, name: 'Caramelized Pear Tart', price: 8.5, categoryName: 'Desserts', description: lorem, image: '/public/placeholder.svg' },
])

const filter = ref<'all' | 'Starters' | 'Mains' | 'Desserts'>('all')

const fullMenu = computed(() => {
  // Merge API products with curated ones (avoid ID collisions by keeping custom IDs high)
  const merged = [...signatureDishes.value, ...products.value.map(p => ({ ...p, description: lorem, image: '/public/placeholder.svg' }))]
  if (filter.value === 'all') return merged
  return merged.filter(p => p.categoryName === filter.value)
})

const lorem = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam ut sapien non metus posuere tempus.'

function logout() {
  signOut()
  router.replace({ name: 'Intro' })
}

onMounted(async () => {
  const { data } = await api.get<ProductDto[]>('/Products')
  products.value = data
})

function getDescription(p: ProductDto): string {
  return `A delightful ${p.categoryName.toLowerCase()} crafted by our master chefs.`
}
</script>

<style scoped>
</style>


