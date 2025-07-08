import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import * as bootstrap from 'bootstrap'

// Bootstrapをグローバルに設定
window.bootstrap = bootstrap

createApp(App).mount('#app')
