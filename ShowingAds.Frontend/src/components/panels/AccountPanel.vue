<template>
    <div id="account-panel" class="card">
        <div class="card-body text-center">
            <h2 class="card-title text-uppercase text-muted">текущее время</h2>
            <h1 id="current-time" class="card-text">{{ time }}</h1>
            <div class="card-body text-center mt-2">
                <h2 class="card-title text-uppercase text-muted">учетная запись:</h2>
                <h3 class="card-text">Логин: {{ this.$store.getters.StateUser.username }}</h3>
                <h3 class="card-text">Email: {{ this.$store.getters.StateUser.email }}</h3>
                <button v-if="this.$store.getters.StateUser.role == 'User'" href="../control_panel/workers/" class="btn btn-outline-success ajax-all-workers ml-2 mt-2">Работники</button>
                <button v-if="this.$store.getters.StateUser.role == 'User'" href="../control_panel/contents/" class="btn btn-outline-primary ajax-all-content ml-2 mt-2" @click="showContents">Альтернативный контент</button>
                <button href="../control_panel/clients/" class="btn btn-outline-primary ajax-all-clients ml-2 mt-2" @click="showClients">Клиенты</button>
                <button href="../control_panel/devices/" class="btn btn-outline-primary ajax-all-devices ml-2 mt-2" @click="showDevices">Приставки</button>
                <button class="btn btn-outline-warning ml-2 mt-2">Изменить пароль</button>
                <button class="btn btn-outline-warning ml-2 mt-2">Изменить Email</button>
                <button class="btn btn-outline-danger ml-2 mt-2" @click="logout">Выйти</button>
            </div>
        </div>
    </div>
</template>

<script>
    import ContentsModal from '../modals/ContentsModal.vue'

    export default {
        name: 'account-panel',
        data() {
            return {
                interval: null,
                time: null
            }
        },
        beforeDestroy() {
            clearInterval(this.interval)
        },
        created() {
            this.timeTick()
            this.interval = setInterval(this.timeTick, 1000)
        },
        methods: {
            timeTick: function() {
                this.time = Intl.DateTimeFormat(navigator.language, {
                    hour: 'numeric',
                    minute: 'numeric',
                    second: 'numeric'
                }).format()
            },
            showContents: function() {
                this.$modal.show(
                    ContentsModal, {},
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            showClients: function() {
            },
            showDevices: function() {
            },
            logout: async function() {
                await this.$store.dispatch('LogOut')
                this.$router.push('/login')
            }
        }
    }
</script>

<style scoped>
</style>