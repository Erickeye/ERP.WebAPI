// Token 存取集中在這裡，頁面與 API wrapper 不直接碰 localStorage
const TOKEN_KEY = 'jwt'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token) {
  localStorage.setItem(TOKEN_KEY, token)
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY)
}

// 從 JWT 的 permissions claim（JSON 字串的整數陣列）取出權限代碼；Token 缺漏或格式錯誤時回傳空陣列
export function getPermissions() {
  const token = getToken()
  if (!token) return []

  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const bytes = Uint8Array.from(atob(payload), (char) => char.charCodeAt(0))
    const claims = JSON.parse(new TextDecoder().decode(bytes))
    const permissions = typeof claims.permissions === 'string' ? JSON.parse(claims.permissions) : claims.permissions
    return Array.isArray(permissions) ? permissions.map(Number) : []
  } catch {
    return []
  }
}

export function hasPermission(permission) {
  return getPermissions().includes(permission)
}
