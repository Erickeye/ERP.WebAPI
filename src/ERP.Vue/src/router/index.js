import { createRouter, createWebHistory } from 'vue-router'
import Layout from '../components/DashboardLayout.vue'
import DayOffCreate from '@/views/DayOffCreate.vue'
import DayOffList from '@/views/DayOffList.vue'
import LoginPage from '../views/LoginPage.vue'

const routes = [
  { path: '/', name: 'login', component: LoginPage },
  { path: '/layout', name: 'layout', component: Layout },
  { path: '/dayoff/create', component: DayOffCreate },
  { path: '/dayoff/index', component: DayOffList }
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