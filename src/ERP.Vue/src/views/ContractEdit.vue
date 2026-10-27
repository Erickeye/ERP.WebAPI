<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { approvalApi } from '@/api/approvalApi'
import { companyApi } from '@/api/companyApi'
import { contractApi } from '@/api/contractApi'
import { sharedApi } from '@/api/sharedApi'
import { supplierApi } from '@/api/supplierApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import {
  CONTRACT_ATTACHMENT_MAX_BYTES,
  CONTRACT_STATUS,
  CONTRACT_TABLE_TYPE,
  CONTRACT_TYPE,
  CONTRACT_TYPE_OPTIONS,
  approvalStepLabel,
  contractStatusTagType,
  contractValidityTagType
} from '@/utils/contract'
import { formatDateTime, toDateOnly } from '@/utils/dateUtils'
import { parseDownloadFileName, readBlobErrorMessage, saveBlob } from '@/utils/file'

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
const uploading = ref(false)
const downloading = ref(false)
const progressLoading = ref(false)
const progressRows = ref([])
const progressError = ref('')
const formError = ref('')
const customerOptions = ref([])
// 填單用的供應商與公司選單：只列啟用中；編輯時原本的對象即使已停用也會回傳（isActive 為 false）
const supplierOptions = ref([])
const companyOptions = ref([])

const contractId = computed(() => (route.params.id ? Number(route.params.id) : null))
const isCreate = computed(() => contractId.value === null)

function createEmptyForm() {
  return {
    no: '',
    status: CONTRACT_STATUS.DRAFT,
    statusDisplay: '草稿',
    validity: null,
    validityDisplay: '',
    // 目前使用者能做的動作：由後端判斷，前端不自行推算
    canApprove: false,
    canEdit: false,
    canDelete: false,
    canRevoke: false,
    canVoid: false,
    type: CONTRACT_TYPE.SUPPLIER,
    supplierId: null,
    customerId: null,
    companyId: null,
    contractNo: '',
    // el-date-picker daterange 的值：[起日, 迄日]
    dateRange: [],
    amount: null,
    remark: '',
    hasAttachment: false
  }
}

const form = reactive(createEmptyForm())

const isSupplierContract = computed(() => form.type === CONTRACT_TYPE.SUPPLIER)

const rules = {
  type: [{ required: true, message: '請選擇合約類型', trigger: 'change' }],
  supplierId: [{ required: true, message: '請選擇供應商', trigger: 'change' }],
  customerId: [{ required: true, message: '請選擇客戶', trigger: 'change' }],
  companyId: [{ required: true, message: '請選擇簽約公司', trigger: 'change' }],
  contractNo: [{ required: true, message: '請填寫合約編號', trigger: 'blur' }],
  // daterange 的值是陣列，required 要指定 type: 'array' 才會把空陣列視為未填
  dateRange: [{ type: 'array', required: true, message: '請選擇合約起訖日', trigger: 'change' }],
  amount: [
    { required: true, message: '請填寫合約總金額', trigger: 'blur' },
    {
      validator: (rule, value, callback) =>
        value === null || value === undefined || value > 0 ? callback() : callback(new Error('合約總金額必須大於 0')),
      trigger: 'blur'
    }
  ]
}

// 新增一律可編輯；既有單據由後端的 canEdit 決定（草稿或已退回，且有編輯權限），其餘唯讀
const editable = computed(() => isCreate.value || form.canEdit)
// 既有的草稿與已退回才能送簽；新增尚未儲存的單據要先儲存
const sendable = computed(() => !isCreate.value && editable.value)
// 草稿還沒有簽核紀錄；其餘狀態顯示現有簽核進度
const showProgress = computed(() => !isCreate.value && form.status !== CONTRACT_STATUS.DRAFT)
const busy = computed(
  () =>
    loading.value ||
    saving.value ||
    deleting.value ||
    sending.value ||
    approving.value ||
    rejecting.value ||
    revoking.value ||
    voiding.value ||
    uploading.value ||
    downloading.value
)
// 目前選的供應商或公司若已停用（只會出現在編輯原本的單據時），提醒送簽前要改選；後端送簽時也會擋下
const selectedSupplierDisabled = computed(() => {
  const selected = supplierOptions.value.find((supplier) => supplier.value === form.supplierId)
  return !!selected && selected.isActive === false
})
const selectedCompanyDisabled = computed(() => {
  const selected = companyOptions.value.find((company) => company.value === form.companyId)
  return !!selected && selected.isActive === false
})
const readonlyHint = computed(() => {
  // 尚未載入或載入失敗（沒有單號）時不顯示唯讀提示
  if (editable.value || loading.value || !form.no) return ''
  return `目前狀態為【${form.statusDisplay}】，您僅能檢視（只有草稿與已退回的合約單，且需有「合約單編輯」權限才能編輯）`
})

