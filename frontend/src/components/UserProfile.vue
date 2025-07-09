<template>
  <div class="user-profile-wrapper">
    <div class="dropdown">
      <button
        class="btn btn-light dropdown-toggle"
        type="button"
        id="userDropdown"
        data-bs-toggle="dropdown"
        aria-expanded="false"
      >
        <i class="fas fa-user me-2"></i>
        {{ displayName }}
      </button>
      <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown">
        <li>
          <h6 class="dropdown-header">
            <i class="fas fa-user-circle me-2"></i>
            {{ user?.username }}
          </h6>
        </li>
        <li><hr class="dropdown-divider"></li>
        <li>
          <span class="dropdown-item-text">
            <small class="text-muted">
              <i class="fas fa-envelope me-2"></i>
              {{ user?.email }}
            </small>
          </span>
        </li>
        <li v-if="user?.firstName || user?.lastName">
          <span class="dropdown-item-text">
            <small class="text-muted">
              <i class="fas fa-id-card me-2"></i>
              {{ fullName }}
            </small>
          </span>
        </li>
        <li v-if="user?.createdAt">
          <span class="dropdown-item-text">
            <small class="text-muted">
              <i class="fas fa-calendar me-2"></i>
              登録日: {{ formatDate(user.createdAt) }}
            </small>
          </span>
        </li>
        <li><hr class="dropdown-divider"></li>
        <li>
          <button class="dropdown-item" @click="showProfile">
            <i class="fas fa-user-edit me-2"></i>
            プロフィール
          </button>
        </li>
        <li>
          <button class="dropdown-item" @click="showOrderHistory">
            <i class="fas fa-history me-2"></i>
            注文履歴
          </button>
        </li>
        <li>
          <button class="dropdown-item text-danger" @click="handleLogout">
            <i class="fas fa-sign-out-alt me-2"></i>
            ログアウト
          </button>
        </li>
      </ul>
    </div>

    <!-- Profile Modal -->
    <div class="modal fade" id="profileModal" tabindex="-1" aria-labelledby="profileModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="profileModalLabel">
              <i class="fas fa-user-circle me-2"></i>
              プロフィール
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-md-4 text-center mb-3">
                <div class="profile-avatar">
                  <i class="fas fa-user-circle fa-5x text-primary"></i>
                </div>
              </div>
              <div class="col-md-8">
                <div class="profile-info">
                  <div class="mb-3">
                    <label class="form-label fw-bold">ユーザー名</label>
                    <p class="form-control-plaintext">{{ user?.username || '未設定' }}</p>
                  </div>
                  <div class="mb-3">
                    <label class="form-label fw-bold">メールアドレス</label>
                    <p class="form-control-plaintext">{{ user?.email || '未設定' }}</p>
                  </div>
                  <div class="row">
                    <div class="col-md-6 mb-3">
                      <label class="form-label fw-bold">名前</label>
                      <p class="form-control-plaintext">{{ user?.firstName || '未設定' }}</p>
                    </div>
                    <div class="col-md-6 mb-3">
                      <label class="form-label fw-bold">姓</label>
                      <p class="form-control-plaintext">{{ user?.lastName || '未設定' }}</p>
                    </div>
                  </div>
                  <div class="mb-3" v-if="user?.createdAt">
                    <label class="form-label fw-bold">登録日</label>
                    <p class="form-control-plaintext">{{ formatDate(user.createdAt) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">閉じる</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import authService from '../services/authService'

export default {
  name: 'UserProfile',
  props: {
    user: {
      type: Object,
      required: true,
      validator(value) {
        return value && typeof value === 'object' && value.username
      }
    }
  },
  computed: {
    displayName() {
      if (!this.user) return '未設定'
      if (this.user.firstName && this.user.lastName) {
        return `${this.user.firstName} ${this.user.lastName}`
      } else if (this.user.firstName) {
        return this.user.firstName
      } else if (this.user.lastName) {
        return this.user.lastName
      }
      return this.user.username || '未設定'
    },
    fullName() {
      if (!this.user) return '未設定'
      const parts = []
      if (this.user.firstName) parts.push(this.user.firstName)
      if (this.user.lastName) parts.push(this.user.lastName)
      return parts.join(' ') || '未設定'
    }
  },
  methods: {
    handleLogout() {
      authService.logout()
      this.$emit('logout')
    },
    showProfile() {
      const modal = new bootstrap.Modal(document.getElementById('profileModal'))
      modal.show()
    },
    showOrderHistory() {
      const modal = new bootstrap.Modal(document.getElementById('orderHistoryModal'))
      modal.show()
    },
    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleDateString('ja-JP', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    }
  }
}
</script>

<style scoped>
.dropdown-toggle::after {
  margin-left: 0.5rem;
}

.dropdown-menu {
  min-width: 250px;
}

.dropdown-header {
  font-size: 1rem;
  color: #495057;
}

.dropdown-item-text {
  padding: 0.25rem 1rem;
}

.dropdown-item:hover {
  background-color: #f8f9fa;
}

.text-danger:hover {
  background-color: #f8d7da !important;
}

.profile-avatar {
  margin-bottom: 1rem;
}

.profile-info .form-label {
  color: #6c757d;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.form-control-plaintext {
  padding: 0.375rem 0;
  margin-bottom: 0;
  font-size: 1rem;
  line-height: 1.5;
  color: #212529;
  background-color: transparent;
  border: solid transparent;
  border-width: 1px 0;
}

.modal-content {
  border-radius: 10px;
}

.btn-light {
  background-color: white;
  border-color: #dee2e6;
  color: #495057;
}

.btn-light:hover {
  background-color: #f8f9fa;
  border-color: #dee2e6;
  color: #495057;
}

.btn-light:focus {
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
</style>
