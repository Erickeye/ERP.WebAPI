<script setup>
import { computed, nextTick, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { supplierApi } from '@/api/supplierApi'
import { supplierPriceApi } from '@/api/supplierPriceApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import { hasPermission } from '@/utils/auth'
import { formatDateTime } from '@/utils/dateUtils'
import { PERMISSION } from '@/utils/permissions'

const route = useRoute()
const router = useRouter()

// 按鈕依權限顯示（後端仍會驗證）
const canEdit = hasPermission(PERMISSION.SUPPLIER_EDIT)
const canDelete = hasPermission(PERMISSION.SUPPLIER_DELETE)

const supplierId = computed(() => Number(route.params.supplierId))

const supplierLoading = ref(false)
const supplier = ref(null)
const supplierError = ref('')

const loading = ref(false)
const saving = ref(false)
const loadingDetail = ref(false)
const deleting = ref(false)
const actingId = ref(null)
const pageError = ref('')
const rows = ref([])
const search = reactive({
  pageNumber: 1,
  pageSize: 10,
  sortColumn: '',
  sortDirection: 'asc',
  // 關鍵字同時比對料號與品名
  keyword: ''
})
const pagination = reactive({ totalCount: 0 })

const dialogVisible = ref(false)
const dialogMode = ref('add')
const formRef = ref(null)
const formError = ref('')
const form = reactive(createEmptyForm())

const isEditMode = computed(() => dialogMode.value === 'edit')
const dialogTitle = computed(() => (isEditMode.value ? '編輯供應商報價' : '新增供應商報價'))
const busy = computed(() => saving.value || loadingDetail.value)

function validatePrice(rule, value, callback) {
  if (value === null || value === undefined || value === '') {
    callback(new Error('請填寫單價'))
  } else if (!(value > 0)) {
    callback(new Error('單價須大於 0'))
  } else {
    callback()
  }
}

// 必填與長度規則對應後端 SupplierPriceAddVM；後端才是最後一道防線
const rules = {
  no: [
    { required: true, message: '請填寫料號', trigger: 'blur' },
    { max: 64, message: '料號長度不可超過 64 個字元', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '請填寫品名', trigger: 'blur' },
    { max: 128, message: '品名長度不可超過 128 個字元', trigger: 'blur' }
  ],
  unit: [
    { required: true, message: '請填寫單位', trigger: 'blur' },
    { max: 16, message: '單位長度不可超過 16 個字元', trigger: 'blur' }
  ],
  price: [{ validator: validatePrice, trigger: 'change' }],
  note: [{ max: 256, message: '備註長度不可超過 256 個字元', trigger: 'blur' }]
}

function createEmptyForm() {
  return { id: 0, no: '', name: '', unit: '', price: null, note: '' }
}

function normalizeRow(row = {}) {
  return {
    id: row.id,
    no: row.no ?? '',
    name: row.name ?? '',
    unit: row.unit ?? '',
    price: row.price ?? null,
    note: row.note ?? '',
    updateTime: row.updateTime ?? ''
  }
}

function normalizeSupplier(data = {}) {
  return { id: data.id, no: data.no ?? '', name: data.name ?? '' }
}

function formatPrice(price) {
  if (price === null || price === undefined) return ''
  return Number(price).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function buildQuery() {
  return {
    supplierId: supplierId.value,
    pageNumber: search.pageNumber,
    pageSize: search.pageSize,
    // 空字串送出前轉 undefined，讓後端使用預設排序（料號）
    sortColumn: search.sortColumn || undefined,
    sortDirection: search.sortColumn ? search.sortDirection : undefined,
    keyword: search.keyword.trim() || undefined
  }
}

function buildPayload() {
  return {
    id: form.id,
    supplierId: supplierId.value,
    no: form.no.trim(),
    name: form.name.trim(),
    unit: form.unit.trim(),
    price: form.price,
    // 備註清空時送 null，讓後端存成空值
    note: form.note?.trim() || null
  }
}

async function loadSupplier() {
  supplierLoading.value = true
  supplierError.value = ''
  supplier.value = null

  try {
    const response = await supplierApi.get(supplierId.value)
    if (!isSuccess(response)) {
      supplierError.value = getApiErrorMessage(response, '找不到這間供應商，可能已被刪除')
      ElMessage.error(supplierError.value)
      return false
    }
    supplier.value = normalizeSupplier(unwrap(response) ?? {})
    return true
  } catch (error) {
    supplierError.value = getApiErrorMessage(error, '無法取得供應商資料')
    ElMessage.error(supplierError.value)
    return false
  } finally {
    supplierLoading.value = false
  }
}

// 後送先回時只採用最新一次查詢的結果（切換供應商或快速翻頁）
let latestLoadId = 0

async function loadRows() {
  const loadId = ++latestLoadId
  loading.value = true
  pageError.value = ''

  try {
    const response = await supplierPriceApi.index(buildQuery())
    if (loadId !== latestLoadId) return
    if (!isSuccess(response)) {
      pageError.value = getApiErrorMessage(response, '無法取得供應商報價列表')
      ElMessage.error(pageError.value)
      return
    }

    const data = unwrap(response) ?? {}
    rows.value = (data.items ?? []).map(normalizeRow)
    pagination.totalCount = data.totalCount ?? 0
  } catch (error) {
    if (loadId !== latestLoadId) return
    pageError.value = getApiErrorMessage(error, '無法取得供應商報價列表')
    ElMessage.error(pageError.value)
  } finally {
    if (loadId === latestLoadId) loading.value = false
  }
}

async function initPage() {
  rows.value = []
  pagination.totalCount = 0
  search.keyword = ''
  search.sortColumn = ''
  search.sortDirection = 'asc'
  search.pageNumber = 1
  pageError.value = ''
  dialogVisible.value = false

  // 供應商不存在就不查報價，只顯示提示
  if (await loadSupplier()) {
    await loadRows()
  }
}

function changePage(pageNumber) {
  search.pageNumber = pageNumber
  loadRows()
}

function changePageSize(pageSize) {
  search.pageSize = pageSize
  search.pageNumber = 1
  loadRows()
}

// 查詢按鈕與 Enter 都走這裡，回到第 1 頁
function searchRows() {
  search.pageNumber = 1
  return loadRows()
}

function resetSearch() {
  search.keyword = ''
  return searchRows()
}

function changeSort({ prop, order }) {
  search.sortColumn = order ? prop : ''
  search.sortDirection = order === 'descending' ? 'desc' : 'asc'
  search.pageNumber = 1
  loadRows()
}

function goBack() {
  router.push({ name: 'supplier-list' })
}

async function resetFormState() {
  Object.assign(form, createEmptyForm())
  formError.value = ''
  await nextTick()
  formRef.value?.clearValidate()
}

async function openAdd() {
  dialogMode.value = 'add'
  dialogVisible.value = true
  await resetFormState()
}

// 編輯先取單筆再填入，不直接把列表列當完整資料
async function openEdit(row) {
  dialogMode.value = 'edit'
  dialogVisible.value = true
  await resetFormState()

  loadingDetail.value = true
  try {
    const response = await supplierPriceApi.get(row.id)
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '無法取得供應商報價資料')
      ElMessage.error(formError.value)
      return
    }
    const data = normalizeRow(unwrap(response) ?? {})
    Object.assign(form, { id: data.id, no: data.no, name: data.name, unit: data.unit, price: data.price, note: data.note })
  } catch (error) {
    formError.value = getApiErrorMessage(error, '無法取得供應商報價資料')
    ElMessage.error(formError.value)
  } finally {
    loadingDetail.value = false
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
    const response = isEditMode.value
      ? await supplierPriceApi.edit(buildPayload())
      : await supplierPriceApi.add(buildPayload())
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '儲存失敗')
      ElMessage.error(formError.value)
      return
    }

    ElMessage.success('儲存成功')
    dialogVisible.value = false
  } catch (error) {
    formError.value = getApiErrorMessage(error, '儲存失敗')
    ElMessage.error(formError.value)
    return
  } finally {
    saving.value = false
  }

  // 儲存已成功，重新載入失敗由 loadRows 自行顯示錯誤
  await loadRows()
}

