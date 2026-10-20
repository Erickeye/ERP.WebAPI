<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const activeCard = ref(null)

const go = (id) => {
  router.push({ name: id }).catch(() => {
    router.push({ path: `/${id}` }).catch(() => {})
  })
}

const cards = [
  { id: 'staff', title: '員工管理', desc: '建立與管理員工資料', color: 'var(--card-blue)',
    subItems: [
      { id: 'staff-list', title: '員工' },
      { id: 'dayoff-create', title: '請假單', path: '/dayoff/create' },
      { id: 'business-trip-list', title: '出差單', path: '/business-trip/index' },
      { id: 'overtime-list', title: '加班單' },
    ]
  },
  { id: 'customer', title: '客戶管理', desc: '管理客戶與聯絡人', color: 'var(--card-green)',
    subItems: [
      { id: 'customer-list', title: '客戶' },
      { id: 'contact-list', title: '聯絡人' },
    ]
  },
  { id: 'settings', title: '模組設定', desc: '系統模組與權限設定', color: 'var(--card-orange)',
    subItems: [
      { id: 'company-list', title: '公司管理', path: '/company/index' },
      { id: 'module-list', title: '模組' },
      { id: 'permission-list', title: '權限' },
    ]
  },
]

const toggleCard = (id) => {
  activeCard.value = activeCard.value === id ? null : id
}
</script>

<template>
  <section class="dashboard">
    <header class="dashboard-header">
      <h1>管理中心</h1>
      <p class="subtitle">快速存取常用功能：員工管理、客戶管理、模組設定</p>
    </header>

    <div class="dashboard-content">
      <div class="cards">
        <article
          v-for="card in cards"
          :key="card.id"
          class="card"
          :style="{ '--card-color': card.color }"
          @click="toggleCard(card.id)"
          role="button"
          tabindex="0"
          @keyup.enter="toggleCard(card.id)"
        >
          <div class="card-left">
            <div class="icon">👥</div>
          </div>
          <div class="card-right">
            <h3>{{ card.title }}</h3>
            <p class="desc">{{ card.desc }}</p>
          </div>
        </article>
      </div>

      <div v-if="activeCard" class="sub-items">
        <h2>功能</h2>
        <ul>
          <li
            v-for="item in cards.find((c) => c.id === activeCard).subItems"
            :key="item.id"
          >
            <button @click="item.path ? router.push(item.path) : go(item.id)">
              {{ item.title }}
            </button>
          </li>
        </ul>
      </div>
    </div>
  </section>
</template>

<style scoped>
:root {
  --bg: #1e293b; /* 深藍背景 */
  --card-bg: #334155; /* 深灰藍卡片背景 */
  --muted: #94a3b8; /* 淡灰文字 */
  --card-blue: #3b82f6; /* 藍色強調 */
  --card-green: #10b981; /* 綠色強調 */
  --card-orange: #f59e0b; /* 橙色強調 */
  --text-color: #e2e8f0; /* 白色文字 */
}

body {
  color: var(--text-color);
  background-color: var(--bg);
}

.dashboard {
  padding: 3rem;
  background: var(--bg);
  min-height: calc(100vh - 2rem);
}

.dashboard-header {
  margin-bottom: 1.25rem;
}

.dashboard-header h1 {
  margin: 0 0 0.25rem 0;
  font-size: 1.75rem;
}

.subtitle {
  margin: 0;
  color: var(--muted);
}

.dashboard-content {
width: 800px;
  display: grid;
  grid-template-columns: 2fr 1fr; /* 主項目和次項目固定比例 */
  gap: 2rem;
}

.cards {
  flex: 2;
  /* overflow-y: auto; 防止主項目被壓縮，允許滾动 */
}

.card {
  display: flex;
  gap: 1rem;
  align-items: center;
  padding: 1.5rem; /* 增加內距 */
  background: var(--card-bg);
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.6);
  cursor: pointer;
  transition: transform 150ms ease, box-shadow 150ms ease;
  border: 4px solid #6da7ff;
  margin: 1rem 0;
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.8);
}

.card-left .icon {
  font-size: 1.75rem;
  color: var(--text-color);
}

.card-right h3 {
  margin: 0 0 0.25rem 0;
  color: var(--text-color);
}

.card-right .desc {
  margin: 0;
  color: var(--muted);
  font-size: 0.9rem;
}

.sub-items {
  padding: 1rem;
  background: var(--card-bg);
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.6);
}

.sub-items h2 {
  margin: 0 0 1rem 0;
  color: var(--text-color);
}

.sub-items ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sub-items li {
  margin-bottom: 0.5rem;
}

.sub-items button {
  background: var(--card-blue);
  color: var(--card-bg);
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.sub-items button:hover {
  background: #2563eb;
}

@media (max-width: 480px) {
  .dashboard {
    padding: 1rem;
  }
}
</style>