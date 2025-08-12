# ECShop API設計書

## 1. API概要

### 1.1 基本情報
- **ベースURL**: `http://localhost:5000/api`
- **認証方式**: JWT Bearer Token
- **データ形式**: JSON
- **文字エンコーディング**: UTF-8
- **HTTPSサポート**: 本番環境推奨

### 1.2 共通レスポンス形式

#### 成功レスポンス
```json
{
  "data": {...},
  "message": "Success"
}
```

#### エラーレスポンス
```json
{
  "error": "エラーメッセージ",
  "details": "詳細情報（オプション）"
}
```

### 1.3 認証ヘッダー
```
Authorization: Bearer {JWT_TOKEN}
```

## 2. 認証API

### 2.1 ユーザー登録
- **エンドポイント**: `POST /api/auth/register`
- **認証**: 不要
- **説明**: 新規ユーザーを登録し、JWTトークンを返す

#### リクエスト
```json
{
  "username": "string (required, max: 50)",
  "email": "string (required, email format, max: 100)",
  "password": "string (required, min: 6)",
  "firstName": "string (required, max: 50)",
  "lastName": "string (required, max: 50)"
}
```

#### レスポンス (200 OK)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "firstName": "太郎",
    "lastName": "田中",
    "createdAt": "2024-01-01T00:00:00Z",
    "isAdmin": false
  }
}
```

#### エラーレスポンス
- **400 Bad Request**: ユーザー名またはメールアドレスが既に存在
- **400 Bad Request**: バリデーションエラー

### 2.2 ログイン
- **エンドポイント**: `POST /api/auth/login`
- **認証**: 不要
- **説明**: ユーザー認証を行い、JWTトークンを返す

#### リクエスト
```json
{
  "username": "string (required)",
  "password": "string (required)"
}
```

#### レスポンス (200 OK)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "firstName": "太郎",
    "lastName": "田中",
    "createdAt": "2024-01-01T00:00:00Z",
    "isAdmin": false
  }
}
```

#### エラーレスポンス
- **400 Bad Request**: 認証情報が無効

### 2.3 プロフィール取得
- **エンドポイント**: `GET /api/auth/profile`
- **認証**: 必要
- **説明**: 現在のユーザー情報を取得

#### レスポンス (200 OK)
```json
{
  "id": 1,
  "username": "testuser",
  "email": "test@example.com",
  "firstName": "太郎",
  "lastName": "田中",
  "createdAt": "2024-01-01T00:00:00Z",
  "isAdmin": false
}
```

#### エラーレスポンス
- **401 Unauthorized**: 認証が必要
- **404 Not Found**: ユーザーが見つからない

## 3. 商品API

### 3.1 商品一覧取得
- **エンドポイント**: `GET /api/products`
- **認証**: 不要
- **説明**: 商品一覧を検索・フィルタリングして取得

#### クエリパラメータ
- `search` (string, optional): 商品名・説明での検索
- `minPrice` (decimal, optional): 最低価格
- `maxPrice` (decimal, optional): 最高価格
- `sortBy` (string, optional): ソート項目 (price, name, date)
- `sortOrder` (string, optional): ソート順 (asc, desc)

