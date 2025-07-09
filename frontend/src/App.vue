<template>
  <div id="app">
    <!-- ナビゲーションバー -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
      <div class="container-fluid">
        <a class="navbar-brand fw-bold" href="#">
          <i class="bi bi-shop me-2"></i>
          ECショップ
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-item">
              <a class="nav-link active" href="#">
                <i class="bi bi-house me-1"></i>
                ホーム
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">
                <i class="bi bi-grid me-1"></i>
                カテゴリ
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">
                <i class="bi bi-info-circle me-1"></i>
                お問い合わせ
              </a>
            </li>
          </ul>
          <ul class="navbar-nav">
            <!-- 認証状態に応じた表示 -->
            <li v-show="!isAuthenticated" class="nav-item">
              <button class="btn btn-outline-light me-2" @click="showLogin">
                <i class="bi bi-box-arrow-in-right me-1"></i>
                ログイン
              </button>
            </li>
            <li v-show="!isAuthenticated" class="nav-item">
              <button class="btn btn-light" @click="showRegister">
                <i class="bi bi-person-plus me-1"></i>
                新規登録
              </button>
            </li>
            <li v-if="isAuthenticated && currentUser" class="nav-item">
              <UserProfile 
                :key="`user-${currentUser.id || Date.now()}`" 
                :user="currentUser" 
                @logout="handleLogout" 
              />
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- ウェルカムメッセージ -->
    <div v-if="isAuthenticated && currentUser" class="alert alert-success alert-dismissible fade show m-0" role="alert">
      <div class="container">
        <i class="bi bi-check-circle me-2"></i>
        <strong>{{ currentUser?.username }}</strong>さん、ようこそ！
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
      </div>
    </div>

    <!-- メインコンテンツ -->
    <main>
      <ProductList />
    </main>

    <!-- 認証モーダル -->
    <LoginForm 
      @login-success="handleLoginSuccess" 
      @switch-to-register="switchToRegister" 
    />
    <RegisterForm 
      @register-success="handleRegisterSuccess" 
      @switch-to-login="switchToLogin" 
    />

    <!-- Order History -->
    <OrderHistory />

    <!-- Admin Dashboard -->
    <AdminDashboard v-if="currentUser?.isAdmin" :current-user="currentUser" />

    <!-- フッター -->
    <footer class="bg-dark text-light py-4 mt-5">
      <div class="container">
        <div class="row">
          <div class="col-md-6">
            <h5>ECショップ</h5>
            <p class="mb-0">お客様に最高のショッピング体験をお届けします。</p>
          </div>
          <div class="col-md-6 text-md-end">
            <p class="mb-0">&copy; 2024 ECショップ. All rights reserved.</p>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
import ProductList from './components/ProductList.vue'
import LoginForm from './components/LoginForm.vue'
import RegisterForm from './components/RegisterForm.vue'
import UserProfile from './components/UserProfile.vue'
import OrderHistory from './components/OrderHistory.vue'
import AdminDashboard from './components/AdminDashboard.vue'
import authService from './services/authService'

export default {
  name: 'App',
  components: {
    ProductList,
    LoginForm,
    RegisterForm,
    UserProfile,
    OrderHistory,
    AdminDashboard
  },
  data() {
    return {
      isAuthenticated: false,
      currentUser: null
    }
  },
  methods: {
    showLogin() {
      const modal = new bootstrap.Modal(document.getElementById('loginModal'))
      modal.show()
    },
    showRegister() {
      const modal = new bootstrap.Modal(document.getElementById('registerModal'))
      modal.show()
    },
    switchToRegister() {
      // Hide login modal and show register modal
      const loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'))
      const registerModal = new bootstrap.Modal(document.getElementById('registerModal'))
      loginModal.hide()
      setTimeout(() => {
        registerModal.show()
      }, 300)
    },
    switchToLogin() {
      // Hide register modal and show login modal
      const registerModal = bootstrap.Modal.getInstance(document.getElementById('registerModal'))
      const loginModal = new bootstrap.Modal(document.getElementById('loginModal'))
      registerModal.hide()
      setTimeout(() => {
        loginModal.show()
      }, 300)
    },
    handleLoginSuccess(user) {
      this.isAuthenticated = true
      this.currentUser = user
      console.log('Login successful:', user)
    },
    handleRegisterSuccess(user) {
      this.isAuthenticated = true
      this.currentUser = user
      console.log('Registration successful:', user)
    },
    async handleLogout() {
      // First set authentication to false
      this.isAuthenticated = false
      
      // Wait for Vue to update the DOM
      await this.$nextTick()
      
      // Then clear user data
      this.currentUser = null
      console.log('Logged out')
    },
    checkAuthStatus() {
      // Check if user is already authenticated on app load
      if (authService.isAuthenticated()) {
        this.isAuthenticated = true
        this.currentUser = authService.getCurrentUser()
      }
    }
  },
  mounted() {
    this.checkAuthStatus()
  }
}
</script>

<style>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
}

body {
  background-color: #f8f9fa;
}

/* Bootstrap Icons CDN */
@import url("https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css");

/* Fade transition for UserProfile */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
