<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-7xl mx-auto px-4 py-10">
      <div class="flex items-center justify-between mb-6">
        <div class="text-yellow-300 font-bold">Waiter</div>
        <button @click="logout" class="rounded bg-yellow-500 text-emerald-950 font-semibold px-3 py-1">Sign out</button>
      </div>
      <h1 class="text-3xl font-bold text-yellow-300 mb-6">Waiter Dashboard</h1>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="md:col-span-2">
          <h2 class="text-2xl font-semibold mb-4">Products ({{ products.length }} available)</h2>
          <div v-if="!tableId" class="rounded-lg border border-yellow-500/30 p-6 bg-emerald-900/60 text-emerald-100">
            Please select a table first.
          </div>
          <div v-else class="grid gap-6 grid-cols-1 sm:grid-cols-2">
            <div v-if="products.length === 0" class="col-span-2 text-center text-emerald-200">
              No products available. Loading...
            </div>
            <div v-for="p in products" :key="p.id" class="rounded-lg border border-yellow-500/30 bg-emerald-900/60 overflow-hidden">
              <div class="relative">
                <img src="/public/placeholder.svg" class="w-full h-28 object-cover opacity-90" />
              </div>
              <div class="p-4">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="font-bold text-yellow-200">{{ p.name }}</p>
                    <p class="text-sm text-emerald-200">{{ p.categoryName }}</p>
                  </div>
                  <p class="text-yellow-300 font-semibold">$ {{ p.price.toFixed(2) }}</p>
                </div>
                <p class="text-sm mt-2 text-emerald-200">{{ p.description }}</p>
                <div class="mt-3 flex items-center gap-2">
                  <input type="number" min="1" v-model.number="quantities[p.id]" class="w-20 rounded bg-emerald-950 border border-emerald-700 px-2 py-1" />
                  <button @click="addItem(p)" class="px-3 py-1 rounded bg-yellow-500 text-emerald-900 font-semibold">Add</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div>
          <h2 class="text-2xl font-semibold mb-4">Create Order</h2>
          <div class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60">
            <label class="block text-sm mb-2">Select a Table</label>
            <div class="grid grid-cols-3 gap-3 mb-4">
              <button
                v-for="n in tables"
                :key="n"
                @click="selectTable(n)"
                :class="[
                  'relative h-20 rounded-xl border text-center font-bold',
                  tableId === n ? 'bg-yellow-400 text-emerald-950 border-yellow-300' : (isBusy(n) ? 'bg-emerald-900/60 border-yellow-500/50 text-yellow-300' : 'bg-emerald-950/70 border-emerald-700 text-yellow-200')
                ]"
              >
                <span class="absolute inset-0 flex items-center justify-center text-2xl">{{ n }}</span>
                <span class="sr-only">Table {{ n }}</span>
              </button>
            </div>

            <h3 class="mt-4 font-semibold">Items</h3>
            <ul class="mt-2 space-y-1">
              <li v-for="item in orderItems" :key="item.productId" class="flex justify-between text-sm">
                <span>{{ getProductName(item.productId) }}</span>
                <span>x{{ item.quantity }}</span>
              </li>
            </ul>

            <button @click="submitOrder" :disabled="!orderItems.length || !tableId" class="mt-4 w-full px-4 py-2 rounded bg-yellow-500 disabled:opacity-50 text-emerald-900 font-bold shadow-[0_0_20px_rgba(234,179,8,0.35)]">Submit Order</button>
          </div>

          <h2 class="text-2xl font-semibold mt-8 mb-4">Recent Orders</h2>
          <div class="space-y-3">
            <div v-for="o in orders" :key="o.id" class="rounded border border-yellow-500/30 p-3 bg-emerald-900/60">
              <div class="flex justify-between">
                <p>Table {{ o.tableNumber }}</p>
                <p class="font-semibold" :class="statusClass(o.status)">{{ o.status }}</p>
              </div>
              <ul class="mt-2 text-sm">
                <li v-for="i in o.items" :key="i.productId">{{ i.productName }} x{{ i.quantity }}</li>
              </ul>
              <p class="text-xs text-emerald-200 mt-1">Waiter: {{ o.waiterName || 'Unknown' }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { signOut, getCurrentUser } from '../store/auth'
import api from '../api/axios'
interface ProductDto { id: number; name: string; price: number; categoryName: string }
interface OrderItemDto { productId: number; quantity: number; productName?: string; productPrice?: number }
interface OrderDto { id: number; tableNumber: number; status: string; items: OrderItemDto[]; waiterName?: string; waiterEmail?: string }

const apiProducts = ref<ProductDto[]>([])
const orders = ref<OrderDto[]>([])
const quantities = reactive<Record<number, number>>({})
const orderItems = ref<OrderItemDto[]>([])
const tableId = ref<number | null>(null)
const tables = Array.from({ length: 12 }, (_, i) => i + 1)
const lorem = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla facilisi.'
const busyTables = ref<Set<number>>(new Set())
const currentUser = getCurrentUser()
const waiterEmail = currentUser?.email || ''
const router = useRouter()

    // Only use real database products for waiter (no signature dishes)
    const products = computed(() => {
      return apiProducts.value.map(p => ({ ...p, description: lorem, image: '/public/placeholder.svg' }))
    })
function selectTable(n: number) {
  if (!canEditTable(n)) {
    alert('This table is being managed by another waiter')
    return
  }
  tableId.value = n
  console.log('Selected table:', n, 'Products available:', products.value.length)
}

onMounted(async () => {
  try {
    const [prodRes, orderRes] = await Promise.all([
      api.get<ProductDto[]>('/Products'),
      api.get<OrderDto[]>('/Orders')
    ])
    apiProducts.value = prodRes.data
    orders.value = orderRes.data
    console.log('Loaded API products:', apiProducts.value.length)
    console.log('Total products (API + signature):', products.value.length)
    console.log('Loaded orders:', orders.value.length)
  } catch (error) {
    console.error('Error loading data:', error)
  }
})

function addItem(p: ProductDto) {
  const qty = quantities[p.id] || 1
  const existing = orderItems.value.find(i => i.productId === p.id)
  if (existing) existing.quantity += qty
  else orderItems.value.push({ productId: p.id, quantity: qty })
  quantities[p.id] = 1
}

function getProductName(id: number) {
  return products.value.find(p => p.id === id)?.name || `#${id}`
}

async function submitOrder() {
  if (!tableId.value || orderItems.value.length === 0) return
  
  const payload = { tableNumber: tableId.value, items: orderItems.value.map(i => ({ productId: i.productId, quantity: i.quantity })) }
  await api.post('/Orders', payload)
  orderItems.value = []
  tableId.value = null
  const { data } = await api.get<OrderDto[]>('/Orders')
  orders.value = data
  busyTables.value.add(payload.tableNumber)
}

function statusClass(status: string) {
  return status === 'Pending' ? 'text-yellow-300' : status === 'Done' ? 'text-emerald-300' : 'text-red-300'
}

function isBusy(n: number) {
  return busyTables.value.has(n) || orders.value.some(o => o.tableNumber === n && (o.status === 'Pending' || o.status === 'Preparing'))
}

function canEditTable(n: number) {
  // Can only edit if no other waiter has an active order on this table
  const activeOrder = orders.value.find(o => o.tableNumber === n && (o.status === 'Pending' || o.status === 'Preparing'))
  return !activeOrder || activeOrder.waiterEmail === waiterEmail
}

function logout() {
  signOut()
  router.replace({ name: 'Intro' })
}
</script>

<style scoped>
</style>


