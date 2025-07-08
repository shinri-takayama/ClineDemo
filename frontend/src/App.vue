<template>
  <div id="app">
    <!-- ナビゲーションバー -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
      <div class="container-fluid">
        <a class="navbar-brand fw-bold" href="#">
          <i class="bi bi-shop me-2"></i>
          ECショップ
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-item">
              <a class="nav-link active" href="#">
                <i class="bi bi-house me-1"></i>
                ホーム
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">
                <i class="bi bi-grid me-1"></i>
                カテゴリ
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">
                <i class="bi bi-info-circle me-1"></i>
                お問い合わせ
              </a>
            </li>
          </ul>
          <ul class="navbar-nav">
            <li class="nav-item">
              <a class="nav-link" href="#">
                <i class="bi bi-person me-1"></i>
                ログイン
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- メインコンテンツ -->
    <main>
      <ProductList />
    </main>

    <!-- フッター -->
    <footer class="bg-dark text-light py-4 mt-5">
      <div class="container">
        <div class="row">
          <div class="col-md-6">
            <h5>ECショップ</h5>
            <p class="mb-0">お客様に最高のショッピング体験をお届けします。</p>
          </div>
          <div class="col-md-6 text-md-end">
            <p class="mb-0">&copy; 2024 ECショップ. All rights reserved.</p>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
import ProductList from './components/ProductList.vue'

export default {
  name: 'App',
  components: {
    ProductList
  }
}
</script>

<style>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
}

body {
  background-color: #f8f9fa;
}

/* Bootstrap Icons CDN */
@import url("https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css");
</style>
