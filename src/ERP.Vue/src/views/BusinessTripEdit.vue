<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { approvalApi } from '@/api/approvalApi'
import { businessTripApi } from '@/api/businessTripApi'
import { companyApi } from '@/api/companyApi'
import { sharedApi } from '@/api/sharedApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import {
  BUSINESS_TRIP_STATUS,
  BUSINESS_TRIP_TABLE_TYPE,
  approvalStepLabel,
  businessTripStatusTagType,
  canDeleteBusinessTrip
} from '@/utils/businessTrip'
import { countInclusiveDays, formatDateTime, toDateOnly } from '@/utils/dateUtils'

const route = useRoute()
const router = useRouter()

const formRef = ref(null)
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const sending = ref(false)
const approving = ref(false)
const rejecting = ref(false)
const revoking = ref(false)
const voiding = ref(false)
const progressLoading = ref(false)
const progressRows = ref([])
const progressError = ref('')
const formError = ref('')
const staffOptions = ref([])
const customerOptions = ref([])
// 填單用的公司選單：只列啟用中；編輯時原本的公司即使已停用也會回傳（isActive 為 false）
const companyOptions = ref([])

const tripId = computed(() => (route.params.id ? Number(route.params.id) : null))
const isCreate = computed(() => tripId.value === null)

function createEmptyForm() {
  return {
    no: '',
    status: 0,
    statusDisplay: '草稿',
    // 是否輪到目前使用者簽核：由後端判斷，前端不自行推算
    canApprove: false,
    // 編輯、撤銷、作廢同樣由後端判斷
    canEdit: false,
    canRevoke: false,
    canVoid: false,
    companyId: null,
    companyName: '',
    companyTaxInvoiceNumber: '',
    travelerStaffId: null,
    destination: '',
    reason: '',
    // el-date-picker daterange 的值：[起日, 迄日]
    dateRange: [],
    customerId: null,
    note: ''
  }
}

const form = reactive(createEmptyForm())

const rules = {
  companyId: [{ required: true, message: '請選擇單據公司', trigger: 'change' }],
  travelerStaffId: [{ required: true, message: '請選擇出差人', trigger: 'change' }],
  destination: [{ required: true, message: '請填寫出差地點', trigger: 'blur' }],
  reason: [{ required: true, message: '請填寫出差事由', trigger: 'blur' }],
  // daterange 的值是陣列，required 要指定 type: 'array' 才會把空陣列視為未填
  dateRange: [{ type: 'array', required: true, message: '請選擇出差起訖日', trigger: 'change' }]
}

// 新增一律可編輯；既有單據由後端的 canEdit 決定（草稿或已退回，且是建單人或出差人），其餘唯讀
const editable = computed(() => isCreate.value || form.canEdit)
const deletable = computed(() => !isCreate.value && form.canEdit && canDeleteBusinessTrip(form.status))
// 出差天數＝迄日－起日＋1，只供顯示，不送到後端
const tripDays = computed(() => countInclusiveDays(form.dateRange?.[0], form.dateRange?.[1]))
// 既有的草稿與已退回才能送簽；新增尚未儲存的單據要先儲存
const sendable = computed(() => !isCreate.value && editable.value)
// 草稿還沒有簽核紀錄；其餘狀態顯示現有簽核進度
const showProgress = computed(() => !isCreate.value && form.status !== BUSINESS_TRIP_STATUS.DRAFT)
const busy = computed(
  () =>
    loading.value ||
    saving.value ||
    deleting.value ||
    sending.value ||
    approving.value ||
    rejecting.value ||
    revoking.value ||
    voiding.value
)
// 目前選的公司若已停用（只會出現在編輯原本的單據時），提醒送簽前要改選；後端送簽時也會擋下
const selectedCompanyDisabled = computed(() => {
  const selected = companyOptions.value.find((company) => company.value === form.companyId)
  return !!selected && selected.isActive === false
})
const readonlyHint = computed(() => {
  // 尚未載入或載入失敗（沒有單號）時不顯示唯讀提示
  if (editable.value || loading.value || !form.no) return ''
  // 能不能編輯由後端的 canEdit 決定；原因可能是狀態，也可能是自己不是建單人或出差人
  return `目前狀態為【${form.statusDisplay}】，您僅能檢視（只有建單人或出差人能編輯草稿與已退回的出差單）`
})

