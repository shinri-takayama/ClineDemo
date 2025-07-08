import axios from 'axios'

const API_BASE_URL = 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

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
