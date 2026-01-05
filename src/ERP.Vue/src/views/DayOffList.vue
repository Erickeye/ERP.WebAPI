<template>
  <div class="day-off-list">
    <h1>請假單檢視列表</h1>

    <!-- Search Bar -->
    <div class="search-bar">
      <label for="start-date">請假日期開始:</label>
      <input type="date" id="start-date" v-model="searchParams.StartDate" />

      <label for="end-date">請假日期結束:</label>
      <input type="date" id="end-date" v-model="searchParams.EndDate" />

      <button @click="fetchDayOffList" :disabled="isLoading">
        {{ isLoading ? '搜尋中...' : '搜尋' }}
      </button>
    </div>

    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

    <!-- List Table -->
    <table class="day-off-table" v-if="dayOffList.length">
      <thead>
        <tr>
          <th>請假人</th>
          <th>申請日期</th>
          <th>代理人</th>
          <th>假別</th>
          <th>事由</th>
          <th>開始日期</th>
          <th>結束日期</th>
          <th>簽核主管</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in dayOffList" :key="item.id">
         <td>{{ item.leaveTaker }}</td>
          <td>{{ formatDate(item.applicationDate) }}</td>
          <td>{{ item.proxy }}</td>
          <td>{{ item.leaveTypeDisplay }}</td>
          <td>{{ item.reason }}</td>
          <td>{{ formatDateTime(item.beginDate) }}</td>
          <td>{{ formatDateTime(item.endDate) }}</td>
          <td>{{ item.authorizer }}</td>
        </tr>
      </tbody>
    </table>
    <div v-else>無資料</div>
  </div>
</template>

<script>
import { fetchDayOffList } from '@/api/dayoff';
import { formatDate,formatDateTime } from '@/utils/dateUtils';

export default {
  data() {
    return {
      searchParams: {
        StartDate: null,
        EndDate: null,
      },
      dayOffList: [],
      isLoading: false,
      errorMessage: '',
    };
  },
  methods: {
    async fetchDayOffList() {
      this.isLoading = true;
      this.errorMessage = '';
      try {
        const response = await fetchDayOffList(this.searchParams);
        this.dayOffList = response.data.data.items;
        console.log("dayOffList",this.dayOffList);
      } catch (error) {
        this.errorMessage = '無法取得請假單資料，請稍後再試。';
      } finally {
        this.isLoading = false;
      }
    },
    formatDate,formatDateTime,
  },
};
</script>

<style scoped>
.day-off-list {
  padding: 20px;
}

.search-bar {
  margin-bottom: 20px;
}

.search-bar label {
  margin-right: 10px;
}

.search-bar input {
  margin-right: 20px;
}

.day-off-table {
  width: 100%;
  border-collapse: collapse;
}

.day-off-table th, .day-off-table td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}

.day-off-table th {
  background-color: #f4f4f4;
}
</style>