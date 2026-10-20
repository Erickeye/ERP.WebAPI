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
