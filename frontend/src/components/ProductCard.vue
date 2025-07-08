<template>
  <div class="card h-100 shadow-sm">
    <img :src="product.imageUrl" class="card-img-top" :alt="product.name" style="height: 200px; object-fit: cover;">
    <div class="card-body d-flex flex-column">
      <h5 class="card-title">{{ product.name }}</h5>
      <p class="card-text text-muted small flex-grow-1">{{ product.description }}</p>
      <div class="mt-auto">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <span class="h5 text-primary mb-0">¥{{ formatPrice(product.price) }}</span>
          <small class="text-muted">在庫: {{ product.stock }}個</small>
        </div>
        <button 
          class="btn btn-primary w-100" 
          @click="addToCart"
          :disabled="product.stock === 0"
        >
          <i class="bi bi-cart-plus me-1"></i>
          {{ product.stock === 0 ? '在庫切れ' : 'カートに追加' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ProductCard',
  props: {
    product: {
      type: Object,
      required: true
    }
  },
  methods: {
    formatPrice(price) {
      return new Intl.NumberFormat('ja-JP').format(price)
    },
    addToCart() {
      // ローカルストレージからカート情報を取得
      let cart = JSON.parse(localStorage.getItem('cart') || '[]')
      
      // 既にカートに同じ商品があるかチェック
      const existingItem = cart.find(item => item.id === this.product.id)
      
      if (existingItem) {
        // 既存の商品の数量を増やす
        existingItem.quantity += 1
      } else {
        // 新しい商品をカートに追加
        cart.push({
          id: this.product.id,
          name: this.product.name,
          price: this.product.price,
          imageUrl: this.product.imageUrl,
          quantity: 1
        })
      }
      
      // ローカルストレージに保存
      localStorage.setItem('cart', JSON.stringify(cart))
      
      // 親コンポーネントに通知
      this.$emit('added-to-cart', {
        product: this.product,
        cartItemCount: cart.reduce((total, item) => total + item.quantity, 0)
      })
      
      // 成功メッセージを表示
      this.showSuccessMessage()
    },
    showSuccessMessage() {
      // 簡単な成功メッセージ表示
      const button = event.target
      const originalText = button.innerHTML
      button.innerHTML = '<i class="bi bi-check-circle me-1"></i>追加完了!'
      button.classList.remove('btn-primary')
      button.classList.add('btn-success')
      
      setTimeout(() => {
        button.innerHTML = originalText
        button.classList.remove('btn-success')
        button.classList.add('btn-primary')
      }, 1500)
    }
  }
}
</script>

<style scoped>
.card {
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15) !important;
}

.btn {
  transition: all 0.2s ease-in-out;
}
</style>
