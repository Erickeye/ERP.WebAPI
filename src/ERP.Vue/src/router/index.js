import { createRouter, createWebHistory } from 'vue-router'
import Layout from '../components/DashboardLayout.vue'
import DayOffCreate from '@/views/DayOffCreate.vue'
import DayOffList from '@/views/DayOffList.vue'
import BusinessTripList from '@/views/BusinessTripList.vue'
import BusinessTripEdit from '@/views/BusinessTripEdit.vue'
import CompanyList from '@/views/CompanyList.vue'
import CompanyEdit from '@/views/CompanyEdit.vue'
import LoginPage from '../views/LoginPage.vue'

const routes = [
  { path: '/', name: 'login', component: LoginPage },
  { path: '/layout', name: 'layout', component: Layout },
  { path: '/dayoff/create', component: DayOffCreate },
  { path: '/dayoff/index', component: DayOffList },
  { path: '/business-trip/index', name: 'business-trip-list', component: BusinessTripList },
  { path: '/business-trip/create', name: 'business-trip-create', component: BusinessTripEdit },
  { path: '/business-trip/:id(\\d+)', name: 'business-trip-edit', component: BusinessTripEdit },
  { path: '/company/index', name: 'company-list', component: CompanyList },
  { path: '/company/create', name: 'company-create', component: CompanyEdit },
  { path: '/company/:id(\\d+)', name: 'company-edit', component: CompanyEdit }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('jwt')

  if (to.name !== 'login' && !token) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router