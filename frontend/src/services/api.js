import axios from 'axios'

const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) config.headers.Authorization = `Bearer ${token}`
    return config
  },
  (error) => Promise.reject(error)
)

api.interceptors.response.use(
  (r) => r,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      console.warn('認証エラー: 再ログインしてください')
    }
    return Promise.reject(error)
  }
)

export const productService = {
  getAll: () => api.get('/admin/products'),
  getById: (id) => api.get(`/admin/products/${id}`),
  create: (product) => api.post('/admin/products', product),
  update: (id, product) => api.put(`/admin/products/${id}`, { ...product, id }),
  delete: (id) => api.delete(`/admin/products/${id}`)
}

export default api