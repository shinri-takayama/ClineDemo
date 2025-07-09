<template>
  <div class="modal fade" id="orderDetailsModal" tabindex="-1" aria-labelledby="orderDetailsModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="orderDetailsModalLabel">
            <i class="fas fa-receipt me-2"></i>
            注文詳細 <span v-if="order">#{{ order.id }}</span>
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <!-- Loading State -->
          <div v-if="loading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">読み込み中...</span>
            </div>
            <p class="mt-2 text-muted">注文詳細を読み込んでいます...</p>
          </div>

          <!-- Error State -->
          <div v-else-if="error" class="alert alert-danger">
            <i class="fas fa-exclamation-triangle me-2"></i>
            {{ error }}
            <button class="btn btn-outline-danger btn-sm ms-2" @click="loadOrderDetails">
              再試行
            </button>
          </div>

          <!-- Order Details -->
          <div v-else-if="order">
            <!-- Order Summary -->
            <div class="row mb-4">
              <div class="col-md-6">
                <div class="card">
                  <div class="card-header">
                    <h6 class="mb-0">
                      <i class="fas fa-info-circle me-2"></i>
                      注文情報
                    </h6>
                  </div>
                  <div class="card-body">
                    <div class="row mb-2">
                      <div class="col-4"><strong>注文番号:</strong></div>
                      <div class="col-8">#{{ order.id }}</div>
                    </div>
                    <div class="row mb-2">
                      <div class="col-4"><strong>注文日時:</strong></div>
                      <div class="col-8">{{ formatDate(order.orderDate) }}</div>
                    </div>
                    <div class="row mb-2">
                      <div class="col-4"><strong>ステータス:</strong></div>
                      <div class="col-8">
                        <span :class="`badge bg-${getStatusColor(order.status)}`">
                          {{ order.statusText }}
                        </span>
                      </div>
                    </div>
                    <div class="row">
                      <div class="col-4"><strong>合計金額:</strong></div>
                      <div class="col-8">
                        <span class="fw-bold text-primary fs-5">{{ formatPrice(order.totalAmount) }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="card">
                  <div class="card-header">
                    <h6 class="mb-0">
                      <i class="fas fa-truck me-2"></i>
                      配送先情報
                    </h6>
                  </div>
                  <div class="card-body">
                    <div class="mb-2">
                      <strong>{{ order.shippingName }}</strong>
                    </div>
                    <div class="mb-1">
                      〒{{ order.shippingPostalCode }}
                    </div>
                    <div class="mb-1">
                      {{ order.shippingPrefecture }}{{ order.shippingCity }}
                    </div>
                    <div class="mb-2">
                      {{ order.shippingAddressLine }}
                    </div>
                    <div v-if="order.shippingPhone" class="mb-2">
                      <i class="fas fa-phone me-1"></i>
                      {{ order.shippingPhone }}
                    </div>
                    <div v-if="order.notes" class="mt-3">
                      <small class="text-muted">備考:</small>
                      <div class="border rounded p-2 bg-light">
                        {{ order.notes }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Order Items -->
            <div class="card">
              <div class="card-header">
                <h6 class="mb-0">
                  <i class="fas fa-shopping-bag me-2"></i>
                  注文商品 ({{ order.items.length }}点)
                </h6>
              </div>
              <div class="card-body p-0">
                <div class="table-responsive">
                  <table class="table table-hover mb-0">
                    <thead class="table-light">
                      <tr>
                        <th>商品名</th>
                        <th class="text-center">数量</th>
                        <th class="text-end">単価</th>
                        <th class="text-end">小計</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="item in order.items" :key="item.id">
                        <td>
                          <div class="fw-bold">{{ item.productName }}</div>
                          <small v-if="item.productDescription" class="text-muted">
                            {{ item.productDescription }}
                          </small>
                        </td>
                        <td class="text-center">
                          <span class="badge bg-secondary">{{ item.quantity }}</span>
                        </td>
                        <td class="text-end">{{ formatPrice(item.price) }}</td>
                        <td class="text-end fw-bold">{{ formatPrice(item.subtotal) }}</td>
                      </tr>
                    </tbody>
                    <tfoot class="table-light">
                      <tr>
                        <th colspan="3" class="text-end">合計金額:</th>
                        <th class="text-end text-primary fs-5">{{ formatPrice(order.totalAmount) }}</th>
                      </tr>
                    </tfoot>
                  </table>
                </div>
              </div>
            </div>

            <!-- Order Timeline (Future Enhancement) -->
            <div class="card mt-4">
              <div class="card-header">
                <h6 class="mb-0">
                  <i class="fas fa-clock me-2"></i>
                  注文履歴
                </h6>
              </div>
              <div class="card-body">
                <div class="timeline">
                  <div class="timeline-item">
                    <div class="timeline-marker bg-primary"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">注文受付</h6>
                      <small class="text-muted">{{ formatDate(order.orderDate) }}</small>
                      <p class="mb-0 text-muted">ご注文を受け付けました。</p>
                    </div>
                  </div>
                  <div v-if="order.status >= 1" class="timeline-item">
                    <div class="timeline-marker bg-info"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">注文確定</h6>
                      <small class="text-muted">{{ formatDate(order.updatedAt) }}</small>
                      <p class="mb-0 text-muted">ご注文が確定されました。</p>
                    </div>
                  </div>
                  <div v-if="order.status >= 2" class="timeline-item">
                    <div class="timeline-marker bg-warning"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">処理中</h6>
                      <small class="text-muted">{{ formatDate(order.updatedAt) }}</small>
                      <p class="mb-0 text-muted">商品の準備を行っています。</p>
                    </div>
                  </div>
                  <div v-if="order.status >= 3" class="timeline-item">
                    <div class="timeline-marker bg-success"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">発送完了</h6>
                      <small class="text-muted">{{ formatDate(order.updatedAt) }}</small>
                      <p class="mb-0 text-muted">商品を発送いたしました。</p>
                    </div>
                  </div>
                  <div v-if="order.status >= 4" class="timeline-item">
                    <div class="timeline-marker bg-success"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">配送完了</h6>
                      <small class="text-muted">{{ formatDate(order.updatedAt) }}</small>
                      <p class="mb-0 text-muted">商品をお届けしました。</p>
                    </div>
                  </div>
                  <div v-if="order.status === 5" class="timeline-item">
                    <div class="timeline-marker bg-danger"></div>
                    <div class="timeline-content">
                      <h6 class="mb-1">キャンセル</h6>
                      <small class="text-muted">{{ formatDate(order.updatedAt) }}</small>
                      <p class="mb-0 text-muted">ご注文がキャンセルされました。</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="goBackToHistory">
            <i class="fas fa-arrow-left me-2"></i>
            注文履歴に戻る
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
            閉じる
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import orderService from '../services/orderService'

export default {
  name: 'OrderDetails',
  props: {
    orderId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      order: null,
      loading: false,
      error: null
    }
  },
  watch: {
    orderId: {
      immediate: true,
      handler(newOrderId) {
        if (newOrderId) {
          this.loadOrderDetails()
        }
      }
    }
  },
  methods: {
    async loadOrderDetails() {
      this.loading = true
      this.error = null

      try {
        this.order = await orderService.getOrder(this.orderId)
      } catch (error) {
        console.error('Failed to load order details:', error)
        if (error.response && error.response.status === 401) {
          this.error = 'ログインが必要です。'
        } else if (error.response && error.response.status === 404) {
          this.error = '注文が見つかりません。'
        } else {
          this.error = '注文詳細の読み込みに失敗しました。'
        }
      } finally {
        this.loading = false
      }
    },

    goBackToHistory() {
      // Close details modal
      const modal = bootstrap.Modal.getInstance(document.getElementById('orderDetailsModal'))
      modal.hide()
      
      // Show history modal after a short delay
      setTimeout(() => {
        const historyModal = new bootstrap.Modal(document.getElementById('orderHistoryModal'))
        historyModal.show()
      }, 300)
      
      // Emit close event
      this.$emit('close')
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
  }
}
</script>

<style scoped>
.timeline {
  position: relative;
  padding-left: 2rem;
}

.timeline::before {
  content: '';
  position: absolute;
  left: 0.75rem;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #dee2e6;
}

.timeline-item {
  position: relative;
  margin-bottom: 2rem;
}

.timeline-marker {
  position: absolute;
  left: -2rem;
  top: 0.25rem;
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  border: 2px solid white;
  box-shadow: 0 0 0 2px #dee2e6;
}

.timeline-content {
  padding-left: 1rem;
}

.timeline-content h6 {
  margin-bottom: 0.25rem;
}

.timeline-content small {
  display: block;
  margin-bottom: 0.5rem;
}

.card-header h6 {
  color: #495057;
}

.table th {
  border-top: none;
  font-weight: 600;
  color: #495057;
}

.badge {
  font-size: 0.75rem;
}

.spinner-border {
  width: 2rem;
  height: 2rem;
}

@media (max-width: 768px) {
  .timeline {
    padding-left: 1.5rem;
  }
  
  .timeline-marker {
    left: -1.5rem;
  }
}
</style>
