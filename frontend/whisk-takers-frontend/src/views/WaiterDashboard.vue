<template>
  <main class="min-h-screen overflow-x-hidden bg-emerald-950 text-emerald-50">
    <header class="flex flex-wrap items-center justify-between gap-3 border-b border-emerald-800 px-4 py-3 sm:px-6">
      <div><p class="text-xs uppercase tracking-widest text-emerald-300">Whisk Takers</p><h1 class="text-xl font-bold text-yellow-300">{{ tr('Floor & tables', 'Salla dhe tavolinat') }}</h1></div>
      <div class="flex flex-wrap items-center gap-3"><PreferencesControls /><span class="hidden text-sm text-emerald-300 sm:inline">{{ currentUser?.name }}</span><button @click="logout" class="rounded-lg bg-yellow-500 px-3 py-2 text-sm font-semibold text-emerald-950">{{ tr('Sign out', 'Dil') }}</button></div>
    </header>
    <section class="px-3 py-3 sm:px-6">
      <div class="mb-3 flex flex-wrap items-center justify-between gap-2"><div><h2 class="text-lg font-semibold">{{ areaDisplayName(selectedArea?.name) }}</h2><p class="text-xs text-emerald-300">{{ tr('Choose a table to start an order.', 'Zgjidh tavolinën për të nisur porosinë.') }} <span v-if="floorPlanSync === 'error'" class="text-rose-300">{{ tr('Layout sync error', 'Gabim në sinkronizimin e planit') }}</span></p></div><div class="flex flex-wrap gap-2"><button v-for="area in diningAreas" :key="area.id" @click="selectedAreaId = area.id" class="rounded-full border px-4 py-2 text-sm font-semibold" :class="selectedAreaId === area.id ? 'border-yellow-300 bg-yellow-400 text-emerald-950' : 'border-emerald-700 bg-emerald-900/60 text-emerald-100'"><span class="mr-2 inline-block h-2.5 w-2.5 rounded-full" :style="{backgroundColor:area.color}"></span>{{ areaDisplayName(area.name) }}</button></div></div>
      <div class="relative h-[clamp(480px,calc(100dvh-165px),780px)] overflow-hidden rounded-2xl border" :style="{ borderColor: selectedArea?.color, backgroundColor: `${selectedArea?.color || '#0f766e'}20`, backgroundImage: 'radial-gradient(#ffffff30 1px, transparent 1px)', backgroundSize: '24px 24px' }">
        <button v-for="table in areaTables" :key="table.id" @click="selectTable(table.id)" class="absolute flex flex-col items-center justify-center rounded-2xl border-2 px-2 text-center shadow-lg transition hover:scale-105 focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-yellow-300" :class="tableId === table.id ? 'border-yellow-200 bg-yellow-400 text-emerald-950' : isBusy(table.id) ? 'border-amber-300 bg-emerald-900 text-amber-100' : 'border-white/50 text-white'" :style="{ left: `${table.x}%`, top: `${table.y}px`, width: `${table.width}px`, height: `${table.height}px`, backgroundColor: tableId === table.id ? undefined : isBusy(table.id) ? '#064e3b' : selectedArea?.color, zIndex: 2 }">
          <span class="font-bold">{{ displayTableName(table.name) }}</span><span class="mt-1 text-xs opacity-80">{{ table.seats }} {{ tr('seats', 'vende') }}</span><span class="mt-1 text-[10px] opacity-70">{{ isBusy(table.id) ? tr('In use', 'Në përdorim') : tr('Available', 'E lirë') }}</span>
        </button>
        <p v-if="!areaTables.length" class="p-6 text-sm text-emerald-200">{{ tr('No tables in this area yet.', 'Nuk ka tavolina në këtë hapësirë.') }}</p>
      </div>
    </section>

    <section class="px-3 pb-8 sm:px-6">
      <div class="rounded-2xl border border-emerald-800 bg-emerald-900/50 p-4 shadow-lg sm:p-6">
        <div class="mb-5">
          <h2 class="text-xl font-bold text-yellow-200">{{ tr('My orders', 'Porositë e mia') }}</h2>
          <p class="mt-1 text-sm text-emerald-200">{{ tr('Your order history and today’s completed sales.', 'Historiku i porosive dhe shitjet e përfunduara sot.') }}</p>
        </div>
        <div class="mb-6 grid gap-3 sm:grid-cols-3">
          <article class="rounded-xl border border-emerald-700 bg-emerald-950/80 p-4">
            <p class="text-sm text-emerald-300">{{ tr('Orders today', 'Porosi sot') }}</p>
            <p class="mt-2 text-2xl font-bold text-white">{{ todayOrders.length }}</p>
          </article>
          <article class="rounded-xl border border-emerald-700 bg-emerald-950/80 p-4">
            <p class="text-sm text-emerald-300">{{ tr('Completed today', 'Të përfunduara sot') }}</p>
            <p class="mt-2 text-2xl font-bold text-white">{{ todayCompletedOrders.length }}</p>
          </article>
          <article class="rounded-xl border border-yellow-500/40 bg-emerald-950/80 p-4">
            <p class="text-sm text-emerald-300">{{ tr('Sales realized today', 'Shitjet e realizuara sot') }}</p>
            <p class="mt-2 text-2xl font-bold text-yellow-200">{{ money(todaySales) }}</p>
          </article>
        </div>
        <div v-if="myOrders.length" class="space-y-3">
          <article v-for="order in myOrders" :key="order.id" class="rounded-xl border border-emerald-800 bg-emerald-950/70 p-4">
            <div class="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h3 class="font-semibold text-white">{{ tr('Order', 'Porosia') }} #{{ order.id }} · {{ tableName(order.tableNumber) }}</h3>
                <p class="mt-1 text-xs text-emerald-300">{{ formatOrderDate(order.createdAt) }}</p>
              </div>
              <span class="rounded-full border px-3 py-1 text-sm font-semibold" :class="orderStatusClass(order.status)">{{ orderStatusLabel(order.status) }}</span>
            </div>
            <ul class="mt-3 space-y-1 text-sm text-emerald-100">
              <li v-for="item in order.items" :key="item.productId">{{ item.productName || `#${item.productId}` }} × {{ item.quantity }}</li>
            </ul>
            <p class="mt-3 border-t border-emerald-800 pt-3 text-right font-semibold text-yellow-200">{{ money(orderGrandTotal(order)) }}</p>
          </article>
        </div>
        <p v-else class="rounded-xl border border-dashed border-emerald-700 p-6 text-center text-sm text-emerald-300">{{ tr('Your orders will appear here after you send your first order.', 'Porositë e tua do të shfaqen këtu pasi të dërgosh porosinë e parë.') }}</p>
      </div>
    </section>

    <div v-if="tableId !== null" class="fixed inset-0 z-40 bg-black/50" @click.self="closeOrder"></div>
    <aside class="fixed inset-y-0 right-0 z-50 flex w-full max-w-[480px] flex-col border-l border-emerald-700 bg-emerald-950 shadow-2xl transition-transform duration-300" :class="tableId === null ? 'translate-x-full' : 'translate-x-0'" :aria-hidden="tableId === null" :inert="tableId === null">
      <header class="flex items-center justify-between border-b border-emerald-800 px-5 py-4"><div><p class="text-xs text-emerald-300">{{ areaDisplayName(selectedArea?.name) }}</p><h2 class="text-xl font-bold text-yellow-200">{{ selectedTable ? displayTableName(selectedTable.name) : '' }}</h2></div><button @click="closeOrder" class="rounded-full border border-emerald-700 px-3 py-1 text-xl leading-7 hover:bg-emerald-900" :aria-label="tr('Close order panel', 'Mbyll panelin e porosisë')">×</button></header>
      <div class="flex-1 overflow-y-auto p-4">
        <div class="mb-3 flex items-center justify-between"><h3 class="font-semibold">{{ tr('Menu', 'Menuja') }}</h3><span class="text-xs text-emerald-300">{{ visibleProducts.length }} {{ tr('items', 'artikuj') }}</span></div>
        <div class="mb-4 grid grid-cols-2 gap-2 sm:grid-cols-3" role="group" :aria-label="tr('Filter menu by category', 'Filtro menunë sipas kategorisë')">
          <button v-for="filter in menuFilters" :key="filter.value" type="button" class="flex min-h-12 items-center justify-center gap-2 rounded-xl border px-2 py-2 text-sm font-semibold transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-yellow-300" :class="menuFilter === filter.value ? 'border-yellow-300 bg-yellow-400 text-emerald-950' : 'border-emerald-700 bg-emerald-900/60 text-emerald-100 hover:border-yellow-400'" :aria-pressed="menuFilter === filter.value" @click="menuFilter = filter.value">
            <component :is="filter.icon" class="h-4 w-4 shrink-0" aria-hidden="true" />
            <span>{{ filter.label }}</span>
          </button>
        </div>
        <div class="grid grid-cols-1 gap-2 sm:grid-cols-2">
          <button v-for="p in visibleProducts" :key="p.id" @click="addItem(p)" class="flex min-h-20 items-center gap-3 rounded-xl border border-emerald-800 bg-emerald-900/60 p-3 text-left hover:border-yellow-400"><img :src="photoFor(p.id)" @error="fallbackImage" alt="" class="h-14 w-14 rounded-lg object-cover"/><span class="min-w-0 flex-1"><strong class="block whitespace-normal text-sm text-yellow-100">{{ p.name }}</strong><small class="block truncate text-emerald-300">{{ categoryLabel(p.categoryName) }}</small></span><b class="text-sm">€{{ p.price.toFixed(2) }}</b></button>
        </div>
        <p v-if="!visibleProducts.length" class="py-8 text-center text-sm text-emerald-300">{{ products.length ? tr('No items in this menu filter.', 'Nuk ka artikuj në këtë filtër.') : tr('Menu is loading or empty.', 'Menuja po ngarkohet ose është bosh.') }}</p>
        <section class="mt-6 border-t border-emerald-800 pt-4"><div class="flex items-center justify-between"><h3 class="font-semibold">{{ tr('Order', 'Porosia') }}</h3><span class="text-sm text-emerald-300">{{ orderItems.length }} {{ tr('lines', 'rreshta') }}</span></div>
          <ul v-if="orderItems.length" class="mt-3 space-y-3"><li v-for="item in orderItems" :key="item.productId" class="flex items-center justify-between gap-3 rounded-lg bg-emerald-900/70 p-3"><div class="min-w-0"><p class="truncate font-medium">{{ getProductName(item.productId) }}</p><p class="text-xs text-emerald-300">€{{ (getProductPrice(item.productId) * item.quantity).toFixed(2) }}</p></div><div class="flex items-center gap-2"><button @click="changeQuantity(item.productId, -1)" class="h-8 w-8 rounded bg-emerald-950 text-lg" :aria-label="tr('Remove one', 'Hiq një')">−</button><span class="w-5 text-center">{{ item.quantity }}</span><button @click="changeQuantity(item.productId, 1)" class="h-8 w-8 rounded bg-emerald-950 text-lg" :aria-label="tr('Add one', 'Shto një')">＋</button></div></li></ul>
          <p v-else class="mt-3 rounded-lg border border-dashed border-emerald-700 p-4 text-sm text-emerald-300">{{ tr('Tap a dish to add one portion.', 'Prek pjatën për të shtuar një porcion.') }}</p>
          <div v-if="orderItems.length" class="mt-4 flex justify-between border-t border-emerald-800 pt-3 font-bold"><span>{{ tr('Total', 'Totali') }}</span><span>€{{ orderTotal.toFixed(2) }}</span></div>
          <p v-if="orderError" class="mt-3 text-sm text-rose-300">{{ orderError }}</p><button @click="submitOrder" :disabled="!orderItems.length || submitting" class="mt-4 w-full rounded-xl bg-yellow-400 px-4 py-3 font-bold text-emerald-950 disabled:opacity-50">{{ submitting ? tr('Sending…', 'Duke dërguar…') : tr('Send order to kitchen', 'Dërgo porosinë në kuzhinë') }}</button>
        </section>
      </div>
    </aside>
  </main>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import { CakeSlice, Coffee, Croissant, LayoutGrid, Soup, UtensilsCrossed } from 'lucide-vue-next'
