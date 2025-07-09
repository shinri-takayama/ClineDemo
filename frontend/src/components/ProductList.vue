<template>
  <div class="container-fluid py-4">
    <!-- ヘッダー -->
    <div class="row mb-4">
      <div class="col-12">
        <div class="d-flex justify-content-between align-items-center">
          <div>
            <h1 class="h2 mb-1">商品一覧</h1>
            <p class="text-muted mb-0">お気に入りの商品を見つけてください</p>
          </div>
          <div class="d-flex align-items-center">
            <button class="btn btn-outline-primary position-relative me-3" @click="showCart">
              <i class="bi bi-cart3"></i>
              カート
              <span v-if="cartItemCount > 0" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                {{ cartItemCount }}
              </span>
            </button>
            <button class="btn btn-outline-secondary" @click="loadProducts">
              <i class="bi bi-arrow-clockwise"></i>
              更新
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- ローディング表示 -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">読み込み中...</span>
      </div>
      <p class="mt-2 text-muted">商品を読み込んでいます...</p>
    </div>

    <!-- エラー表示 -->
    <div v-else-if="error" class="alert alert-danger" role="alert">
      <i class="bi bi-exclamation-triangle me-2"></i>
      {{ error }}
      <button class="btn btn-outline-danger btn-sm ms-2" @click="loadProducts">
        再試行
      </button>
    </div>

    <!-- 商品一覧 -->
    <div v-else class="row">
      <div v-if="products.length === 0" class="col-12">
        <div class="text-center py-5">
          <i class="bi bi-box-seam display-1 text-muted"></i>
          <h3 class="mt-3 text-muted">商品がありません</h3>
          <p class="text-muted">現在表示できる商品がありません。</p>
        </div>
      </div>
      <div v-else class="col-lg-3 col-md-4 col-sm-6 mb-4" v-for="product in products" :key="product.id">
        <ProductCard 
          :product="product" 
          @added-to-cart="onAddedToCart"
        />
      </div>
    </div>

    <!-- カート表示モーダル -->
    <div class="modal fade" id="cartModal" tabindex="-1" aria-labelledby="cartModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="cartModalLabel">
              <i class="bi bi-cart3 me-2"></i>ショッピングカート
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div v-if="cartItems.length === 0" class="text-center py-4">
              <i class="bi bi-cart-x display-4 text-muted"></i>
              <p class="mt-3 text-muted">カートは空です</p>
            </div>
            <div v-else>
              <div class="row mb-3" v-for="item in cartItems" :key="item.id">
                <div class="col-2">
                  <img :src="item.imageUrl" :alt="item.name" class="img-fluid rounded">
                </div>
                <div class="col-6">
                  <h6 class="mb-1">{{ item.name }}</h6>
                  <small class="text-muted">¥{{ formatPrice(item.price) }}</small>
                </div>
                <div class="col-2">
                  <div class="input-group input-group-sm">
                    <button class="btn btn-outline-secondary" @click="updateQuantity(item.id, item.quantity - 1)">-</button>
                    <input type="text" class="form-control text-center" :value="item.quantity" readonly>
                    <button class="btn btn-outline-secondary" @click="updateQuantity(item.id, item.quantity + 1)">+</button>
                  </div>
                </div>
                <div class="col-2 text-end">
                  <strong>¥{{ formatPrice(item.price * item.quantity) }}</strong>
                  <br>
                  <button class="btn btn-sm btn-outline-danger mt-1" @click="removeFromCart(item.id)">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              </div>
              <hr>
              <div class="row">
                <div class="col-12 text-end">
                  <h5>合計: ¥{{ formatPrice(cartTotal) }}</h5>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">閉じる</button>
            <button v-if="cartItems.length > 0" type="button" class="btn btn-primary" @click="proceedToCheckout">
              <i class="bi bi-credit-card me-1"></i>
              購入手続きへ
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Checkout Form -->
    <CheckoutForm 
      :cart-items="cartItems" 
      @order-created="onOrderCreated"
    />
  </div>
</template>

<script>
import ProductCard from './ProductCard.vue'
import CheckoutForm from './CheckoutForm.vue'
import { productService } from '../services/api'

export default {
  name: 'ProductList',
  components: {
    ProductCard,
    CheckoutForm
  },
  data() {
    return {
      products: [],
      loading: false,
      error: null,
      cartItems: [],
      cartItemCount: 0
    }
  },
  computed: {
    cartTotal() {
      return this.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0)
    }
  },
  mounted() {
    this.loadProducts()
    this.loadCart()
  },
  methods: {
    async loadProducts() {
      this.loading = true
      this.error = null
      
      try {
        const response = await productService.getAllProducts()
        this.products = response.data
      } catch (error) {
        console.error('商品の読み込みに失敗しました:', error)
        this.error = '商品の読み込みに失敗しました。ネットワーク接続を確認してください。'
      } finally {
        this.loading = false
      }
    },
    loadCart() {
      this.cartItems = JSON.parse(localStorage.getItem('cart') || '[]')
      this.cartItemCount = this.cartItems.reduce((total, item) => total + item.quantity, 0)
    },
    onAddedToCart(data) {
      this.cartItemCount = data.cartItemCount
      this.loadCart()
    },
    showCart() {
      this.loadCart()
      const modal = new bootstrap.Modal(document.getElementById('cartModal'))
      modal.show()
    },
    updateQuantity(productId, newQuantity) {
      if (newQuantity <= 0) {
        this.removeFromCart(productId)
        return
      }
      
      const item = this.cartItems.find(item => item.id === productId)
      if (item) {
        item.quantity = newQuantity
        localStorage.setItem('cart', JSON.stringify(this.cartItems))
        this.loadCart()
      }
    },
    removeFromCart(productId) {
      this.cartItems = this.cartItems.filter(item => item.id !== productId)
      localStorage.setItem('cart', JSON.stringify(this.cartItems))
      this.loadCart()
    },
    formatPrice(price) {
      return new Intl.NumberFormat('ja-JP').format(price)
    },
    proceedToCheckout() {
      // Close cart modal
      const cartModal = bootstrap.Modal.getInstance(document.getElementById('cartModal'))
      cartModal.hide()
      
      // Open checkout modal
      setTimeout(() => {
        const checkoutModal = new bootstrap.Modal(document.getElementById('checkoutModal'))
        checkoutModal.show()
      }, 300)
    },
    onOrderCreated(order) {
      // Clear cart
      localStorage.removeItem('cart')
      this.loadCart()
      
      // Show success message
      alert(`注文が完了しました！\n注文番号: ${order.id}\n合計金額: ¥${this.formatPrice(order.totalAmount)}`)
      
      // Reload products to update stock
      this.loadProducts()
    }
  }
}
</script>

<style scoped>
.container-fluid {
  max-width: 1400px;
}

.modal-body img {
  max-height: 60px;
  object-fit: cover;
}

.input-group-sm .form-control {
  max-width: 50px;
}
</style>
