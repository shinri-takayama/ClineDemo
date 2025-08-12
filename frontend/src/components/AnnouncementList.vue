<template>
  <!-- お知らせ一覧モーダル -->
  <div class="modal fade show" style="display: block;" tabindex="-1">
    <div class="modal-dialog modal-xl">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="bi bi-megaphone me-2"></i>
            お知らせ
          </h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <div class="modal-body">
          <div class="announcement-list">
    <!-- ヘッダー部分 -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>
        <i class="bi bi-megaphone me-2"></i>
        お知らせ
      </h2>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary" @click="loadAnnouncements">
          <i class="bi bi-arrow-clockwise"></i> 更新
        </button>
        <button 
          v-if="isAdmin" 
          class="btn btn-primary" 
          @click="showCreateForm = true"
        >
          <i class="bi bi-plus-circle"></i> 新規作成
        </button>
      </div>
    </div>

    <!-- 検索・フィルタ部分 -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row g-3">
          <!-- キーワード検索 -->
          <div class="col-md-4">
            <label class="form-label">キーワード検索</label>
            <input 
              type="text" 
              class="form-control" 
              v-model="filters.keyword"
              placeholder="タイトルや内容で検索"
              @input="debounceSearch"
            />
          </div>
          
          <!-- カテゴリフィルタ -->
          <div class="col-md-3">
            <label class="form-label">カテゴリ</label>
            <select class="form-select" v-model="filters.category" @change="applyFilters">
              <option value="">すべて</option>
              <option v-for="category in categories" :key="category" :value="category">
                {{ category }}
              </option>
            </select>
          </div>
          
          <!-- 公開状態フィルタ（管理者のみ） -->
          <div class="col-md-3" v-if="isAdmin">
            <label class="form-label">公開状態</label>
            <select class="form-select" v-model="filters.published" @change="applyFilters">
              <option :value="undefined">すべて</option>
              <option :value="true">公開中</option>
              <option :value="false">非公開</option>
            </select>
          </div>
          
          <!-- 検索ボタン -->
          <div class="col-md-2 d-flex align-items-end">
            <button class="btn btn-outline-secondary w-100" @click="clearFilters">
              <i class="bi bi-x-circle"></i> クリア
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- ローディング表示 -->
    <div v-if="loading" class="text-center py-4">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">読み込み中...</span>
      </div>
      <p class="mt-2">お知らせを読み込み中...</p>
    </div>

    <!-- エラー表示 -->
    <div v-else-if="error" class="alert alert-danger">
      <i class="bi bi-exclamation-triangle me-2"></i>
      {{ error }}
      <button class="btn btn-outline-danger btn-sm ms-2" @click="loadAnnouncements">
        再試行
      </button>
    </div>

    <!-- お知らせなし -->
    <div v-else-if="announcements.length === 0" class="text-center py-5">
      <i class="bi bi-info-circle display-1 text-muted"></i>
      <h3 class="mt-3 text-muted">お知らせがありません</h3>
      <p class="text-muted">現在表示できるお知らせはありません。</p>
    </div>

    <!-- お知らせ一覧 -->
    <div v-else class="row">
      <div class="col-12" v-for="announcement in announcements" :key="announcement.id">
        <div class="card mb-3 announcement-card" @click="viewDetails(announcement.id)">
          <div class="card-body">
            <div class="row">
              <div class="col-md-8">
                <div class="d-flex align-items-center mb-2">
                  <span class="badge me-2" :class="getCategoryBadgeClass(announcement.category)">
                    {{ announcement.category }}
                  </span>
                  <span v-if="!announcement.isPublished" class="badge bg-secondary">
                    非公開
                  </span>
                </div>
                <h5 class="card-title mb-2">{{ announcement.title }}</h5>
                <p class="card-text text-muted mb-1">
                  <small>
                    <i class="bi bi-calendar me-1"></i>
                    {{ formatDate(announcement.publishDate) }}
                  </small>
                </p>
              </div>
              <div class="col-md-4 text-md-end">
                <div class="btn-group" v-if="isAdmin">
                  <button 
                    class="btn btn-outline-primary btn-sm" 
                    @click.stop="editAnnouncement(announcement)"
                  >
                    <i class="bi bi-pencil"></i> 編集
                  </button>
                  <button 
                    class="btn btn-outline-danger btn-sm" 
                    @click.stop="confirmDelete(announcement)"
                  >
                    <i class="bi bi-trash"></i> 削除
                  </button>
                </div>
                <button v-else class="btn btn-outline-primary btn-sm">
                  <i class="bi bi-eye"></i> 詳細を見る
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- お知らせ詳細モーダル -->
    <AnnouncementDetail 
      v-if="selectedAnnouncementId"
      :announcement-id="selectedAnnouncementId" 
      @close="selectedAnnouncementId = null"
    />

    <!-- お知らせ作成・編集フォーム -->
    <AnnouncementForm 
      v-if="showCreateForm || editingAnnouncement"
      :announcement="editingAnnouncement"
      @close="closeForm"
      @saved="handleSaved"
    />

    <!-- 削除確認モーダル -->
    <div class="modal fade" id="deleteModal" tabindex="-1" v-if="deletingAnnouncement">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">お知らせ削除確認</h5>
            <button type="button" class="btn-close" @click="closeDeleteModal"></button>
          </div>
          <div class="modal-body">
            <p>以下のお知らせを削除してもよろしいですか？</p>
            <div class="alert alert-warning">
              <strong>{{ deletingAnnouncement.title }}</strong>
            </div>
            <p class="text-danger">
              <i class="bi bi-exclamation-triangle me-1"></i>
              この操作は取り消せません。
            </p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeDeleteModal">
              キャンセル
            </button>
            <button type="button" class="btn btn-danger" @click="deleteAnnouncement" :disabled="deleting">
              <span v-if="deleting" class="spinner-border spinner-border-sm me-2"></span>
              削除する
            </button>
          </div>
        </div>
      </div>
    </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import announcementService from '../services/announcementService';
