import api from './axios'

export interface ProductDto { id: number; name: string; description: string; imageUrl?: string; price: number; categoryName: string }

export async function fetchProducts() {
  const { data } = await api.get<ProductDto[]>('/Products')
  return data
}


