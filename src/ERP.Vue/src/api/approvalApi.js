import http from './http'

// 通用簽核端點：這裡只提供「進度查詢」。
// 送簽、核准、拒絕、撤銷、作廢會被後端擋下，出差單的簽核動作請走 businessTripApi。
export const approvalApi = {
  getProgress(tableType, tableId) {
    return http.get('/Approval/GetApprovalProgress', { params: { tableType, tableId } })
  }
}
