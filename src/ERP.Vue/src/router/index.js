import { createRouter, createWebHistory } from 'vue-router'
import Layout from '../components/DashboardLayout.vue'
import DayOffCreate from '@/views/DayOffCreate.vue'
import DayOffList from '@/views/DayOffList.vue'
import BusinessTripList from '@/views/BusinessTripList.vue'
import BusinessTripEdit from '@/views/BusinessTripEdit.vue'
import ContractList from '@/views/ContractList.vue'
import ContractEdit from '@/views/ContractEdit.vue'
import CompanyList from '@/views/CompanyList.vue'
import CompanyEdit from '@/views/CompanyEdit.vue'
import SupplierList from '@/views/SupplierList.vue'
import SupplierEdit from '@/views/SupplierEdit.vue'
import SupplierPriceList from '@/views/SupplierPriceList.vue'
import { PERMISSION } from '@/utils/permissions'
import { hasPermission } from '@/utils/auth'
import LoginPage from '../views/LoginPage.vue'

const routes = [
  { path: '/', name: 'login', component: LoginPage },
  { path: '/layout', name: 'layout', component: Layout },
  { path: '/dayoff/create', component: DayOffCreate },
  { path: '/dayoff/index', component: DayOffList },
  { path: '/business-trip/index', name: 'business-trip-list', component: BusinessTripList },
  { path: '/business-trip/create', name: 'business-trip-create', component: BusinessTripEdit },
  { path: '/business-trip/:id(\\d+)', name: 'business-trip-edit', component: BusinessTripEdit },
  // 合約單頁面要「合約單檢視」權限才能進入（前端只是 UX，後端依權限、建單人與簽核紀錄限縮可見範圍）
  { path: '/contract/index', name: 'contract-list', component: ContractList, meta: { permission: PERMISSION.CONTRACT_VIEW } },
  { path: '/contract/create', name: 'contract-create', component: ContractEdit, meta: { permission: PERMISSION.CONTRACT_VIEW } },
  { path: '/contract/:id(\\d+)', name: 'contract-edit', component: ContractEdit, meta: { permission: PERMISSION.CONTRACT_VIEW } },
  { path: '/company/index', name: 'company-list', component: CompanyList },
  { path: '/company/create', name: 'company-create', component: CompanyEdit },
  { path: '/company/:id(\\d+)', name: 'company-edit', component: CompanyEdit },
  { path: '/supplier/index', name: 'supplier-list', component: SupplierList },
  { path: '/supplier/create', name: 'supplier-create', component: SupplierEdit },
  { path: '/supplier/:id(\\d+)', name: 'supplier-edit', component: SupplierEdit },
  {
    path: '/supplier/:supplierId(\\d+)/price',
    name: 'supplier-price-list',
    component: SupplierPriceList,
    meta: { permission: PERMISSION.SUPPLIER_VIEW }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('jwt')

  if (to.name !== 'login' && !token) {
    next({ name: 'login' })
  } else if (to.meta.permission && !hasPermission(to.meta.permission)) {
    // 沒有該頁權限（前端只是 UX，後端仍會驗證）
    next({ name: 'layout' })
  } else {
    next()
  }
})

export default router