function applyTrip(data = {}) {
  Object.assign(form, {
    no: data.no ?? '',
    status: data.status ?? 0,
    statusDisplay: data.statusDisplay ?? '',
    canApprove: data.canApprove ?? false,
    canEdit: data.canEdit ?? false,
    canRevoke: data.canRevoke ?? false,
    canVoid: data.canVoid ?? false,
    companyId: data.companyId ?? null,
    companyName: data.companyName ?? '',
    companyTaxInvoiceNumber: data.companyTaxInvoiceNumber ?? '',
    travelerStaffId: data.travelerStaffId ?? null,
    destination: data.destination ?? '',
    reason: data.reason ?? '',
    dateRange: data.startDate && data.endDate ? [toDateOnly(data.startDate), toDateOnly(data.endDate)] : [],
    customerId: data.customerId ?? null,
    note: data.note ?? ''
  })
}

function buildPayload() {
  const [startDate, endDate] = form.dateRange
  return {
    id: tripId.value ?? 0,
    companyId: form.companyId,
    travelerStaffId: form.travelerStaffId,
    destination: form.destination.trim(),
    reason: form.reason.trim(),
    startDate,
    endDate,
    customerId: form.customerId || null,
    note: form.note?.trim() || null
  }
}

async function loadOptions() {
  try {
    const [staffResponse, customerResponse] = await Promise.all([
      sharedApi.getStaffSelect(),
      sharedApi.getCustomerSelect()
    ])
    if (isSuccess(staffResponse)) {
      staffOptions.value = unwrap(staffResponse)?.items ?? []
    }
    if (isSuccess(customerResponse)) {
      customerOptions.value = unwrap(customerResponse)?.items ?? []
    }
  } catch (error) {
    formError.value = getApiErrorMessage(error, '無法取得下拉選單資料')
    ElMessage.error(formError.value)
  }
}

// 載入公司選單；includeId 是單據原本的公司，已停用也會一併列出
async function loadCompanyOptions(includeId, token) {
  try {
    const response = await companyApi.getSelect(includeId ?? undefined)
    if (token !== loadToken) return
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法取得公司下拉選單')
      ElMessage.error(formError.value)
      return
    }
    companyOptions.value = unwrap(response)?.items ?? []
  } catch (error) {
    if (token !== loadToken) return
    formError.value = getApiErrorMessage(error, '無法取得公司下拉選單')
    ElMessage.error(formError.value)
  }
}

// 選了別間公司後，原本的統編就不是目前選的公司，先清掉
function changeCompany() {
  form.companyTaxInvoiceNumber = ''
}

// 新增時預設出差人為本人；取不到時顯示原因，仍可自行選擇出差人
async function applyDefaultTraveler() {
  try {
    const response = await businessTripApi.getCurrentStaff()
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法帶入預設出差人')
      ElMessage.error(formError.value)
      return
    }
    form.travelerStaffId = unwrap(response)?.value ?? null
  } catch (error) {
    formError.value = getApiErrorMessage(error, '無法帶入預設出差人')
    ElMessage.error(formError.value)
  }
}

// 切換單據時，只接受最後一次請求的結果，避免後送先回覆蓋畫面
let loadToken = 0

// 簽核進度只供顯示：查詢失敗不影響單據本身，錯誤顯示在進度區塊
async function loadProgress(token) {
  progressRows.value = []
  progressError.value = ''
  if (!showProgress.value) return

  progressLoading.value = true
  try {
    const response = await approvalApi.getProgress(BUSINESS_TRIP_TABLE_TYPE, form.no)
    if (token !== loadToken) return
    if (!isSuccess(response)) {
      progressError.value = getApiErrorMessage(response, '無法取得簽核進度')
      ElMessage.error(progressError.value)
      return
    }
    progressRows.value = (unwrap(response)?.items ?? []).map((item) => ({
      id: item.id,
      stepLabel: approvalStepLabel(item.stepOrder),
      approver: item.userName || item.roleName || '',
      statusDisplay: item.statusDisplay ?? '',
      approvalTime: formatDateTime(item.approvalTime),
      memo: item.memo ?? ''
    }))
  } catch (error) {
    if (token !== loadToken) return
    progressError.value = getApiErrorMessage(error, '無法取得簽核進度')
    ElMessage.error(progressError.value)
  } finally {
    if (token === loadToken) progressLoading.value = false
  }
}

