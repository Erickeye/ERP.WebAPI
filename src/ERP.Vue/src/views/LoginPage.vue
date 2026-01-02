<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const account = ref('');
const password = ref('');
const errorMessage = ref('');

const login = async () => {
  try {
    const response = await axios.post('https://localhost:7129/api/Login/Login', {
      account: account.value,
      password: password.value,
    });

    if(response.data.errorCode != 0) {
      errorMessage.value = response.data.errorMessage || '錯誤';
      return;
    }

    const token = response.data.data.accessToken; // 假設 API 回傳的 JWT 是在 data.token
    localStorage.setItem('jwt', token); // 儲存 JWT 到 localStorage

    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`; // 設置全域 Header

    router.push('/layout'); // 導向 layout 頁面
  } catch (error) {
    errorMessage.value = error.response?.data?.message || '登入失敗，請稍後再試';
  }
};
</script>

<template>
  <div class="login-container">
    <h1>登入</h1>
    <form @submit.prevent="login">
      <div>
        <label for="account">帳號</label>
        <input id="account" v-model="account" type="text" placeholder="請輸入帳號" required />
      </div>
      <div>
        <label for="password">密碼</label>
        <input id="password" v-model="password" type="password" placeholder="請輸入密碼" required />
      </div>
      <button type="submit">登入</button>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </form>
  </div>
</template>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 0 auto;
  padding: 2rem;
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  margin-bottom: 1.5rem;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

label {
  font-weight: bold;
}

input {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  padding: 0.5rem 1rem;
  background: #3b82f6;
  color: #fff;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background: #2563eb;
}

.error {
  color: red;
  text-align: center;
}
</style>