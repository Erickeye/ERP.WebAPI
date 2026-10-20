<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { businessTripApi } from '@/api/businessTripApi'
import { companyApi } from '@/api/companyApi'
import { sharedApi } from '@/api/sharedApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'
import { BUSINESS_TRIP_STATUS_OPTIONS, businessTripStatusTagType } from '@/utils/businessTrip'
import { toDateOnly } from '@/utils/dateUtils'

const router = useRouter()

const loading = ref(false)
const pageError = ref('')
const rows = ref([])
const search = reactive({
  pageNumber: 1,
  pageSize: 10,
  sortColumn: '',
  sortDirection: 'asc',
  // 單號關鍵字、出差人、出差單狀態
  no: '',
  travelerStaffId: null,
  companyId: null,
  status: null,
  // el-date-picker daterange 的值：[區間開始, 區間結束]，以「與出差起訖有交集」查詢
  dateRange: [],
  // 只看輪到我簽核的出差單
  pendingMyApproval: false
})
const staffOptions = ref([])
// 公司篩選選單：只列啟用中的公司（已停用公司的歷史單據仍會顯示在列表，只是不能用它篩選）
const companyOptions = ref([])
const pagination = reactive({ totalCount: 0 })

function normalizeRow(row = {}) {
  return {
    id: row.id,
    no: row.no ?? '',
    companyName: row.companyName ?? '',
    travelerName: row.travelerName ?? '',
    destination: row.destination ?? '',
    reason: row.reason ?? '',
    startDate: toDateOnly(row.startDate),
    endDate: toDateOnly(row.endDate),
    days: row.days ?? 0,
    status: row.status,
    statusDisplay: row.statusDisplay ?? '',
    canApprove: row.canApprove ?? false,
    createUserName: row.createUserName ?? ''
  }
}

async function loadStaffOptions() {
  try {
    const response = await sharedApi.getStaffSelect()
    if (isSuccess(response)) {
      staffOptions.value = unwrap(response)?.items ?? []
    }
  } catch (error) {
    // 出差人下拉載入失敗不影響列表查詢，只提示
    ElMessage.warning(getApiErrorMessage(error, '無法取得出差人下拉選單'))
  }
}

async function loadCompanyOptions() {
  try {
    const response = await companyApi.getSelect()
    if (isSuccess(response)) {
      companyOptions.value = unwrap(response)?.items ?? []
    }
  } catch (error) {
    // 公司下拉載入失敗不影響列表查詢，只提示
    ElMessage.warning(getApiErrorMessage(error, '無法取得公司下拉選單'))
  }
}

function buildQuery() {
  const [periodStart, periodEnd] = search.dateRange ?? []

  return {
    pageNumber: search.pageNumber,
    pageSize: search.pageSize,
    // 空字串送出前轉 undefined，讓後端使用預設排序（出差起日由新到舊）
    sortColumn: search.sortColumn || undefined,
    sortDirection: search.sortColumn ? search.sortDirection : undefined,
    // 空字串、null 與未選日期送出前轉 undefined，讓後端略過該條件（狀態 0 是草稿，不能用 || 轉換）
    no: search.no.trim() || undefined,
    travelerStaffId: search.travelerStaffId ?? undefined,
    companyId: search.companyId ?? undefined,
    status: search.status ?? undefined,
    periodStart: periodStart || undefined,
    periodEnd: periodEnd || undefined,
    pendingMyApproval: search.pendingMyApproval || undefined
  }
}

async function loadRows() {
  loading.value = true
  pageError.value = ''

  try {
    const response = await businessTripApi.index(buildQuery())
    if (!isSuccess(response)) {
      pageError.value = getApiErrorMessage(response, '無法取得出差單列表')
      ElMessage.error(pageError.value)
      return
    }

    const data = unwrap(response) ?? {}
    rows.value = (data.items ?? []).map(normalizeRow)
    pagination.totalCount = data.totalCount ?? 0
  } catch (error) {
    pageError.value = getApiErrorMessage(error, '無法取得出差單列表')
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

// 查詢按鈕、Enter 與篩選條件改變都走這裡，回到第 1 頁
function searchRows() {
  search.pageNumber = 1
  return loadRows()
}

function resetSearch() {
  search.no = ''
  search.travelerStaffId = null
  search.companyId = null
  search.status = null
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
  router.push({ name: 'business-trip-create' })
}

function openRow(row) {
  router.push({ name: 'business-trip-edit', params: { id: row.id } })
}

onMounted(() => {
  loadStaffOptions()
  loadCompanyOptions()
  loadRows()
})
</script>

<template>
  <div class="business-trip-list">
    <div class="page-header">
      <h1>出差單</h1>
      <el-button type="primary" @click="openCreate">新增出差單</el-button>
    </div>

    <el-form class="search-bar" inline @submit.prevent="searchRows">
      <el-form-item label="單號">
        <el-input v-model="search.no" placeholder="單號關鍵字" clearable />
      </el-form-item>
      <el-form-item label="公司">
        <el-select v-model="search.companyId" placeholder="全部" clearable filterable>
          <el-option v-for="company in companyOptions" :key="company.value" :label="company.text" :value="company.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="出差人">
        <el-select v-model="search.travelerStaffId" placeholder="全部" clearable filterable>
          <el-option v-for="staff in staffOptions" :key="staff.value" :label="staff.text" :value="staff.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="狀態">
        <el-select v-model="search.status" placeholder="全部" clearable>
          <el-option
            v-for="option in BUSINESS_TRIP_STATUS_OPTIONS"
            :key="option.value"
            :label="option.label"
            :value="option.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="出差期間">
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
      empty-text="沒有出差單"
      @sort-change="changeSort"
    >
      <el-table-column prop="no" label="單號" min-width="130" sortable="custom" />
      <el-table-column prop="companyName" label="公司" min-width="110" />
      <el-table-column prop="travelerName" label="出差人" min-width="100" />
      <el-table-column prop="destination" label="出差地點" min-width="140" />
      <el-table-column prop="reason" label="出差事由" min-width="180" show-overflow-tooltip />
      <el-table-column prop="startDate" label="起日" width="120" sortable="custom" />
      <el-table-column prop="endDate" label="迄日" width="120" sortable="custom" />
      <el-table-column prop="days" label="天數" width="80" align="right" />
      <el-table-column prop="status" label="狀態" width="170" sortable="custom">
        <template #default="{ row }">
          <el-tag :type="businessTripStatusTagType(row.status)">{{ row.statusDisplay }}</el-tag>
          <el-tag v-if="row.canApprove" class="pending-tag" type="danger" effect="plain">待我簽核</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createUserName" label="建單人" min-width="100" />
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
.business-trip-list {
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

.pending-tag {
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
