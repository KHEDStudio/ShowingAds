<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <ClientChannelCard v-for="clientChannel in clientChannels" :key="clientChannel.id" :clientChannel="clientChannel" :clickCallback="addOrder" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import { v4 as uuidv4 } from 'uuid'
    import ClientChannelCard from '../cards/ClientChannelCard.vue'

    export default {
        name: 'AddOrderModal',
        props: {
            channel: Object
        },
        components: {
            ClientChannelCard
        },
        computed: {
            clientChannels: function() {
                return this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels.filter(x => x.channel == this.channel.id) : []
            }
        },
        methods: {
            ...mapActions(['postModel']),
            client: function(order) {
                let clientChannel = this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels.find(x => x.id == order.ads_client_channel) : null
                if (clientChannel == null)
                    return null
                return this.$store.state.manager.clients ? this.$store.state.manager.clients.find(x => x.id == clientChannel.ads_client) : null
            },
            addOrder: async function(clientChannel) {
                let order = {
                    id: uuidv4(),
                    order_field: new Date().toISOString(),
                    channel: clientChannel.channel,
                    ads_client_channel: clientChannel.id
                }
                let notify = this.notify
                let channel = this.channel
                let client = this.client
                let removeOrder = this.removeOrder
                this.$store.state.manager.orders.push(order)
                let params = {
                    model: order,
                    modelName: 'Order',
                    callback: function(response) {
                        if (response.status != 200) {
                            removeOrder(order)
                            notify('Уведомление!', `Клиента «${client(order).name}» не удалось добавить в очередь канала «${channel.name}»!`, 'danger')
                        } else {
                            notify('Уведомление!', `Клиент «${client(order).name}» добавлен в очередь канала «${channel.name}»!`, 'success')
                        }
                    }
                }
                await this.postModel(params)
            },
            removeOrder: function(order) {
                this.$store.state.manager.orders = this.$store.state.manager.orders.filter(x => x.id != order.id)
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