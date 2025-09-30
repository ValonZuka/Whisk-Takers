<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-6xl mx-auto px-4 py-10">
      <div class="flex items-center justify-between mb-6">
        <div class="text-yellow-300 font-bold">Admin</div>
        <div class="flex gap-2">
          <button @click="refreshData" class="rounded bg-emerald-600 text-white font-semibold px-3 py-1 hover:bg-emerald-700 transition">
            Refresh
          </button>
          <button @click="logout" class="rounded bg-yellow-500 text-emerald-950 font-semibold px-3 py-1">Sign out</button>
        </div>
      </div>
      
      <!-- Management Links -->
      <div class="mb-6 flex gap-4">
        <router-link 
          to="/fabrika" 
          class="rounded bg-blue-600 text-white font-semibold px-4 py-2 hover:bg-blue-700 transition"
        >
          🏭 Manage Fabrika
        </router-link>
        <router-link 
          to="/puntori" 
          class="rounded bg-purple-600 text-white font-semibold px-4 py-2 hover:bg-purple-700 transition"
        >
          👷 Manage Puntori
        </router-link>
      </div>
      <h1 class="text-3xl font-bold text-yellow-300 mb-6">Admin Dashboard</h1>

      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <p class="text-sm text-emerald-200">Total Products</p>
          <p class="text-3xl font-bold text-yellow-300">{{ products.length }}</p>
        </div>
        <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <p class="text-sm text-emerald-200">Total Orders</p>
          <p class="text-3xl font-bold text-yellow-300">{{ orders.length }}</p>
        </div>
        <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <p class="text-sm text-emerald-200">Recent Sales</p>
          <p class="text-3xl font-bold text-yellow-300">${{ todaysSales.toFixed(2) }}</p>
          <p class="text-xs text-emerald-300 mt-1">{{ todaysOrders.length }} completed orders</p>
        </div>
        <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <p class="text-sm text-emerald-200">Pending Orders</p>
          <p class="text-3xl font-bold text-yellow-300">{{ pending }}</p>
        </div>
      </div>

      <!-- Sales Analytics Section -->
      <div class="mt-8">
        <h2 class="text-2xl font-semibold mb-4">Recent Sales Analytics</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60">
            <h3 class="text-lg font-semibold text-yellow-300 mb-3">Best Selling Dishes</h3>
            <div v-if="bestSellers.length === 0" class="text-emerald-300 text-sm">
              No completed orders recently
            </div>
            <ul v-else class="space-y-2">
              <li v-for="(dish, index) in bestSellers" :key="index" class="flex justify-between items-center">
                <span class="text-emerald-200">{{ index + 1 }}. {{ dish.productName }}</span>
                <span class="text-yellow-300 font-semibold">${{ dish.totalSales.toFixed(2) }}</span>
              </li>
            </ul>
          </div>
          <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60">
            <h3 class="text-lg font-semibold text-yellow-300 mb-3">Worst Selling Dishes</h3>
            <div v-if="worstSellers.length === 0" class="text-emerald-300 text-sm">
              No completed orders recently
            </div>
            <ul v-else class="space-y-2">
              <li v-for="(dish, index) in worstSellers" :key="index" class="flex justify-between items-center">
                <span class="text-emerald-200">{{ index + 1 }}. {{ dish.productName }}</span>
                <span class="text-yellow-300 font-semibold">${{ dish.totalSales.toFixed(2) }}</span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <h2 class="text-2xl font-semibold mb-4">Products</h2>
          <ul class="space-y-2">
            <li v-for="p in products" :key="p.id" class="rounded border border-yellow-500/30 p-3 bg-emerald-900/60 flex justify-between shadow-sm hover:shadow-[0_0_15px_rgba(234,179,8,0.2)] transition">
              <span>{{ p.name }} ({{ p.categoryName }})</span>
              <span class="text-yellow-300 font-semibold">$ {{ p.price.toFixed(2) }}</span>
            </li>
          </ul>
        </div>
        <div>
          <h2 class="text-2xl font-semibold mb-4">Recent Orders</h2>
          <ul class="space-y-2">
            <li v-for="o in recentOrders" :key="o.id" class="rounded border border-yellow-500/30 p-3 bg-emerald-900/60 shadow-sm hover:shadow-[0_0_15px_rgba(16,185,129,0.2)] transition">
              <div class="flex justify-between">
                <span>Table {{ o.tableNumber }}</span>
                <span :class="statusClass(o.status)">{{ o.status }}</span>
              </div>
              <div class="text-sm text-emerald-200 mt-1">
                Total: ${{ getOrderTotal(o).toFixed(2) }}
              </div>
              <ul class="mt-2 text-sm">
                <li v-for="i in o.items" :key="i.productId">{{ i.productName }} x{{ i.quantity }}</li>
              </ul>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { signOut } from '../store/auth'
