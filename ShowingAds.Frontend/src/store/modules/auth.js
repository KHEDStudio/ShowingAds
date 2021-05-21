import axios from 'axios'
import jwt_decode from 'jwt-decode'

const loginURL = 'http://84.38.188.128:3670/'

const state = {
    token: null,
    user: null
}

const getters = {
    isAuthenticated: state => !!state.token,
    StateToken: state => state.token,
    StateUser: state => state.user
}

const actions = {
    async LogIn({ commit }, user) {
        await axios.post('login/', JSON.stringify(Object.fromEntries(user)), {
                baseURL: loginURL,
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(async response => {
                let token = response.data.access_token
                await commit('setToken', token)
                let decoded = jwt_decode(token)
                let user = {
                    id: decoded.Id,
                    guidId: decoded.GuidId,
                    username: decoded.Name,
                    email: decoded.MailAddress,
                    role: decoded.Role
                }
                await commit('setUser', user)
            })
    },
    async LogOut({ commit }) {
        await commit('logout')
    }
}

const mutations = {
    setToken(state, token) {
        state.token = token
    },
    setUser(state, user) {
        state.user = user
    },
    logout(state) {
        state.token = null
        state.user = null
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}