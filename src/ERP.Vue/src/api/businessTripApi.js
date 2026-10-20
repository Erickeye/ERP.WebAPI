import http from './http'

export const businessTripApi = {
  index(params) {
    return http.get('/_1060BusinessTrip/Index', { params })
  },
  get(id) {
    return http.get('/_1060BusinessTrip/Get', { params: { id } })
  },
  getCurrentStaff() {
    return http.get('/_1060BusinessTrip/GetCurrentStaff')
  },
  add(data) {
    return http.post('/_1060BusinessTrip/Add', data)
  },
  edit(data) {
    return http.post('/_1060BusinessTrip/Edit', data)
  },
  delete(id) {
    return http.delete('/_1060BusinessTrip/Delete', { params: { id } })
  },
  // 送出簽核：簽核引擎以單號識別單據，tableType 由後端指定為出差單
  sendApproval(no) {
    return http.post('/_1060BusinessTrip/SendApproval', { tableId: no })
  },
  // 核准：memo 選填（簽核意見）
  approve(no, memo) {
    return http.post('/_1060BusinessTrip/Approval', { tableId: no, memo: memo || null })
  },
  // 拒絕：memo 為拒絕原因，必填（後端引擎也會檢查）
  reject(no, memo) {
    return http.post('/_1060BusinessTrip/RejectApproval', { tableId: no, memo })
  },
  // 撤銷：建單人收回簽核中的簽核，成功後單據變成已退回
  revoke(no) {
    return http.post('/_1060BusinessTrip/RevokeApproval', { tableId: no })
  },
  // 作廢：最後一關簽核人終止已核准的出差單，成功後單據變成已作廢
  voidTrip(no) {
    return http.post('/_1060BusinessTrip/VoidApproval', { tableId: no })
  }
}
