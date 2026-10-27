<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { companyApi } from '@/api/companyApi'
import { contractApi } from '@/api/contractApi'
import { sharedApi } from '@/api/sharedApi'
import { supplierApi } from '@/api/supplierApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import { hasPermission } from '@/utils/auth'
import {
  CONTRACT_STATUS_OPTIONS,
  CONTRACT_TYPE_OPTIONS,
  CONTRACT_VALIDITY_OPTIONS,
  contractStatusTagType,
  contractValidityTagType
} from '@/utils/contract'
import { toDateOnly } from '@/utils/dateUtils'
import { PERMISSION } from '@/utils/permissions'

const router = useRouter()

const canEdit = hasPermission(PERMISSION.CONTRACT_EDIT)

const loading = ref(false)
const pageError = ref('')
const rows = ref([])
const search = reactive({
  pageNumber: 1,
  pageSize: 10,
  sortColumn: '',
  sortDirection: 'asc',
  // 單號或合約編號關鍵字
  keyword: '',
  type: null,
  supplierId: null,
  customerId: null,
  companyId: null,
  status: null,
  validity: null,
  // el-date-picker daterange 的值：[區間開始, 區間結束]，以「與合約起訖有交集」查詢
  dateRange: [],
  // 只看輪到我簽核的合約單
  pendingMyApproval: false
})
const supplierOptions = ref([])
const customerOptions = ref([])
// 公司篩選選單：只列啟用中的公司（已停用公司的歷史單據仍會顯示在列表，只是不能用它篩選）
const companyOptions = ref([])
const pagination = reactive({ totalCount: 0 })

function normalizeRow(row = {}) {
  return {
    id: row.id,
    no: row.no ?? '',
    contractNo: row.contractNo ?? '',
    typeDisplay: row.typeDisplay ?? '',
    partyName: row.partyName ?? '',
    companyName: row.companyName ?? '',
    startDate: toDateOnly(row.startDate),
    endDate: toDateOnly(row.endDate),
    amount: row.amount ?? 0,
    status: row.status,
    statusDisplay: row.statusDisplay ?? '',
    validity: row.validity ?? null,
    validityDisplay: row.validityDisplay ?? '',
    canApprove: row.canApprove ?? false
  }
}

