import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import * as bootstrap from 'bootstrap'

// Font Awesome CSS
import '@fortawesome/fontawesome-free/css/all.css'

// Bootstrapをグローバルに設定
window.bootstrap = bootstrap

createApp(App).mount('#app')
