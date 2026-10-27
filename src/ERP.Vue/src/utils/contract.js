// 合約單狀態（對應後端 ContractStatus）
export const CONTRACT_STATUS = {
  DRAFT: 0,
  IN_APPROVAL: 1,
  APPROVED: 2,
  REJECTED: 3,
  VOIDED: 4
}

// 列表篩選的狀態選項（值對應後端 ContractStatus）
export const CONTRACT_STATUS_OPTIONS = [
  { value: CONTRACT_STATUS.DRAFT, label: '草稿' },
  { value: CONTRACT_STATUS.IN_APPROVAL, label: '簽核中' },
  { value: CONTRACT_STATUS.APPROVED, label: '已核准' },
  { value: CONTRACT_STATUS.REJECTED, label: '已退回' },
  { value: CONTRACT_STATUS.VOIDED, label: '已作廢' }
]

// 合約類型（對應後端 ContractType）：決定合約對象是供應商還是客戶
export const CONTRACT_TYPE = {
  SUPPLIER: 0,
  CUSTOMER: 1
}

export const CONTRACT_TYPE_OPTIONS = [
  { value: CONTRACT_TYPE.SUPPLIER, label: '供應商合約' },
  { value: CONTRACT_TYPE.CUSTOMER, label: '客戶合約' }
]

// 有效性（對應後端 ContractValidity）：已核准合約單依日期計算，由後端回傳
export const CONTRACT_VALIDITY = {
  EFFECTIVE: 1,
  EXPIRED: 2
}

export const CONTRACT_VALIDITY_OPTIONS = [
  { value: CONTRACT_VALIDITY.EFFECTIVE, label: '生效中' },
  { value: CONTRACT_VALIDITY.EXPIRED, label: '已到期' }
]

// 簽核引擎的單據類型（對應後端 TableType.合約單），查簽核進度用
export const CONTRACT_TABLE_TYPE = 11

// 合約掃描檔限制（對應後端：只接受 PDF，不超過 10 MB；後端仍會檢查）
export const CONTRACT_ATTACHMENT_MAX_BYTES = 10 * 1024 * 1024

// el-tag 的顏色類型
export function contractStatusTagType(status) {
  switch (status) {
    case CONTRACT_STATUS.IN_APPROVAL:
      return 'warning'
    case CONTRACT_STATUS.APPROVED:
      return 'success'
    case CONTRACT_STATUS.REJECTED:
      return 'danger'
    default:
      return 'info'
  }
}

export function contractValidityTagType(validity) {
  return validity === CONTRACT_VALIDITY.EFFECTIVE ? 'success' : 'info'
}

// 簽核紀錄的關卡 0 是系統建立的「送出簽核」紀錄
export function approvalStepLabel(stepOrder) {
  return stepOrder === 0 ? '送簽' : `第 ${stepOrder} 關`
}
