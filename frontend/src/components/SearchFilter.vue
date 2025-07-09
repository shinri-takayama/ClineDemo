<template>
  <div class="search-filter-container">
    <div class="card shadow-sm">
      <div class="card-body">
        <div class="row g-3">
          <!-- Search Input -->
          <div class="col-md-4">
            <label class="form-label small text-muted">商品検索</label>
            <div class="input-group">
              <span class="input-group-text">
                <i class="fas fa-search"></i>
              </span>
              <input
                type="text"
                class="form-control form-control-sm"
                placeholder="商品を検索..."
                v-model="searchQuery"
                @input="onSearchInput"
              >
              <button 
                v-if="searchQuery" 
                class="btn btn-outline-secondary btn-sm" 
                type="button"
                @click="clearSearch"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <!-- Price Range Filter -->
          <div class="col-md-3">
            <label class="form-label small text-muted">価格範囲</label>
            <div class="d-flex align-items-center">
              <input
                type="number"
                class="form-control form-control-sm me-2"
                placeholder="最小"
                v-model.number="minPrice"
                @input="onFilterChange"
                min="0"
              >
              <span class="text-muted">〜</span>
              <input
                type="number"
                class="form-control form-control-sm ms-2"
                placeholder="最大"
                v-model.number="maxPrice"
                @input="onFilterChange"
                min="0"
              >
            </div>
          </div>

          <!-- Sort Options -->
          <div class="col-md-3">
            <label class="form-label small text-muted">並び替え</label>
            <select 
              class="form-select form-select-sm"
              v-model="sortOption"
              @change="onFilterChange"
            >
              <option value="">デフォルト</option>
              <option value="price-asc">価格: 安い順</option>
              <option value="price-desc">価格: 高い順</option>
              <option value="name-asc">名前: あいうえお順</option>
              <option value="name-desc">名前: 逆順</option>
              <option value="date-desc">新着順</option>
              <option value="date-asc">古い順</option>
            </select>
          </div>

          <!-- Filter Actions -->
          <div class="col-md-2">
            <label class="form-label small text-muted">&nbsp;</label>
            <div class="d-flex gap-2">
              <button 
                class="btn btn-outline-primary btn-sm flex-fill"
                @click="applyFilters"
                :disabled="isLoading"
              >
                <i class="fas fa-filter me-1"></i>
                適用
              </button>
              <button 
                class="btn btn-outline-secondary btn-sm"
                @click="resetFilters"
                :disabled="isLoading"
              >
                <i class="fas fa-undo me-1"></i>
                リセット
              </button>
            </div>
          </div>
        </div>

        <!-- Active Filters Display -->
        <div v-if="hasActiveFilters" class="mt-3">
          <div class="d-flex flex-wrap gap-2 align-items-center">
            <small class="text-muted">適用中のフィルタ:</small>
            
            <span v-if="searchQuery" class="badge bg-primary">
              検索: "{{ searchQuery }}"
              <button class="btn-close btn-close-white ms-1" @click="clearSearch" style="font-size: 0.6em;"></button>
            </span>
            
            <span v-if="minPrice !== null || maxPrice !== null" class="badge bg-info">
              価格: {{ formatPriceRange }}
              <button class="btn-close btn-close-white ms-1" @click="clearPriceFilter" style="font-size: 0.6em;"></button>
            </span>
            
            <span v-if="sortOption" class="badge bg-success">
              並び替え: {{ getSortLabel(sortOption) }}
              <button class="btn-close btn-close-white ms-1" @click="clearSort" style="font-size: 0.6em;"></button>
            </span>
          </div>
        </div>

        <!-- Results Count -->
        <div v-if="resultsCount !== null" class="mt-2">
          <small class="text-muted">
            <i class="fas fa-info-circle me-1"></i>
            {{ resultsCount }}件の商品が見つかりました
          </small>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'SearchFilter',
  props: {
    isLoading: {
      type: Boolean,
      default: false
    },
    resultsCount: {
      type: Number,
      default: null
    }
  },
  data() {
    return {
      searchQuery: '',
      minPrice: null,
      maxPrice: null,
      sortOption: '',
      searchTimeout: null
    }
  },
  computed: {
    hasActiveFilters() {
      return this.searchQuery || 
             this.minPrice !== null || 
             this.maxPrice !== null || 
             this.sortOption
    },
    formatPriceRange() {
      if (this.minPrice !== null && this.maxPrice !== null) {
        return `¥${this.formatPrice(this.minPrice)} 〜 ¥${this.formatPrice(this.maxPrice)}`
      } else if (this.minPrice !== null) {
        return `¥${this.formatPrice(this.minPrice)} 以上`
      } else if (this.maxPrice !== null) {
        return `¥${this.formatPrice(this.maxPrice)} 以下`
      }
      return ''
    }
  },
  methods: {
    onSearchInput() {
      // Debounce search input
      if (this.searchTimeout) {
        clearTimeout(this.searchTimeout)
      }
      
      this.searchTimeout = setTimeout(() => {
        this.applyFilters()
      }, 500)
    },

    onFilterChange() {
      this.applyFilters()
    },

    applyFilters() {
      const filters = {
        search: this.searchQuery || null,
        minPrice: this.minPrice,
        maxPrice: this.maxPrice,
        sortBy: null,
        sortOrder: null
      }

      // Parse sort option
      if (this.sortOption) {
        const [sortBy, sortOrder] = this.sortOption.split('-')
        filters.sortBy = sortBy
        filters.sortOrder = sortOrder
      }

      this.$emit('filters-changed', filters)
      this.saveFiltersToStorage()
    },

    resetFilters() {
      this.searchQuery = ''
      this.minPrice = null
      this.maxPrice = null
      this.sortOption = ''
      
      this.$emit('filters-changed', {
        search: null,
        minPrice: null,
        maxPrice: null,
        sortBy: null,
        sortOrder: null
      })
      
      this.clearFiltersFromStorage()
    },

    clearSearch() {
      this.searchQuery = ''
      this.applyFilters()
    },

    clearPriceFilter() {
      this.minPrice = null
      this.maxPrice = null
      this.applyFilters()
    },

    clearSort() {
      this.sortOption = ''
      this.applyFilters()
    },

    getSortLabel(sortOption) {
      const labels = {
        'price-asc': '価格: 安い順',
        'price-desc': '価格: 高い順',
        'name-asc': '名前: あいうえお順',
        'name-desc': '名前: 逆順',
        'date-desc': '新着順',
        'date-asc': '古い順'
      }
      return labels[sortOption] || sortOption
    },

    formatPrice(price) {
      return new Intl.NumberFormat('ja-JP').format(price)
    },

    saveFiltersToStorage() {
      const filters = {
        searchQuery: this.searchQuery,
        minPrice: this.minPrice,
        maxPrice: this.maxPrice,
        sortOption: this.sortOption
      }
      localStorage.setItem('productFilters', JSON.stringify(filters))
    },

    loadFiltersFromStorage() {
      try {
        const saved = localStorage.getItem('productFilters')
        if (saved) {
          const filters = JSON.parse(saved)
          this.searchQuery = filters.searchQuery || ''
          this.minPrice = filters.minPrice
          this.maxPrice = filters.maxPrice
          this.sortOption = filters.sortOption || ''
        }
      } catch (error) {
        console.error('Failed to load filters from storage:', error)
      }
    },

    clearFiltersFromStorage() {
      localStorage.removeItem('productFilters')
    }
  },

  mounted() {
    this.loadFiltersFromStorage()
    if (this.hasActiveFilters) {
      this.applyFilters()
    }
  }
}
</script>

<style scoped>
.search-filter-container {
  margin-bottom: 1.5rem;
}

.card {
  border: 1px solid #e3e6f0;
  border-radius: 0.5rem;
}

.input-group-text {
  background-color: #f8f9fc;
  border-color: #d1d3e2;
  color: #6c757d;
}

.form-control:focus,
.form-select:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.badge {
  font-size: 0.75rem;
  padding: 0.375rem 0.75rem;
}

.badge .btn-close {
  padding: 0;
  margin: 0;
  opacity: 0.8;
}

.badge .btn-close:hover {
  opacity: 1;
}

.btn-sm {
  font-size: 0.875rem;
}

@media (max-width: 768px) {
  .col-md-4,
  .col-md-3,
  .col-md-2 {
    margin-bottom: 0.5rem;
  }
  
  .d-flex.gap-2 {
    flex-direction: column;
  }
  
  .d-flex.gap-2 .btn {
    margin-bottom: 0.25rem;
  }
}
</style>