import { useRouter } from 'vue-router'
import { signOut, getCurrentUser } from '../store/auth'
import api from '../api/axios'
import { diningAreas, diningTables, floorPlanSync, loadFloorPlan } from '../store/tables'
import { isAlbanian, tr } from '../store/preferences'
import PreferencesControls from '../components/PreferencesControls.vue'
interface ProductDto { id: number; name: string; description?: string; price: number; categoryName: string; imageUrl?: string }
interface OrderItemDto { productId: number; quantity: number; productName?: string; productPrice?: number }
interface OrderDto { id: number; tableNumber: number; status: string; createdAt: string; items: OrderItemDto[]; waiterName?: string; waiterEmail?: string }
const apiProducts = ref<ProductDto[]>([]), orders = ref<OrderDto[]>([]), orderItems = ref<OrderItemDto[]>([])
const tableId = ref<number | null>(null), selectedAreaId = ref(diningAreas.value[0]?.id || ''), orderError = ref(''), submitting = ref(false)
const currentUser = getCurrentUser(), router = useRouter(), busyTables = ref<Set<number>>(new Set())
watch(diningAreas, areas => { if (!areas.some(area => area.id === selectedAreaId.value)) selectedAreaId.value = areas[0]?.id || '' })
const selectedArea = computed(() => diningAreas.value.find(area => area.id === selectedAreaId.value))
const areaTables = computed(() => diningTables.value.filter(table => table.areaId === selectedAreaId.value))
const selectedTable = computed(() => diningTables.value.find(table => table.id === tableId.value))
const products = computed(() => apiProducts.value)
const menuFilter = ref('all')
const menuCategories = computed(() => [...new Set(products.value.map(product => product.categoryName).filter(Boolean))])
const menuFilters = computed(() => [
  { value: 'all', label: tr('All categories', 'Të gjitha kategoritë'), icon: LayoutGrid },
  ...menuCategories.value.map(category => ({
    value: category,
    label: category === 'Drinks' ? tr('Drinks', 'Pije')
      : category === 'Bakery Specials' ? tr('Bakery Specials', 'Specialitete furre')
      : category === 'Breakfast & Appetizers' ? tr('Breakfast & Appetizers', 'Mëngjes dhe antipasta')
      : category === 'Desserts' ? tr('Desserts', 'Ëmbëlsira')
      : category,
    icon: categoryIcon(category)
  }))
])
const visibleProducts = computed(() => menuFilter.value === 'all'
  ? products.value
  : products.value.filter(product => product.categoryName === menuFilter.value))
