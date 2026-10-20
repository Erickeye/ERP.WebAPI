<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { companyApi } from '@/api/companyApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import { toDateOnly } from '@/utils/dateUtils'

const route = useRoute()
const router = useRouter()

const formRef = ref(null)
const loading = ref(false)
const saving = ref(false)
const toggling = ref(false)
const deleting = ref(false)
const formError = ref('')

const companyId = computed(() => (route.params.id ? Number(route.params.id) : null))
const isCreate = computed(() => companyId.value === null)
const busy = computed(() => loading.value || saving.value || toggling.value || deleting.value)

function createEmptyForm() {
  return {
    // 新增的公司一律啟用中；既有公司由後端回傳
    isActive: true,
    attribName: '',
    name: '',
    taxInvoiceNumber: '',
    taxSerialNumber: '',
    owner: '',
    contactPhone: '',
    faxPhone: '',
    registeredAddress: '',
    deliveryAddress: '',
    taxInvoiceAddress: '',
    bankName: '',
    checkingAccount: '',
    remittanceAccount: '',
    payDays: '',
    foundedDate: '',
    invoiceForm: ''
  }
}

const form = reactive(createEmptyForm())

// 必填與格式規則對應後端 CompanyAddVM；後端才是最後一道防線
const rules = {
  attribName: [{ required: true, message: '請填寫公司簡稱', trigger: 'blur' }],
  name: [{ required: true, message: '請填寫公司全名', trigger: 'blur' }],
  taxInvoiceNumber: [
    { required: true, message: '請填寫統一編號', trigger: 'blur' },
    { pattern: /^\d{8}$/, message: '統一編號須為 8 碼數字', trigger: 'blur' }
  ],
  taxSerialNumber: [
    { required: true, message: '請填寫稅籍編號', trigger: 'blur' },
    { len: 9, message: '稅籍編號須為 9 碼', trigger: 'blur' }
  ],
  owner: [{ required: true, message: '請填寫負責人', trigger: 'blur' }],
  registeredAddress: [{ required: true, message: '請填寫登記地址', trigger: 'blur' }]
}

function applyCompany(data = {}) {
  Object.assign(form, {
    isActive: data.isActive ?? true,
    attribName: data.attribName ?? '',
    name: data.name ?? '',
    taxInvoiceNumber: data.taxInvoiceNumber ?? '',
    taxSerialNumber: data.taxSerialNumber ?? '',
    owner: data.owner ?? '',
    contactPhone: data.contactPhone ?? '',
    faxPhone: data.faxPhone ?? '',
    registeredAddress: data.registeredAddress ?? '',
    deliveryAddress: data.deliveryAddress ?? '',
    taxInvoiceAddress: data.taxInvoiceAddress ?? '',
    bankName: data.bankName ?? '',
    checkingAccount: data.checkingAccount ?? '',
    remittanceAccount: data.remittanceAccount ?? '',
    payDays: data.payDays ?? '',
    foundedDate: toDateOnly(data.foundedDate),
    invoiceForm: data.invoiceForm ?? ''
  })
}

// 選填欄位空字串送出前轉 null，讓後端存成空值
function optionalText(value) {
  return value?.trim() || null
}

function buildPayload() {
  return {
    id: companyId.value ?? 0,
    attribName: form.attribName.trim(),
    name: form.name.trim(),
    taxInvoiceNumber: form.taxInvoiceNumber.trim(),
    taxSerialNumber: form.taxSerialNumber.trim(),
    owner: form.owner.trim(),
    contactPhone: optionalText(form.contactPhone),
    faxPhone: optionalText(form.faxPhone),
    registeredAddress: form.registeredAddress.trim(),
    deliveryAddress: optionalText(form.deliveryAddress),
    taxInvoiceAddress: optionalText(form.taxInvoiceAddress),
    bankName: optionalText(form.bankName),
    checkingAccount: optionalText(form.checkingAccount),
    remittanceAccount: optionalText(form.remittanceAccount),
    payDays: optionalText(form.payDays),
    foundedDate: form.foundedDate || null,
    invoiceForm: optionalText(form.invoiceForm)
  }
}

async function loadCompany() {
  formError.value = ''
  Object.assign(form, createEmptyForm())
  if (isCreate.value) return

  loading.value = true
  try {
    const response = await companyApi.get(companyId.value)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法取得公司資料')
      ElMessage.error(formError.value)
      return
    }
    applyCompany(unwrap(response) ?? {})
  } catch (error) {
    formError.value = getApiErrorMessage(error, '無法取得公司資料')
    ElMessage.error(formError.value)
  } finally {
    loading.value = false
  }
}

async function save() {
  try {
    await formRef.value.validate()
  } catch {
    return // 欄位驗證未通過，錯誤已顯示在欄位上
  }

  saving.value = true
  formError.value = ''

  try {
    const response = isCreate.value
      ? await companyApi.add(buildPayload())
      : await companyApi.edit(buildPayload())
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '儲存失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('儲存成功')
    if (isCreate.value) {
      // 新增成功後回列表，列表預設把啟用中的公司排在前面
      await router.replace({ name: 'company-list' })
    } else {
      await loadCompany()
    }
  } catch (error) {
    formError.value = getApiErrorMessage(error, '儲存失敗')
    ElMessage.error(formError.value)
  } finally {
    saving.value = false
  }
}

