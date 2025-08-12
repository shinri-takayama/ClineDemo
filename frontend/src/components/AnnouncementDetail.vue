<template>
  <!-- お知らせ詳細モーダル -->
  <div class="modal fade show" style="display: block;" tabindex="-1">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="bi bi-megaphone me-2"></i>
            お知らせ詳細
          </h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <!-- ローディング表示 -->
        <div v-if="loading" class="modal-body text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">読み込み中...</span>
          </div>
          <p class="mt-3">お知らせを読み込み中...</p>
        </div>
        
        <!-- エラー表示 -->
        <div v-else-if="error" class="modal-body">
          <div class="alert alert-danger">
            <i class="bi bi-exclamation-triangle me-2"></i>
            {{ error }}
          </div>
        </div>
        
        <!-- お知らせ詳細表示 -->
        <div v-else-if="announcement" class="modal-body">
          <!-- ヘッダー情報 -->
          <div class="mb-4">
            <div class="d-flex align-items-center mb-3">
              <span class="badge me-2" :class="getCategoryBadgeClass(announcement.category)">
                {{ announcement.category }}
              </span>
              <span v-if="!announcement.isPublished" class="badge bg-secondary">
                非公開
              </span>
            </div>
            
            <h3 class="mb-3">{{ announcement.title }}</h3>
            
            <div class="row text-muted small">
              <div class="col-md-6">
                <i class="bi bi-calendar me-1"></i>
                公開日: {{ formatDate(announcement.publishDate) }}
              </div>
              <div class="col-md-6">
                <i class="bi bi-clock me-1"></i>
                作成日: {{ formatDate(announcement.createdAt) }}
              </div>
            </div>
            
            <div v-if="announcement.updatedAt !== announcement.createdAt" class="text-muted small mt-1">
              <i class="bi bi-pencil me-1"></i>
              最終更新: {{ formatDate(announcement.updatedAt) }}
            </div>
          </div>
          
          <!-- 区切り線 -->
          <hr>
          
          <!-- お知らせ内容 -->
          <div class="announcement-content">
            <div class="content-text" v-html="formattedContent"></div>
          </div>
        </div>
        
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            閉じる
          </button>
        </div>
      </div>
    </div>
  </div>
  
  <!-- モーダル背景 -->
  <div class="modal-backdrop fade show"></div>
</template>

<script>
import announcementService from '../services/announcementService';

export default {
  name: 'AnnouncementDetail',
  props: {
    announcementId: {
      type: Number,
      required: true
    }
  },
  emits: ['close'],
  data() {
    return {
      announcement: null,
      loading: false,
      error: null
    };
  },
  computed: {
    /**
     * お知らせ内容を改行を考慮してフォーマット
     */
    formattedContent() {
      if (!this.announcement || !this.announcement.content) {
        return '';
      }
      
      // 改行文字を<br>タグに変換
      return this.announcement.content
        .replace(/\n/g, '<br>')
        .replace(/\r/g, '');
    }
  },
  mounted() {
    this.loadAnnouncement();
    
    // ESCキーでモーダルを閉じる
    document.addEventListener('keydown', this.handleKeydown);
  },
  beforeUnmount() {
    // イベントリスナーを削除
    document.removeEventListener('keydown', this.handleKeydown);
  },
  methods: {
    /**
     * お知らせ詳細を読み込む
     */
    async loadAnnouncement() {
      this.loading = true;
      this.error = null;
      
      try {
        // 1. announcementService.getAnnouncementを呼び出し
        // 2. propsで受け取ったannouncementIdを渡す
        // 3. APIから詳細データを取得
        this.announcement = await announcementService.getAnnouncement(this.announcementId);
      } catch (error) {
        console.error('お知らせ詳細の読み込みに失敗しました:', error);
        this.error = error.message;
      } finally {
        this.loading = false;
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
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
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

.announcement-content {
  line-height: 1.8;
  font-size: 1rem;
}

.content-text {
  white-space: pre-wrap;
  word-wrap: break-word;
}

.badge {
  font-size: 0.75em;
}

.modal-dialog {
  margin: 1.75rem auto;
}

@media (max-width: 576px) {
  .modal-dialog {
    margin: 0.5rem;
  }
}

/* 改行を適切に表示 */
.content-text >>> br {
  line-height: 1.8;
}

/* リンクスタイル（将来的にリンクが含まれる場合） */
.content-text >>> a {
  color: #0d6efd;
  text-decoration: underline;
}

.content-text >>> a:hover {
  color: #0a58ca;
}
</style>
