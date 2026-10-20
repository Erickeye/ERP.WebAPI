import axios from 'axios'

const http = axios.create({
  baseURL: 'https://localhost:7129/api', // 後端
})

export const getHeaders = () => {
  const headers = {};
  const token = localStorage.getItem('jwt');
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  return headers;
};

// 自動帶入 Authorization，新的 API wrapper 不需要再各自傳 headers
http.interceptors.request.use((config) => {
  if (!config.headers.Authorization) {
    const token = localStorage.getItem('jwt');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

export default http
