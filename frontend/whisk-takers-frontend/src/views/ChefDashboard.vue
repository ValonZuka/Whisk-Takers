<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-5xl mx-auto px-4 py-10">
      <div class="flex items-center justify-between mb-6">
        <div class="text-yellow-300 font-bold">{{ tr('Chef', 'Kuzhinier') }}</div>
        <div class="flex items-center gap-3"><PreferencesControls /><button @click="logout" class="rounded bg-yellow-500 text-emerald-950 font-semibold px-3 py-2">{{ tr('Sign out', 'Dil') }}</button></div>
      </div>
      <h1 class="text-3xl font-bold text-yellow-300 mb-6">{{ tr('Chef Dashboard', 'Paneli i kuzhinierit') }}</h1>

      <section class="mb-8 rounded-xl border border-emerald-700/70 bg-emerald-900/50 p-4 shadow-lg sm:p-5">
        <div class="grid gap-4 sm:grid-cols-2">
          <div>
            <span class="mb-2 block text-sm font-semibold text-emerald-100">{{ tr('Order status', 'Statusi i porosisë') }}</span>
            <details class="group relative">
              <summary class="flex min-h-14 cursor-pointer list-none items-center justify-between rounded-lg border border-emerald-600 bg-emerald-950 px-4 py-3 text-base text-emerald-50 shadow-sm outline-none transition hover:border-yellow-400 focus-visible:ring-2 focus-visible:ring-yellow-400/40">
                <span>{{ statusFilter === 'all' ? tr('All Orders', 'Të gjitha porositë') : tr(statusFilter, statusFilter === 'Pending' ? 'Në pritje' : statusFilter === 'Preparing' ? 'Në përgatitje' : 'Gati') }}</span>
                <span class="ml-3 text-emerald-300 transition group-open:rotate-180" aria-hidden="true">⌄</span>
              </summary>
              <div class="absolute z-20 mt-2 max-h-64 w-full overflow-y-auto rounded-lg border border-emerald-600 bg-emerald-950 p-2 shadow-xl">
                <button v-for="option in ['all', 'Pending', 'Preparing', 'Ready'] as const" :key="option" type="button" class="min-h-12 w-full rounded-md px-4 py-3 text-left text-base transition hover:bg-emerald-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-yellow-400" :class="statusFilter === option ? 'bg-emerald-800 text-yellow-200' : 'text-emerald-50'" @click="statusFilter = option; closeFilter($event)">
                  {{ option === 'all' ? tr('All Orders', 'Të gjitha porositë') : tr(option, option === 'Pending' ? 'Në pritje' : option === 'Preparing' ? 'Në përgatitje' : 'Gati') }}
                </button>
              </div>
            </details>
          </div>
          <div>
            <span class="mb-2 block text-sm font-semibold text-emerald-100">{{ tr('Restaurant area', 'Zona e restorantit') }}</span>
            <details class="group relative">
              <summary class="flex min-h-14 cursor-pointer list-none items-center justify-between rounded-lg border border-emerald-600 bg-emerald-950 px-4 py-3 text-base text-emerald-50 shadow-sm outline-none transition hover:border-yellow-400 focus-visible:ring-2 focus-visible:ring-yellow-400/40">
                <span>{{ areaFilter === 'all' ? tr('All restaurant areas', 'Të gjitha zonat') : areaFilter }}</span>
                <span class="ml-3 text-emerald-300 transition group-open:rotate-180" aria-hidden="true">⌄</span>
              </summary>
              <div class="absolute z-20 mt-2 max-h-64 w-full overflow-y-auto rounded-lg border border-emerald-600 bg-emerald-950 p-2 shadow-xl">
                <button type="button" class="min-h-12 w-full rounded-md px-4 py-3 text-left text-base text-emerald-50 transition hover:bg-emerald-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-yellow-400" :class="areaFilter === 'all' ? 'bg-emerald-800 text-yellow-200' : ''" @click="areaFilter = 'all'; closeFilter($event)">{{ tr('All restaurant areas', 'Të gjitha zonat') }}</button>
                <button v-for="area in areas" :key="area" type="button" class="mt-2 min-h-12 w-full rounded-md px-4 py-3 text-left text-base transition hover:bg-emerald-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-yellow-400" :class="areaFilter === area ? 'bg-emerald-800 text-yellow-200' : 'text-emerald-50'" @click="areaFilter = area; closeFilter($event)">{{ area }}</button>
              </div>
            </details>
          </div>
        </div>
      </section>

      <div class="flex flex-col">
      <section class="order-2 mb-8 rounded-xl border border-yellow-500/30 bg-emerald-900/60 p-4">
        <details>
        <summary class="flex cursor-pointer list-none flex-wrap items-center justify-between gap-3"><span><span class="text-lg font-semibold text-yellow-200">{{ tr('Menu management', 'Menaxhimi i menusë') }}</span><span class="mt-1 block text-sm text-emerald-200">{{ tr('Open only when you need to edit dishes.', 'Hape vetëm kur duhet të ndryshosh pjatat.') }}</span></span><span class="rounded bg-emerald-800 px-3 py-2 text-sm">{{ tr('Open menu editor', 'Hap redaktorin') }}</span></summary>
        <div class="pt-4"><div class="flex flex-wrap items-center justify-between gap-3"><p class="text-sm text-emerald-200">{{ tr('Add dishes and ingredients, edit existing items, or load the sample menu.', 'Shto pjata dhe përbërës, ndrysho artikujt, ose ngarko menunë shembull.') }}</p><button @click="seedMenu" class="rounded bg-emerald-700 px-3 py-2 text-sm">{{ tr('Load sample menu', 'Ngarko menunë shembull') }}</button></div>
        <p v-if="menuMessage" class="mt-3 text-sm" :class="menuError ? 'text-red-300' : 'text-emerald-300'">{{ menuMessage }}</p>
        <form @submit.prevent="saveProduct" class="mt-4 grid gap-2 md:grid-cols-2"><input v-model="menuForm.name" required :placeholder="tr('Dish name', 'Emri i pjatës')" class="rounded border border-emerald-700 bg-emerald-950 px-3 py-2"/><input v-model.number="menuForm.price" required type="number" min="0.01" step="0.01" :placeholder="tr('Price (€)', 'Çmimi (€)')" class="rounded border border-emerald-700 bg-emerald-950 px-3 py-2"/><input v-model.number="menuForm.unitCost" type="number" min="0" step="0.01" :placeholder="tr('Cost per serving (€)', 'Kostoja për porcion (€)')" class="rounded border border-emerald-700 bg-emerald-950 px-3 py-2"/><label class="flex items-center gap-2 rounded border border-emerald-700 bg-emerald-950 px-3 py-2 text-sm">{{ tr('Prep time (minutes)', 'Koha e përgatitjes (minuta)') }}<input v-model.number="menuForm.prepMinutes" required type="number" min="1" max="240" class="w-20 rounded bg-transparent text-right"/></label><input v-model="menuForm.imageUrl" type="url" :placeholder="tr('Photo URL (optional)', 'Lidhja e fotos (opsionale)')" class="rounded border border-emerald-700 bg-emerald-950 px-3 py-2 md:col-span-2"/><textarea v-model="menuForm.description" :placeholder="tr('Ingredients and description', 'Përbërësit dhe përshkrimi')" class="rounded border border-emerald-700 bg-emerald-950 px-3 py-2 md:col-span-2"/><div class="flex gap-2"><select v-model.number="menuForm.categoryId" required class="min-w-0 flex-1 rounded border border-emerald-700 bg-emerald-950 px-3 py-2"><option :value="0" disabled>{{ tr('Choose category', 'Zgjidh kategorinë') }}</option><option v-for="category in categories" :key="category.id" :value="category.id">{{ category.name }}</option></select><button class="rounded bg-yellow-500 px-4 py-2 font-semibold text-emerald-950">{{ editingProductId ? tr('Save changes', 'Ruaj ndryshimet') : tr('Add dish', 'Shto pjatën') }}</button><button v-if="editingProductId" type="button" @click="resetMenuForm" class="rounded border border-emerald-700 px-3 py-2">{{ tr('Cancel', 'Anulo') }}</button></div></form>
        <form @submit.prevent="createCategory" class="mt-3 flex gap-2"><input v-model="newCategory" required :placeholder="tr('Create a category', 'Krijo kategori')" class="min-w-0 flex-1 rounded border border-emerald-700 bg-emerald-950 px-3 py-2"/><button class="rounded border border-emerald-700 px-3 py-2 text-sm">{{ tr('Add category', 'Shto kategori') }}</button></form>
        <div class="mt-4 grid gap-2 md:grid-cols-2"><article v-for="product in menuProducts" :key="product.id" class="flex items-center justify-between gap-3 rounded-lg bg-emerald-950/70 p-3"><div class="min-w-0"><p class="font-semibold">{{ product.name }} <span class="text-yellow-300">€{{ product.price.toFixed(2) }}</span><span class="ml-2 text-xs text-emerald-300">{{ product.prepMinutes }} min</span></p><p class="truncate text-xs text-emerald-200">{{ product.description || product.categoryName }}</p></div><button @click="editProduct(product)" class="shrink-0 rounded border border-emerald-700 px-3 py-1 text-sm">{{ tr('Edit', 'Ndrysho') }}</button></article></div>
        </div>
        </details>
      </section>

      <div class="order-1 space-y-4">
        <div v-for="o in filteredOrders" :key="o.id" class="rounded-lg border border-yellow-500/30 p-4 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)]">
          <div class="flex justify-between items-center">
            <div>
              <p class="font-semibold">{{ tableName(o.tableNumber) }} <span class="text-sm font-normal text-emerald-200">· {{ tableArea(o.tableNumber) }}</span></p>
              <p class="text-sm text-emerald-200">#{{ o.id }}</p>
            </div>
            <div class="flex items-center gap-2">
              <span class="mr-2" :class="statusClass(o.status)">{{ o.status }}</span>
              <button @click="updateStatus(o.id, 'Preparing')" v-if="o.status === 'Pending'" class="px-3 py-1 rounded bg-yellow-400 text-emerald-950 font-semibold shadow">{{ tr('Start Preparing', 'Fillo përgatitjen') }} · {{ plannedPrepMinutes(o) }}m</button>
              <button v-if="o.status === 'Preparing' && !o.preparationStartedAtUtc" @click="updateStatus(o.id, 'Preparing')" class="rounded border border-slate-500 px-2 py-1 text-xs">{{ tr('Start timer', 'Nis kohëmatësin') }}</button>
              <span v-if="o.status === 'Preparing' && o.preparationStartedAtUtc" class="rounded bg-slate-950 px-2 py-1 font-mono text-sm" :class="remainingSeconds(o) <= 0 ? 'text-rose-300' : 'text-emerald-200'">{{ timerLabel(o) }}</span>
              <button @click="extendTimer(o.id, 5)" v-if="o.status === 'Preparing' && o.preparationStartedAtUtc" class="rounded border border-slate-500 px-2 py-1 text-xs">+5m</button>
              <button @click="extendTimer(o.id, 10)" v-if="o.status === 'Preparing' && o.preparationStartedAtUtc" class="rounded border border-slate-500 px-2 py-1 text-xs">+10m</button>
              <button @click="updateStatus(o.id, 'Ready')" v-if="o.status === 'Preparing'" class="px-3 py-1 rounded bg-emerald-400 text-emerald-950 font-semibold shadow">{{ tr('Mark Ready', 'Shëno gati') }}</button>
              <button @click="updateStatus(o.id, 'Done')" v-if="o.status === 'Preparing'" class="px-3 py-1 rounded border border-green-400 text-green-200 font-semibold">{{ tr('Finish early', 'Përfundo para kohe') }}</button>
              <button @click="updateStatus(o.id, 'Done')" v-if="o.status === 'Ready'" class="px-3 py-1 rounded bg-green-400 text-emerald-950 font-semibold shadow">{{ tr('Mark Done', 'Shëno përfunduar') }}</button>
              <button @click="updateStatus(o.id, 'Canceled')" v-if="o.status !== 'Done' && o.status !== 'Canceled'" class="px-3 py-1 rounded bg-red-400 text-emerald-950 font-semibold shadow">{{ tr('Cancel', 'Anulo') }}</button>
            </div>
          </div>
          <ul class="mt-3 text-sm">
            <li v-for="i in o.items" :key="i.productId">{{ i.productName }} x{{ i.quantity }}</li>
          </ul>
        </div>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import PreferencesControls from '../components/PreferencesControls.vue'
