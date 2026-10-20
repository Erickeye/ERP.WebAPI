<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { companyApi } from '@/api/companyApi'
import { getApiErrorMessage, isSuccess, unwrap } from '@/utils/api'

const router = useRouter()

const loading = ref(false)
// 停用、啟用、刪除各自維護狀態，並記下正在處理的公司，避免同一列重複送出
const toggling = ref(false)
const deleting = ref(false)
const actingId = ref(null)
const pageError = ref('')
const rows = ref([])
const search = reactive({
  pageNumber: 1,
  pageSize: 10,
  sortColumn: '',
  sortDirection: 'asc',
  // 關鍵字同時比對公司簡稱、公司全名與統一編號
  keyword: '',
  // null 代表全部；true 啟用中；false 已停用
  isActive: null
})
const pagination = reactive({ totalCount: 0 })

const ACTIVE_OPTIONS = [
  { value: true, label: '啟用中' },
  { value: false, label: '已停用' }
]

function normalizeRow(row = {}) {
  return {
    id: row.id,
    attribName: row.attribName ?? '',
    name: row.name ?? '',
    taxInvoiceNumber: row.taxInvoiceNumber ?? '',
    owner: row.owner ?? '',
    isActive: row.isActive ?? false
  }
}

function buildQuery() {
  return {
    pageNumber: search.pageNumber,
    pageSize: search.pageSize,
    // 空字串送出前轉 undefined，讓後端使用預設排序（啟用中的排前面）
    sortColumn: search.sortColumn || undefined,
    sortDirection: search.sortColumn ? search.sortDirection : undefined,
    // 空字串與 null（全部）送出前轉 undefined，讓後端略過該條件（false 是已停用，不能用 || 轉換）
    keyword: search.keyword.trim() || undefined,
    isActive: search.isActive ?? undefined
  }
}

async function loadRows() {
  loading.value = true
  pageError.value = ''

  try {
    const response = await companyApi.index(buildQuery())
    if (!isSuccess(response)) {
      pageError.value = getApiErrorMessage(response, '無法取得公司列表')
      ElMessage.error(pageError.value)
      return
    }

    const data = unwrap(response) ?? {}
    rows.value = (data.items ?? []).map(normalizeRow)
    pagination.totalCount = data.totalCount ?? 0
  } catch (error) {
    pageError.value = getApiErrorMessage(error, '無法取得公司列表')
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
  search.keyword = ''
  search.isActive = null
  return searchRows()
}

function changeSort({ prop, order }) {
  search.sortColumn = order ? prop : ''
  search.sortDirection = order === 'descending' ? 'desc' : 'asc'
  search.pageNumber = 1
  loadRows()
}

function openCreate() {
  router.push({ name: 'company-create' })
}

function openRow(row) {
  router.push({ name: 'company-edit', params: { id: row.id } })
}

// 停用會讓公司不再出現在填單選項，需要確認；啟用沒有風險，直接執行
async function toggleActive(row) {
  if (row.isActive) {
    try {
      await ElMessageBox.confirm(
        `確定要停用「${row.attribName}」？停用後新單據不能再選這間公司，已填的單據仍顯示原公司。`,
        '停用公司',
        { confirmButtonText: '停用', cancelButtonText: '取消', type: 'warning' }
      )
    } catch {
      return // 使用者取消
    }
  }

  toggling.value = true
  actingId.value = row.id

  try {
    const response = row.isActive ? await companyApi.disable(row.id) : await companyApi.enable(row.id)
    if (!isSuccess(response)) {
      ElMessage.error(getApiErrorMessage(response, row.isActive ? '停用失敗' : '啟用失敗'))
      return
    }
    ElMessage.success(row.isActive ? '已停用' : '已啟用')
  } catch (error) {
    ElMessage.error(getApiErrorMessage(error, row.isActive ? '停用失敗' : '啟用失敗'))
    return
  } finally {
    toggling.value = false
    actingId.value = null
  }

  // 動作已成功，重新載入失敗由 loadRows 自行顯示錯誤
  await loadRows()
}

async function deleteRow(row) {
  try {
    await ElMessageBox.confirm(
      `確定要刪除「${row.attribName}」？此操作無法復原；已被單據使用過的公司無法刪除，請改用停用。`,
      '刪除公司',
      { confirmButtonText: '刪除', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  actingId.value = row.id

  try {
    const response = await companyApi.delete(row.id)
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

  await loadRows()
}

onMounted(loadRows)
</script>

<template>
  <div class="company-list">
    <div class="page-header">
      <h1>公司管理</h1>
      <el-button type="primary" @click="openCreate">新增公司</el-button>
    </div>

    <el-form class="search-bar" inline @submit.prevent="searchRows">
      <el-form-item label="關鍵字">
        <el-input v-model="search.keyword" placeholder="公司簡稱、全名或統一編號" clearable />
      </el-form-item>
      <el-form-item label="狀態">
        <el-select v-model="search.isActive" placeholder="全部" clearable>
          <el-option v-for="option in ACTIVE_OPTIONS" :key="option.label" :label="option.label" :value="option.value" />
        </el-select>
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
      empty-text="沒有公司"
      @sort-change="changeSort"
    >
      <el-table-column prop="attribName" label="公司簡稱" min-width="120" sortable="custom" />
      <el-table-column prop="name" label="公司全名" min-width="200" show-overflow-tooltip />
      <el-table-column prop="taxInvoiceNumber" label="統一編號" width="120" sortable="custom" />
      <el-table-column prop="owner" label="負責人" min-width="100" />
      <el-table-column prop="isActive" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="row.isActive ? 'success' : 'info'">{{ row.isActive ? '啟用中' : '已停用' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="220" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="openRow(row)">編輯</el-button>
          <el-button
            link
            :type="row.isActive ? 'warning' : 'success'"
            :loading="toggling && actingId === row.id"
            :disabled="toggling || deleting"
            @click="toggleActive(row)"
          >
            {{ row.isActive ? '停用' : '啟用' }}
          </el-button>
          <el-button
            link
            type="danger"
            :loading="deleting && actingId === row.id"
            :disabled="toggling || deleting"
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
  </div>
</template>

<style scoped>
.company-list {
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

.page-error {
  margin-bottom: 16px;
}

.list-pagination {
  margin-top: 16px;
  justify-content: flex-end;
}
</style>
