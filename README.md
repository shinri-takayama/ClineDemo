# ECショップ - C# + Vue.js ECサイト

C# ASP.NET Core Web APIとVue.jsを使用したECサイトのサンプルアプリケーションです。

## 機能

- 商品一覧表示（カード形式）
- ショッピングカート機能
- ローカルストレージでのカート管理
- **ユーザー認証機能**
  - ユーザー登録・ログイン
  - JWT トークンベース認証
  - プロフィール管理
  - セッション永続化
- **注文機能**
  - 注文作成・確定
  - 配送先情報入力
  - 在庫管理（注文時に在庫減算）
  - 注文ステータス管理
- **注文履歴機能**
  - 注文履歴一覧表示
  - 注文詳細表示
  - 注文ステータス追跡
  - 配送先情報確認
- レスポンシブデザイン

## 技術スタック

### バックエンド
- C# ASP.NET Core 8.0
- Entity Framework Core
- SQLite データベース
- **JWT 認証** (Microsoft.AspNetCore.Authentication.JwtBearer)
- **パスワードハッシュ化** (BCrypt.Net-Next)
- Swagger UI

### フロントエンド
- Vue.js 3
- Bootstrap 5
- Axios
- **Font Awesome** (アイコン)
- Bootstrap Icons

## セットアップ手順

### 1. 必要な環境
- .NET 8.0 SDK
- Node.js (v16以上)
- npm または yarn

### 2. バックエンドの起動

```bash
# ECShop.APIディレクトリに移動
cd ECShop/ECShop.API

# パッケージの復元
dotnet restore

# データベースの作成（初回のみ）
dotnet ef database update

# APIサーバーの起動
dotnet run
```

APIサーバーは `http://localhost:5000` で起動します。
Swagger UIは `http://localhost:5000/swagger` でアクセスできます。

### 3. フロントエンドの起動

```bash
# frontendディレクトリに移動
cd ECShop/frontend

# 依存関係のインストール
npm install

# 開発サーバーの起動
npm run serve
```

フロントエンドは `http://localhost:8080` で起動します。

## プロジェクト構成

```
ECShop/
├── ECShop.sln                    # Visual Studioソリューションファイル
├── ECShop.API/                   # バックエンドAPI
│   ├── Controllers/              # APIコントローラー
│   ├── Data/                     # データベースコンテキスト
│   ├── Models/                   # データモデル
│   ├── Program.cs                # アプリケーションエントリーポイント
│   └── appsettings.json          # 設定ファイル
├── frontend/                     # フロントエンドVue.js
│   ├── public/                   # 静的ファイル
│   ├── src/
│   │   ├── components/           # Vueコンポーネント
│   │   ├── services/             # API通信サービス
│   │   ├── App.vue               # メインアプリケーション
│   │   └── main.js               # エントリーポイント
│   ├── package.json              # npm設定
│   └── vue.config.js             # Vue CLI設定
└── README.md                     # このファイル
```

## API エンドポイント

### 商品API
- `GET /api/products` - 全商品取得
- `GET /api/products/{id}` - 特定商品取得
- `POST /api/products` - 商品作成
- `PUT /api/products/{id}` - 商品更新
- `DELETE /api/products/{id}` - 商品削除

### 認証API
- `POST /api/auth/register` - ユーザー登録
- `POST /api/auth/login` - ログイン
- `GET /api/auth/profile` - プロフィール取得 (要認証)

### 注文API
- `POST /api/orders` - 注文作成 (要認証)
- `GET /api/orders` - 注文履歴取得 (要認証)
- `GET /api/orders/{id}` - 注文詳細取得 (要認証)
- `PUT /api/orders/{id}/status` - 注文ステータス更新 (要認証)

## 主要コンポーネント

### ProductCard.vue
- 商品情報をカード形式で表示
- カートへの追加機能
- ホバーエフェクト

### ProductList.vue
- 商品一覧の表示
- カート表示モーダル
- ローディング・エラー処理

### 認証コンポーネント
- **LoginForm.vue** - ログインフォーム
- **RegisterForm.vue** - ユーザー登録フォーム
- **UserProfile.vue** - プロフィール表示・ログアウト機能

### 注文コンポーネント
- **CheckoutForm.vue** - 注文手続きフォーム
- **OrderHistory.vue** - 注文履歴一覧表示
- **OrderDetails.vue** - 注文詳細表示

## カート機能

- ローカルストレージを使用してカート情報を永続化
- 商品の追加・削除・数量変更
- 合計金額の計算

## 注文機能

### 注文フロー
1. **商品をカートに追加**
2. **カートを確認** - 数量変更・削除可能
3. **「購入手続きへ」ボタンをクリック**
4. **配送先情報を入力**
   - お名前（必須）
   - 郵便番号（必須）
   - 都道府県（必須）
   - 市区町村（必須）
   - 住所（必須）
   - 電話番号（任意）
   - 備考（任意）
5. **注文内容を確認**
6. **「注文を確定する」ボタンで注文完了**
7. **注文番号と合計金額を表示**
8. **カートが自動的にクリア**
9. **商品在庫が自動更新**

### 注文ステータス
- **注文確認中** (Pending) - 注文受付直後
- **注文確定** (Confirmed) - 注文が確定
- **処理中** (Processing) - 商品準備中
- **発送済み** (Shipped) - 商品発送完了
- **配送完了** (Delivered) - 商品配送完了
- **キャンセル** (Cancelled) - 注文キャンセル

### 注文履歴
- **注文履歴一覧**: ユーザープロフィールから「注文履歴」をクリック
- **注文詳細**: 注文をクリックして詳細表示
- **ステータス追跡**: 注文の進行状況をタイムラインで表示
- **配送先確認**: 注文時の配送先情報を確認

## 今後の拡張予定

- 商品検索・フィルタリング
- 商品カテゴリ管理
- 決済機能統合
- 管理者機能
- 商品レビュー・評価機能
- お気に入り機能
- クーポン・割引機能

## ライセンス

このプロジェクトはサンプル用途で作成されています。