import { useRouter } from 'vue-router'
import { signOut } from '../store/auth'
import api from '../api/axios'
import { diningAreas, diningTables, loadFloorPlan } from '../store/tables'
import { tr } from '../store/preferences'
interface OrderItemDto { productId: number; quantity: number; productName?: string; prepMinutes?: number }
interface OrderDto { id: number; tableNumber: number; status: string; items: OrderItemDto[]; waiterName?: string; preparationStartedAtUtc?: string | null; timerExtensionMinutes: number }
interface MenuProduct { id: number; name: string; description: string; imageUrl: string; price: number; unitCost: number; prepMinutes: number; categoryName: string }
interface MenuCategory { id: number; name: string }

const orders = ref<OrderDto[]>([])
const statusFilter = ref<'all' | 'Pending' | 'Preparing' | 'Ready'>('all')
const areaFilter = ref('all')
const menuProducts = ref<MenuProduct[]>([]), categories = ref<MenuCategory[]>([])
const menuForm = ref({ id: 0, name: '', description: '', imageUrl: '', price: 0, unitCost: 0, prepMinutes: 10, categoryId: 0 })
const editingProductId = ref(0), newCategory = ref(''), menuMessage = ref(''), menuError = ref(false)
const currentTime = ref(Date.now())
const router = useRouter()
const areas = computed(() => diningAreas.value.map(area => area.name))

