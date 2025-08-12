<template>
  <!-- お知らせ作成・編集フォームモーダル -->
  <div class="modal fade show" style="display: block;" tabindex="-1">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="bi bi-megaphone me-2"></i>
            {{ isEditing ? 'お知らせ編集' : 'お知らせ作成' }}
          </h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <!-- タイトル -->
            <div class="mb-3">
              <label for="title" class="form-label">
                タイトル <span class="text-danger">*</span>
              </label>
              <input
                type="text"
                class="form-control"
                id="title"
                v-model="form.title"
                :class="{ 'is-invalid': errors.title }"
                placeholder="お知らせのタイトルを入力してください"
                maxlength="200"
                required
              />
              <div class="invalid-feedback" v-if="errors.title">
                {{ errors.title }}
              </div>
              <div class="form-text">
                {{ form.title.length }}/200文字
              </div>
            </div>

            <!-- カテゴリ -->
            <div class="mb-3">
              <label for="category" class="form-label">
                カテゴリ <span class="text-danger">*</span>
              </label>
              <select
                class="form-select"
                id="category"
                v-model="form.category"
                :class="{ 'is-invalid': errors.category }"
                required
              >
                <option value="">カテゴリを選択してください</option>
                <option v-for="category in categories" :key="category" :value="category">
                  {{ category }}
                </option>
              </select>
              <div class="invalid-feedback" v-if="errors.category">
                {{ errors.category }}
              </div>
            </div>

            <!-- 内容 -->
            <div class="mb-3">
              <label for="content" class="form-label">
                内容 <span class="text-danger">*</span>
              </label>
              <textarea
                class="form-control"
                id="content"
                v-model="form.content"
                :class="{ 'is-invalid': errors.content }"
                rows="8"
                placeholder="お知らせの内容を入力してください"
                maxlength="2000"
                required
              ></textarea>
              <div class="invalid-feedback" v-if="errors.content">
                {{ errors.content }}
              </div>
              <div class="form-text">
                {{ form.content.length }}/2000文字
              </div>
            </div>

            <!-- 公開日 -->
            <div class="mb-3">
              <label for="publishDate" class="form-label">
                公開日 <span class="text-danger">*</span>
              </label>
              <input
                type="datetime-local"
                class="form-control"
                id="publishDate"
                v-model="form.publishDate"
                :class="{ 'is-invalid': errors.publishDate }"
                required
              />
              <div class="invalid-feedback" v-if="errors.publishDate">
                {{ errors.publishDate }}
              </div>
            </div>

            <!-- 公開フラグ -->
            <div class="mb-3">
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="checkbox"
                  id="isPublished"
                  v-model="form.isPublished"
                />
                <label class="form-check-label" for="isPublished">
                  公開する
                </label>
              </div>
              <div class="form-text">
                チェックを外すと下書き状態になります
              </div>
            </div>

            <!-- エラー表示 -->
            <div v-if="error" class="alert alert-danger">
              <i class="bi bi-exclamation-triangle me-2"></i>
              {{ error }}
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              キャンセル
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ isEditing ? '更新する' : '作成する' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import announcementService from '../services/announcementService';

