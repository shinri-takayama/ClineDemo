import axios from 'axios'

const API_BASE_URL = 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add JWT token to requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor to handle authentication errors
api.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      // Token expired or invalid, remove from localStorage
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      // Optionally redirect to login or emit an event
      console.warn('Authentication failed. Please log in again.')
    }
    return Promise.reject(error)
  }
)

export const productService = {
  // 全商品を取得
  getAllProducts: () => api.get('/products'),
  
  // 特定の商品を取得
  getProduct: (id) => api.get(`/products/${id}`),
  
  // 商品を作成
  createProduct: (product) => api.post('/products', product),
  
  // 商品を更新
  updateProduct: (id, product) => api.put(`/products/${id}`, product),
  
  // 商品を削除
  deleteProduct: (id) => api.delete(`/products/${id}`)
}

export default api
