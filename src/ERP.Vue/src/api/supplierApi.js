import http from './http'

export const supplierApi = {
  index(params) {
    return http.get('/_4060Supplier/Index', { params })
  },
  get(id) {
    return http.get('/_4060Supplier/Get', { params: { id } })
  },
  // 填單用的供應商選單：只列啟用中；includeId 是單據原本的供應商，已停用也會一併回傳（isActive 為 false）
  getSelect(includeId) {
    return http.get('/_4060Supplier/GetSelect', { params: { includeId } })
  },
  add(data) {
    return http.post('/_4060Supplier/Add', data)
  },
  edit(data) {
    return http.post('/_4060Supplier/Edit', data)
  },
  disable(id) {
    return http.post('/_4060Supplier/Disable', null, { params: { id } })
  },
  enable(id) {
    return http.post('/_4060Supplier/Enable', null, { params: { id } })
  },
  delete(id) {
    return http.delete('/_4060Supplier/Delete', { params: { id } })
  }
}
