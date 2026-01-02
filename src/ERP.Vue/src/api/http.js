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

export default http
