import http from './http'

export const contractApi = {
  index(params) {
    return http.get('/_3050Contract/Index', { params })
  },
  get(id) {
    return http.get('/_3050Contract/Get', { params: { id } })
  },
  add(data) {
    return http.post('/_3050Contract/Add', data)
  },
  edit(data) {
    return http.post('/_3050Contract/Edit', data)
  },
  delete(id) {
    return http.delete('/_3050Contract/Delete', { params: { id } })
  },
  // 上傳或更換合約掃描檔：multipart/form-data，欄位名稱對照後端 ContractAttachmentVM（Id、File）；
  // 不手動設定 Content-Type，讓瀏覽器建立 boundary
  uploadAttachment(id, file) {
    const formData = new FormData()
    formData.append('Id', id)
    formData.append('File', file)
    return http.post('/_3050Contract/UploadAttachment', formData)
  },
  // 下載合約掃描檔：以 blob 接收；後端失敗時回的是 JSON 的 ResultModel，由 utils/file.js 判斷
  downloadAttachment(id) {
    return http.get('/_3050Contract/DownloadAttachment', { params: { id }, responseType: 'blob' })
  },
  // 送出簽核：簽核引擎以單號識別單據，tableType 由後端指定為合約單
  sendApproval(no) {
    return http.post('/_3050Contract/SendApproval', { tableId: no })
  },
  // 核准：memo 選填（簽核意見）
  approve(no, memo) {
    return http.post('/_3050Contract/Approval', { tableId: no, memo: memo || null })
  },
  // 拒絕：memo 為拒絕原因，必填（後端引擎也會檢查）
  reject(no, memo) {
    return http.post('/_3050Contract/RejectApproval', { tableId: no, memo })
  },
  // 撤銷：送簽人收回簽核中的簽核，成功後單據變成已退回
  revoke(no) {
    return http.post('/_3050Contract/RevokeApproval', { tableId: no })
  },
  // 作廢：最後一關簽核人終止已核准的合約單，成功後單據變成已作廢
  voidContract(no) {
    return http.post('/_3050Contract/VoidApproval', { tableId: no })
  }
}
