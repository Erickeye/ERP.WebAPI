<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { supplierApi } from '@/api/supplierApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'

const route = useRoute()
const router = useRouter()

const formRef = ref(null)
const loading = ref(false)
const saving = ref(false)
const toggling = ref(false)
const deleting = ref(false)
const formError = ref('')

const supplierId = computed(() => (route.params.id ? Number(route.params.id) : null))
const isCreate = computed(() => supplierId.value === null)
const busy = computed(() => loading.value || saving.value || toggling.value || deleting.value)

function createEmptyForm() {
  return {
    // 新增的供應商一律啟用中；既有供應商由後端回傳
    isActive: true,
    no: '',
    name: '',
    taxIdNumber: '',
    owner: '',
    contactPhone: '',
    faxPhone: '',
    salesperson: '',
    address: '',
    bankName: '',
    subBankName: '',
    checkingAccount: '',
    remittanceAccount: '',
    payDays: null,
    creditLine: null
  }
}

const form = reactive(createEmptyForm())

// 必填與格式規則對應後端 SupplierAddVM；後端才是最後一道防線
const rules = {
  no: [{ required: true, message: '請填寫供應商編號', trigger: 'blur' }],
  name: [{ required: true, message: '請填寫名稱', trigger: 'blur' }],
  // 統一編號選填，有填才檢查 8 碼數字
  taxIdNumber: [{ pattern: /^\d{8}$/, message: '統一編號須為 8 碼數字', trigger: 'blur' }]
}

function applySupplier(data = {}) {
  Object.assign(form, {
    isActive: data.isActive ?? true,
    no: data.no ?? '',
    name: data.name ?? '',
    taxIdNumber: data.taxIdNumber ?? '',
    owner: data.owner ?? '',
    contactPhone: data.contactPhone ?? '',
    faxPhone: data.faxPhone ?? '',
    salesperson: data.salesperson ?? '',
    address: data.address ?? '',
    bankName: data.bankName ?? '',
    subBankName: data.subBankName ?? '',
    checkingAccount: data.checkingAccount ?? '',
    remittanceAccount: data.remittanceAccount ?? '',
    payDays: data.payDays ?? null,
    creditLine: data.creditLine ?? null
  })
}

// 選填欄位空字串送出前轉 null，讓後端存成空值
function optionalText(value) {
  return value?.trim() || null
}

function buildPayload() {
  return {
    id: supplierId.value ?? 0,
    no: form.no.trim(),
    name: form.name.trim(),
    taxIdNumber: optionalText(form.taxIdNumber),
    owner: optionalText(form.owner),
    contactPhone: optionalText(form.contactPhone),
    faxPhone: optionalText(form.faxPhone),
    salesperson: optionalText(form.salesperson),
    address: optionalText(form.address),
    bankName: optionalText(form.bankName),
    subBankName: optionalText(form.subBankName),
    checkingAccount: optionalText(form.checkingAccount),
    remittanceAccount: optionalText(form.remittanceAccount),
    // 數字欄位清空時 el-input-number 會是 null，直接送 null
    payDays: form.payDays ?? null,
    creditLine: form.creditLine ?? null
  }
}

async function loadSupplier() {
  formError.value = ''
  Object.assign(form, createEmptyForm())
  if (isCreate.value) return

  loading.value = true
  try {
    const response = await supplierApi.get(supplierId.value)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法取得供應商資料')
      ElMessage.error(formError.value)
      return
    }
    applySupplier(unwrap(response) ?? {})
  } catch (error) {
    formError.value = getApiErrorMessage(error, '無法取得供應商資料')
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
      ? await supplierApi.add(buildPayload())
      : await supplierApi.edit(buildPayload())
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '儲存失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('儲存成功')
    if (isCreate.value) {
      // 新增成功後回列表，列表預設把啟用中的供應商排在前面
      await router.replace({ name: 'supplier-list' })
    } else {
      await loadSupplier()
    }
  } catch (error) {
    formError.value = getApiErrorMessage(error, '儲存失敗')
    ElMessage.error(formError.value)
  } finally {
    saving.value = false
  }
}