function closeFilter(event: MouseEvent) {
  (event.currentTarget as HTMLElement).closest('details')?.removeAttribute('open')
}

const filteredOrders = computed(() => {
  return orders.value.filter(o => (statusFilter.value === 'all' || o.status === statusFilter.value) && (areaFilter.value === 'all' || tableArea(o.tableNumber) === areaFilter.value))
})

function tableName(id: number) { return diningTables.value.find(t => t.id === id)?.name || `Table ${id}` }
function tableArea(id: number) { const areaId = diningTables.value.find(t => t.id === id)?.areaId; return diningAreas.value.find(area => area.id === areaId)?.name || 'Other area' }

let clockHandle = 0
let floorSyncHandle = 0
let orderSyncHandle = 0
onMounted(() => { void load(); void loadMenu(); void loadFloorPlan(); clockHandle = window.setInterval(() => { currentTime.value = Date.now() }, 1000); floorSyncHandle = window.setInterval(() => { void loadFloorPlan() }, 15000); orderSyncHandle = window.setInterval(() => { void load() }, 10000) })
onUnmounted(() => { window.clearInterval(clockHandle); window.clearInterval(floorSyncHandle); window.clearInterval(orderSyncHandle) })

async function load() {
  const { data } = await api.get<OrderDto[]>('/Orders')
  orders.value = data
}