async function deleteRow(row) {
  try {
    await ElMessageBox.confirm(`確定要刪除料號「${row.no}」的供應商報價？此操作無法復原。`, '刪除供應商報價', {
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
      type: 'warning'
    })
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  actingId.value = row.id

  try {
    const response = await supplierPriceApi.delete(row.id)
    if (!isSuccess(response)) {
      ElMessage.error(getApiErrorMessage(response, '刪除失敗'))
      return
    }
    ElMessage.success('已刪除')
  } catch (error) {
    ElMessage.error(getApiErrorMessage(error, '刪除失敗'))
    return
  } finally {
    deleting.value = false
    actingId.value = null
  }

  // 刪掉最後一頁的最後一筆時退回上一頁
  if (rows.value.length === 1 && search.pageNumber > 1) {
    search.pageNumber -= 1
  }
  await loadRows()
}

watch(supplierId, initPage, { immediate: true })
</script>

<template>
  <div class="supplier-price-list">
    <div class="page-header">
      <div class="page-title">
        <h1>供應商報價</h1>
        <span v-if="supplier" class="supplier-info">{{ supplier.no }} {{ supplier.name }}</span>
      </div>
      <div class="header-actions">
        <el-button @click="goBack">返回供應商列表</el-button>
        <el-button v-if="canEdit && supplier" type="primary" @click="openAdd">新增供應商報價</el-button>
      </div>
    </div>

    <el-alert v-if="supplierError" class="page-error" type="error" :title="supplierError" :closable="false" show-icon />

    <template v-if="supplier || supplierLoading">
      <el-form class="search-bar" inline @submit.prevent="searchRows">
        <el-form-item label="關鍵字">
          <el-input v-model="search.keyword" placeholder="料號或品名" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" native-type="submit" :loading="loading">查詢</el-button>
          <el-button :disabled="loading" @click="resetSearch">重設</el-button>
        </el-form-item>
      </el-form>

      <el-alert v-if="pageError" class="page-error" type="error" :title="pageError" :closable="false" show-icon />

      <el-table
        v-loading="loading || supplierLoading"
        :data="rows"
        row-key="id"
        empty-text="沒有供應商報價"
        @sort-change="changeSort"
      >
        <el-table-column prop="no" label="料號" min-width="130" sortable="custom" />
        <el-table-column prop="name" label="品名" min-width="200" sortable="custom" show-overflow-tooltip />
        <el-table-column prop="unit" label="單位" width="90" />
        <el-table-column prop="price" label="單價" width="140" align="right" sortable="custom">
          <template #default="{ row }">{{ formatPrice(row.price) }}</template>
        </el-table-column>
        <el-table-column prop="note" label="備註" min-width="160" show-overflow-tooltip />
        <el-table-column prop="updateTime" label="最後修改時間" width="180" sortable="custom">
          <template #default="{ row }">{{ formatDateTime(row.updateTime) }}</template>
        </el-table-column>
        <el-table-column v-if="canEdit || canDelete" label="操作" width="140" fixed="right">
          <template #default="{ row }">
            <el-button v-if="canEdit" link type="primary" :disabled="deleting" @click="openEdit(row)">編輯</el-button>
            <el-button
              v-if="canDelete"
              link
              type="danger"
              :loading="deleting && actingId === row.id"
              :disabled="deleting"
              @click="deleteRow(row)"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        class="list-pagination"
        background
        layout="total, sizes, prev, pager, next"
        :total="pagination.totalCount"
        :current-page="search.pageNumber"
        :page-size="search.pageSize"
        :page-sizes="[10, 20, 50]"
        :disabled="loading"
        @current-change="changePage"
        @size-change="changePageSize"
      />
    </template>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="520px" :close-on-click-modal="false">
      <el-alert v-if="formError" class="form-alert" type="error" :title="formError" :closable="false" show-icon />

      <el-form ref="formRef" v-loading="loadingDetail" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="料號" prop="no">
          <el-input v-model.trim="form.no" maxlength="64" show-word-limit placeholder="同一供應商不可重複" />
        </el-form-item>
        <el-form-item label="品名" prop="name">
          <el-input v-model.trim="form.name" maxlength="128" show-word-limit />
        </el-form-item>
        <el-form-item label="單位" prop="unit">
          <el-input v-model.trim="form.unit" maxlength="16" show-word-limit />
        </el-form-item>
        <el-form-item label="單價" prop="price">
          <el-input-number
            v-model="form.price"
            :min="0"
            :max="9999999999.99"
            :precision="2"
            :step="1"
            controls-position="right"
          />
          <span class="price-hint">新台幣、未稅，須大於 0</span>
        </el-form-item>
        <el-form-item label="備註" prop="note">
          <el-input v-model.trim="form.note" type="textarea" :rows="3" maxlength="256" show-word-limit />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button :disabled="saving" @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" :disabled="busy" @click="save">儲存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.supplier-price-list {
  padding: 20px;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.page-title {
  display: flex;
  align-items: baseline;
  gap: 12px;
}

.supplier-info {
  color: var(--el-text-color-regular);
}

.header-actions {
  display: flex;
  gap: 8px;
}

.search-bar {
  margin-bottom: 4px;
}

.page-error,
.form-alert {
  margin-bottom: 16px;
}

.price-hint {
  margin-left: 12px;
  color: var(--el-text-color-secondary);
  font-size: 12px;
}

.list-pagination {
  margin-top: 16px;
  justify-content: flex-end;
}
</style>
