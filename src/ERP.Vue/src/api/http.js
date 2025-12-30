import axios from 'axios'

const http = axios.create({
  baseURL: 'https://localhost:7129/api', // 後端
})

export default http
