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
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text">Логотип слева</span>
            </div>
            <input :disabled="saving" type="file" accept="image/*" ref="left" class="form-control" @change="logoLeft" />
        </div>
        <div class="align-items-center mb-3">
            <div v-if="logo_left != '00000000-0000-0000-0000-000000000000'">
                <button :disabled="saving" type="button" class="close float-right" @click="logoLeft">
                    <b-icon-x variant="danger" />
                </button>
                <p class="text-muted float-right">Выбрано: {{ logo_left }}</p>
            </div>
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Логотип справа</span>
            </div>
            <input :disabled="saving" type="file" accept="image/*" ref="right" class="form-control" @change="logoRight" />
        </div>
        <div class="align-items-center mb-3">
            <div v-if="logo_right != '00000000-0000-0000-0000-000000000000'">
                <button :disabled="saving" type="button" class="close float-right" @click="logoRight">
                    <b-icon-x variant="danger" />
                </button>
                <p class="text-muted float-right">Выбрано: {{ logo_right }}</p>
            </div>
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Бегущая строка</span>
            </div>
            <input :disabled="saving" type="text" class="form-control" placeholder="..." v-model="ticker" />
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Интервал бегущей строки</span>
            </div>
            <input :disabled="saving" type="time" class="form-control" ref="ticker" @change="tickerIntervalChanged" />
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Время перезагрузки</span>
            </div>
            <input :disabled="saving" type="time" class="form-control" ref="reload" @change="reloadTimeChanged" />
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Ориентация экрана</span>
            </div>
            <b-form-select :disabled="saving" v-model="orientation" :options="orientations" />
        </div>
        <button :disabled="saving" type="submit" class="btn btn-primary float-right mb-3" @click="save">Сохранить</button>
    </div>
</template>

<script>
    import { v4 as uuidv4 } from 'uuid'
    import { mapActions } from "vuex"

    export default {
        name: 'ChannelForm',
        props: {
            channel: Object
        },
        data: function() {
            return {
                id: uuidv4(),
                founder: this.$store.getters.StateUser.username,
                name: '',
                description: '',
                logo_left: '00000000-0000-0000-0000-000000000000',
                logo_right: '00000000-0000-0000-0000-000000000000',
                ticker: '',
                ticker_interval: 0,
                reload_time: 0,
                orientation: 0,
                account: this.$store.getters.StateUser.id,
                workers: [],
                contents: [],

                saving: false,
                orientations: [
                    {
                        value: 0,
                        text: 'Горизонтальная'
                    },
                    {
                        value: 1,
                        text: 'Вертикальная'
                    }
                ]
            }
        },
        methods: {
            ...mapActions(['postModel', 'uploadLogo']),
            logoLeft: function() {
                this.logo_left = '00000000-0000-0000-0000-000000000000'
            },
            logoRight: function() {
                this.logo_right = '00000000-0000-0000-0000-000000000000'
            },
            tickerIntervalChanged: function(event) {
                this.ticker_interval = event.target.valueAsNumber * 10000
            },
            reloadTimeChanged: function(event) {
                this.reload_time = event.target.valueAsNumber * 10000
            },
            save: async function() {
                try {
                    this.saving = true
                    if (this.$refs.left.files.length)
                        await this.uploadLeftLogo()
                    if (this.$refs.right.files.length)
                        await this.uploadRightLogo()

                    let channel = {
                        id: this.id,
                        founder: this.founder,
                        name: this.name,
                        description: this.description,
                        logo_left: this.logo_left,
                        logo_right: this.logo_right,
                        ticker: this.ticker,
                        ticker_interval: this.ticker_interval,
                        reload_time: this.reload_time,
                        orientation: this.orientation,
                        account: this.account,
                        workers: this.workers,
                        contents: this.contents
                    }
                    let that = this
                    let notify = this.notify
                    let params = {
                        model: channel,
                        modelName: 'Channel',
                        callback: function(response) {
                            if (response.status != 200) {
                                notify('Уведомление!', `Канал «${channel.name}» не удалось сохранить!`, 'danger')
                            } else {
                                that.$emit('close')
                                setTimeout(() => notify('Уведомление!', `Канал «${channel.name}» сохранен!`, 'success'), 0)
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
            uploadLeftLogo: async function() {
                let that = this
                let params = {
                    logo: this.$refs.left.files[0],
                    callback: function(response) {
                        if (response.status == 201)
                            that.logo_left = response.data.file
                    }
                }
                await this.uploadLogo(params)
            },
            uploadRightLogo: async function() {
                let that = this
                let params = {
                    logo: this.$refs.right.files[0],
                    callback: function(response) {
                        if (response.status == 201)
                            that.logo_right = response.data.file
                    }
                }
                await this.uploadLogo(params)
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
            if (this.channel) {
                this.id = this.channel.id
                this.founder = this.channel.founder
                this.name = this.channel.name
                this.description = this.channel.description
                this.logo_left = this.channel.logo_left
                this.logo_right = this.channel.logo_right
                this.ticker = this.channel.ticker
                this.ticker_interval = this.channel.ticker_interval
                this.reload_time = this.channel.reload_time
                this.orientation = this.channel.orientation
                this.account = this.channel.account
                this.workers = this.channel.workers
                this.contents = this.channel.contents
            }
        },
        mounted: function() {
            this.$refs.ticker.valueAsNumber = this.ticker_interval / 10000
            this.$refs.reload.valueAsNumber = this.reload_time / 10000
        }
    }
</script>

<style scoped>
</style>