function categoryIcon(category: string) {
  const name = category.toLowerCase()
  if (name.includes('drink') || name.includes('beverage')) return Coffee
  if (name.includes('bakery')) return Croissant
  if (name.includes('appetizer') || name.includes('breakfast')) return Soup
  if (name.includes('dessert')) return CakeSlice
  return UtensilsCrossed
}
const orderTotal = computed(() => orderItems.value.reduce((sum, item) => sum + getProductPrice(item.productId) * item.quantity, 0))
function displayTableName(name: string) { return name.replace(/^Table\s+(\d+)$/i, `${tr('Table', 'Tavolina')} $1`) }
function areaDisplayName(name?: string) { if (!name) return tr('Restaurant', 'Restoranti'); const key = name.toLowerCase(); if (key === 'main room') return tr('Main room', 'Salla kryesore'); if (key === 'terrace') return tr('Terrace', 'Tarracë'); if (key === 'private room') return tr('Private room', 'Sallë private'); return name }
function categoryLabel(category: string) { const key = category.toLowerCase(); return key.includes('dessert') ? tr('Desserts', 'Ëmbëlsira') : key.includes('drink') ? tr('Drinks', 'Pije') : key.includes('breakfast') || key.includes('appetizer') ? tr('Breakfast & Appetizers', 'Mëngjes dhe antipasta') : key.includes('bakery') ? tr('Bakery Specials', 'Specialitete furre') : category }
function tableName(id: number) { const name = diningTables.value.find(table => table.id === id)?.name; return name ? displayTableName(name) : `${tr('Table', 'Tavolina')} ${id} · ${tr('archived', 'e arkivuar')}` }
const myOrders = computed(() => orders.value
  .filter(order => !!currentUser?.email && order.waiterEmail?.toLowerCase() === currentUser.email.toLowerCase())
  .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()))
