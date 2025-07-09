<template>
  <div class="modal fade" id="loginModal" tabindex="-1" aria-labelledby="loginModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="loginModalLabel">ログイン</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleLogin">
            <div class="mb-3">
              <label for="username" class="form-label">ユーザー名</label>
              <input
                type="text"
                class="form-control"
                id="username"
                v-model="form.username"
                :class="{ 'is-invalid': errors.username }"
                required
              >
              <div v-if="errors.username" class="invalid-feedback">
                {{ errors.username }}
              </div>
            </div>
            <div class="mb-3">
              <label for="password" class="form-label">パスワード</label>
              <input
                type="password"
                class="form-control"
                id="password"
                v-model="form.password"
                :class="{ 'is-invalid': errors.password }"
                required
              >
              <div v-if="errors.password" class="invalid-feedback">
                {{ errors.password }}
              </div>
            </div>
            <div v-if="errorMessage" class="alert alert-danger" role="alert">
              {{ errorMessage }}
            </div>
            <div class="d-grid">
              <button type="submit" class="btn btn-primary" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                {{ loading ? 'ログイン中...' : 'ログイン' }}
              </button>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <p class="text-muted mb-0">
            アカウントをお持ちでない方は
            <a href="#" @click="switchToRegister" class="text-decoration-none">新規登録</a>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import authService from '../services/authService'

export default {
  name: 'LoginForm',
  data() {
    return {
      form: {
        username: '',
        password: ''
      },
      errors: {},
      errorMessage: '',
      loading: false
    }
  },
  methods: {
    async handleLogin() {
      this.loading = true
      this.errors = {}
      this.errorMessage = ''

      // Basic validation
      if (!this.form.username.trim()) {
        this.errors.username = 'ユーザー名を入力してください'
      }
      if (!this.form.password) {
        this.errors.password = 'パスワードを入力してください'
      }

      if (Object.keys(this.errors).length > 0) {
        this.loading = false
        return
      }

      try {
        const result = await authService.login(this.form.username, this.form.password)
        
        if (result.success) {
          this.$emit('login-success', result.user)
          this.resetForm()
          // Close modal
          const modal = bootstrap.Modal.getInstance(document.getElementById('loginModal'))
          modal.hide()
        } else {
          this.errorMessage = typeof result.error === 'string' ? result.error : 'ログインに失敗しました'
        }
      } catch (error) {
        console.error('Login error:', error)
        this.errorMessage = 'ログインに失敗しました'
      } finally {
        this.loading = false
      }
    },
    switchToRegister() {
      this.$emit('switch-to-register')
    },
    resetForm() {
      this.form = {
        username: '',
        password: ''
      }
      this.errors = {}
      this.errorMessage = ''
      this.loading = false
    }
  },
  mounted() {
    // Reset form when modal is hidden
    const modal = document.getElementById('loginModal')
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

.btn-primary {
  background-color: #007bff;
  border-color: #007bff;
}

.btn-primary:hover {
  background-color: #0056b3;
  border-color: #0056b3;
}

.form-control:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
</style>
