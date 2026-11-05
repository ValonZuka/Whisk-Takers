import { nextTick, ref, watch } from 'vue'
import api from '../api/axios'
import { getCurrentUser } from './auth'

export interface DiningArea { id: string; name: string; color: string }
export interface DiningTable { id: number; name: string; areaId: string; seats: number; x: number; y: number; width: number; height: number }

const defaultAreas: DiningArea[] = [
  { id: 'main-room', name: 'Main room', color: '#0f766e' },
  { id: 'terrace', name: 'Terrace', color: '#256d4d' },
  { id: 'private-room', name: 'Private room', color: '#6d4b8e' }
]
const defaultTables: DiningTable[] = Array.from({ length: 12 }, (_, i) => ({
  id: i + 1,
  name: `Table ${i + 1}`,
  areaId: i < 6 ? 'main-room' : i < 9 ? 'terrace' : 'private-room',
  seats: 4,
  x: 3 + (i % 3) * 30,
  y: 24 + Math.floor((i % 6) / 3) * 140,
  width: 132,
  height: 96
}))

function readAreas(): DiningArea[] {
  try {
    const saved = localStorage.getItem('whisk-areas')
    const parsed = saved ? JSON.parse(saved) : null
    return Array.isArray(parsed) && parsed.length ? parsed : defaultAreas
  } catch { return defaultAreas }
}
function readTables(): DiningTable[] {
  try {
    const saved = localStorage.getItem('whisk-tables')
    const parsed = saved ? JSON.parse(saved) : null
    if (!Array.isArray(parsed) || !parsed.length) return defaultTables
    // Migrate the previous area-name table format into movable floor-plan records.
    const legacy = parsed.some((table: Record<string, unknown>) => !table.areaId)
    return parsed.map((table: Record<string, unknown>, index: number) => {
      if (legacy) {
        const areaName = String(table.area || 'Main room')
        let area = diningAreas.value.find(candidate => candidate.name === areaName)
        if (!area) {
          area = { id: `area-${index + 1}`, name: areaName, color: '#0f766e' }
          diningAreas.value.push(area)
        }
        return { ...table, areaId: area.id, x: 3 + (index % 4) * 24, y: 24 + Math.floor(index / 4) * 130, width: 132, height: 96 } as DiningTable
      }
      return { width: 132, height: 96, ...table } as DiningTable
    })
  } catch { return defaultTables }
}

export const diningAreas = ref<DiningArea[]>(readAreas())
export const diningTables = ref<DiningTable[]>([])
export const floorPlanSync = ref<'loading' | 'saved' | 'offline' | 'error'>('loading')
diningTables.value = readTables()
let nextTableId = Math.max(Number(localStorage.getItem('whisk-next-table-id')) || 1, ...diningTables.value.map(table => table.id + 1))
function reserveTableId() { const id = nextTableId++; localStorage.setItem('whisk-next-table-id', String(nextTableId)); return id }
watch(diningAreas, value => localStorage.setItem('whisk-areas', JSON.stringify(value)), { deep: true, immediate: true })
watch(diningTables, value => localStorage.setItem('whisk-tables', JSON.stringify(value)), { deep: true, immediate: true })

let serverReady = false
let saveTimer: ReturnType<typeof setTimeout> | undefined
watch([diningAreas, diningTables], () => {
  if (!serverReady || getCurrentUser()?.role !== 'admin') return
  if (saveTimer) clearTimeout(saveTimer)
  saveTimer = setTimeout(() => { void saveFloorPlan() }, 500)
}, { deep: true })

export async function loadFloorPlan() {
  serverReady = false
  floorPlanSync.value = 'loading'
  try {
    const { data } = await api.get<{ areas: DiningArea[]; tables: DiningTable[] }>('/FloorPlan')
    if (Array.isArray(data.areas) && data.areas.length && Array.isArray(data.tables)) {
      diningAreas.value = data.areas
      diningTables.value = data.tables
      nextTableId = Math.max(nextTableId, ...diningTables.value.map(table => table.id + 1))
      localStorage.setItem('whisk-next-table-id', String(nextTableId))
    }
    await nextTick()
    serverReady = true
    floorPlanSync.value = 'saved'
  } catch (error: any) {
    await nextTick()
    serverReady = error?.response?.status === 404
    if (serverReady && getCurrentUser()?.role === 'admin') await saveFloorPlan()
    else if (serverReady) floorPlanSync.value = 'offline'
    else { floorPlanSync.value = 'error'; console.warn('Floor plan sync unavailable; using this device’s saved plan.') }
  }
}

export function createDiningTable(areaId: string, x: number, y: number): DiningTable {
  const id = reserveTableId()
  return { id, name: `Table ${id}`, areaId, seats: 4, x, y, width: 132, height: 96 }
}

export async function saveFloorPlan() {
  if (getCurrentUser()?.role !== 'admin') return false
  if (saveTimer) clearTimeout(saveTimer)
  try {
    const tables = diningTables.value.map(table => ({
      ...table,
      width: Math.round(table.width),
      height: Math.round(table.height)
    }))
    await api.put('/FloorPlan', { areas: diningAreas.value, tables })
    floorPlanSync.value = 'saved'
    return true
  } catch (error) {
    floorPlanSync.value = 'error'
    console.warn('Could not sync floor plan with the server.', error)
    return false
  }
}