function applyContract(data = {}) {
  Object.assign(form, {
    no: data.no ?? '',
    status: data.status ?? CONTRACT_STATUS.DRAFT,
    statusDisplay: data.statusDisplay ?? '',
    validity: data.validity ?? null,
    validityDisplay: data.validityDisplay ?? '',
    canApprove: data.canApprove ?? false,
    canEdit: data.canEdit ?? false,
    canDelete: data.canDelete ?? false,
    canRevoke: data.canRevoke ?? false,
    canVoid: data.canVoid ?? false,
    type: data.type ?? CONTRACT_TYPE.SUPPLIER,
    supplierId: data.supplierId ?? null,
    customerId: data.customerId ?? null,
    companyId: data.companyId ?? null,
    contractNo: data.contractNo ?? '',
    dateRange: data.startDate && data.endDate ? [toDateOnly(data.startDate), toDateOnly(data.endDate)] : [],
    amount: data.amount ?? null,
    remark: data.remark ?? '',
    hasAttachment: data.hasAttachment ?? false
  })
}

function buildPayload() {
  const [startDate, endDate] = form.dateRange
  return {
    id: contractId.value ?? 0,
    type: form.type,
    // 合約對象依類型恰好一個：供應商合約不送客戶，客戶合約不送供應商
    supplierId: isSupplierContract.value ? form.supplierId : null,
    customerId: isSupplierContract.value ? null : form.customerId,
    companyId: form.companyId,
    contractNo: form.contractNo.trim(),
    startDate,
    endDate,
    amount: form.amount,
    remark: form.remark?.trim() || null
  }
}

// 切換單據時，只接受最後一次請求的結果，避免後送先回覆蓋畫面
let loadToken = 0

function reportError(response, fallback) {
  formError.value = getApiErrorMessage(response, fallback)
  ElMessage.error(formError.value)
}

async function loadCustomerOptions() {
  try {
    const response = await sharedApi.getCustomerSelect()
    if (!isSuccess(response)) {
      reportError(response, '無法取得客戶下拉選單')
      return
    }
    customerOptions.value = unwrap(response)?.items ?? []
  } catch (error) {
    reportError(error, '無法取得客戶下拉選單')
  }
}

// 載入供應商與公司選單；includeId 是單據原本的對象，已停用也會一併列出
async function loadSelectOptions(supplierIncludeId, companyIncludeId, token) {
  try {
    const [supplierResponse, companyResponse] = await Promise.all([
      supplierApi.getSelect(supplierIncludeId ?? undefined),
      companyApi.getSelect(companyIncludeId ?? undefined)
    ])
    if (token !== loadToken) return
    if (!isSuccess(supplierResponse)) {
      reportError(supplierResponse, '無法取得供應商下拉選單')
    } else {
      supplierOptions.value = unwrap(supplierResponse)?.items ?? []
    }
    if (!isSuccess(companyResponse)) {
      reportError(companyResponse, '無法取得公司下拉選單')
    } else {
      companyOptions.value = unwrap(companyResponse)?.items ?? []
    }
  } catch (error) {
    if (token !== loadToken) return
    reportError(error, '無法取得供應商與公司下拉選單')
  }
}

