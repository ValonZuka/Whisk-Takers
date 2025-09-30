import api from './axios'

export interface PuntoriDto {
  id: number
  emri: string
  mbiemri: string
  pozita: string
  fabrikaId: number
  fabrikaEmri?: string
}

export interface PuntoriCreateDto {
  emri: string
  mbiemri: string
  pozita: string
  fabrikaId: number
}

export interface PuntoriUpdateDto {
  emri: string
  mbiemri: string
  pozita: string
  fabrikaId: number
}

export async function fetchPuntori() {
  const { data } = await api.get<PuntoriDto[]>('/Puntori')
  return data
}

export async function fetchPuntoriById(id: number) {
  const { data } = await api.get<PuntoriDto>(`/Puntori/${id}`)
  return data
}

export async function fetchPuntoriByFabrika(fabrikaId: number) {
  const { data } = await api.get<PuntoriDto[]>(`/Puntori/by-fabrika/${fabrikaId}`)
  return data
}

export async function createPuntori(puntori: PuntoriCreateDto) {
  const { data } = await api.post<PuntoriDto>('/Puntori', puntori)
  return data
}

export async function updatePuntori(id: number, puntori: PuntoriUpdateDto) {
  const { data } = await api.put(`/Puntori/${id}`, puntori)
  return data
}

export async function deletePuntori(id: number) {
  const { data } = await api.delete(`/Puntori/${id}`)
  return data
}

export async function seedPuntori() {
  const { data } = await api.post('/Puntori/seed')
  return data
}
