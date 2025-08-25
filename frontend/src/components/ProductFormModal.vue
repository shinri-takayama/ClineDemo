<template>
  <div class="modal fade" id="productFormModal" tabindex="-1" aria-labelledby="productFormModalLabel" aria-hidden="true" ref="modal">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="productFormModalLabel">{{ product ? '商品編集' : '商品追加' }}</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="mb-3">
              <label for="productName" class="form-label">商品名</label>
              <input type="text" class="form-control" id="productName" v-model="form.name" required>
            </div>
            <div class="mb-3">
              <label for="productDescription" class="form-label">説明</label>
              <textarea class="form-control" id="productDescription" rows="3" v-model="form.description" required></textarea>
            </div>
            <div class="row">
              <div class="col-md-6 mb-3">
                <label for="productPrice" class="form-label">価格</label>
                <input type="number" class="form-control" id="productPrice" v-model.number="form.price" required min="0">
              </div>
              <div class="col-md-6 mb-3">
                <label for="productStock" class="form-label">在庫</label>
                <input type="number" class="form-control" id="productStock" v-model.number="form.stock" required min="0">
              </div>
            </div>
            <div class="mb-3">
              <label for="productImageUrl" class="form-label">画像URL</label>
              <input type="url" class="form-control" id="productImageUrl" v-model="form.imageUrl">
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">キャンセル</button>
              <button type="submit" class="btn btn-primary">{{ product ? '更新' : '登録' }}</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Modal } from 'bootstrap'

export default {
  name: 'ProductFormModal',
  props: {
    product: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      form: {
        name: '',
        description: '',
        price: 0,
        stock: 0,
        imageUrl: ''
      },
      modalInstance: null,
      _editingId: undefined
    }
  },
  watch: {
    product: {
      handler(newVal) {
        if (newVal) {
          // 必要項目のみコピー（id は送らない）
          this.form = {
            name: newVal.name ?? '',
            description: newVal.description ?? '',
            price: newVal.price ?? 0,
            stock: newVal.stock ?? 0,
            imageUrl: newVal.imageUrl ?? ''
          }
          this._editingId = newVal.id
        } else {
          this.resetForm()
        }
      },
      immediate: true
    }
  },
  mounted() {
    this.modalInstance = new Modal(this.$refs.modal)
  },
  methods: {
    handleSubmit() { this.$emit('save', { ...this.form }, this._editingId) },
    resetForm() {
      this.form = {
        name: '',
        description: '',
        price: 0,
        stock: 0,
        imageUrl: ''
      }
      this._editingId = undefined
    },
    show() {
      this.modalInstance?.show()
    },
    hide() {
      this.modalInstance?.hide()
    }
  }
}
</script>
