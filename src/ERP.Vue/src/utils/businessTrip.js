// 出差單狀態（對應後端 BusinessTripStatus）
export const BUSINESS_TRIP_STATUS = {
  DRAFT: 0,
  IN_APPROVAL: 1,
  APPROVED: 2,
  REJECTED: 3,
  VOIDED: 4
}

// 列表篩選的狀態選項（值對應後端 BusinessTripStatus）
export const BUSINESS_TRIP_STATUS_OPTIONS = [
  { value: BUSINESS_TRIP_STATUS.DRAFT, label: '草稿' },
  { value: BUSINESS_TRIP_STATUS.IN_APPROVAL, label: '簽核中' },
  { value: BUSINESS_TRIP_STATUS.APPROVED, label: '已核准' },
  { value: BUSINESS_TRIP_STATUS.REJECTED, label: '已退回' },
  { value: BUSINESS_TRIP_STATUS.VOIDED, label: '已作廢' }
]

// 簽核引擎的單據類型（對應後端 TableType.出差單），查簽核進度用
export const BUSINESS_TRIP_TABLE_TYPE = 10

// 簽核紀錄的關卡 0 是系統建立的「送出簽核」紀錄
export function approvalStepLabel(stepOrder) {
  return stepOrder === 0 ? '送簽' : `第 ${stepOrder} 關`
}

// 只有草稿可以刪除
export function canDeleteBusinessTrip(status) {
  return status === BUSINESS_TRIP_STATUS.DRAFT
}

// el-tag 的顏色類型
export function businessTripStatusTagType(status) {
  switch (status) {
    case BUSINESS_TRIP_STATUS.IN_APPROVAL:
      return 'warning'
    case BUSINESS_TRIP_STATUS.APPROVED:
      return 'success'
    case BUSINESS_TRIP_STATUS.REJECTED:
      return 'danger'
    default:
      return 'info'
  }
}