export default {
  name: 'AnnouncementForm',
  props: {
    announcement: {
      type: Object,
      default: null
    }
  },
  emits: ['close', 'saved'],
  data() {
    return {
      form: {
        title: '',
        content: '',
        category: '',
        isPublished: false,
        publishDate: ''
      },
      categories: [],
      errors: {},
      loading: false,
      error: null
    };
  },
  computed: {
    isEditing() {
      return this.announcement !== null;
    }
  },
  mounted() {
    this.loadCategories();
    this.initializeForm();
    
    // ESCキーでモーダルを閉じる
    document.addEventListener('keydown', this.handleKeydown);
  },
  beforeUnmount() {
    // イベントリスナーを削除
    document.removeEventListener('keydown', this.handleKeydown);
  },
  methods: {
    /**
     * カテゴリ一覧を読み込む
     */
    loadCategories() {
      this.categories = announcementService.getCategories();
    },

    /**
     * フォームを初期化
     */
    initializeForm() {
      if (this.isEditing) {
        // 編集モード：既存データを設定
        this.form = {
          title: this.announcement.title || '',
          content: this.announcement.content || '',
          category: this.announcement.category || '',
          isPublished: this.announcement.isPublished || false,
          publishDate: this.formatDateForInput(this.announcement.publishDate)
        };
      } else {
        // 新規作成モード：デフォルト値を設定
        const now = new Date();
        this.form = {
          title: '',
          content: '',
          category: '',
          isPublished: false,
          publishDate: this.formatDateForInput(now.toISOString())
        };
      }
    },

    /**
     * 日付をinput[type="datetime-local"]用にフォーマット
     */
    formatDateForInput(dateString) {
      const date = new Date(dateString);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      const hours = String(date.getHours()).padStart(2, '0');
      const minutes = String(date.getMinutes()).padStart(2, '0');
      
      return `${year}-${month}-${day}T${hours}:${minutes}`;
    },

    /**
     * フォーム送信処理
     */
    async handleSubmit() {
      // 1. バリデーション実行
      if (!this.validateForm()) {
        return;
      }

      this.loading = true;
      this.error = null;

      try {
        // 2. 送信データを準備
        const announcementData = {
          title: this.form.title.trim(),
          content: this.form.content.trim(),
          category: this.form.category,
          isPublished: this.form.isPublished,
          publishDate: new Date(this.form.publishDate)
        };

        if (this.isEditing) {
          // 3a. 編集モード：updateAnnouncementを呼び出し
          await announcementService.updateAnnouncement(this.announcement.id, announcementData);
        } else {
          // 3b. 新規作成モード：createAnnouncementを呼び出し
          await announcementService.createAnnouncement(announcementData);
        }

        // 4. 成功時：親コンポーネントに通知
        this.$emit('saved');
      } catch (error) {
        console.error('お知らせの保存に失敗しました:', error);
        this.error = error.message;
      } finally {
        this.loading = false;
      }
    },

    /**
     * フォームバリデーション
     */
    validateForm() {
      this.errors = {};

      // タイトルのバリデーション
      if (!this.form.title.trim()) {
        this.errors.title = 'タイトルは必須です';
      } else if (this.form.title.length > 200) {
        this.errors.title = 'タイトルは200文字以内で入力してください';
      }

      // カテゴリのバリデーション
      if (!this.form.category) {
        this.errors.category = 'カテゴリは必須です';
      }

      // 内容のバリデーション
      if (!this.form.content.trim()) {
        this.errors.content = '内容は必須です';
      } else if (this.form.content.length > 2000) {
        this.errors.content = '内容は2000文字以内で入力してください';
      }

      // 公開日のバリデーション
      if (!this.form.publishDate) {
        this.errors.publishDate = '公開日は必須です';
      } else {
        const publishDate = new Date(this.form.publishDate);
        if (isNaN(publishDate.getTime())) {
          this.errors.publishDate = '有効な日付を入力してください';
        }
      }

      // エラーがない場合はtrue
      return Object.keys(this.errors).length === 0;
    },

    /**
     * キーボードイベントハンドラ
     */
    handleKeydown(event) {
      // ESCキーが押された場合はモーダルを閉じる
      if (event.key === 'Escape') {
        this.$emit('close');
      }
    }
  }
};
</script>

<style scoped>
.modal {
  z-index: 1055;
}

.modal-backdrop {
  z-index: 1050;
}

.form-text {
  font-size: 0.875rem;
  color: #6c757d;
}

.text-danger {
  color: #dc3545 !important;
}

.is-invalid {
  border-color: #dc3545;
}

.invalid-feedback {
  display: block;
  width: 100%;
  margin-top: 0.25rem;
  font-size: 0.875rem;
  color: #dc3545;
}

.spinner-border-sm {
  width: 1rem;
  height: 1rem;
}

.modal-dialog {
  margin: 1.75rem auto;
}

@media (max-width: 576px) {
  .modal-dialog {
    margin: 0.5rem;
  }
}

/* テキストエリアのリサイズを縦方向のみに制限 */
textarea {
  resize: vertical;
}

/* フォーカス時のスタイル */
.form-control:focus,
.form-select:focus {
  border-color: #86b7fe;
  outline: 0;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}

/* チェックボックスのスタイル */
.form-check-input:checked {
  background-color: #0d6efd;
  border-color: #0d6efd;
}
</style>
