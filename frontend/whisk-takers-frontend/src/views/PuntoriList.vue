<template>
  <div class="min-h-screen bg-emerald-950 text-emerald-50">
    <div class="max-w-6xl mx-auto px-4 py-10">
      <div class="flex items-center justify-between mb-6">
        <div class="flex items-center gap-4">
          <router-link 
            to="/" 
            class="text-emerald-300 hover:text-emerald-200 transition"
          >
            ← Back to Home
          </router-link>
          <h1 class="text-3xl font-bold text-yellow-300">Puntori Management</h1>
        </div>
        <div class="flex gap-2">
          <button 
            @click="showCreateForm = true" 
            class="rounded bg-emerald-600 text-white font-semibold px-4 py-2 hover:bg-emerald-700 transition"
          >
            Add Puntori
          </button>
          <button 
            @click="seedData" 
            class="rounded bg-yellow-600 text-white font-semibold px-4 py-2 hover:bg-yellow-700 transition"
          >
            Seed Data
          </button>
          <button 
            @click="loadPuntori" 
            class="rounded bg-blue-600 text-white font-semibold px-4 py-2 hover:bg-blue-700 transition"
          >
            Refresh
          </button>
        </div>
      </div>

      <!-- Filters -->
      <div class="mb-6 flex gap-4 items-center">
        <div>
          <label class="block text-emerald-200 text-sm font-medium mb-2">Filter by Fabrika:</label>
          <select 
            v-model="selectedFabrikaId" 
            @change="loadPuntori"
            class="px-3 py-2 bg-emerald-800 border border-emerald-600 rounded-md text-emerald-50 focus:outline-none focus:ring-2 focus:ring-yellow-500"
          >
            <option value="">All Fabrika</option>
            <option v-for="fabrika in fabrika" :key="fabrika.id" :value="fabrika.id">
              {{ fabrika.emriFabrikes }}
            </option>
          </select>
        </div>
      </div>

      <!-- Create/Edit Form Modal -->
      <div v-if="showCreateForm || editingPuntori" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div class="bg-emerald-900 p-6 rounded-lg w-96 max-w-full mx-4">
          <h2 class="text-xl font-bold text-yellow-300 mb-4">
            {{ editingPuntori ? 'Edit Puntori' : 'Create New Puntori' }}
          </h2>
          
          <form @submit.prevent="savePuntori" class="space-y-4">
            <div>
              <label class="block text-emerald-200 text-sm font-medium mb-2">Emri</label>
              <input 
                v-model="formData.emri"
                type="text" 
                required
                class="w-full px-3 py-2 bg-emerald-800 border border-emerald-600 rounded-md text-emerald-50 focus:outline-none focus:ring-2 focus:ring-yellow-500"
                placeholder="Enter first name"
              />
            </div>
            
            <div>
              <label class="block text-emerald-200 text-sm font-medium mb-2">Mbiemri</label>
              <input 
                v-model="formData.mbiemri"
                type="text" 
                required
                class="w-full px-3 py-2 bg-emerald-800 border border-emerald-600 rounded-md text-emerald-50 focus:outline-none focus:ring-2 focus:ring-yellow-500"
                placeholder="Enter last name"
              />
            </div>
            
            <div>
              <label class="block text-emerald-200 text-sm font-medium mb-2">Pozita</label>
              <input 
                v-model="formData.pozita"
                type="text" 
                required
                class="w-full px-3 py-2 bg-emerald-800 border border-emerald-600 rounded-md text-emerald-50 focus:outline-none focus:ring-2 focus:ring-yellow-500"
                placeholder="Enter position"
              />
            </div>
            
            <div>
              <label class="block text-emerald-200 text-sm font-medium mb-2">Fabrika</label>
              <select 
                v-model="formData.fabrikaId"
                required
                class="w-full px-3 py-2 bg-emerald-800 border border-emerald-600 rounded-md text-emerald-50 focus:outline-none focus:ring-2 focus:ring-yellow-500"
              >
                <option value="">Select Fabrika</option>
                <option v-for="fabrika in fabrika" :key="fabrika.id" :value="fabrika.id">
                  {{ fabrika.emriFabrikes }}
                </option>
              </select>
            </div>
            
            <div class="flex gap-2 pt-4">
              <button 
                type="submit" 
                class="flex-1 bg-emerald-600 text-white font-semibold py-2 px-4 rounded-md hover:bg-emerald-700 transition"
              >
                {{ editingPuntori ? 'Update' : 'Create' }}
              </button>
              <button 
                type="button" 
                @click="cancelForm"
                class="flex-1 bg-gray-600 text-white font-semibold py-2 px-4 rounded-md hover:bg-gray-700 transition"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Puntori List -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div 
          v-for="puntori in puntori" 
          :key="puntori.id"
          class="rounded-lg border border-yellow-500/30 p-6 bg-emerald-900/60 shadow-[0_0_25px_rgba(16,185,129,0.15)] hover:shadow-[0_0_35px_rgba(16,185,129,0.25)] transition-all"
        >
          <div class="flex justify-between items-start mb-4">
            <div>
              <h3 class="text-xl font-bold text-yellow-300">{{ puntori.emri }} {{ puntori.mbiemri }}</h3>
              <p class="text-emerald-300">{{ puntori.pozita }}</p>
            </div>
            <div class="flex gap-2">
              <button 
                @click="editPuntori(puntori)"
                class="text-blue-400 hover:text-blue-300 transition"
                title="Edit"
              >
                ✏️
              </button>
              <button 
                @click="deletePuntori(puntori.id)"
                class="text-red-400 hover:text-red-300 transition"
                title="Delete"
              >
                🗑️
              </button>
            </div>
          </div>
          
          <div class="border-t border-emerald-700 pt-4">
            <div class="flex items-center gap-2">
              <span class="text-sm text-emerald-300">🏭</span>
              <span class="text-emerald-200">{{ puntori.fabrikaEmri || 'Unknown Fabrika' }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="puntori.length === 0" class="text-center py-12">
        <div class="text-6xl mb-4">👷</div>
        <h3 class="text-xl font-semibold text-emerald-300 mb-2">No Puntori Found</h3>
        <p class="text-emerald-400 mb-4">Get started by creating your first puntori or seeding sample data.</p>
        <button 
          @click="seedData" 
          class="rounded bg-yellow-600 text-white font-semibold px-6 py-2 hover:bg-yellow-700 transition"
        >
          Seed Sample Data
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { fetchPuntori, createPuntori, updatePuntori, deletePuntori, seedPuntori, fetchPuntoriByFabrika, type PuntoriDto, type PuntoriCreateDto } from '../api/puntori'
import { fetchFabrika, type FabrikaDto } from '../api/fabrika'

const puntori = ref<PuntoriDto[]>([])
const fabrika = ref<FabrikaDto[]>([])
const showCreateForm = ref(false)
const editingPuntori = ref<PuntoriDto | null>(null)
const selectedFabrikaId = ref<number | ''>('')
const formData = ref<PuntoriCreateDto>({
  emri: '',
  mbiemri: '',
  pozita: '',
  fabrikaId: 0
})

async function loadPuntori() {
  try {
    if (selectedFabrikaId.value) {
      puntori.value = await fetchPuntoriByFabrika(selectedFabrikaId.value as number)
    } else {
      puntori.value = await fetchPuntori()
    }
  } catch (error) {
    console.error('Error loading puntori:', error)
    alert('Error loading puntori. Please make sure the backend server is running and the database is set up.')
  }
}

async function loadFabrika() {
  try {
    fabrika.value = await fetchFabrika()
  } catch (error) {
    console.error('Error loading fabrika:', error)
  }
}

async function savePuntori() {
  try {
    if (editingPuntori.value) {
      await updatePuntori(editingPuntori.value.id, formData.value)
    } else {
      await createPuntori(formData.value)
    }
    
    await loadPuntori()
    cancelForm()
  } catch (error) {
    console.error('Error saving puntori:', error)
  }
}

function editPuntori(puntori: PuntoriDto) {
  editingPuntori.value = puntori
  formData.value = {
    emri: puntori.emri,
    mbiemri: puntori.mbiemri,
    pozita: puntori.pozita,
    fabrikaId: puntori.fabrikaId
  }
}

async function deletePuntori(id: number) {
  if (confirm('Are you sure you want to delete this puntori?')) {
    try {
      await deletePuntori(id)
      await loadPuntori()
    } catch (error) {
      console.error('Error deleting puntori:', error)
    }
  }
}

async function seedData() {
  try {
    await seedPuntori()
    await loadPuntori()
    alert('Sample data seeded successfully!')
  } catch (error) {
    console.error('Error seeding data:', error)
    alert('Error seeding data. Please make sure the backend server is running and the database is set up.')
  }
}

function cancelForm() {
  showCreateForm.value = false
  editingPuntori.value = null
  formData.value = {
    emri: '',
    mbiemri: '',
    pozita: '',
    fabrikaId: 0
  }
}

onMounted(async () => {
  await loadFabrika()
  await loadPuntori()
})
</script>