const todayOrders = computed(() => myOrders.value.filter(order => isToday(order.createdAt)))
const todayCompletedOrders = computed(() => todayOrders.value.filter(order => order.status === 'Done'))
const todaySales = computed(() => todayCompletedOrders.value.reduce((sum, order) => sum + orderGrandTotal(order), 0))
function isToday(value: string) { const date = new Date(value); return !Number.isNaN(date.getTime()) && date.toDateString() === new Date().toDateString() }
function formatOrderDate(value: string) { const date = new Date(value); return Number.isNaN(date.getTime()) ? '' : date.toLocaleString(isAlbanian.value ? 'sq-XK' : 'en-GB') }
function money(value: number) { return new Intl.NumberFormat(isAlbanian.value ? 'sq-XK' : 'en-GB', { style: 'currency', currency: 'EUR' }).format(value) }
function orderGrandTotal(order: OrderDto) { return order.items.reduce((sum, item) => sum + (item.productPrice || 0) * item.quantity, 0) }
function orderStatusLabel(status: string) { return status === 'Pending' ? tr('Pending', 'Në pritje') : status === 'Preparing' ? tr('Preparing', 'Në përgatitje') : status === 'Ready' ? tr('Ready', 'Gati') : status === 'Done' ? tr('Completed', 'Përfunduar') : status === 'Canceled' ? tr('Canceled', 'Anuluar') : status }
function orderStatusClass(status: string) { return status === 'Done' ? 'border-emerald-500/50 bg-emerald-900 text-emerald-200' : status === 'Canceled' ? 'border-rose-500/50 bg-rose-950 text-rose-200' : status === 'Ready' ? 'border-sky-500/50 bg-sky-950 text-sky-200' : status === 'Preparing' ? 'border-amber-500/50 bg-amber-950 text-amber-200' : 'border-slate-500/50 bg-slate-900 text-slate-200' }
function photoFor(id: number) { return products.value.find(row => row.id === id)?.imageUrl || '/menu-placeholder.svg' }
function fallbackImage(event: Event) { const image = event.target as HTMLImageElement; image.onerror = null; image.src = '/menu-placeholder.svg' }
function selectTable(id: number) { if (!canEditTable(id)) { orderError.value = tr('Another waiter is handling this active table.', 'Një kamarier tjetër po kujdeset për këtë tavolinë.'); window.alert(orderError.value); return } orderError.value = ''; tableId.value = id }
function closeOrder() { tableId.value = null; orderItems.value = []; orderError.value = '' }
let floorSyncHandle = 0, orderSyncHandle = 0
async function loadOrders() { try { const { data } = await api.get<OrderDto[]>('/Orders'); orders.value = data } catch { /* keep the last loaded order list */ } }
onMounted(async () => { await loadFloorPlan(); try { const { data } = await api.get<ProductDto[]>('/Products'); apiProducts.value = data } catch { orderError.value = tr('Could not load the menu.', 'Menuja nuk u ngarkua.') }; await loadOrders(); floorSyncHandle = window.setInterval(() => { void loadFloorPlan() }, 10000); orderSyncHandle = window.setInterval(() => { void loadOrders() }, 10000) })
onUnmounted(() => { window.clearInterval(floorSyncHandle); window.clearInterval(orderSyncHandle) })
function addItem(p: ProductDto) { const existing = orderItems.value.find(i => i.productId === p.id); if (existing) existing.quantity++; else orderItems.value.push({ productId: p.id, quantity: 1 }) }
function getProductName(id: number) { return products.value.find(p => p.id === id)?.name || `#${id}` }
function getProductPrice(id: number) { return products.value.find(p => p.id === id)?.price || 0 }
function changeQuantity(id: number, change: number) { const row = orderItems.value.find(item => item.productId === id); if (!row) return; row.quantity += change; if (row.quantity <= 0) orderItems.value = orderItems.value.filter(item => item.productId !== id) }
function isBusy(id: number) { return busyTables.value.has(id) || orders.value.some(o => o.tableNumber === id && ['Pending','Preparing','Ready'].includes(o.status)) }
function canEditTable(id: number) { const active = orders.value.find(o => o.tableNumber === id && ['Pending','Preparing','Ready'].includes(o.status)); return !active || active.waiterEmail === currentUser?.email }
async function submitOrder() { if (tableId.value === null || !orderItems.value.length) return; submitting.value = true; orderError.value = ''; const id = tableId.value; try { await api.post('/Orders', { tableNumber: id, items: orderItems.value.map(i => ({ productId: i.productId, quantity: i.quantity })) }); busyTables.value.add(id); closeOrder(); await loadOrders() } catch (error: any) { orderError.value = error.response?.data?.message || tr('Order could not be sent. Please try again.', 'Porosia nuk u dërgua. Provo përsëri.') } finally { submitting.value = false } }
function logout() { signOut(); router.replace({ name: 'Intro' }) }
</script>
