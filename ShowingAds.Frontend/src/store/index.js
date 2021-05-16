import Vuex from "vuex"
import Vue from "vue"
import createPersistedState from "vuex-persistedstate"
import auth from "./modules/auth"
import manager from './modules/manager'

Vue.use(Vuex)

export default new Vuex.Store({
    modules: {
        auth,
        manager
    },
    plugins: [createPersistedState()]
})