import authService from '../services/authService';
import AnnouncementDetail from './AnnouncementDetail.vue';
import AnnouncementForm from './AnnouncementForm.vue';

export default {
  name: 'AnnouncementList',
  emits: ['close'],
  components: {
    AnnouncementDetail,
    AnnouncementForm
  },
  data() {
    return {
      announcements: [],
      loading: false,
      error: null,
      filters: {
        keyword: '',
        category: '',
        published: undefined
      },
      categories: [],
      selectedAnnouncementId: null,
      showCreateForm: false,
      editingAnnouncement: null,
      deletingAnnouncement: null,
      deleting: false,
      searchTimeout: null,
      deleteModalInstance: null
    };
  },
  computed: {
    isAdmin() {
      return authService.isAdmin();
    }
  },
  mounted() {
    this.loadCategories();
    this.loadAnnouncements();
  },
  methods: {
    /**
     * お知らせ一覧を読み込む
     */
    async loadAnnouncements() {
      this.loading = true;
      this.error = null;
      
      try {
        // 1. announcementService.getAnnouncementsを呼び出し
        // 2. filtersオブジェクトをパラメータとして渡す
        // 3. クエリパラメータが自動的に構築される
        this.announcements = await announcementService.getAnnouncements(this.filters);
      } catch (error) {
        console.error('お知らせ一覧の読み込みに失敗しました:', error);
        this.error = error.message;
      } finally {
        this.loading = false;
      }
    },

    /**
     * カテゴリ一覧を読み込む
     */
    loadCategories() {
      this.categories = announcementService.getCategories();
    },

    /**
     * フィルタを適用してお知らせを再読み込み
     */
    applyFilters() {
      this.loadAnnouncements();
    },

    /**
     * 検索のデバウンス処理
     */
    debounceSearch() {
      // 既存のタイマーをクリア
      if (this.searchTimeout) {
        clearTimeout(this.searchTimeout);
      }
      
      // 300ms後に検索実行
      this.searchTimeout = setTimeout(() => {
        this.applyFilters();
      }, 300);
    },

    /**
     * フィルタをクリア
     */
    clearFilters() {
      this.filters = {
        keyword: '',
        category: '',
        published: undefined
      };
      this.loadAnnouncements();
    },

    /**
     * お知らせ詳細を表示
     */
    viewDetails(announcementId) {
      this.selectedAnnouncementId = announcementId;
    },

    /**
     * お知らせ編集
     */
    editAnnouncement(announcement) {
      this.editingAnnouncement = { ...announcement };
    },

    /**
     * フォームを閉じる
     */
    closeForm() {
      this.showCreateForm = false;
      this.editingAnnouncement = null;
    },

    /**
     * お知らせ保存後の処理
     */
    handleSaved() {
      this.closeForm();
      this.loadAnnouncements();
    },

    /**
     * 削除確認
     */
    async confirmDelete(announcement) {
      this.deletingAnnouncement = announcement;
      
      // DOM更新を待つ
      await this.$nextTick();
      
      // モーダル要素の存在確認
      const modalElement = document.getElementById('deleteModal');
      if (modalElement) {
        // Bootstrap モーダルを表示
        this.deleteModalInstance = new bootstrap.Modal(modalElement);
        this.deleteModalInstance.show();
        
        // モーダルが閉じられた時のイベントリスナーを追加
        modalElement.addEventListener('hidden.bs.modal', this.handleDeleteModalHidden);
      } else {
        console.error('削除確認モーダルが見つかりません');
        // フォールバック: ブラウザ標準の確認ダイアログ
        if (confirm(`「${announcement.title}」を削除してもよろしいですか？\n\nこの操作は取り消せません。`)) {
          await this.deleteAnnouncement();
        }
        this.deletingAnnouncement = null;
      }
    },

    /**
     * 削除モーダルが閉じられた時の処理
     */
    handleDeleteModalHidden() {
      // モーダルインスタンスをクリーンアップ
      if (this.deleteModalInstance) {
        this.deleteModalInstance.dispose();
        this.deleteModalInstance = null;
      }
      
      // イベントリスナーを削除
      const modalElement = document.getElementById('deleteModal');
      if (modalElement) {
        modalElement.removeEventListener('hidden.bs.modal', this.handleDeleteModalHidden);
      }
      
      // Vue.jsの状態をリセット
      this.deletingAnnouncement = null;
    },

    /**
     * 削除モーダルを閉じる
     */
    closeDeleteModal() {
      if (this.deleteModalInstance) {
        this.deleteModalInstance.hide();
      } else {
        // フォールバック
        this.deletingAnnouncement = null;
      }
    },

    /**
     * お知らせ削除実行
     */
    async deleteAnnouncement() {
      if (!this.deletingAnnouncement) return;

      this.deleting = true;
      
      try {
        // 1. announcementService.deleteAnnouncementを呼び出し
        // 2. 削除対象のIDを渡す
        // 3. 管理者権限チェックはサービス内で実行される
        await announcementService.deleteAnnouncement(this.deletingAnnouncement.id);
        
        // 4. 成功時はモーダルを閉じて一覧を再読み込み
        this.deletingAnnouncement = null;
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        modal.hide();
        
        this.loadAnnouncements();
        
        // 成功メッセージ表示（簡易版）
        alert('お知らせを削除しました。');
      } catch (error) {
        console.error('お知らせ削除に失敗しました:', error);
        alert(`削除に失敗しました: ${error.message}`);
      } finally {
        this.deleting = false;
      }
    },

    /**
     * 日付フォーマット
     */
    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('ja-JP', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    },

    /**
     * カテゴリバッジのCSSクラスを取得
     */
    getCategoryBadgeClass(category) {
      const categoryClasses = {
        '重要': 'bg-danger',
        '一般': 'bg-primary',
        'メンテナンス': 'bg-warning text-dark',
        'キャンペーン': 'bg-success',
        'システム': 'bg-info text-dark',
        'その他': 'bg-secondary'
      };
      return categoryClasses[category] || 'bg-secondary';
    }
  }
};
</script>

<style scoped>
.announcement-card {
  cursor: pointer;
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.announcement-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.btn-group .btn {
  border-radius: 0.375rem;
}

.btn-group .btn:not(:last-child) {
  margin-right: 0.25rem;
}

.badge {
  font-size: 0.75em;
}

.spinner-border-sm {
  width: 1rem;
  height: 1rem;
}
</style>