// 停用會讓公司不再出現在填單選項，需要確認；啟用沒有風險，直接執行
async function toggleActive() {
  const disabling = form.isActive
  if (disabling) {
    try {
      await ElMessageBox.confirm(
        `確定要停用「${form.attribName}」？停用後新單據不能再選這間公司，已填的單據仍顯示原公司。`,
        '停用公司',
        { confirmButtonText: '停用', cancelButtonText: '取消', type: 'warning' }
      )
    } catch {
      return // 使用者取消
    }
  }

  toggling.value = true
  formError.value = ''

  try {
    const response = disabling ? await companyApi.disable(companyId.value) : await companyApi.enable(companyId.value)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, disabling ? '停用失敗' : '啟用失敗')
      ElMessage.error(formError.value)
      return
    }
    ElMessage.success(disabling ? '已停用' : '已啟用')
  } catch (error) {
    formError.value = getApiErrorMessage(error, disabling ? '停用失敗' : '啟用失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    toggling.value = false
  }

  // 動作已成功，重新載入失敗由 loadCompany 自行顯示錯誤
  await loadCompany()
}

async function deleteCompany() {
  try {
    await ElMessageBox.confirm(
      `確定要刪除「${form.attribName}」？此操作無法復原；已被單據使用過的公司無法刪除，請改用停用。`,
      '刪除公司',
      { confirmButtonText: '刪除', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  formError.value = ''

  try {
    const response = await companyApi.delete(companyId.value)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '刪除失敗')
      ElMessage.error(formError.value)
      return
    }
    ElMessage.success('已刪除')
  } catch (error) {
    formError.value = getApiErrorMessage(error, '刪除失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    deleting.value = false
  }

  await router.replace({ name: 'company-list' })
}

function goBack() {
  router.push({ name: 'company-list' })
}

onMounted(loadCompany)
</script>

<template>
  <div class="company-edit">
    <div class="page-header">
      <h1>{{ isCreate ? '新增公司' : `公司 ${form.attribName}` }}</h1>
      <el-tag v-if="!isCreate" :type="form.isActive ? 'success' : 'info'">{{ form.isActive ? '啟用中' : '已停用' }}</el-tag>
    </div>

    <el-alert v-if="formError" class="form-alert" type="error" :title="formError" :closable="false" show-icon />

    <el-form
      ref="formRef"
      v-loading="loading"
      class="company-form"
      :model="form"
      :rules="rules"
      label-width="110px"
    >
      <el-form-item label="公司簡稱" prop="attribName">
        <el-input v-model.trim="form.attribName" maxlength="32" show-word-limit placeholder="填單與列表顯示用，不可與其他公司重複" />
      </el-form-item>

      <el-form-item label="公司全名" prop="name">
        <el-input v-model.trim="form.name" maxlength="32" show-word-limit />
      </el-form-item>

      <el-form-item label="統一編號" prop="taxInvoiceNumber">
        <el-input v-model.trim="form.taxInvoiceNumber" maxlength="8" placeholder="8 碼數字，不可與其他公司重複" />
      </el-form-item>

      <el-form-item label="稅籍編號" prop="taxSerialNumber">
        <el-input v-model.trim="form.taxSerialNumber" maxlength="9" placeholder="9 碼" />
      </el-form-item>

      <el-form-item label="負責人" prop="owner">
        <el-input v-model.trim="form.owner" maxlength="32" show-word-limit />
      </el-form-item>

      <el-form-item label="登記地址" prop="registeredAddress">
        <el-input v-model.trim="form.registeredAddress" maxlength="64" show-word-limit />
      </el-form-item>

      <el-form-item label="聯絡電話">
        <el-input v-model.trim="form.contactPhone" maxlength="32" />
      </el-form-item>

      <el-form-item label="傳真">
        <el-input v-model.trim="form.faxPhone" maxlength="32" />
      </el-form-item>

      <el-form-item label="送貨地址">
        <el-input v-model.trim="form.deliveryAddress" maxlength="64" show-word-limit />
      </el-form-item>

      <el-form-item label="發票地址">
        <el-input v-model.trim="form.taxInvoiceAddress" maxlength="64" show-word-limit />
      </el-form-item>

      <el-form-item label="銀行名稱">
        <el-input v-model.trim="form.bankName" maxlength="32" />
      </el-form-item>

      <el-form-item label="支票帳號">
        <el-input v-model.trim="form.checkingAccount" maxlength="32" />
      </el-form-item>

      <el-form-item label="匯款帳號">
        <el-input v-model.trim="form.remittanceAccount" maxlength="32" />
      </el-form-item>

      <el-form-item label="付款天數">
        <el-input v-model.trim="form.payDays" maxlength="8" />
      </el-form-item>

      <el-form-item label="成立日期">
        <el-date-picker v-model="form.foundedDate" type="date" value-format="YYYY-MM-DD" placeholder="選填" />
      </el-form-item>

      <el-form-item label="發票格式">
        <el-input v-model.trim="form.invoiceForm" maxlength="8" />
      </el-form-item>
    </el-form>

    <div class="form-actions">
      <el-button @click="goBack">返回列表</el-button>
      <el-button type="primary" :loading="saving" :disabled="busy" @click="save">儲存</el-button>
      <el-button
        v-if="!isCreate"
        :type="form.isActive ? 'warning' : 'success'"
        :loading="toggling"
        :disabled="busy"
        @click="toggleActive"
      >
        {{ form.isActive ? '停用' : '啟用' }}
      </el-button>
      <el-button v-if="!isCreate" type="danger" :loading="deleting" :disabled="busy" @click="deleteCompany">
        刪除
      </el-button>
    </div>
  </div>
</template>

<style scoped>
.company-edit {
  padding: 20px;
  max-width: 720px;
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

.form-actions {
  display: flex;
  gap: 8px;
  margin-top: 16px;
}
</style>