async function loadTrip() {
  const token = ++loadToken
  formError.value = ''
  Object.assign(form, createEmptyForm())
  progressRows.value = []
  progressError.value = ''

  if (isCreate.value) {
    await loadCompanyOptions(null, token)
    // 只有一間啟用中的公司時自動帶入，少選一個欄位
    const activeCompanies = companyOptions.value.filter((company) => company.isActive !== false)
    if (token === loadToken && activeCompanies.length === 1) {
      form.companyId = activeCompanies[0].value
    }
    await applyDefaultTraveler()
    return
  }

  loading.value = true
  try {
    const response = await businessTripApi.get(tripId.value)
    if (token !== loadToken) return
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法取得出差單')
      ElMessage.error(formError.value)
      return
    }
    applyTrip(unwrap(response) ?? {})
    await loadCompanyOptions(form.companyId, token)
  } catch (error) {
    if (token !== loadToken) return
    formError.value = getApiErrorMessage(error, '無法取得出差單')
    ElMessage.error(formError.value)
    return
  } finally {
    if (token === loadToken) loading.value = false
  }

  await loadProgress(token)
}

async function saveDraft() {
  try {
    await formRef.value.validate()
  } catch {
    return // 欄位驗證未通過，錯誤已顯示在欄位上
  }

  saving.value = true
  formError.value = ''

  try {
    const response = isCreate.value
      ? await businessTripApi.add(buildPayload())
      : await businessTripApi.edit(buildPayload())
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '儲存失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('儲存成功')
    if (isCreate.value) {
      // 新增成功後改到該單的編輯頁，route 參數改變會重新載入
      await router.replace({ name: 'business-trip-edit', params: { id: unwrap(response).id } })
    } else {
      await loadTrip()
    }
  } catch (error) {
    formError.value = getApiErrorMessage(error, '儲存失敗')
    ElMessage.error(formError.value)
  } finally {
    saving.value = false
  }
}