import api from '../api/axios'

interface ProductDto { id: number; name: string; price: number; categoryName: string }
interface OrderItemDto { productId: number; quantity: number; productName?: string; productPrice?: number }
interface OrderDto { id: number; tableNumber: number; status: string; items: OrderItemDto[]; createdAt?: string }

const products = ref<ProductDto[]>([])
const orders = ref<OrderDto[]>([])
const router = useRouter()

// Computed properties for analytics
const pending = computed(() => orders.value.filter(o => o.status === 'Pending').length)

const todaysOrders = computed(() => {
  const now = new Date()
  const today = now.toDateString()
  const yesterday = new Date(now.getTime() - 24 * 60 * 60 * 1000).toDateString()
  
  const filtered = orders.value.filter(order => {
    if (!order.createdAt) return false
    const orderDate = new Date(order.createdAt).toDateString()
    const isToday = orderDate === today
    const isYesterday = orderDate === yesterday
    const isDone = order.status === 'Done'
    const isRecent = isToday || isYesterday // Include both today and yesterday for demo purposes
    console.log(`Order ${order.id}: date=${orderDate}, today=${today}, yesterday=${yesterday}, isToday=${isToday}, isYesterday=${isYesterday}, status=${order.status}, isDone=${isDone}`)
    return isRecent && isDone
  })
  console.log('Recent completed orders:', filtered)
  return filtered
})

const todaysSales = computed(() => {
  return todaysOrders.value.reduce((total, order) => {
    return total + getOrderTotal(order)
  }, 0)
})

const recentOrders = computed(() => {
  return orders.value
    .sort((a, b) => new Date(b.createdAt || 0).getTime() - new Date(a.createdAt || 0).getTime())
    .slice(0, 5)
})

const bestSellers = computed(() => {
  const salesMap = new Map<number, { productName: string; totalSales: number; quantity: number }>()
  
  todaysOrders.value.forEach(order => {
    order.items.forEach(item => {
      const price = item.productPrice || 0
      const total = price * item.quantity
      
      if (salesMap.has(item.productId)) {
        const existing = salesMap.get(item.productId)!
        existing.totalSales += total
        existing.quantity += item.quantity
      } else {
        salesMap.set(item.productId, {
          productName: item.productName || `Product #${item.productId}`,
          totalSales: total,
          quantity: item.quantity
        })
      }
    })
  })
  
  return Array.from(salesMap.values())
    .sort((a, b) => b.totalSales - a.totalSales)
    .slice(0, 5)
})

const worstSellers = computed(() => {
  const salesMap = new Map<number, { productName: string; totalSales: number; quantity: number }>()
  
  todaysOrders.value.forEach(order => {
    order.items.forEach(item => {
      const price = item.productPrice || 0
      const total = price * item.quantity
      
      if (salesMap.has(item.productId)) {
        const existing = salesMap.get(item.productId)!
        existing.totalSales += total
        existing.quantity += item.quantity
      } else {
        salesMap.set(item.productId, {
          productName: item.productName || `Product #${item.productId}`,
          totalSales: total,
          quantity: item.quantity
        })
      }
    })
  })
  
  return Array.from(salesMap.values())
    .sort((a, b) => a.totalSales - b.totalSales)
    .slice(0, 5)
})

async function loadData() {
  try {
    const [prodRes, orderRes] = await Promise.all([
      api.get<ProductDto[]>('/Products'),
      api.get<OrderDto[]>('/Orders')
    ])
    products.value = prodRes.data
    orders.value = orderRes.data
  } catch (error) {
    console.error('Error loading data:', error)
  }
}

async function refreshData() {
  await loadData()
}

onMounted(async () => {
  await loadData()
  
  // Auto-refresh every 10 seconds to catch order status updates
  setInterval(async () => {
    await loadData()
  }, 10000)
})

function getOrderTotal(order: OrderDto): number {
  return order.items.reduce((total, item) => {
    const price = item.productPrice || 0
    return total + (price * item.quantity)
  }, 0)
}

function statusClass(status: string) {
  switch (status) {
    case 'Pending': return 'text-yellow-300'
    case 'Preparing': return 'text-blue-300'
    case 'Ready': return 'text-emerald-300'
    case 'Done': return 'text-green-300'
    case 'Canceled': return 'text-red-300'
    default: return 'text-gray-300'
  }
}

function logout() {
  signOut()
  router.replace({ name: 'Intro' })
}
</script>

<style scoped>
</style>


