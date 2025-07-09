<template>
  <div class="modal fade" id="registerModal" tabindex="-1" aria-labelledby="registerModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="registerModalLabel">新規登録</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleRegister">
            <div class="row">
              <div class="col-md-6 mb-3">
                <label for="firstName" class="form-label">名前</label>
                <input
                  type="text"
                  class="form-control"
                  id="firstName"
                  v-model="form.firstName"
                  :class="{ 'is-invalid': errors.firstName }"
                >
                <div v-if="errors.firstName" class="invalid-feedback">
                  {{ errors.firstName }}
                </div>
              </div>
              <div class="col-md-6 mb-3">
                <label for="lastName" class="form-label">姓</label>
                <input
                  type="text"
                  class="form-control"
                  id="lastName"
                  v-model="form.lastName"
                  :class="{ 'is-invalid': errors.lastName }"
                >
                <div v-if="errors.lastName" class="invalid-feedback">
                  {{ errors.lastName }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <label for="registerUsername" class="form-label">ユーザー名 <span class="text-danger">*</span></label>
              <input
                type="text"
                class="form-control"
                id="registerUsername"
                v-model="form.username"
                :class="{ 'is-invalid': errors.username }"
                required
              >
              <div v-if="errors.username" class="invalid-feedback">
                {{ errors.username }}
              </div>
            </div>
            <div class="mb-3">
              <label for="email" class="form-label">メールアドレス <span class="text-danger">*</span></label>
              <input
                type="email"
                class="form-control"
                id="email"
                v-model="form.email"
                :class="{ 'is-invalid': errors.email }"
                required
              >
              <div v-if="errors.email" class="invalid-feedback">
                {{ errors.email }}
              </div>
            </div>
            <div class="mb-3">
              <label for="registerPassword" class="form-label">パスワード <span class="text-danger">*</span></label>
              <input
                type="password"
                class="form-control"
                id="registerPassword"
                v-model="form.password"
                :class="{ 'is-invalid': errors.password }"
                required
              >
              <div class="form-text">6文字以上で入力してください</div>
              <div v-if="errors.password" class="invalid-feedback">
                {{ errors.password }}
              </div>
            </div>
            <div class="mb-3">
              <label for="confirmPassword" class="form-label">パスワード確認 <span class="text-danger">*</span></label>
              <input
                type="password"
                class="form-control"
                id="confirmPassword"
                v-model="form.confirmPassword"
                :class="{ 'is-invalid': errors.confirmPassword }"
                required
              >
              <div v-if="errors.confirmPassword" class="invalid-feedback">
                {{ errors.confirmPassword }}
              </div>
            </div>
            <div v-if="errorMessage" class="alert alert-danger" role="alert">
              {{ errorMessage }}
            </div>
            <div class="d-grid">
              <button type="submit" class="btn btn-success" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                {{ loading ? '登録中...' : '新規登録' }}
              </button>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <p class="text-muted mb-0">
            既にアカウントをお持ちの方は
            <a href="#" @click="switchToLogin" class="text-decoration-none">ログイン</a>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import authService from '../services/authService'

export default {
  name: 'RegisterForm',
  data() {
    return {
      form: {
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        firstName: '',
        lastName: ''
      },
      errors: {},
      errorMessage: '',
      loading: false
    }
  },
  methods: {
    async handleRegister() {
      this.loading = true
      this.errors = {}
      this.errorMessage = ''

      // Validation
      if (!this.form.username.trim()) {
        this.errors.username = 'ユーザー名を入力してください'
      } else if (this.form.username.length < 3) {
        this.errors.username = 'ユーザー名は3文字以上で入力してください'
      }

      if (!this.form.email.trim()) {
        this.errors.email = 'メールアドレスを入力してください'
      } else if (!this.isValidEmail(this.form.email)) {
        this.errors.email = '有効なメールアドレスを入力してください'
      }

      if (!this.form.password) {
        this.errors.password = 'パスワードを入力してください'
      } else if (this.form.password.length < 6) {
        this.errors.password = 'パスワードは6文字以上で入力してください'
      }

      if (!this.form.confirmPassword) {
        this.errors.confirmPassword = 'パスワード確認を入力してください'
      } else if (this.form.password !== this.form.confirmPassword) {
        this.errors.confirmPassword = 'パスワードが一致しません'
      }

      if (Object.keys(this.errors).length > 0) {
        this.loading = false
        return
      }

      try {
        const userData = {
          username: this.form.username,
          email: this.form.email,
          password: this.form.password,
          firstName: this.form.firstName,
          lastName: this.form.lastName
        }

        const result = await authService.register(userData)
        
        if (result.success) {
          this.$emit('register-success', result.user)
          this.resetForm()
          // Close modal
          const modal = bootstrap.Modal.getInstance(document.getElementById('registerModal'))
          modal.hide()
        } else {
          this.errorMessage = typeof result.error === 'string' ? result.error : '登録に失敗しました'
        }
      } catch (error) {
        console.error('Registration error:', error)
        this.errorMessage = '登録に失敗しました'
      } finally {
        this.loading = false
      }
    },
    switchToLogin() {
      this.$emit('switch-to-login')
    },
    isValidEmail(email) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      return emailRegex.test(email)
    },
    resetForm() {
      this.form = {
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        firstName: '',
        lastName: ''
      }
      this.errors = {}
      this.errorMessage = ''
      this.loading = false
    }
  },
  mounted() {
    // Reset form when modal is hidden
    const modal = document.getElementById('registerModal')
    modal.addEventListener('hidden.bs.modal', () => {
      this.resetForm()
    })
  }
}
</script>

<style scoped>
.modal-content {
  border-radius: 10px;
}

.btn-success {
  background-color: #28a745;
  border-color: #28a745;
}

.btn-success:hover {
  background-color: #218838;
  border-color: #1e7e34;
}

.form-control:focus {
  border-color: #28a745;
  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25);
}

.text-danger {
  color: #dc3545 !important;
}
</style>