function formatAmount(value) {
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 下拉選單載入失敗不影響列表查詢，只提示
async function loadSelect(request, target, message) {
  try {
    const response = await request()
    if (isSuccess(response)) {
      target.value = unwrap(response)?.items ?? []
    } else {
      ElMessage.warning(getApiErrorMessage(response, message))
    }
  } catch (error) {
    ElMessage.warning(getApiErrorMessage(error, message))
  }
}

function loadOptions() {
  loadSelect(() => supplierApi.getSelect(), supplierOptions, '無法取得供應商下拉選單')
  loadSelect(() => sharedApi.getCustomerSelect(), customerOptions, '無法取得客戶下拉選單')
  loadSelect(() => companyApi.getSelect(), companyOptions, '無法取得公司下拉選單')
}

function buildQuery() {
  const [periodStart, periodEnd] = search.dateRange ?? []

  return {
    pageNumber: search.pageNumber,
    pageSize: search.pageSize,
    // 空字串送出前轉 undefined，讓後端使用預設排序（合約起日由新到舊）
    sortColumn: search.sortColumn || undefined,
    sortDirection: search.sortColumn ? search.sortDirection : undefined,
    // 空字串、null 與未選日期送出前轉 undefined，讓後端略過該條件（類型與狀態 0 是有效值，不能用 || 轉換）
    keyword: search.keyword.trim() || undefined,
    type: search.type ?? undefined,
    supplierId: search.supplierId ?? undefined,
    customerId: search.customerId ?? undefined,
    companyId: search.companyId ?? undefined,
    status: search.status ?? undefined,
    validity: search.validity ?? undefined,
    periodStart: periodStart || undefined,
    periodEnd: periodEnd || undefined,
    pendingMyApproval: search.pendingMyApproval || undefined
  }
}

async function loadRows() {
  loading.value = true
  pageError.value = ''

  try {
    const response = await contractApi.index(buildQuery())
    if (!isSuccess(response)) {
      pageError.value = getApiErrorMessage(response, '無法取得合約單列表')
      ElMessage.error(pageError.value)
      return
    }

    const data = unwrap(response) ?? {}
    rows.value = (data.items ?? []).map(normalizeRow)
    pagination.totalCount = data.totalCount ?? 0
  } catch (error) {
    pageError.value = getApiErrorMessage(error, '無法取得合約單列表')
    ElMessage.error(pageError.value)
  } finally {
    loading.value = false
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
  search.type = null
  search.supplierId = null
  search.customerId = null
  search.companyId = null
  search.status = null
  search.validity = null
  search.dateRange = []
  search.pendingMyApproval = false
  return searchRows()
}

function changeSort({ prop, order }) {
  search.sortColumn = order ? prop : ''
  search.sortDirection = order === 'descending' ? 'desc' : 'asc'
  search.pageNumber = 1
  loadRows()
}

function openCreate() {
  router.push({ name: 'contract-create' })
}

function openRow(row) {
  router.push({ name: 'contract-edit', params: { id: row.id } })
}

onMounted(() => {
  loadOptions()
  loadRows()
})
</script>

<template>
  <div class="contract-list">
    <div class="page-header">
      <h1>合約單</h1>
      <el-button v-if="canEdit" type="primary" @click="openCreate">新增合約單</el-button>
    </div>

    <el-form class="search-bar" inline @submit.prevent="searchRows">
      <el-form-item label="關鍵字">
        <el-input v-model="search.keyword" placeholder="單號或合約編號" clearable />
      </el-form-item>
      <el-form-item label="類型">
        <el-select v-model="search.type" placeholder="全部" clearable>
          <el-option v-for="option in CONTRACT_TYPE_OPTIONS" :key="option.value" :label="option.label" :value="option.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="供應商">
        <el-select v-model="search.supplierId" placeholder="全部" clearable filterable>
          <el-option v-for="supplier in supplierOptions" :key="supplier.value" :label="supplier.text" :value="supplier.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="客戶">
        <el-select v-model="search.customerId" placeholder="全部" clearable filterable>
          <el-option v-for="customer in customerOptions" :key="customer.value" :label="customer.text" :value="customer.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="簽約公司">
        <el-select v-model="search.companyId" placeholder="全部" clearable filterable>
          <el-option v-for="company in companyOptions" :key="company.value" :label="company.text" :value="company.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="狀態">
        <el-select v-model="search.status" placeholder="全部" clearable>
          <el-option v-for="option in CONTRACT_STATUS_OPTIONS" :key="option.value" :label="option.label" :value="option.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="有效性">
        <el-select v-model="search.validity" placeholder="全部" clearable>
          <el-option v-for="option in CONTRACT_VALIDITY_OPTIONS" :key="option.value" :label="option.label" :value="option.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="合約期間">
        <el-date-picker
          v-model="search.dateRange"
          type="daterange"
          value-format="YYYY-MM-DD"
          range-separator="至"
          start-placeholder="開始"
          end-placeholder="結束"
        />
      </el-form-item>
      <el-form-item>
        <el-checkbox v-model="search.pendingMyApproval">只看待我簽核</el-checkbox>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" native-type="submit" :loading="loading">查詢</el-button>
        <el-button :disabled="loading" @click="resetSearch">重設</el-button>
      </el-form-item>
    </el-form>

    <el-alert v-if="pageError" class="page-error" type="error" :title="pageError" :closable="false" show-icon />

    <el-table
      v-loading="loading"
      :data="rows"
      row-key="id"
      empty-text="沒有合約單"
      @sort-change="changeSort"
    >
      <el-table-column prop="no" label="單號" min-width="130" sortable="custom" />
      <el-table-column prop="contractNo" label="合約編號" min-width="130" sortable="custom" />
      <el-table-column prop="typeDisplay" label="類型" width="110" />
      <el-table-column prop="partyName" label="合約對象" min-width="140" show-overflow-tooltip />
      <el-table-column prop="companyName" label="簽約公司" min-width="110" />
      <el-table-column prop="startDate" label="起日" width="120" sortable="custom" />
      <el-table-column prop="endDate" label="迄日" width="120" sortable="custom" />
      <el-table-column prop="amount" label="合約總金額" width="140" align="right" sortable="custom">
        <template #default="{ row }">{{ formatAmount(row.amount) }}</template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="230" sortable="custom">
        <template #default="{ row }">
          <el-tag :type="contractStatusTagType(row.status)">{{ row.statusDisplay }}</el-tag>
          <el-tag v-if="row.validityDisplay" class="row-tag" :type="contractValidityTagType(row.validity)" effect="plain">
            {{ row.validityDisplay }}
          </el-tag>
          <el-tag v-if="row.canApprove" class="row-tag" type="danger" effect="plain">待我簽核</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="90" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="openRow(row)">檢視</el-button>
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
  </div>
</template>

<style scoped>
.contract-list {
  padding: 20px;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.search-bar {
  margin-bottom: 4px;
}

.row-tag {
  margin-left: 6px;
}

.page-error {
  margin-bottom: 16px;
}

.list-pagination {
  margin-top: 16px;
  justify-content: flex-end;
}
</style>
