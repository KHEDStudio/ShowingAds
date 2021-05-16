<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <ClientCard v-for="client in clients" :key="client.id" :client="client" :clickCallback="addClient" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import { v4 as uuidv4 } from 'uuid'
    import ClientCard from '../cards/ClientCard.vue'

    export default {
        name: 'AddClientModal',
        props: {
            channel: Object
        },
        components: {
            ClientCard
        },
        computed: {
            clientChannels: function() {
                return this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels : []
            },
            clients: function() {
                return this.$store.state.manager.clients ? this.$store.state.manager.clients.filter(x => this.isClientAdded(x) == undefined) : []
            }
        },
        methods: {
            ...mapActions(['postModel']),
            isClientAdded: function(client) {
                return this.clientChannels.find(x => x.ads_client == client.id && x.channel == this.channel.id)
            },
            adsVideos: function(client) {
                let videoIds = []
                let videos = this.$store.state.manager.clientVideos ? this.$store.state.manager.clientVideos.filter(x => x.ads_client == client.id) : []
                videos.forEach(x => videoIds.push(x.id))
                return videoIds
            },
            addClient: async function(client) {
                if (this.isClientAdded(client) == undefined) {
                    let clientChannel = {
                        id: uuidv4(),
                        interval: 0,
                        channel: this.channel.id,
                        ads_client: client.id,
                        ads_videos: this.adsVideos(client)
                    }
                    let notify = this.notify
                    let channel = this.channel
                    let removeClientChannel = this.removeClientChannel
                    this.$store.state.manager.clientChannels.push(clientChannel)
                    let params = {
                        model: clientChannel,
                        modelName: 'ClientChannel',
                        callback: function(response) {
                            if (response.status != 200) {
                                removeClientChannel(clientChannel)
                                notify('Уведомление!', `Клиента «${client.name}» не удалось добавить в канал «${channel.name}»!`, 'danger')
                            } else {
                                notify('Уведомление!', `Клиент «${client.name}» добавлен в канал «${channel.name}»!`, 'success')
                            }
                        }
                    }
                    await this.postModel(params)
                }
            },
            removeClientChannel: function(clientChannel) {
                this.$store.state.manager.clientChannels = this.$store.state.manager.clientChannels.filter(x => x.id != clientChannel.id)
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