async function loadMenu() {
  try { const [p, c] = await Promise.all([api.get<MenuProduct[]>('/Products'), api.get<MenuCategory[]>('/Categories')]); menuProducts.value = p.data; categories.value = c.data }
  catch (error) { console.error('Could not load menu', error) }
}
function editProduct(product: MenuProduct) { editingProductId.value = product.id; menuForm.value = { id: product.id, name: product.name, description: product.description || '', imageUrl: product.imageUrl || '', price: product.price, unitCost: product.unitCost || 0, prepMinutes: product.prepMinutes || 10, categoryId: categories.value.find(category => category.name === product.categoryName)?.id || 0 } }
function resetMenuForm() { editingProductId.value = 0; menuForm.value = { id: 0, name: '', description: '', imageUrl: '', price: 0, unitCost: 0, prepMinutes: 10, categoryId: 0 } }
async function saveProduct() { try { const { id, ...payload } = menuForm.value; if (editingProductId.value) await api.put(`/Products/${id}`, payload); else await api.post('/Products', payload); menuError.value = false; menuMessage.value = tr('Menu item saved.', 'Artikulli u ruajt.'); resetMenuForm(); await loadMenu() } catch { menuError.value = true; menuMessage.value = tr('Could not save this menu item.', 'Artikulli nuk u ruajt.') } }
async function createCategory() { try { await api.post('/Categories', { name: newCategory.value.trim() }); newCategory.value = ''; await loadMenu(); menuError.value = false; menuMessage.value = tr('Category added.', 'Kategoria u shtua.') } catch { menuError.value = true; menuMessage.value = tr('Could not add category.', 'Kategoria nuk u shtua.') } }
async function seedMenu() { try { const { data } = await api.post<{ added: number }>('/Products/seed-menu'); menuError.value = false; menuMessage.value = tr(`Added ${data.added} dishes.`, `U shtuan ${data.added} pjata.`); await loadMenu() } catch { menuError.value = true; menuMessage.value = tr('Could not load the sample menu.', 'Nuk u ngarkua menuja shembull.') } }

