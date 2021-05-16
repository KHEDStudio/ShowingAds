import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import axios from 'axios'
import VModal from 'vue-js-modal'
import Notifications from 'vue-notification'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

axios.interceptors.response.use(undefined, (error) => {
    if (error) {
        const originalRequest = error.config
        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true
            store.dispatch('LogOut')
            return router.push('/login')
        }
    }
})

Vue.use(VModal, { dialog: true })
Vue.use(Notifications)

Vue.use(BootstrapVue)
Vue.use(IconsPlugin)

Vue.config.productionTip = true

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app')
