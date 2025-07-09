<template>
  <div class="modal fade" id="orderHistoryModal" tabindex="-1" aria-labelledby="orderHistoryModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="orderHistoryModalLabel">
            <i class="fas fa-history me-2"></i>
            注文履歴
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <!-- Loading State -->
          <div v-if="loading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">読み込み中...</span>
            </div>
            <p class="mt-2 text-muted">注文履歴を読み込んでいます...</p>
          </div>

          <!-- Error State -->
          <div v-else-if="error" class="alert alert-danger">
            <i class="fas fa-exclamation-triangle me-2"></i>
            {{ error }}
            <button class="btn btn-outline-danger btn-sm ms-2" @click="loadOrders">
              再試行
            </button>
          </div>

          <!-- Empty State -->
          <div v-else-if="orders.length === 0" class="text-center py-5">
            <i class="fas fa-shopping-bag display-1 text-muted"></i>
            <h4 class="mt-3 text-muted">注文履歴がありません</h4>
            <p class="text-muted">まだ注文をされていません。商品を購入してみましょう！</p>
          </div>

          <!-- Orders List -->
          <div v-else>
            <div class="row mb-3">
              <div class="col-12">
                <p class="text-muted mb-0">{{ orders.length }}件の注文履歴</p>
              </div>
            </div>

            <div class="order-list">
              <div 
                v-for="order in orders" 
                :key="order.id" 
                class="card mb-3 order-card"
                @click="showOrderDetails(order.id)"
              >
                <div class="card-body">
                  <div class="row align-items-center">
                    <div class="col-md-2">
                      <div class="order-number">
                        <small class="text-muted">注文番号</small>
                        <div class="fw-bold">#{{ order.id }}</div>
                      </div>
                    </div>
                    <div class="col-md-3">
                      <div class="order-date">
                        <small class="text-muted">注文日時</small>
                        <div>{{ formatDate(order.orderDate) }}</div>
                      </div>
                    </div>
                    <div class="col-md-2">
                      <div class="order-status">
                        <small class="text-muted">ステータス</small>
                        <div>
                          <span :class="`badge bg-${getStatusColor(order.status)}`">
                            {{ order.statusText }}
                          </span>
                        </div>
                      </div>
                    </div>
                    <div class="col-md-2">
                      <div class="order-items">
                        <small class="text-muted">商品数</small>
                        <div>{{ order.itemCount }}点</div>
                      </div>
                    </div>
                    <div class="col-md-2">
                      <div class="order-total">
                        <small class="text-muted">合計金額</small>
                        <div class="fw-bold text-primary">{{ formatPrice(order.totalAmount) }}</div>
                      </div>
                    </div>
                    <div class="col-md-1 text-end">
                      <i class="fas fa-chevron-right text-muted"></i>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
            閉じる
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Order Details Modal -->
  <OrderDetails 
    v-if="selectedOrderId" 
    :order-id="selectedOrderId" 
    @close="selectedOrderId = null"
  />
</template>

<script>
import orderService from '../services/orderService'
import OrderDetails from './OrderDetails.vue'

export default {
  name: 'OrderHistory',
  components: {
    OrderDetails
  },
  data() {
    return {
      orders: [],
      loading: false,
      error: null,
      selectedOrderId: null
    }
  },
  methods: {
    async loadOrders() {
      this.loading = true
      this.error = null

      try {
        this.orders = await orderService.getOrders()
      } catch (error) {
        console.error('Failed to load orders:', error)
        if (error.response && error.response.status === 401) {
          this.error = 'ログインが必要です。'
        } else {
          this.error = '注文履歴の読み込みに失敗しました。'
        }
      } finally {
        this.loading = false
      }
    },

    showOrderDetails(orderId) {
      this.selectedOrderId = orderId
      // Hide order history modal
      const modal = bootstrap.Modal.getInstance(document.getElementById('orderHistoryModal'))
      modal.hide()
      
      // Show order details modal after a short delay
      setTimeout(() => {
        const detailsModal = new bootstrap.Modal(document.getElementById('orderDetailsModal'))
        detailsModal.show()
      }, 300)
    },

    formatDate(dateString) {
      return orderService.formatDate(dateString)
    },

    formatPrice(price) {
      return orderService.formatPrice(price)
    },

    getStatusColor(status) {
      return orderService.getStatusColor(status)
    }
  },
  mounted() {
    // Load orders when modal is shown
    const modal = document.getElementById('orderHistoryModal')
    if (modal) {
      modal.addEventListener('shown.bs.modal', () => {
        this.loadOrders()
      })
    }
  }
}
</script>

<style scoped>
.order-card {
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid #dee2e6;
}

.order-card:hover {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
  border-color: #007bff;
}

.order-number,
.order-date,
.order-status,
.order-items,
.order-total {
  margin-bottom: 0.5rem;
}

.order-number .fw-bold {
  color: #007bff;
  font-size: 1.1rem;
}

.order-total .fw-bold {
  font-size: 1.1rem;
}

.badge {
  font-size: 0.75rem;
  padding: 0.375rem 0.75rem;
}

.modal-xl {
  max-width: 1200px;
}

.order-list {
  max-height: 60vh;
  overflow-y: auto;
}

.spinner-border {
  width: 2rem;
  height: 2rem;
}

@media (max-width: 768px) {
  .order-card .row > div {
    margin-bottom: 0.5rem;
  }
  
  .order-card .col-md-1 {
    display: none;
  }
}
</style>