async function updateStatus(id: number, status: 'Pending' | 'Preparing' | 'Ready' | 'Done' | 'Canceled') {
  try {
    const { data } = await api.put(`/Orders/${id}/status`, { status })
    const order = orders.value.find(o => o.id === id)
    if (order) {
      order.status = status
      if (status === 'Preparing') { order.preparationStartedAtUtc = data.preparationStartedAtUtc; order.timerExtensionMinutes = data.timerExtensionMinutes }
    }
  } catch (error) {
    console.error('Failed to update status:', error)
  }
}

function plannedPrepMinutes(order: OrderDto) { return Math.max(1, ...order.items.map(item => item.prepMinutes || menuProducts.value.find(product => product.id === item.productId)?.prepMinutes || 10)) }
function remainingSeconds(order: OrderDto) { if (!order.preparationStartedAtUtc) return 0; const started = order.preparationStartedAtUtc; const utcValue = /(?:Z|[+-]\d{2}:\d{2})$/i.test(started) ? started : `${started}Z`; const dueAt = new Date(utcValue).getTime() + (plannedPrepMinutes(order) + order.timerExtensionMinutes) * 60000; return Math.ceil((dueAt - currentTime.value) / 1000) }
function timerLabel(order: OrderDto) { const seconds = Math.abs(remainingSeconds(order)); const value = `${Math.floor(seconds / 60).toString().padStart(2, '0')}:${(seconds % 60).toString().padStart(2, '0')}`; return remainingSeconds(order) < 0 ? `${tr('Overdue', 'Me vonesë')} ${value}` : value }
async function extendTimer(id: number, minutes: number) { try { const { data } = await api.post<{ timerExtensionMinutes: number }>(`/Orders/${id}/timer-extension`, { minutes }); const order = orders.value.find(row => row.id === id); if (order) order.timerExtensionMinutes = data.timerExtensionMinutes } catch (error) { console.error('Could not extend timer', error) } }

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
