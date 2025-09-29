import api from './axios'

export interface OrderItemPayload { productId: number; quantity: number }
export interface OrderPayload { tableNumber: number; items: OrderItemPayload[] }

export async function fetchOrders() {
  const { data } = await api.get('/Orders')
  return data
}

export async function createOrder(payload: OrderPayload) {
  const { data } = await api.post('/Orders', payload)
  return data
}

export async function updateOrder(id: number, payload: OrderPayload) {
  await api.put(`/Orders/${id}`, payload)
}


