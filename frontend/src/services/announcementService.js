import authService from './authService';

class AnnouncementService {
  constructor() {
    this.baseURL = 'http://localhost:5000/api/announcements';
  }

  /**
   * お知らせ一覧取得（クエリパラメータ対応）
   * @param {Object} filters - フィルタ条件
   * @param {string} filters.category - カテゴリ
   * @param {string} filters.keyword - キーワード
   * @param {boolean} filters.published - 公開状態
   * @param {Date} filters.fromDate - 開始日
   * @param {Date} filters.toDate - 終了日
   * @returns {Promise<Array>} お知らせ一覧
   */
  async getAnnouncements(filters = {}) {
    try {
      // 1. クエリパラメータを構築
      const params = new URLSearchParams();
      
      if (filters.category) {
        params.append('category', filters.category);
      }
      
      if (filters.keyword) {
        params.append('keyword', filters.keyword);
      }
      
      if (filters.published !== undefined) {
        params.append('published', filters.published.toString());
      }
      
      if (filters.fromDate) {
        params.append('fromDate', filters.fromDate.toISOString());
      }
      
      if (filters.toDate) {
        params.append('toDate', filters.toDate.toISOString());
      }

      // 2. URLを構築
      const url = params.toString() ? `${this.baseURL}?${params}` : this.baseURL;

      // 3. APIリクエスト送信
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      // 4. エラーハンドリング
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: お知らせの取得に失敗しました`);
      }

      // 5. レスポンスをJSONで返却
      return await response.json();
    } catch (error) {
      console.error('お知らせ一覧取得エラー:', error);
      throw new Error('お知らせの取得に失敗しました。ネットワーク接続を確認してください。');
    }
  }

  /**
   * お知らせ詳細取得
   * @param {number} id - お知らせID
   * @returns {Promise<Object>} お知らせ詳細
   */
  async getAnnouncement(id) {
    try {
      // 1. IDの妥当性チェック
      if (!id || id <= 0) {
        throw new Error('無効なお知らせIDです');
      }

      // 2. APIリクエスト送信
      const response = await fetch(`${this.baseURL}/${id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      // 3. エラーハンドリング
      if (response.status === 404) {
        throw new Error('お知らせが見つかりません');
      }

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: お知らせの取得に失敗しました`);
      }

      // 4. レスポンスをJSONで返却
      return await response.json();
    } catch (error) {
      console.error('お知らせ詳細取得エラー:', error);
      throw error;
    }
  }

  /**
   * お知らせ作成（管理者のみ）
   * @param {Object} announcementData - お知らせデータ
   * @param {string} announcementData.title - タイトル
   * @param {string} announcementData.content - 内容
   * @param {string} announcementData.category - カテゴリ
   * @param {boolean} announcementData.isPublished - 公開フラグ
   * @param {Date} announcementData.publishDate - 公開日
   * @returns {Promise<Object>} 作成されたお知らせ
   */
  async createAnnouncement(announcementData) {
    try {
      // 1. 認証チェック
      if (!authService.isAuthenticated()) {
        throw new Error('ログインが必要です');
      }

      if (!authService.isAdmin()) {
        throw new Error('管理者権限が必要です');
      }

      // 2. データの妥当性チェック
      if (!announcementData.title || !announcementData.title.trim()) {
        throw new Error('タイトルは必須です');
      }

      if (!announcementData.content || !announcementData.content.trim()) {
        throw new Error('内容は必須です');
      }

      if (!announcementData.category || !announcementData.category.trim()) {
        throw new Error('カテゴリは必須です');
      }

      if (!announcementData.publishDate) {
        throw new Error('公開日は必須です');
      }

      // 3. リクエストデータを準備
      const requestData = {
        title: announcementData.title.trim(),
        content: announcementData.content.trim(),
        category: announcementData.category.trim(),
        isPublished: announcementData.isPublished || false,
        publishDate: announcementData.publishDate instanceof Date 
          ? announcementData.publishDate.toISOString() 
          : announcementData.publishDate
      };

      // 4. APIリクエスト送信
      const response = await fetch(this.baseURL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...authService.getAuthHeader()
        },
        body: JSON.stringify(requestData)
      });

      // 5. エラーハンドリング
      if (response.status === 401) {
        authService.logout();
        throw new Error('認証が必要です。再度ログインしてください。');
      }

      if (response.status === 403) {
        throw new Error('管理者権限が必要です');
      }

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(`お知らせの作成に失敗しました: ${errorData}`);
      }

      // 6. レスポンスをJSONで返却
      return await response.json();
    } catch (error) {
      console.error('お知らせ作成エラー:', error);
      throw error;
    }
  }

  /**
   * お知らせ更新（管理者のみ）
   * @param {number} id - お知らせID
   * @param {Object} announcementData - 更新データ
   * @returns {Promise<void>}
   */
  async updateAnnouncement(id, announcementData) {
    try {
      // 1. 認証チェック
      if (!authService.isAuthenticated()) {
        throw new Error('ログインが必要です');
      }

      if (!authService.isAdmin()) {
        throw new Error('管理者権限が必要です');
      }

      // 2. IDの妥当性チェック
      if (!id || id <= 0) {
        throw new Error('無効なお知らせIDです');
      }

      // 3. データの妥当性チェック
      if (!announcementData.title || !announcementData.title.trim()) {
        throw new Error('タイトルは必須です');
      }

      if (!announcementData.content || !announcementData.content.trim()) {
        throw new Error('内容は必須です');
      }

      if (!announcementData.category || !announcementData.category.trim()) {
        throw new Error('カテゴリは必須です');
      }

      if (!announcementData.publishDate) {
        throw new Error('公開日は必須です');
      }

      // 4. リクエストデータを準備
      const requestData = {
        title: announcementData.title.trim(),
        content: announcementData.content.trim(),
        category: announcementData.category.trim(),
        isPublished: announcementData.isPublished || false,
        publishDate: announcementData.publishDate instanceof Date 
          ? announcementData.publishDate.toISOString() 
          : announcementData.publishDate
      };

      // 5. APIリクエスト送信
      const response = await fetch(`${this.baseURL}/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          ...authService.getAuthHeader()
        },
        body: JSON.stringify(requestData)
      });