// 簽核進度只供顯示：查詢失敗不影響單據本身，錯誤顯示在進度區塊
async function loadProgress(token) {
  progressRows.value = []
  progressError.value = ''
  if (!showProgress.value) return

  progressLoading.value = true
  try {
    const response = await approvalApi.getProgress(CONTRACT_TABLE_TYPE, form.no)
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

async function loadContract() {
  const token = ++loadToken
  formError.value = ''
  Object.assign(form, createEmptyForm())
  progressRows.value = []
  progressError.value = ''
  // 新的載入接手後，舊請求的 finally 不會再恢復 loading（token 不符），所以在這裡先恢復，避免切換單據時卡在載入中
  loading.value = false
  progressLoading.value = false

  if (isCreate.value) {
    await loadSelectOptions(null, null, token)
    // 只有一間啟用中的公司時自動帶入，少選一個欄位
    const activeCompanies = companyOptions.value.filter((company) => company.isActive !== false)
    if (token === loadToken && activeCompanies.length === 1) {
      form.companyId = activeCompanies[0].value
    }
    return
  }

  loading.value = true
  try {
    const response = await contractApi.get(contractId.value)
    if (token !== loadToken) return
    if (!isSuccess(response)) {
      reportError(response, '無法取得合約單')
      return
    }
    applyContract(unwrap(response) ?? {})
    await loadSelectOptions(form.supplierId, form.companyId, token)
  } catch (error) {
    if (token !== loadToken) return
    reportError(error, '無法取得合約單')
    return
  } finally {
    if (token === loadToken) loading.value = false
  }

  await loadProgress(token)
}

// 切換合約類型時，清掉另一種對象，避免送出兩個對象
function changeType() {
  form.supplierId = null
  form.customerId = null
  formRef.value?.clearValidate(['supplierId', 'customerId'])
}

async function validateForm() {
  try {
    await formRef.value.validate()
    return true
  } catch {
    return false // 欄位驗證未通過，錯誤已顯示在欄位上
  }
}

async function confirmAction(message, title, confirmButtonText) {
  try {
    await ElMessageBox.confirm(message, title, { confirmButtonText, cancelButtonText: '取消', type: 'warning' })
    return true
  } catch {
    return false // 使用者取消
  }
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

// 簽核動作共用流程：呼叫 API、檢查 isSuccess、成功後重新載入（重新載入失敗由 loadContract 自行顯示，不影響動作結果）
async function runApprovalAction(busyRef, request, successText, failText) {
  busyRef.value = true
  formError.value = ''

  try {
    const response = await request()
    if (!isSuccess(response)) {
      reportError(response, failText)
      return false
    }
    ElMessage.success(successText)
  } catch (error) {
    reportError(error, failText)
    return false
  } finally {
    busyRef.value = false
  }

  await loadContract()
  return true
}

async function saveDraft() {
  if (!(await validateForm())) return

  saving.value = true
  formError.value = ''

  try {
    const response = isCreate.value ? await contractApi.add(buildPayload()) : await contractApi.edit(buildPayload())
    if (!isSuccess(response)) {
      reportError(response, '儲存失敗')
      return
    }

    ElMessage.success('儲存成功')
    if (isCreate.value) {
      // 新增成功後改到該單的編輯頁（可接著上傳掃描檔），route 參數改變會重新載入
      await router.replace({ name: 'contract-edit', params: { id: unwrap(response).id } })
    } else {
      await loadContract()
    }
  } catch (error) {
    reportError(error, '儲存失敗')
  } finally {
    saving.value = false
  }
}

// 送簽前先把畫面上的內容存成草稿，確保簽核看到的就是畫面上的內容
async function sendApproval() {
  if (!(await validateForm())) return
  if (
    !(await confirmAction(
      `確定要將合約單「${form.no}」送出簽核？送出前會先儲存目前填寫的內容。`,
      '送出簽核',
      '送出簽核'
    ))
  ) {
    return
  }

  sending.value = true
  formError.value = ''

  try {
    const saveResponse = await contractApi.edit(buildPayload())
    if (!isSuccess(saveResponse)) {
      reportError(saveResponse, '儲存失敗，未送出簽核')
      return
    }

    const response = await contractApi.sendApproval(form.no)
    if (!isSuccess(response)) {
      // 內容已儲存、簽核未送出（例如尚未上傳掃描檔、沒有啟用的簽核設定）：錯誤留在頁面上，單據維持原狀態
      reportError(response, '送出簽核失敗')
      return
    }

    ElMessage.success('已送出簽核')
  } catch (error) {
    reportError(error, '送出簽核失敗')
    return
  } finally {
    sending.value = false
  }

  await loadContract()
}

async function approveContract() {
  const { confirmed, memo } = await promptApprovalMemo({
    title: '核准',
    message: `確定要核准合約單「${form.no}」？`,
    required: false
  })
  if (!confirmed) return

  await runApprovalAction(approving, () => contractApi.approve(form.no, memo), '核准成功', '核准失敗')
}

async function rejectContract() {
  const { confirmed, memo } = await promptApprovalMemo({
    title: '拒絕',
    message: `確定要拒絕合約單「${form.no}」？拒絕原因會顯示給建單人。`,
    required: true
  })
  if (!confirmed) return

  await runApprovalAction(rejecting, () => contractApi.reject(form.no, memo), '已拒絕', '拒絕失敗')
}

async function revokeContract() {
  const confirmed = await confirmAction(
    `確定要撤銷合約單「${form.no}」的簽核？撤銷後單據變成已退回，可修改後重新送簽。`,
    '撤銷簽核',
    '撤銷簽核'
  )
  if (!confirmed) return

  await runApprovalAction(revoking, () => contractApi.revoke(form.no), '已撤銷簽核', '撤銷失敗')
}

async function voidContract() {
  const confirmed = await confirmAction(
    `確定要作廢合約單「${form.no}」？作廢後單據不可編輯或重送，需要時要另開新單，此操作無法復原。`,
    '作廢合約單',
    '作廢'
  )
  if (!confirmed) return

  await runApprovalAction(voiding, () => contractApi.voidContract(form.no), '已作廢', '作廢失敗')
}

async function deleteContract() {
  if (!(await confirmAction(`確定要刪除合約單「${form.no}」？此操作無法復原。`, '刪除合約單', '刪除'))) return

  deleting.value = true
  formError.value = ''

  try {
    const response = await contractApi.delete(contractId.value)
    if (!isSuccess(response)) {
      reportError(response, '刪除失敗')
      return
    }

    ElMessage.success('已刪除')
    router.push({ name: 'contract-list' })
  } catch (error) {
    reportError(error, '刪除失敗')
  } finally {
    deleting.value = false
  }
}

// 上傳前先擋下明顯不合規定的檔案（後端仍會檢查）；回傳 false 不送出
function beforeAttachmentUpload(file) {
  if (!file.name.toLowerCase().endsWith('.pdf')) {
    ElMessage.error('合約掃描檔只接受 PDF 檔案')
    return false
  }
  if (file.size > CONTRACT_ATTACHMENT_MAX_BYTES) {
    ElMessage.error('合約掃描檔大小不可超過 10 MB')
    return false
  }
  return true
}

async function uploadAttachment({ file }) {
  uploading.value = true
  formError.value = ''

  try {
    const response = await contractApi.uploadAttachment(contractId.value, file)
    if (!isSuccess(response)) {
      reportError(response, '上傳失敗')
      return
    }

    ElMessage.success('合約掃描檔已上傳')
    form.hasAttachment = true
  } catch (error) {
    reportError(error, '上傳失敗')
  } finally {
    uploading.value = false
  }
}

async function downloadAttachment() {
  downloading.value = true
  formError.value = ''

  try {
    const response = await contractApi.downloadAttachment(contractId.value)
    // 後端失敗時回的是 JSON 的 ResultModel，不可下載出一個壞檔案
    const errorMessage = await readBlobErrorMessage(response.data, '下載失敗')
    if (errorMessage) {
      formError.value = errorMessage
      ElMessage.error(errorMessage)
      return
    }

    saveBlob(response.data, parseDownloadFileName(response.headers?.['content-disposition'], `${form.no}.pdf`))
  } catch (error) {
    reportError(error, '下載失敗')
  } finally {
    downloading.value = false
  }
}

function goBack() {
  router.push({ name: 'contract-list' })
}

watch(contractId, loadContract)

onMounted(async () => {
  // 先載入客戶選單再載入單據：loadContract 開頭會清掉 formError，同時進行會蓋掉選單載入失敗的錯誤
  await loadCustomerOptions()
  await loadContract()
})
</script>

<template>
  <div class="contract-edit">
    <div class="page-header">
      <h1>{{ isCreate ? '新增合約單' : `合約單 ${form.no}` }}</h1>
      <el-tag v-if="!isCreate" :type="contractStatusTagType(form.status)">{{ form.statusDisplay }}</el-tag>
      <el-tag v-if="form.validityDisplay" :type="contractValidityTagType(form.validity)" effect="plain">
        {{ form.validityDisplay }}
      </el-tag>
    </div>

    <el-alert v-if="readonlyHint" class="form-alert" type="info" :title="readonlyHint" :closable="false" show-icon />
    <el-alert v-if="formError" class="form-alert" type="error" :title="formError" :closable="false" show-icon />

    <el-form
      ref="formRef"
      v-loading="loading"
      class="contract-form"
      :model="form"
      :rules="rules"
      :disabled="!editable"
      label-width="110px"
    >
      <el-form-item label="合約類型" prop="type">
        <el-radio-group v-model="form.type" @change="changeType">
          <el-radio-button v-for="option in CONTRACT_TYPE_OPTIONS" :key="option.value" :value="option.value">
            {{ option.label }}
          </el-radio-button>
        </el-radio-group>
      </el-form-item>

      <el-form-item v-if="isSupplierContract" label="供應商" prop="supplierId">
        <el-select v-model="form.supplierId" placeholder="請選擇供應商" filterable>
          <el-option
            v-for="supplier in supplierOptions"
            :key="supplier.value"
            :label="supplier.isActive === false ? `${supplier.text}（已停用）` : supplier.text"
            :value="supplier.value"
          />
        </el-select>
        <span v-if="editable && selectedSupplierDisabled" class="disabled-hint">此供應商已停用，送簽前請改選其他供應商</span>
      </el-form-item>

      <el-form-item v-else label="客戶" prop="customerId">
        <el-select v-model="form.customerId" placeholder="請選擇客戶" filterable>
          <el-option v-for="customer in customerOptions" :key="customer.value" :label="customer.text" :value="customer.value" />
        </el-select>
      </el-form-item>

      <el-form-item label="簽約公司" prop="companyId">
        <el-select v-model="form.companyId" placeholder="請選擇簽約公司" filterable>
          <el-option
            v-for="company in companyOptions"
            :key="company.value"
            :label="company.isActive === false ? `${company.text}（已停用）` : company.text"
            :value="company.value"
          />
        </el-select>
        <span v-if="editable && selectedCompanyDisabled" class="disabled-hint">此公司已停用，送簽前請改選其他公司</span>
      </el-form-item>

      <el-form-item label="合約編號" prop="contractNo">
        <el-input v-model.trim="form.contractNo" placeholder="紙本合約上的編號" maxlength="64" show-word-limit />
      </el-form-item>

      <el-form-item label="合約起訖日" prop="dateRange">
        <el-date-picker
          v-model="form.dateRange"
          type="daterange"
          value-format="YYYY-MM-DD"
          range-separator="至"
          start-placeholder="起日"
          end-placeholder="迄日"
        />
      </el-form-item>

      <el-form-item label="合約總金額" prop="amount">
        <el-input-number v-model="form.amount" :min="0.01" :precision="2" :step="1000" controls-position="right" />
        <span class="field-hint">框架合約沒有固定金額時填預估金額</span>
      </el-form-item>

      <el-form-item label="備註">
        <el-input v-model.trim="form.remark" type="textarea" :rows="3" maxlength="1024" show-word-limit />
      </el-form-item>
    </el-form>

    <section class="attachment">
      <h2>合約掃描檔</h2>
      <p v-if="isCreate" class="field-hint">請先儲存草稿，再上傳合約掃描檔（PDF，10 MB 以內）</p>
      <template v-else>
        <span v-if="form.hasAttachment">已上傳</span>
        <span v-else class="field-hint">尚未上傳（送出簽核前必須上傳）</span>
        <el-button v-if="form.hasAttachment" :loading="downloading" :disabled="busy" @click="downloadAttachment">
          下載
        </el-button>
        <el-upload
          v-if="editable"
          class="attachment-upload"
          action="#"
          accept=".pdf,application/pdf"
          :show-file-list="false"
          :before-upload="beforeAttachmentUpload"
          :http-request="uploadAttachment"
          :disabled="busy"
        >
          <el-button type="primary" :loading="uploading" :disabled="busy">
            {{ form.hasAttachment ? '更換掃描檔' : '上傳掃描檔' }}
          </el-button>
        </el-upload>
      </template>
    </section>

    <div class="form-actions">
      <el-button @click="goBack">返回列表</el-button>
      <el-button v-if="editable" type="primary" :loading="saving" :disabled="busy" @click="saveDraft">
        儲存草稿
      </el-button>
      <el-button v-if="sendable" type="success" :loading="sending" :disabled="busy" @click="sendApproval">
        送出簽核
      </el-button>
      <el-button v-if="form.canApprove" type="success" :loading="approving" :disabled="busy" @click="approveContract">
        核准
      </el-button>
      <el-button v-if="form.canApprove" type="danger" :loading="rejecting" :disabled="busy" @click="rejectContract">
        拒絕
      </el-button>
      <el-button v-if="form.canRevoke" type="warning" :loading="revoking" :disabled="busy" @click="revokeContract">
        撤銷簽核
      </el-button>
      <el-button v-if="form.canVoid" type="danger" :loading="voiding" :disabled="busy" @click="voidContract">
        作廢
      </el-button>
      <el-button v-if="form.canDelete" type="danger" :loading="deleting" :disabled="busy" @click="deleteContract">
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
.contract-edit {
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

.field-hint {
  margin-left: 12px;
  color: var(--el-text-color-secondary);
}

.disabled-hint {
  margin-left: 12px;
  color: var(--el-color-warning);
}

.attachment {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 20px;
  padding-left: 110px;
}

.attachment h2 {
  width: 100%;
  margin: 0;
  font-size: 16px;
}

.attachment-upload {
  display: inline-block;
}

.form-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  padding-left: 110px;
}

.approval-progress {
  margin-top: 32px;
}

.approval-progress h2 {
  margin-bottom: 12px;
  font-size: 16px;
}
</style>
