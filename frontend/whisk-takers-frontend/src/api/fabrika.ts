import api from './axios'

export interface FabrikaDto { 
  id: number
  emriFabrikes: string
  adresaFabrikes: string
  puntori: PuntoriDto[]
}

export interface PuntoriDto {
  id: number
  emri: string
  mbiemri: string
  pozita: string
  fabrikaId: number
  fabrikaEmri?: string
}

export interface FabrikaCreateDto {
  emriFabrikes: string
  adresaFabrikes: string
}

export interface PuntoriCreateDto {
  emri: string
  mbiemri: string
  pozita: string
  fabrikaId: number
}

export async function fetchFabrika() {
  const { data } = await api.get<FabrikaDto[]>('/Fabrika')
  return data
}

export async function fetchFabrikaById(id: number) {
  const { data } = await api.get<FabrikaDto>(`/Fabrika/${id}`)
  return data
}

export async function createFabrika(fabrika: FabrikaCreateDto) {
  const { data } = await api.post<FabrikaDto>('/Fabrika', fabrika)
  return data
}

export async function updateFabrika(id: number, fabrika: FabrikaCreateDto) {
  const { data } = await api.put(`/Fabrika/${id}`, fabrika)
  return data
}

export async function deleteFabrika(id: number) {
  const { data } = await api.delete(`/Fabrika/${id}`)
  return data
}

export async function seedFabrika() {
  const { data } = await api.post('/Fabrika/seed')
  return data
}
