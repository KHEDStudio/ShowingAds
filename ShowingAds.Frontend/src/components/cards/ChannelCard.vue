<template>
    <div class="card mb-1">
        <div class="align-items-center pt-2 pr-2 pl-2 mb-1">
            <button v-if="deleteCallback" type="button" class="close float-right">
                <b-icon-x variant="danger" @click="deleteCallback(channel)" />
            </button>
            <button v-if="editCallback" type="button" class="close mr-3 float-right">
                <b-icon-sliders @click="editCallback(channel)" />
            </button>
            <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text float-left btn" type="button" v-b-toggle="`channel-${channel.id}`">
                <h5 class="m-0">{{ channel.name }}</h5>
            </button>
        </div>
        <div v-if="channel.description != ''" class="card-text ml-2 mb-1">
            <p class="m-0">Описание: {{ channel.description }}</p>
        </div>  
        <b-collapse :id="`channel-${channel.id}`" accordion="channel">
            <p class="card-text ml-2 mb-0">Создал: {{ channel.founder }}</p>
            <div class="row mx-auto">
                <div v-if="channel.logo_left == '00000000-0000-0000-0000-000000000000' || channel.logo_left == undefined" class="col-sm-6 p-1"><img class="rounded image-link" style="max-width: 100%" href="/images/background.jpg" src="/images/background.jpg" /></div>
                <div v-else class="col-sm-6 p-1"><img class="rounded image-link" style="max-width: 100%" :href="`http://31.184.219.123:3666/logo?logo=${channel.logo_left}`" :src="`http://31.184.219.123:3666/logo?logo=${channel.logo_left}`" /></div>
                <div v-if="channel.logo_right == '00000000-0000-0000-0000-000000000000' || channel.logo_right == undefined" class="col-sm-6 p-1"><img class="rounded image-link" style="max-width: 100%" href="/images/background.jpg" src="/images/background.jpg" /></div>
                <div v-else class="col-sm-6 p-1"><img class="rounded image-link" style="max-width: 100%" :href="`http://31.184.219.123:3666/logo?logo=${channel.logo_right}`" :src="`http://31.184.219.123:3666/logo?logo=${channel.logo_right}`" /></div>
            </div>
            <p class="card-text ml-2 mb-0">Ориентация экранов: {{ channel.orientation ? 'вертикальная' : 'горизонтальная' }}</p>
            <p v-if="channel.ticker != ''" class="card-text ml-2 mb-0">Бегущая строка: {{ channel.ticker }}</p>
            <p v-if="channel.ticker != ''" class="card-text ml-2 mb-0">Интервал бегущей строки: {{ tickerInterval }} мин</p>
            <p class="card-text ml-2 mb-1">Время перезагрузки приставки (каждый день): {{ reloadTime }}</p>
            <div class="row mx-auto">
                <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text col btn m-1" type="button" v-b-toggle="`contents-${channel.id}`">
                    <h5 class="m-0">Контент</h5>
                </button>
                <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text col btn m-1" type="button" v-b-toggle="`clients-${channel.id}`">
                    <h5 class="m-0">Клиенты</h5>
                </button>
                <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text col btn m-1" type="button" v-b-toggle="`orders-${channel.id}`">
                    <h5 class="m-0">Очередность</h5>
                </button>
                <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text col btn m-1" type="button" v-b-toggle="`devices-${channel.id}`">
                    <h5 class="m-0">Приставки</h5>
                </button>
            </div>
            <div class="accordion mx-auto">
                <b-collapse :id="`contents-${channel.id}`" class="border rounded m-1 p-1" :accordion="`accordion-${channel.id}`">
                    <b-icon-plus font-scale="5" class="card plus mb-1" @click="showAddContentModal" />
                    <ContentCard v-for="content in contents" :key="channel.id + content.id" :content="content" :clickCallback="contentClicked" :deleteCallback="deleteContent" />
                </b-collapse>
                <b-collapse :id="`clients-${channel.id}`" class="border rounded m-1 p-1" :accordion="`accordion-${channel.id}`">
                    <b-icon-plus font-scale="5" class="card plus mb-1" @click="showAddClientModal" />
                    <ClientChannelCard v-for="clientChannel in clientChannels" :key="channel.id + clientChannel.id" :clientChannel="clientChannel" :clickCallback="showClientVideosModal" :editCallback="editClientChannel" :deleteCallback="deleteClientChannel" />
                </b-collapse>
                <b-collapse :id="`orders-${channel.id}`" class="border rounded m-1 p-1" :accordion="`accordion-${channel.id}`">
                    <b-icon-plus font-scale="5" class="card plus mb-1" @click="showAddOrderModal" />
                    <OrderCard v-for="order in orders" :key="channel.id + order.id" :order="order" :deleteCallback="deleteOrder" />
                </b-collapse>
                <b-collapse :id="`devices-${channel.id}`" class="border rounded m-1 p-1" :accordion="`accordion-${channel.id}`">
                    <b-icon-plus font-scale="5" class="card plus mb-1" />
                    <!--<DeviceCard @key="Channel.Id.ToString() + device.Id" Device="device" OnClose="RemoveDevice" />-->
                </b-collapse>
            </div>
        </b-collapse>
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import ContentCard from '../cards/ContentCard.vue'
    import ClientChannelCard from '../cards/ClientChannelCard.vue'
    import OrderCard from '../cards/OrderCard.vue'

    import ClientChannelForm from '../forms/ClientChannelForm.vue'

    import AddContentModal from '../modals/AddContentModal.vue'
    import AddClientModal from '../modals/AddClientModal.vue'
    import AddOrderModal from '../modals/AddOrderModal.vue'
    
    import ClientVideosModal from '../modals/ClientVideosModal.vue'

    export default {
        name: 'Channel',
        components: {
            ContentCard,
            ClientChannelCard,
            OrderCard,
        },
        props: {
            channel: Object,
            editCallback: Function,
            deleteCallback: Function
        },
        computed: {
            user: function() {
                return this.$store.state.manager.users ? this.$store.state.manager.users.find(x => x.id == this.channel.account) : null
            },
            red: function() {
                let user = this.user
                return user ? user.red : 255
            },
            green: function() {
                let user = this.user
                return user ? user.green : 255
            },
            blue: function() {
                let user = this.user
                return user ? user.blue : 255
            },
            tickerInterval: function() {
                return this.msToMinutes(this.channel.ticker_interval / 10000)
            },
            reloadTime: function() {
                return this.msToTime(this.channel.reload_time / 10000)
            },
            contents: function() {
                return this.$store.state.manager.contents ? this.$store.state.manager.contents.filter(x => this.channel.contents.includes(x.id)) : []
            },
            clientChannels: function() {
                return this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels.filter(x => x.channel == this.channel.id) : []
            },
            orders: function() {
                let orders = this.$store.state.manager.orders ? this.$store.state.manager.orders.filter(x => x.channel == this.channel.id) : []
                return orders.sort((a, b) => (a.order_field > b.order_field) ? 1 : ((b.order_field > a.order_field) ? -1 : 0))
            }
        },
        methods: {
            ...mapActions(['postModel', 'deleteModel']),
            msToMinutes: function(milliseconds) {
                let ms = milliseconds % 1000
                milliseconds = (milliseconds - ms) / 1000
                let secs = milliseconds % 60
                milliseconds = (milliseconds - secs) / 60
                return milliseconds
            },
            msToTime: function(milliseconds) {
                let ms = milliseconds % 1000
                milliseconds = (milliseconds - ms) / 1000
                let secs = milliseconds % 60
                milliseconds = (milliseconds - secs) / 60
                let mins = milliseconds % 60
                let hrs = (milliseconds - mins) / 60
                return this.timeFormat(hrs) + ':' + this.timeFormat(mins)
            },
            timeFormat: function(number) {
                return (number < 10 ? `0${number}` : number)
            },
            showAddContentModal: function() {
                this.$modal.show(
                    AddContentModal,
                    {
                        channel: this.channel
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            showAddClientModal: function() {
                this.$modal.show(
                    AddClientModal,
                    {
                        channel: this.channel
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            showAddOrderModal: function() {
                this.$modal.show(
                    AddOrderModal,
                    {
                        channel: this.channel
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            contentClicked: function(content) {
                console.log(content)
            },
            showClientVideosModal: function(clientChannel) {
                this.$modal.show(
                    ClientVideosModal,
                    {
                        clientChannel: clientChannel
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            editClientChannel: function(clientChannel) {
                this.$modal.show(
                    ClientChannelForm,
                    {
                        clientChannel: clientChannel
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            deleteContent: async function(content) {
                let channel = this.channel
                let notify = this.notify
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Исключить альтернативный контент «${content.name}» из канала «${channel.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                if (channel.contents.includes(content.id)) {
                                    channel.contents = channel.contents.filter(x => x != content.id)
                                    let params = {
                                        model: channel,
                                        modelName: 'Channel',
                                        callback: function(response) {
                                            if (response.status != 200) {
                                                channel.contents.push(content.id)
                                                notify('Уведомление!', `Альтернативный контент «${content.name}» не удалось исключить из канала «${channel.name}»!`, 'danger')
                                            } else {
                                                notify('Уведомление!', `Альтернативный контент «${content.name}» исключен из канала «${channel.name}»!`, 'success')
                                            }
                                        }
                                    }
                                    await this.postModel(params)
                                }
                            }
                        },
                        {
                            title: 'Нет',
                            handler: () => {
                                this.$modal.hide('dialog')
                            }
                        }
                    ]
                })
            },
            returnClientChannel: function(clientChannel) {
                this.$store.state.manager.clientChannels.push(clientChannel)
            },
            removeClientChannel: function(clientChannel) {
                this.$store.state.manager.clientChannels = this.$store.state.manager.clientChannels.filter(x => x.id != clientChannel.id)
            },
            deleteClientChannel: async function(clientChannel) {
                let channel = this.channel
                let notify = this.notify
                let client = this.$store.state.manager.clients.find(x => x.id == clientChannel.ads_client)
                let removeClientChannel = this.removeClientChannel
                let returnClientChannel = this.returnClientChannel
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Исключить клиента «${client.name}» из канала «${channel.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                removeClientChannel(clientChannel)
                                let params = {
                                    model: clientChannel,
                                    modelName: 'ClientChannel',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            returnClientChannel(clientChannel)
                                            notify('Уведомление!', `Клиента «${client.name}» не удалось исключить из канала «${channel.name}»!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Клиент «${client.name}» исключен из канала «${channel.name}»!`, 'success')
                                        }
                                    }
                                }
                                await this.deleteModel(params)
                            }
                        },
                        {
                            title: 'Нет',
                            handler: () => {
                                this.$modal.hide('dialog')
                            }
                        }
                    ]
                })
            },
            returnOrder: function(order) {
                this.$store.state.manager.orders.push(order)
            },
            removeOrder: function(order) {
                this.$store.state.manager.orders = this.$store.state.manager.orders.filter(x => x.id != order.id)
            },
            deleteOrder: async function(order) {
                let channel = this.channel
                let notify = this.notify
                let clientChannel = this.$store.state.manager.clientChannels.find(x => x.id == order.ads_client_channel)
                let client = this.$store.state.manager.clients.find(x => x.id == clientChannel.ads_client)
                let removeOrder = this.removeOrder
                let returnOrder = this.returnOrder
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Исключить запись клиента «${client.name}» из очереди канала «${channel.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                removeOrder(order)
                                let params = {
                                    model: order,
                                    modelName: 'Order',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            returnOrder(order)
                                            notify('Уведомление!', `Запись клиента «${client.name}» не удалось исключить из очереди канала «${channel.name}»!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Запись клиента «${client.name}» исключена из очереди канала «${channel.name}»!`, 'success')
                                        }
                                    }
                                }
                                await this.deleteModel(params)
                            }
                        },
                        {
                            title: 'Нет',
                            handler: () => {
                                this.$modal.hide('dialog')
                            }
                        }
                    ]
                })
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