// 送簽前先把畫面上的內容存成草稿，確保簽核看到的就是畫面上的內容
async function sendApproval() {
  try {
    await formRef.value.validate()
  } catch {
    return // 欄位驗證未通過，錯誤已顯示在欄位上
  }

  try {
    await ElMessageBox.confirm(
      `確定要將出差單「${form.no}」送出簽核？送出前會先儲存目前填寫的內容。`,
      '送出簽核',
      { confirmButtonText: '送出簽核', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  sending.value = true
  formError.value = ''

  try {
    const saveResponse = await businessTripApi.edit(buildPayload())
    if (!isSuccess(saveResponse)) {
      formError.value = getApiErrorMessage(saveResponse, '儲存失敗，未送出簽核')
      ElMessage.error(formError.value)
      return
    }

    const response = await businessTripApi.sendApproval(form.no)
    if (!isSuccess(response)) {
      // 內容已儲存、簽核未送出（例如日期重疊、沒有啟用的簽核設定）：錯誤留在頁面上，單據維持原狀態
      formError.value = getApiErrorMessage(response, '送出簽核失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('已送出簽核')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '送出簽核失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    sending.value = false
  }

  // 送簽已成功，重新載入失敗由 loadTrip 自行顯示錯誤，不影響送簽結果
  await loadTrip()
}

// 簽核人填寫意見的輸入框：核准選填，拒絕必填
async function promptApprovalMemo({ title, message, required }) {
  try {
    const { value } = await ElMessageBox.prompt(message, title, {
      confirmButtonText: title,
      cancelButtonText: '取消',
      inputType: 'textarea',
      inputPlaceholder: required ? '請填寫拒絕原因' : '簽核意見（選填）',
      inputValidator: (input) => (required && !input?.trim() ? '請填寫拒絕原因' : true)
    })
    return { confirmed: true, memo: (value ?? '').trim() }
  } catch {
    return { confirmed: false, memo: '' } // 使用者取消
  }
}

async function approveTrip() {
  const { confirmed, memo } = await promptApprovalMemo({
    title: '核准',
    message: `確定要核准出差單「${form.no}」？`,
    required: false
  })
  if (!confirmed) return

  approving.value = true
  formError.value = ''

  try {
    const response = await businessTripApi.approve(form.no, memo)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '核准失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('核准成功')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '核准失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    approving.value = false
  }

  // 核准已成功，重新載入失敗由 loadTrip 自行顯示錯誤，不影響核准結果
  await loadTrip()
}

async function rejectTrip() {
  const { confirmed, memo } = await promptApprovalMemo({
    title: '拒絕',
    message: `確定要拒絕出差單「${form.no}」？拒絕原因會顯示給建單人。`,
    required: true
  })
  if (!confirmed) return

  rejecting.value = true
  formError.value = ''

  try {
    const response = await businessTripApi.reject(form.no, memo)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '拒絕失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('已拒絕')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '拒絕失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    rejecting.value = false
  }

  await loadTrip()
}

async function revokeTrip() {
  try {
    await ElMessageBox.confirm(
      `確定要撤銷出差單「${form.no}」的簽核？撤銷後單據變成已退回，可修改後重新送簽。`,
      '撤銷簽核',
      { confirmButtonText: '撤銷簽核', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  revoking.value = true
  formError.value = ''

  try {
    const response = await businessTripApi.revoke(form.no)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '撤銷失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('已撤銷簽核')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '撤銷失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    revoking.value = false
  }

  await loadTrip()
}

async function voidTrip() {
  try {
    await ElMessageBox.confirm(
      `確定要作廢出差單「${form.no}」？作廢後單據不可編輯或重送，要再出差需另開新單，此操作無法復原。`,
      '作廢出差單',
      { confirmButtonText: '作廢', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  voiding.value = true
  formError.value = ''

  try {
    const response = await businessTripApi.voidTrip(form.no)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '作廢失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('已作廢')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '作廢失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    voiding.value = false
  }

  await loadTrip()
}

async function deleteTrip() {
  try {
    await ElMessageBox.confirm(`確定要刪除出差單「${form.no}」？此操作無法復原。`, '刪除出差單', {
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
      type: 'warning'
    })
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  formError.value = ''

  try {
    const response = await businessTripApi.delete(tripId.value)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '刪除失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('已刪除')
    router.push({ name: 'business-trip-list' })
  } catch (error) {
    formError.value = getApiErrorMessage(error, '刪除失敗')
    ElMessage.error(formError.value)
  } finally {
    deleting.value = false
  }
}

function goBack() {
  router.push({ name: 'business-trip-list' })
}

watch(tripId, loadTrip)

onMounted(() => {
  loadOptions()
  loadTrip()
})
</script>

<template>
  <div class="business-trip-edit">
    <div class="page-header">
      <h1>{{ isCreate ? '新增出差單' : `出差單 ${form.no}` }}</h1>
      <el-tag v-if="!isCreate" :type="businessTripStatusTagType(form.status)">{{ form.statusDisplay }}</el-tag>
    </div>

    <el-alert v-if="readonlyHint" class="form-alert" type="info" :title="readonlyHint" :closable="false" show-icon />
    <el-alert v-if="formError" class="form-alert" type="error" :title="formError" :closable="false" show-icon />

    <el-form
      ref="formRef"
      v-loading="loading"
      class="trip-form"
      :model="form"
      :rules="rules"
      :disabled="!editable"
      label-width="100px"
    >
      <el-form-item label="公司" prop="companyId">
        <el-select v-model="form.companyId" placeholder="請選擇公司" filterable @change="changeCompany">
          <el-option
            v-for="company in companyOptions"
            :key="company.value"
            :label="company.isActive === false ? `${company.text}（已停用）` : company.text"
            :value="company.value"
          />
        </el-select>
        <span v-if="form.companyTaxInvoiceNumber" class="company-tax-id">統編 {{ form.companyTaxInvoiceNumber }}</span>
        <span v-if="editable && selectedCompanyDisabled" class="company-disabled-hint">此公司已停用，送簽前請改選其他公司</span>
      </el-form-item>

      <el-form-item label="出差人" prop="travelerStaffId">
        <el-select v-model="form.travelerStaffId" placeholder="請選擇在職員工" filterable>
          <el-option v-for="staff in staffOptions" :key="staff.value" :label="staff.text" :value="staff.value" />
        </el-select>
      </el-form-item>

      <el-form-item label="出差地點" prop="destination">
        <el-input v-model.trim="form.destination" maxlength="128" show-word-limit />
      </el-form-item>

      <el-form-item label="出差事由" prop="reason">
        <el-input v-model.trim="form.reason" type="textarea" :rows="3" maxlength="256" show-word-limit />
      </el-form-item>

      <el-form-item label="出差起訖日" prop="dateRange">
        <el-date-picker
          v-model="form.dateRange"
          type="daterange"
          value-format="YYYY-MM-DD"
          range-separator="至"
          start-placeholder="起日"
          end-placeholder="迄日"
        />
        <span v-if="tripDays" class="trip-days">共 {{ tripDays }} 天</span>
      </el-form-item>

      <el-form-item label="客戶">
        <el-select v-model="form.customerId" placeholder="選填" clearable filterable>
          <el-option v-for="customer in customerOptions" :key="customer.value" :label="customer.text" :value="customer.value" />
        </el-select>
      </el-form-item>

      <el-form-item label="備註">
        <el-input v-model.trim="form.note" type="textarea" :rows="3" maxlength="1024" show-word-limit />
      </el-form-item>
    </el-form>

    <div class="form-actions">
      <el-button @click="goBack">返回列表</el-button>
      <el-button v-if="editable" type="primary" :loading="saving" :disabled="busy" @click="saveDraft">
        儲存草稿
      </el-button>
      <el-button v-if="sendable" type="success" :loading="sending" :disabled="busy" @click="sendApproval">
        送出簽核
      </el-button>
      <el-button v-if="form.canApprove" type="success" :loading="approving" :disabled="busy" @click="approveTrip">
        核准
      </el-button>
      <el-button v-if="form.canApprove" type="danger" :loading="rejecting" :disabled="busy" @click="rejectTrip">
        拒絕
      </el-button>
      <el-button v-if="form.canRevoke" type="warning" :loading="revoking" :disabled="busy" @click="revokeTrip">
        撤銷簽核
      </el-button>
      <el-button v-if="form.canVoid" type="danger" :loading="voiding" :disabled="busy" @click="voidTrip">
        作廢
      </el-button>
      <el-button v-if="deletable" type="danger" :loading="deleting" :disabled="busy" @click="deleteTrip">
        刪除
      </el-button>
    </div>

    <section v-if="showProgress" class="approval-progress">
      <h2>簽核進度</h2>
      <el-alert v-if="progressError" type="error" :title="progressError" :closable="false" show-icon />
      <el-table v-else v-loading="progressLoading" :data="progressRows" row-key="id" border>
        <el-table-column prop="stepLabel" label="關卡" width="90" />
        <el-table-column prop="approver" label="簽核人" min-width="120" />
        <el-table-column prop="statusDisplay" label="狀態" width="120" />
        <el-table-column prop="approvalTime" label="簽核時間" width="180" />
        <el-table-column prop="memo" label="意見" min-width="160" />
      </el-table>
    </section>
  </div>
</template>

<style scoped>
.business-trip-edit {
  padding: 20px;
  max-width: 760px;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.form-alert {
  margin-bottom: 16px;
}

.trip-days {
  margin-left: 12px;
  color: var(--el-text-color-secondary);
}

.company-tax-id {
  margin-left: 12px;
  color: var(--el-text-color-secondary);
}

.company-disabled-hint {
  margin-left: 12px;
  color: var(--el-color-warning);
}

.form-actions {
  display: flex;
  gap: 8px;
  padding-left: 100px;
}

.approval-progress {
  margin-top: 32px;
}

.approval-progress h2 {
  margin-bottom: 12px;
  font-size: 16px;
}
</style>
