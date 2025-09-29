<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-5xl mx-auto px-4 py-10">
      <div class="flex items-center justify-between mb-6">
        <div class="text-yellow-300 font-bold">Chef</div>
        <button @click="logout" class="rounded bg-yellow-500 text-emerald-950 font-semibold px-3 py-1">Sign out</button>
      </div>
      <h1 class="text-3xl font-bold text-yellow-300 mb-6">Chef Dashboard</h1>

      <div class="mb-6">
        <select v-model="statusFilter" class="rounded bg-emerald-950 border border-emerald-700 px-3 py-2 text-emerald-100">
          <option value="all">All Orders</option>
          <option value="Pending">Pending</option>
          <option value="Preparing">Preparing</option>
          <option value="Ready">Ready</option>
        </select>
      </div>

      <div class="space-y-4">
        <div v-for="o in filteredOrders" :key="o.id" class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <div class="flex justify-between items-center">
            <div>
              <p class="font-semibold">Table {{ o.tableNumber }}</p>
              <p class="text-sm text-emerald-200">#{{ o.id }}</p>
            </div>
            <div class="flex items-center gap-2">
              <span class="mr-2" :class="statusClass(o.status)">{{ o.status }}</span>
              <button @click="updateStatus(o.id, 'Preparing')" v-if="o.status === 'Pending'" class="px-3 py-1 rounded bg-yellow-400 text-emerald-950 font-semibold shadow">Start Preparing</button>
              <button @click="updateStatus(o.id, 'Ready')" v-if="o.status === 'Preparing'" class="px-3 py-1 rounded bg-emerald-400 text-emerald-950 font-semibold shadow">Mark Ready</button>
              <button @click="updateStatus(o.id, 'Done')" v-if="o.status === 'Ready'" class="px-3 py-1 rounded bg-green-400 text-emerald-950 font-semibold shadow">Mark Done</button>
              <button @click="updateStatus(o.id, 'Canceled')" v-if="o.status !== 'Done' && o.status !== 'Canceled'" class="px-3 py-1 rounded bg-red-400 text-emerald-950 font-semibold shadow">Cancel</button>
            </div>
          </div>
          <ul class="mt-3 text-sm">
            <li v-for="i in o.items" :key="i.productId">{{ i.productName }} x{{ i.quantity }}</li>
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
interface OrderItemDto { productId: number; quantity: number; productName?: string }
interface OrderDto { id: number; tableNumber: number; status: string; items: OrderItemDto[]; waiterName?: string }

const orders = ref<OrderDto[]>([])
const statusFilter = ref<'all' | 'Pending' | 'Preparing' | 'Ready'>('all')
const router = useRouter()

const filteredOrders = computed(() => {
  if (statusFilter.value === 'all') return orders.value
  return orders.value.filter(o => o.status === statusFilter.value)
})

onMounted(load)

async function load() {
  const { data } = await api.get<OrderDto[]>('/Orders')
  orders.value = data
}

async function updateStatus(id: number, status: 'Pending' | 'Preparing' | 'Ready' | 'Done' | 'Canceled') {
  try {
    await api.put(`/Orders/${id}/status`, { status })
    const order = orders.value.find(o => o.id === id)
    if (order) order.status = status
  } catch (error) {
    console.error('Failed to update status:', error)
  }
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