#### レスポンス (200 OK)
```json
[
  {
    "id": 1,
    "name": "商品名",
    "description": "商品説明",
    "price": 1000.00,
    "imageUrl": "https://example.com/image.jpg",
    "stock": 10,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

### 3.2 商品詳細取得
- **エンドポイント**: `GET /api/products/{id}`
- **認証**: 不要
- **説明**: 指定IDの商品詳細を取得

#### レスポンス (200 OK)
```json
{
  "id": 1,
  "name": "商品名",
  "description": "商品説明",
  "price": 1000.00,
  "imageUrl": "https://example.com/image.jpg",
  "stock": 10,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### エラーレスポンス
- **404 Not Found**: 商品が見つからない

### 3.3 商品作成
- **エンドポイント**: `POST /api/products`
- **認証**: 必要（管理者権限）
- **説明**: 新しい商品を作成

#### リクエスト
```json
{
  "name": "string (required, max: 100)",
  "description": "string (required, max: 500)",
  "price": "decimal (required, min: 0)",
  "imageUrl": "string (optional, max: 200)",
  "stock": "integer (required, min: 0)"
}
```

#### レスポンス (201 Created)
```json
{
  "id": 1,
  "name": "商品名",
  "description": "商品説明",
  "price": 1000.00,
  "imageUrl": "https://example.com/image.jpg",
  "stock": 10,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### 3.4 商品更新
- **エンドポイント**: `PUT /api/products/{id}`
- **認証**: 必要（管理者権限）
- **説明**: 指定IDの商品を更新

#### リクエスト
```json
{
  "id": 1,
  "name": "string (required, max: 100)",
  "description": "string (required, max: 500)",
  "price": "decimal (required, min: 0)",
  "imageUrl": "string (optional, max: 200)",
  "stock": "integer (required, min: 0)"
}
```

#### レスポンス (204 No Content)

#### エラーレスポンス
- **404 Not Found**: 商品が見つからない
- **400 Bad Request**: IDが一致しない

### 3.5 商品削除
- **エンドポイント**: `DELETE /api/products/{id}`
- **認証**: 必要（管理者権限）
- **説明**: 指定IDの商品を削除

#### レスポンス (204 No Content)

#### エラーレスポンス
- **404 Not Found**: 商品が見つからない

## 4. 注文API

### 4.1 注文一覧取得
- **エンドポイント**: `GET /api/orders`
- **認証**: 必要
- **説明**: 現在のユーザーの注文一覧を取得

#### レスポンス (200 OK)
```json
[
  {
    "id": 1,
    "orderDate": "2024-01-01T00:00:00Z",
    "status": 0,
    "statusText": "注文確認中",
    "totalAmount": 2000.00,
    "itemCount": 2
  }
]
```

### 4.2 注文詳細取得
- **エンドポイント**: `GET /api/orders/{id}`
- **認証**: 必要
- **説明**: 指定IDの注文詳細を取得

#### レスポンス (200 OK)
```json
{
  "id": 1,
  "userId": 1,
  "orderDate": "2024-01-01T00:00:00Z",
  "status": 0,
  "statusText": "注文確認中",
  "totalAmount": 2000.00,
  "shippingName": "田中太郎",
  "shippingPostalCode": "123-4567",
  "shippingPrefecture": "東京都",
  "shippingCity": "渋谷区",
  "shippingAddressLine": "1-1-1 マンション101",
  "shippingPhone": "090-1234-5678",
  "notes": "配送メモ",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "商品名",
      "productDescription": "商品説明",
      "quantity": 2,
      "price": 1000.00,
      "subtotal": 2000.00
    }
  ]
}
```

#### エラーレスポンス
- **404 Not Found**: 注文が見つからない

### 4.3 注文作成
- **エンドポイント**: `POST /api/orders`
- **認証**: 必要
- **説明**: 新しい注文を作成

#### リクエスト
```json
{
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ],
  "shippingName": "string (required, max: 100)",
  "shippingPostalCode": "string (required, max: 10)",
  "shippingPrefecture": "string (required, max: 50)",
  "shippingCity": "string (required, max: 100)",
  "shippingAddressLine": "string (required, max: 200)",
  "shippingPhone": "string (optional, max: 20)",
  "notes": "string (optional, max: 500)"
}
```

#### レスポンス (201 Created)
```json
{
  "id": 1,
  "userId": 1,
  "orderDate": "2024-01-01T00:00:00Z",
  "status": 0,
  "statusText": "注文確認中",
  "totalAmount": 2000.00,
  "shippingName": "田中太郎",
  "shippingPostalCode": "123-4567",
  "shippingPrefecture": "東京都",
  "shippingCity": "渋谷区",
  "shippingAddressLine": "1-1-1 マンション101",
  "shippingPhone": "090-1234-5678",
  "notes": "配送メモ",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z",
  "items": [...]
}
```

#### エラーレスポンス
- **400 Bad Request**: 商品が見つからない、在庫不足

### 4.4 注文ステータス更新
- **エンドポイント**: `PUT /api/orders/{id}/status`
- **認証**: 必要
- **説明**: 注文のステータスを更新

#### リクエスト
```json
{
  "status": 1
}
```

#### レスポンス (204 No Content)

#### エラーレスポンス
- **404 Not Found**: 注文が見つからない

## 5. 管理者API

### 5.1 ユーザー一覧取得
- **エンドポイント**: `GET /api/admin/users`
- **認証**: 必要（管理者権限）
- **説明**: 全ユーザーの一覧を取得

#### レスポンス (200 OK)
```json
[
  {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "firstName": "太郎",
    "lastName": "田中",
    "createdAt": "2024-01-01T00:00:00Z",
    "isAdmin": false
  }
]
```

### 5.2 管理者権限更新
- **エンドポイント**: `PUT /api/admin/users/{id}/admin-status`
- **認証**: 必要（管理者権限）
- **説明**: ユーザーの管理者権限を更新

#### リクエスト
```json
{
  "isAdmin": true
}
```

#### レスポンス (200 OK)
```json
{
  "message": "ユーザー 'testuser' の管理者権限を更新しました。"
}
```

#### エラーレスポンス
- **400 Bad Request**: 最後の管理者の権限は削除できない
- **404 Not Found**: ユーザーが見つからない

### 5.3 全注文取得
- **エンドポイント**: `GET /api/admin/orders`
- **認証**: 必要（管理者権限）
- **説明**: 全ユーザーの注文を取得（ページネーション対応）

#### クエリパラメータ
- `page` (integer, optional, default: 1): ページ番号
- `pageSize` (integer, optional, default: 10): ページサイズ
- `status` (integer, optional): ステータスフィルタ

#### レスポンス (200 OK)
```json
{
  "orders": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

### 5.4 注文ステータス更新（管理者）
- **エンドポイント**: `PUT /api/admin/orders/{id}/status`
- **認証**: 必要（管理者権限）
- **説明**: 任意の注文のステータスを更新

#### リクエスト
```json
{
  "status": 2
}
```

#### レスポンス (200 OK)
```json
{
  "message": "注文 #1 のステータスを更新しました。"
}
```

### 5.5 商品管理（管理者）
- **エンドポイント**: `GET /api/admin/products`
- **認証**: 必要（管理者権限）
- **説明**: 管理者向け商品一覧取得

#### レスポンス (200 OK)
```json
[
  {
    "id": 1,
    "name": "商品名",
    "description": "商品説明",
    "price": 1000.00,
    "imageUrl": "https://example.com/image.jpg",
    "stock": 10,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

### 5.6 ダッシュボード情報取得
- **エンドポイント**: `GET /api/admin/dashboard`
- **認証**: 必要（管理者権限）
- **説明**: 管理者ダッシュボード用の統計情報を取得

#### レスポンス (200 OK)
```json
{
  "summary": {
    "totalUsers": 100,
    "totalProducts": 50,
    "totalOrders": 200,
    "totalRevenue": 500000.00
  },
  "recentOrders": [...],
  "lowStockProducts": [...]
}
```

## 6. エラーコード一覧

### 6.1 HTTPステータスコード
- **200 OK**: 成功
- **201 Created**: 作成成功
- **204 No Content**: 更新・削除成功
- **400 Bad Request**: リクエストエラー
- **401 Unauthorized**: 認証エラー
- **403 Forbidden**: 権限エラー
- **404 Not Found**: リソースが見つからない
- **500 Internal Server Error**: サーバーエラー

### 6.2 カスタムエラーメッセージ
- **認証関連**
  - `ユーザー名が既に存在します`
  - `メールアドレスが既に存在します`
  - `認証情報が無効です`
  - `管理者権限が必要です`

- **商品関連**
  - `商品が見つかりません`
  - `在庫が不足しています`

- **注文関連**
  - `注文が見つかりません`

- **バリデーション関連**
  - `必須項目が入力されていません`
  - `値が範囲外です`
  - `IDが一致しません`

## 7. レート制限

現在未実装ですが、将来的に以下の制限を検討：
- **認証API**: 1分間に5回まで
- **一般API**: 1分間に100回まで
- **管理者API**: 1分間に200回まで

## 8. バージョニング

現在はv1のみ。将来的にAPIバージョニングを検討：
- **URL**: `/api/v1/products`
- **ヘッダー**: `Accept: application/vnd.ecshop.v1+json`

## 9. 開発・テスト

### 9.1 Swagger UI
- **URL**: `http://localhost:5000/swagger`
- **説明**: 開発環境でのAPI仕様確認・テスト

### 9.2 Postmanコレクション
将来的にPostmanコレクションを提供予定

### 9.3 APIテスト
- **単体テスト**: xUnit + ASP.NET Core Test Host
- **統合テスト**: 実際のHTTPリクエスト/レスポンス
