<template>
    <div class="pt-2 pb-2 pr-4 pl-4 text-center">
        <h4>«{{ client.name }}»</h4>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Интервал показа</span>
            </div>
            <input type="time" class="form-control" @change="changed">
        </div>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    export default {
        name: 'ClientChannelForm',
        props: {
            clientChannel: Object
        },
        computed: {
            client: function() {
                return this.$store.state.manager.clients ? this.$store.state.manager.clients.find(x => x.id == this.clientChannel.ads_client) : {}
            }
        },
        methods: {
            ...mapActions(['postModel']),
            changed: async function(event) {
                let notify = this.notify
                let client = this.client
                let clientChannel = this.clientChannel
                let oldValue = clientChannel.interval
                clientChannel.interval = event.target.valueAsNumber * 10000
                let params = {
                    model: clientChannel,
                    modelName: 'ClientChannel',
                    callback: function(response) {
                        if (response.status != 200) {
                            clientChannel.interval = oldValue
                            notify('Уведомление!', `Интервал показа клиента «${client.name}» не удалось изменить!`, 'danger')
                        } else {
                            notify('Уведомление!', `Интервал показа клиента «${client.name}» изменен!`, 'success')
                        }
                    }
                }
                await this.postModel(params)
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
        }
    }
</script>

<style scoped>
</style>