import http from './http'

// 共用下拉選單（SharedController）
export const sharedApi = {
  getStaffSelect() {
    return http.get('/Shared/GetStaffSelect')
  },
  getCustomerSelect() {
    return http.get('/Shared/GetCustomerSelect')
  }
}
