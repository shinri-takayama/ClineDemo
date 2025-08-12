import axios from 'axios'

const API_BASE_URL = 'http://localhost:5000/api'

class AuthService {
  constructor() {
    this.token = localStorage.getItem('token')
    this.user = JSON.parse(localStorage.getItem('user') || 'null')
    
    // Set up axios interceptor to include token in requests
    axios.interceptors.request.use(
      (config) => {
        if (this.token) {
          config.headers.Authorization = `Bearer ${this.token}`
        }
        return config
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    // Set up response interceptor to handle token expiration
    axios.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          this.logout()
        }
        return Promise.reject(error)
      }
    )
  }

  async login(username, password) {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/login`, {
        username,
        password
      })
      
      const { token, user } = response.data
      
      this.token = token
      this.user = user
      
      localStorage.setItem('token', token)
      localStorage.setItem('user', JSON.stringify(user))
      
      return { success: true, user }
    } catch (error) {
      console.error('Login error:', error)
      return { 
        success: false, 
        error: error.response?.data || 'ログインに失敗しました' 
      }
    }
  }

  async register(userData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/register`, userData)
      
      const { token, user } = response.data
      
      this.token = token
      this.user = user
      
      localStorage.setItem('token', token)
      localStorage.setItem('user', JSON.stringify(user))
      
      return { success: true, user }
    } catch (error) {
      console.error('Registration error:', error)
      return { 
        success: false, 
        error: error.response?.data || '登録に失敗しました' 
      }
    }
  }

  async getProfile() {
    try {
      const response = await axios.get(`${API_BASE_URL}/auth/profile`)
      this.user = response.data
      localStorage.setItem('user', JSON.stringify(response.data))
      return { success: true, user: response.data }
    } catch (error) {
      console.error('Get profile error:', error)
      return { 
        success: false, 
        error: error.response?.data || 'プロフィール取得に失敗しました' 
      }
    }
  }

  logout() {
    this.token = null
    this.user = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  isAuthenticated() {
    return !!this.token
  }

  getCurrentUser() {
    return this.user
  }

  getToken() {
    return this.token
  }

  isAdmin() {
    return this.user && this.user.isAdmin === true
  }

  getAuthHeader() {
    if (this.token) {
      return { 'Authorization': `Bearer ${this.token}` }
    }
    return {}
  }
}

export default new AuthService()
