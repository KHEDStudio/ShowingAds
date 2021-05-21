<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <div class="input-group mb-3 mt-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Название</span>
            </div>
            <input :disabled="saving" type="text" class="form-control" placeholder="..." v-model="name" />
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Описание</span>
            </div>
            <input :disabled="saving" type="text" class="form-control" placeholder="..." v-model="description" />
        </div>
        <button :disabled="saving" type="submit" class="btn btn-primary float-right mb-3" @click="save">Сохранить</button>
    </div>
</template>

<script>
    import { v4 as uuidv4 } from 'uuid'
    import { mapActions } from "vuex"

    export default {
        name: 'ClientForm',
        props: {
            client: Object
        },
        data: function() {
            return {
                id: uuidv4(),
                founder: this.$store.getters.StateUser.username,
                name: '',
                description: '',
                account: this.$store.getters.StateUser.id,

                saving: false
            }
        },
        methods: {
            ...mapActions(['postModel']),
            save: async function() {
                try {
                    this.saving = true

                    let client = {
                        id: this.id,
                        founder: this.founder,
                        name: this.name,
                        description: this.description,
                        account: this.account
                    }
                    let that = this
                    let notify = this.notify
                    let params = {
                        model: client,
                        modelName: 'AdvertisingClient',
                        callback: function(response) {
                            if (response.status != 200) {
                                notify('Уведомление!', `Клиента «${client.name}» не удалось сохранить!`, 'danger')
                            } else {
                                that.$emit('close')
                                setTimeout(() => notify('Уведомление!', `Клиент «${client.name}» сохранен!`, 'success'), 0)
                            }
                        }
                    }
                    await this.postModel(params)
                } catch {
                    notify('Уведомление!', `Что-то пошло не так...`, 'danger')
                } finally {
                    this.saving = false
                }
            },
            notify: function(title, text, variant) {
                this.$bvToast.toast(text, {
                    title: title,
                    autoHideDelay: 5000,
                    toaster: 'b-toaster-top-center',
                    variant: variant,
                    appendToast: false
                })
            }
        },
        created: function() {
            if (this.client) {
                this.id = this.client.id
                this.founder = this.client.founder
                this.name = this.client.name
                this.description = this.client.description
                this.account = this.client.account
            }
        }
    }
</script>

<style scoped>
</style>