      // 6. エラーハンドリング
      if (response.status === 401) {
        authService.logout();
        throw new Error('認証が必要です。再度ログインしてください。');
      }

      if (response.status === 403) {
        throw new Error('管理者権限が必要です');
      }

      if (response.status === 404) {
        throw new Error('お知らせが見つかりません');
      }

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(`お知らせの更新に失敗しました: ${errorData}`);
      }

      // 7. 成功時は何も返さない（204 No Content）
    } catch (error) {
      console.error('お知らせ更新エラー:', error);
      throw error;
    }
  }

  /**
   * お知らせ削除（管理者のみ）
   * @param {number} id - お知らせID
   * @returns {Promise<void>}
   */
  async deleteAnnouncement(id) {
    try {
      // 1. 認証チェック
      if (!authService.isAuthenticated()) {
        throw new Error('ログインが必要です');
      }

      if (!authService.isAdmin()) {
        throw new Error('管理者権限が必要です');
      }

      // 2. IDの妥当性チェック
      if (!id || id <= 0) {
        throw new Error('無効なお知らせIDです');
      }

      // 3. APIリクエスト送信
      const response = await fetch(`${this.baseURL}/${id}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          ...authService.getAuthHeader()
        }
      });

      // 4. エラーハンドリング
      if (response.status === 401) {
        authService.logout();
        throw new Error('認証が必要です。再度ログインしてください。');
      }

      if (response.status === 403) {
        throw new Error('管理者権限が必要です');
      }

      if (response.status === 404) {
        throw new Error('お知らせが見つかりません');
      }

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(`お知らせの削除に失敗しました: ${errorData}`);
      }

      // 5. 成功時は何も返さない（204 No Content）
    } catch (error) {
      console.error('お知らせ削除エラー:', error);
      throw error;
    }
  }

  /**
   * カテゴリ一覧取得（固定値）
   * @returns {Array<string>} カテゴリ一覧
   */
  getCategories() {
    return [
      '重要',
      '一般',
      'メンテナンス',
      'キャンペーン',
      'システム',
      'その他'
    ];
  }
}

export default new AnnouncementService();
