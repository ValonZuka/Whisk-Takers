export type UserRole = 'client' | 'waiter' | 'chef' | 'admin'

export interface AuthTokenPayload {
  name: string
  role: UserRole
  email?: string
}

const STORAGE_KEY = 'whisk_auth_token'

export function encodeToken(payload: AuthTokenPayload): string {
  return btoa(JSON.stringify(payload))
}

export function decodeToken(token: string | null): AuthTokenPayload | null {
  if (!token) return null
  try {
    return JSON.parse(atob(token)) as AuthTokenPayload
  } catch {
    return null
  }
}

export function getToken(): string | null {
  return localStorage.getItem(STORAGE_KEY)
}

export function setToken(token: string) {
  localStorage.setItem(STORAGE_KEY, token)
}

export function clearToken() {
  localStorage.removeItem(STORAGE_KEY)
}

export function getCurrentUser(): AuthTokenPayload | null {
  const token = getToken()
  if (!token) return null
  
  try {
    // Parse JWT token payload (simple base64 decode of payload part)
    const parts = token.split('.')
    if (parts.length !== 3) return null
    
    const payload = JSON.parse(atob(parts[1]))
    return {
      name: payload.name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 'Unknown',
      role: payload.role || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] || 'client',
      email: payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || ''
    }
  } catch {
    return null
  }
}

export function signOut() {
  clearToken()
}


