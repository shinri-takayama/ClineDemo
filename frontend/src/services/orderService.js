import api from './api'

const orderService = {
  // Create a new order
  async createOrder(orderData) {
    try {
      const response = await api.post('/orders', orderData)
      return response.data
    } catch (error) {
      console.error('Create order error:', error)
      throw error
    }
  },

  // Get user's order history
  async getOrders() {
    try {
      const response = await api.get('/orders')
      return response.data
    } catch (error) {
      console.error('Get orders error:', error)
      throw error
    }
  },

  // Get specific order details
  async getOrder(orderId) {
    try {
      const response = await api.get(`/orders/${orderId}`)
      return response.data
    } catch (error) {
      console.error('Get order error:', error)
      throw error
    }
  },

  // Update order status
  async updateOrderStatus(orderId, status) {
    try {
      const response = await api.put(`/orders/${orderId}/status`, { status })
      return response.data
    } catch (error) {
      console.error('Update order status error:', error)
      throw error
    }
  },

  // Format order status text
  getStatusText(status) {
    const statusMap = {
      0: '注文確認中',
      1: '注文確定',
      2: '処理中',
      3: '発送済み',
      4: '配送完了',
      5: 'キャンセル'
    }
    return statusMap[status] || '不明'
  },

  // Get status color class
  getStatusColor(status) {
    const colorMap = {
      0: 'warning',    // 注文確認中
      1: 'info',       // 注文確定
      2: 'primary',    // 処理中
      3: 'success',    // 発送済み
      4: 'success',    // 配送完了
      5: 'danger'      // キャンセル
    }
    return colorMap[status] || 'secondary'
  },

  // Format price
  formatPrice(price) {
    return new Intl.NumberFormat('ja-JP', {
      style: 'currency',
      currency: 'JPY'
    }).format(price)
  },

  // Format date
  formatDate(dateString) {
    const date = new Date(dateString)
    return date.toLocaleDateString('ja-JP', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  }
}

export default orderService
