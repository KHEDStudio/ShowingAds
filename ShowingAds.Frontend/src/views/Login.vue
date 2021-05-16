<template>
    <div class="login">
        <div class="flip-card">
            <div class="flip-card-inner">
                <div class="flip-card-front">
                    <div class="signin-form">
                        <form @submit.prevent="submit">
                            <h1>Вход</h1>
                            <input name="username" :disabled="loading" type="text" placeholder="Логин" class="txtb mt-4" v-model="form.username">
                            <input name="password" :disabled="loading" type="password" placeholder="Пароль" class="txtb mt-4" v-model="form.password">
                            <input :disabled="loading" type="submit" value="Войти" class="signin-btn mt-4 mb-2">
                            <div v-if="loading" class="d-flex justify-content-center">
                                <div class="spinner-border text-primary mt-3" role="status">
                                    <span class="sr-only">Загрузка...</span>
                                </div>
                            </div>
                            <h4 v-if="showError" class="text-danger mt-3">Введены неверные данные!</h4>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <div class="shadow"/>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    export default {
        name: 'Login',
        data() {
            return {
                form: {
                    username: '',
                    password: ''
                },
                loading: false,
                showError: false
            }
        },
        methods: {
            ...mapActions(['LogIn']),
            async submit() {
                try {
                    this.loading = true
                    this.showError = false

                    const User = new FormData()
                    User.append('username', this.form.username)
                    User.append('password', this.form.password)
                    
                    await this.LogIn(User)
                    this.$router.push('/')
                } catch (error) {
                    this.showError = true
                } finally {
                    this.loading = false
                }
            }
        }
    }
</script>

<style scoped>
@import '../assets/login.css';
</style>