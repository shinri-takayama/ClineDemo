<template>
  <div class="modal fade" id="checkoutModal" tabindex="-1" aria-labelledby="checkoutModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="checkoutModalLabel">
            <i class="fas fa-shopping-cart me-2"></i>
            注文手続き
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="submitOrder">
            <!-- Order Summary -->
            <div class="mb-4">
              <h6 class="fw-bold mb-3">
                <i class="fas fa-list me-2"></i>
                注文内容
              </h6>
              <div class="border rounded p-3 bg-light">
                <div v-for="item in cartItems" :key="item.id" class="d-flex justify-content-between align-items-center mb-2">
                  <div>
                    <strong>{{ item.name }}</strong>
                    <small class="text-muted d-block">数量: {{ item.quantity }}</small>
                  </div>
                  <div class="text-end">
                    <div class="fw-bold">{{ formatPrice(item.price * item.quantity) }}</div>
                  </div>
                </div>
                <hr>
                <div class="d-flex justify-content-between align-items-center">
                  <strong>合計金額</strong>
                  <strong class="text-primary fs-5">{{ formatPrice(totalAmount) }}</strong>
                </div>
              </div>
            </div>

            <!-- Shipping Information -->
            <div class="mb-4">
              <h6 class="fw-bold mb-3">
                <i class="fas fa-truck me-2"></i>
                配送先情報
              </h6>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label for="shippingName" class="form-label">お名前 <span class="text-danger">*</span></label>
                  <input
                    type="text"
                    class="form-control"
                    id="shippingName"
                    v-model="shippingInfo.name"
                    required
                  >
                </div>
                <div class="col-md-6 mb-3">
                  <label for="shippingPhone" class="form-label">電話番号</label>
                  <input
                    type="tel"
                    class="form-control"
                    id="shippingPhone"
                    v-model="shippingInfo.phone"
                    placeholder="090-1234-5678"
                  >
                </div>
              </div>
              <div class="row">
                <div class="col-md-4 mb-3">
                  <label for="shippingPostalCode" class="form-label">郵便番号 <span class="text-danger">*</span></label>
                  <input
                    type="text"
                    class="form-control"
                    id="shippingPostalCode"
                    v-model="shippingInfo.postalCode"
                    placeholder="123-4567"
                    required
                  >
                </div>
                <div class="col-md-4 mb-3">
                  <label for="shippingPrefecture" class="form-label">都道府県 <span class="text-danger">*</span></label>
                  <select
                    class="form-select"
                    id="shippingPrefecture"
                    v-model="shippingInfo.prefecture"
                    required
                  >
                    <option value="">選択してください</option>
                    <option v-for="prefecture in prefectures" :key="prefecture" :value="prefecture">
                      {{ prefecture }}
                    </option>
                  </select>
                </div>
                <div class="col-md-4 mb-3">
                  <label for="shippingCity" class="form-label">市区町村 <span class="text-danger">*</span></label>
                  <input
                    type="text"
                    class="form-control"
                    id="shippingCity"
                    v-model="shippingInfo.city"
                    required
                  >
                </div>
              </div>
              <div class="mb-3">
                <label for="shippingAddressLine" class="form-label">住所 <span class="text-danger">*</span></label>
                <input
                  type="text"
                  class="form-control"
                  id="shippingAddressLine"
                  v-model="shippingInfo.addressLine"
                  placeholder="番地・建物名・部屋番号"
                  required
                >
              </div>
            </div>

            <!-- Notes -->
            <div class="mb-4">
              <label for="notes" class="form-label">
                <i class="fas fa-sticky-note me-2"></i>
                備考
              </label>
              <textarea
                class="form-control"
                id="notes"
                v-model="notes"
                rows="3"
                placeholder="配送に関するご要望などがございましたらご記入ください"
              ></textarea>
            </div>

            <!-- Error Message -->
            <div v-if="errorMessage" class="alert alert-danger">
              <i class="fas fa-exclamation-triangle me-2"></i>
              {{ errorMessage }}
            </div>

            <!-- Loading State -->
            <div v-if="isLoading" class="text-center">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">処理中...</span>
              </div>
              <p class="mt-2">注文を処理しています...</p>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" :disabled="isLoading">
            キャンセル
          </button>
          <button type="button" class="btn btn-primary" @click="submitOrder" :disabled="isLoading">
            <i class="fas fa-credit-card me-2"></i>
            注文を確定する
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import orderService from '../services/orderService'
import authService from '../services/authService'

