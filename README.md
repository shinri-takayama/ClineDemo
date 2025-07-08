# ECショップ - C# + Vue.js ECサイト

C# ASP.NET Core Web APIとVue.jsを使用したECサイトのサンプルアプリケーションです。

## 機能

- 商品一覧表示（カード形式）
- ショッピングカート機能
- ローカルストレージでのカート管理
- レスポンシブデザイン

## 技術スタック

### バックエンド
- C# ASP.NET Core 8.0
- Entity Framework Core
- SQLite データベース
- Swagger UI

### フロントエンド
- Vue.js 3
- Bootstrap 5
- Axios
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

- `GET /api/products` - 全商品取得
- `GET /api/products/{id}` - 特定商品取得
- `POST /api/products` - 商品作成
- `PUT /api/products/{id}` - 商品更新
- `DELETE /api/products/{id}` - 商品削除

## 主要コンポーネント

### ProductCard.vue
- 商品情報をカード形式で表示
- カートへの追加機能
- ホバーエフェクト

### ProductList.vue
- 商品一覧の表示
- カート表示モーダル
- ローディング・エラー処理

## カート機能

- ローカルストレージを使用してカート情報を永続化
- 商品の追加・削除・数量変更
- 合計金額の計算

## 今後の拡張予定

- ユーザー認証機能
- 注文処理機能
- 商品検索・フィルタリング
- 商品カテゴリ管理
- 決済機能統合

## ライセンス

このプロジェクトはサンプル用途で作成されています。
