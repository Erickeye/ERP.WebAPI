import http from './http'

export const supplierPriceApi = {
  index(params) {
    return http.get('/_4061SupplierPrice/Index', { params })
  },
  get(id) {
    return http.get('/_4061SupplierPrice/Get', { params: { id } })
  },
  add(data) {
    return http.post('/_4061SupplierPrice/Add', data)
  },
  edit(data) {
    return http.post('/_4061SupplierPrice/Edit', data)
  },
  delete(id) {
    return http.delete('/_4061SupplierPrice/Delete', { params: { id } })
  }
}
