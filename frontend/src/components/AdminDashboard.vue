<template>
  <div class="modal fade" id="adminDashboardModal" tabindex="-1" aria-labelledby="adminDashboardModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-fullscreen">
      <div class="modal-content">
        <div class="modal-header bg-primary text-white">
          <h5 class="modal-title" id="adminDashboardModalLabel">
            <i class="fas fa-cogs me-2"></i>
            管理者ダッシュボード
          </h5>
          <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body p-0">
          <!-- Navigation Tabs -->
          <nav class="navbar navbar-expand-lg navbar-light bg-light border-bottom">
            <div class="container-fluid">
              <ul class="nav nav-tabs border-0" id="adminTabs" role="tablist">
                <li class="nav-item" role="presentation">
                  <button class="nav-link active" id="dashboard-tab" data-bs-toggle="tab" data-bs-target="#dashboard" type="button" role="tab">
                    <i class="fas fa-chart-bar me-2"></i>ダッシュボード
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button class="nav-link" id="users-tab" data-bs-toggle="tab" data-bs-target="#users" type="button" role="tab">
                    <i class="fas fa-users me-2"></i>ユーザー管理
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button class="nav-link" id="products-tab" data-bs-toggle="tab" data-bs-target="#products" type="button" role="tab">
                    <i class="fas fa-box me-2"></i>商品管理
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button class="nav-link" id="orders-tab" data-bs-toggle="tab" data-bs-target="#orders" type="button" role="tab">
                    <i class="fas fa-shopping-cart me-2"></i>注文管理
                  </button>
                </li>
              </ul>
            </div>
          </nav>

          <!-- Tab Content -->
          <div class="tab-content p-4" id="adminTabsContent">
            <!-- Dashboard Tab -->
            <div class="tab-pane fade show active" id="dashboard" role="tabpanel">
              <div v-if="dashboardLoading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <p class="mt-2">ダッシュボードを読み込んでいます...</p>
              </div>
              <div v-else-if="dashboardData">
                <!-- Summary Cards -->
                <div class="row mb-4">
                  <div class="col-md-3 mb-3">
                    <div class="card bg-primary text-white">
                      <div class="card-body">
                        <div class="d-flex justify-content-between">
                          <div>
                            <h4 class="mb-0">{{ dashboardData.summary.totalUsers }}</h4>
                            <p class="mb-0">総ユーザー数</p>
                          </div>
                          <i class="fas fa-users fa-2x opacity-75"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-3 mb-3">
                    <div class="card bg-success text-white">
                      <div class="card-body">
                        <div class="d-flex justify-content-between">
                          <div>
                            <h4 class="mb-0">{{ dashboardData.summary.totalProducts }}</h4>
                            <p class="mb-0">総商品数</p>
                          </div>
                          <i class="fas fa-box fa-2x opacity-75"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-3 mb-3">
                    <div class="card bg-info text-white">
                      <div class="card-body">
                        <div class="d-flex justify-content-between">
                          <div>
                            <h4 class="mb-0">{{ dashboardData.summary.totalOrders }}</h4>
                            <p class="mb-0">総注文数</p>
                          </div>
                          <i class="fas fa-shopping-cart fa-2x opacity-75"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-3 mb-3">
                    <div class="card bg-warning text-white">
                      <div class="card-body">
                        <div class="d-flex justify-content-between">
                          <div>
                            <h4 class="mb-0">¥{{ formatPrice(dashboardData.summary.totalRevenue) }}</h4>
                            <p class="mb-0">総売上</p>
                          </div>
                          <i class="fas fa-yen-sign fa-2x opacity-75"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Recent Orders and Low Stock -->
                <div class="row">
                  <div class="col-md-8">
                    <div class="card">
                      <div class="card-header">
                        <h5 class="mb-0">最近の注文</h5>
                      </div>
                      <div class="card-body">
                        <div class="table-responsive">
                          <table class="table table-sm">
                            <thead>
                              <tr>
                                <th>注文番号</th>
                                <th>顧客名</th>
                                <th>注文日</th>
                                <th>ステータス</th>
                                <th>金額</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr v-for="order in dashboardData.recentOrders" :key="order.id">
                                <td>#{{ order.id }}</td>
                                <td>{{ order.shippingName }}</td>
                                <td>{{ formatDate(order.orderDate) }}</td>
                                <td>
                                  <span class="badge bg-secondary">{{ order.statusText }}</span>
                                </td>
                                <td>¥{{ formatPrice(order.totalAmount) }}</td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-4">
                    <div class="card">
                      <div class="card-header">
                        <h5 class="mb-0">在庫少商品</h5>
                      </div>
                      <div class="card-body">
                        <div v-for="product in dashboardData.lowStockProducts" :key="product.id" class="d-flex justify-content-between align-items-center mb-2">
                          <div>
                            <div class="fw-bold">{{ product.name }}</div>
                            <small class="text-muted">在庫: {{ product.stock }}個</small>
                          </div>
                          <span class="badge bg-danger">{{ product.stock }}</span>
                        </div>
                        <div v-if="dashboardData.lowStockProducts.length === 0" class="text-muted text-center">
                          在庫不足の商品はありません
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Users Tab -->
            <div class="tab-pane fade" id="users" role="tabpanel">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <h4>ユーザー管理</h4>
                <button class="btn btn-primary" @click="loadUsers">
                  <i class="fas fa-sync me-2"></i>更新
                </button>
              </div>
              
              <div v-if="usersLoading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <p class="mt-2">ユーザー一覧を読み込んでいます...</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-striped">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>ユーザー名</th>
                      <th>メール</th>
                      <th>名前</th>
                      <th>登録日</th>
                      <th>管理者</th>
                      <th>操作</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="user in users" :key="user.id">
                      <td>{{ user.id }}</td>
                      <td>{{ user.username }}</td>
                      <td>{{ user.email }}</td>
                      <td>{{ user.firstName }} {{ user.lastName }}</td>
                      <td>{{ formatDate(user.createdAt) }}</td>
                      <td>
                        <span :class="`badge ${user.isAdmin ? 'bg-success' : 'bg-secondary'}`">
                          {{ user.isAdmin ? '管理者' : '一般' }}
                        </span>
                      </td>
                      <td>
                        <button 
                          class="btn btn-sm btn-outline-primary"
                          @click="toggleAdminStatus(user)"
                          :disabled="user.id === currentUser?.id"
                        >
                          {{ user.isAdmin ? '管理者解除' : '管理者設定' }}
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Products Tab -->
            <div class="tab-pane fade" id="products" role="tabpanel">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <h4>商品管理</h4>
                <div>
                  <button class="btn btn-success me-2" @click="showAddProductForm">
                    <i class="fas fa-plus me-2"></i>商品追加
                  </button>
                  <button class="btn btn-primary" @click="loadProducts">
                    <i class="fas fa-sync me-2"></i>更新
                  </button>
                </div>
              </div>
              
              <div v-if="productsLoading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <p class="mt-2">商品一覧を読み込んでいます...</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-striped">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>商品名</th>
                      <th>価格</th>
                      <th>在庫</th>
                      <th>作成日</th>
                      <th>操作</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="product in products" :key="product.id">
                      <td>{{ product.id }}</td>
                      <td>{{ product.name }}</td>
                      <td>¥{{ formatPrice(product.price) }}</td>
                      <td>
                        <span :class="`badge ${product.stock <= 5 ? 'bg-danger' : 'bg-success'}`">
                          {{ product.stock }}
                        </span>
                      </td>
                      <td>{{ formatDate(product.createdAt) }}</td>
                      <td>
                        <button class="btn btn-sm btn-outline-primary me-1" @click="editProduct(product)">
                          編集
                        </button>
                        <button class="btn btn-sm btn-outline-danger" @click="deleteProduct(product)">
                          削除
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Orders Tab -->
            <div class="tab-pane fade" id="orders" role="tabpanel">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <h4>注文管理</h4>
                <button class="btn btn-primary" @click="loadOrders">
                  <i class="fas fa-sync me-2"></i>更新
                </button>
              </div>
              
              <div v-if="ordersLoading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <p class="mt-2">注文一覧を読み込んでいます...</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-striped">
                  <thead>
                    <tr>
                      <th>注文番号</th>
                      <th>顧客名</th>
                      <th>注文日</th>
                      <th>ステータス</th>
                      <th>商品数</th>
                      <th>金額</th>
                      <th>操作</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="order in orders" :key="order.id">
                      <td>#{{ order.id }}</td>
                      <td>{{ order.shippingName }}</td>
                      <td>{{ formatDate(order.orderDate) }}</td>
                      <td>
                        <select 
                          class="form-select form-select-sm"
                          :value="order.status"
                          @change="updateOrderStatus(order.id, $event.target.value)"
                        >
                          <option value="0">注文確認中</option>
                          <option value="1">注文確定</option>
                          <option value="2">処理中</option>
                          <option value="3">発送済み</option>
                          <option value="4">配送完了</option>
                          <option value="5">キャンセル</option>
                        </select>
                      </td>
                      <td>{{ order.itemCount }}点</td>
                      <td>¥{{ formatPrice(order.totalAmount) }}</td>
                      <td>
                        <button class="btn btn-sm btn-outline-info" @click="viewOrderDetails(order)">
                          詳細
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import authService from '../services/authService'

