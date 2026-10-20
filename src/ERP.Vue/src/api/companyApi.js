import http from './http'

export const companyApi = {
  index(params) {
    return http.get('/_1080Company/Index', { params })
  },
  get(id) {
    return http.get('/_1080Company/Get', { params: { id } })
  },
  // 填單用的公司選單：只列啟用中；includeId 是單據原本的公司，已停用也會一併回傳（isActive 為 false）
  getSelect(includeId) {
    return http.get('/_1080Company/GetSelect', { params: { includeId } })
  },
  add(data) {
    return http.post('/_1080Company/Add', data)
  },
  edit(data) {
    return http.post('/_1080Company/Edit', data)
  },
  disable(id) {
    return http.post('/_1080Company/Disable', null, { params: { id } })
  },
  enable(id) {
    return http.post('/_1080Company/Enable', null, { params: { id } })
  },
  delete(id) {
    return http.delete('/_1080Company/Delete', { params: { id } })
  }
}