// 停用會讓供應商不再出現在新進貨單的選項，需要確認；啟用沒有風險，直接執行
async function toggleActive() {
  const disabling = form.isActive
  if (disabling) {
    try {
      await ElMessageBox.confirm(
        `確定要停用「${form.name}」？停用後新進貨單不能再選這間供應商，已填的進貨單與庫存仍顯示原供應商。`,
        '停用供應商',
        { confirmButtonText: '停用', cancelButtonText: '取消', type: 'warning' }
      )
    } catch {
      return // 使用者取消
    }
  }

  toggling.value = true
  formError.value = ''

  try {
    const response = disabling ? await supplierApi.disable(supplierId.value) : await supplierApi.enable(supplierId.value)
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

  // 動作已成功，重新載入失敗由 loadSupplier 自行顯示錯誤
  await loadSupplier()
}

async function deleteSupplier() {
  try {
    await ElMessageBox.confirm(
      `確定要刪除「${form.name}」？此操作無法復原；已被進貨單或庫存使用過的供應商無法刪除，請改用停用。`,
      '刪除供應商',
      { confirmButtonText: '刪除', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  formError.value = ''

  try {
    const response = await supplierApi.delete(supplierId.value)
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

  await router.replace({ name: 'supplier-list' })
}

function goBack() {
  router.push({ name: 'supplier-list' })
}

onMounted(loadSupplier)
</script>

<template>
  <div class="supplier-edit">
    <div class="page-header">
      <h1>{{ isCreate ? '新增供應商' : `供應商 ${form.name}` }}</h1>
      <el-tag v-if="!isCreate" :type="form.isActive ? 'success' : 'info'">{{ form.isActive ? '啟用中' : '已停用' }}</el-tag>
    </div>

    <el-alert v-if="formError" class="form-alert" type="error" :title="formError" :closable="false" show-icon />

    <el-form
      ref="formRef"
      v-loading="loading"
      class="supplier-form"
      :model="form"
      :rules="rules"
      label-width="110px"
    >
      <el-form-item label="供應商編號" prop="no">
        <el-input v-model.trim="form.no" maxlength="64" show-word-limit placeholder="不可與其他供應商重複" />
      </el-form-item>

      <el-form-item label="名稱" prop="name">
        <el-input v-model.trim="form.name" maxlength="128" show-word-limit />
      </el-form-item>

      <el-form-item label="統一編號" prop="taxIdNumber">
        <el-input v-model.trim="form.taxIdNumber" maxlength="8" placeholder="選填；有填須為 8 碼數字，不可與其他供應商重複" />
      </el-form-item>

      <el-form-item label="負責人">
        <el-input v-model.trim="form.owner" maxlength="32" show-word-limit />
      </el-form-item>

      <el-form-item label="業務人員">
        <el-input v-model.trim="form.salesperson" maxlength="32" show-word-limit />
      </el-form-item>

      <el-form-item label="聯絡電話">
        <el-input v-model.trim="form.contactPhone" maxlength="32" />
      </el-form-item>

      <el-form-item label="傳真">
        <el-input v-model.trim="form.faxPhone" maxlength="32" />
      </el-form-item>

      <el-form-item label="地址">
        <el-input v-model.trim="form.address" maxlength="128" show-word-limit />
      </el-form-item>

      <el-form-item label="銀行名稱">
        <el-input v-model.trim="form.bankName" maxlength="64" />
      </el-form-item>

      <el-form-item label="分行名稱">
        <el-input v-model.trim="form.subBankName" maxlength="64" />
      </el-form-item>

      <el-form-item label="支票帳號">
        <el-input v-model.trim="form.checkingAccount" maxlength="64" />
      </el-form-item>

      <el-form-item label="匯款帳號">
        <el-input v-model.trim="form.remittanceAccount" maxlength="64" />
      </el-form-item>

      <el-form-item label="付款天數">
        <el-input-number v-model="form.payDays" :min="0" :max="365" :step="1" :precision="0" controls-position="right" />
      </el-form-item>

      <el-form-item label="信用額度">
        <el-input-number v-model="form.creditLine" :min="0" :max="9999999999.99" :precision="2" controls-position="right" />
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
      <el-button v-if="!isCreate" type="danger" :loading="deleting" :disabled="busy" @click="deleteSupplier">
        刪除
      </el-button>
    </div>
  </div>
</template>

<style scoped>
.supplier-edit {
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