export default {
  name: 'AdminDashboard',
  props: {
    currentUser: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      dashboardData: null,
      dashboardLoading: false,
      users: [],
      usersLoading: false,
      products: [],
      productsLoading: false,
      orders: [],
      ordersLoading: false
    }
  },
  methods: {
    async loadDashboard() {
      this.dashboardLoading = true
      try {
        const response = await fetch('http://localhost:5000/api/admin/dashboard', {
          headers: {
            'Authorization': `Bearer ${authService.getToken()}`
          }
        })
        if (response.ok) {
          this.dashboardData = await response.json()
        }
      } catch (error) {
        console.error('Failed to load dashboard:', error)
      } finally {
        this.dashboardLoading = false
      }
    },

    async loadUsers() {
      this.usersLoading = true
      try {
        const response = await fetch('http://localhost:5000/api/admin/users', {
          headers: {
            'Authorization': `Bearer ${authService.getToken()}`
          }
        })
        if (response.ok) {
          this.users = await response.json()
        }
      } catch (error) {
        console.error('Failed to load users:', error)
      } finally {
        this.usersLoading = false
      }
    },

    async loadProducts() {
      this.productsLoading = true
      try {
        const response = await fetch('http://localhost:5000/api/admin/products', {
          headers: {
            'Authorization': `Bearer ${authService.getToken()}`
          }
        })
        if (response.ok) {
          this.products = await response.json()
        }
      } catch (error) {
        console.error('Failed to load products:', error)
      } finally {
        this.productsLoading = false
      }
    },

    async loadOrders() {
      this.ordersLoading = true
      try {
        const response = await fetch('http://localhost:5000/api/admin/orders', {
          headers: {
            'Authorization': `Bearer ${authService.getToken()}`
          }
        })
        if (response.ok) {
          const data = await response.json()
          this.orders = data.orders
        }
      } catch (error) {
        console.error('Failed to load orders:', error)
      } finally {
        this.ordersLoading = false
      }
    },

    async toggleAdminStatus(user) {
      try {
        const response = await fetch(`http://localhost:5000/api/admin/users/${user.id}/admin-status`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${authService.getToken()}`
          },
          body: JSON.stringify({ isAdmin: !user.isAdmin })
        })
        
        if (response.ok) {
          user.isAdmin = !user.isAdmin
          alert(`ユーザー「${user.username}」の管理者権限を更新しました。`)
        } else {
          const error = await response.text()
          alert(`エラー: ${error}`)
        }
      } catch (error) {
        console.error('Failed to update admin status:', error)
        alert('管理者権限の更新に失敗しました。')
      }
    },

    async updateOrderStatus(orderId, newStatus) {
      try {
        const response = await fetch(`http://localhost:5000/api/admin/orders/${orderId}/status`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${authService.getToken()}`
          },
          body: JSON.stringify({ status: parseInt(newStatus) })
        })
        
        if (response.ok) {
          const order = this.orders.find(o => o.id === orderId)
          if (order) {
            order.status = parseInt(newStatus)
          }
          alert('注文ステータスを更新しました。')
        }
      } catch (error) {
        console.error('Failed to update order status:', error)
        alert('注文ステータスの更新に失敗しました。')
      }
    },

    showAddProductForm() {
      alert('商品追加機能は今後実装予定です。')
    },

    editProduct(product) {
      alert(`商品「${product.name}」の編集機能は今後実装予定です。`)
    },

    async deleteProduct(product) {
      if (confirm(`商品「${product.name}」を削除しますか？`)) {
        try {
          const response = await fetch(`http://localhost:5000/api/admin/products/${product.id}`, {
            method: 'DELETE',
            headers: {
              'Authorization': `Bearer ${authService.getToken()}`
            }
          })
          
          if (response.ok) {
            this.products = this.products.filter(p => p.id !== product.id)
            alert('商品を削除しました。')
          }
        } catch (error) {
          console.error('Failed to delete product:', error)
          alert('商品の削除に失敗しました。')
        }
      }
    },

    viewOrderDetails(order) {
      alert(`注文 #${order.id} の詳細表示機能は今後実装予定です。`)
    },

    formatPrice(price) {
      return new Intl.NumberFormat('ja-JP').format(price)
    },

    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString('ja-JP')
    }
  },

  mounted() {
    // Load initial data when modal is shown
    const modal = document.getElementById('adminDashboardModal')
    if (modal) {
      modal.addEventListener('shown.bs.modal', () => {
        this.loadDashboard()
        this.loadUsers()
        this.loadProducts()
        this.loadOrders()
      })
    }
  }
}
</script>

<style scoped>
.modal-fullscreen .modal-body {
  height: calc(100vh - 120px);
  overflow-y: auto;
}

.nav-tabs .nav-link {
  border: none;
  border-bottom: 2px solid transparent;
}

.nav-tabs .nav-link.active {
  border-bottom-color: #007bff;
  background-color: transparent;
}

.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.opacity-75 {
  opacity: 0.75;
}

.timeline {
  position: relative;
}

.timeline::before {
  content: '';
  position: absolute;
  left: 1rem;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #dee2e6;
}

.table th {
  font-weight: 600;
  color: #495057;
  border-top: none;
}

.badge {
  font-size: 0.75rem;
}
</style>