export default {
  name: 'CheckoutForm',
  props: {
    cartItems: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      shippingInfo: {
        name: '',
        phone: '',
        postalCode: '',
        prefecture: '',
        city: '',
        addressLine: ''
      },
      notes: '',
      isLoading: false,
      errorMessage: '',
      prefectures: [
        '北海道', '青森県', '岩手県', '宮城県', '秋田県', '山形県', '福島県',
        '茨城県', '栃木県', '群馬県', '埼玉県', '千葉県', '東京都', '神奈川県',
        '新潟県', '富山県', '石川県', '福井県', '山梨県', '長野県', '岐阜県',
        '静岡県', '愛知県', '三重県', '滋賀県', '京都府', '大阪府', '兵庫県',
        '奈良県', '和歌山県', '鳥取県', '島根県', '岡山県', '広島県', '山口県',
        '徳島県', '香川県', '愛媛県', '高知県', '福岡県', '佐賀県', '長崎県',
        '熊本県', '大分県', '宮崎県', '鹿児島県', '沖縄県'
      ]
    }
  },
  computed: {
    totalAmount() {
      return this.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0)
    }
  },
  methods: {
    async submitOrder() {
      this.errorMessage = ''
      this.isLoading = true

      try {
        // Check authentication first
        if (!authService.isAuthenticated()) {
          this.errorMessage = 'ログインが必要です。先にログインしてください。'
          this.isLoading = false
          return
        }

        // Validate form
        if (!this.validateForm()) {
          this.isLoading = false
          return
        }

        // Prepare order data
        const orderData = {
          items: this.cartItems.map(item => ({
            productId: item.id,
            quantity: item.quantity
          })),
          shippingName: this.shippingInfo.name,
          shippingPostalCode: this.shippingInfo.postalCode,
          shippingPrefecture: this.shippingInfo.prefecture,
          shippingCity: this.shippingInfo.city,
          shippingAddressLine: this.shippingInfo.addressLine,
          shippingPhone: this.shippingInfo.phone || null,
          notes: this.notes || null
        }

        // Create order
        const order = await orderService.createOrder(orderData)
        
        // Emit success event
        this.$emit('order-created', order)
        
        // Close modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('checkoutModal'))
        modal.hide()
        
        // Reset form
        this.resetForm()

      } catch (error) {
        console.error('Order creation failed:', error)
        if (error.response && error.response.data) {
          this.errorMessage = error.response.data.message || error.response.data
        } else {
          this.errorMessage = '注文の処理中にエラーが発生しました。もう一度お試しください。'
        }
      } finally {
        this.isLoading = false
      }
    },

    validateForm() {
      if (!this.shippingInfo.name.trim()) {
        this.errorMessage = 'お名前を入力してください。'
        return false
      }
      if (!this.shippingInfo.postalCode.trim()) {
        this.errorMessage = '郵便番号を入力してください。'
        return false
      }
      if (!this.shippingInfo.prefecture) {
        this.errorMessage = '都道府県を選択してください。'
        return false
      }
      if (!this.shippingInfo.city.trim()) {
        this.errorMessage = '市区町村を入力してください。'
        return false
      }
      if (!this.shippingInfo.addressLine.trim()) {
        this.errorMessage = '住所を入力してください。'
        return false
      }
      return true
    },

    resetForm() {
      this.shippingInfo = {
        name: '',
        phone: '',
        postalCode: '',
        prefecture: '',
        city: '',
        addressLine: ''
      }
      this.notes = ''
      this.errorMessage = ''
    },

    formatPrice(price) {
      return orderService.formatPrice(price)
    }
  }
}
</script>

<style scoped>
.modal-content {
  border-radius: 10px;
}

.bg-light {
  background-color: #f8f9fa !important;
}

.text-danger {
  color: #dc3545 !important;
}

.spinner-border {
  width: 2rem;
  height: 2rem;
}
</style>
