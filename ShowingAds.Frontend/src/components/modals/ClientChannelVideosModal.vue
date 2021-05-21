<template>
    <div class="row pt-2 pb-2 pr-4 pl-4">
        <div class="col-md-4 p-1" v-for="video in videos" :key="video.src.id">
            <ClientVideoCard :video="video.src" :selected="video.selected" :callback="changeSelect" />
        </div>
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import ClientVideoCard from '../cards/ClientVideoCard.vue'

    export default {
        name: 'ClientChannelVideosModal',
        components: {
            ClientVideoCard
        },
        props: {
            clientChannel: Object
        },
        computed: {
            client: function() {
                return this.$store.state.manager.clients ? this.$store.state.manager.clients.find(x => x.id == this.clientChannel.ads_client) : null
            },
            selectedVideos: function() {
                return this.$store.state.manager.clientVideos ? this.$store.state.manager.clientVideos.filter(x => this.clientChannel.ads_videos.includes(x.id)) : []
            },
            allVideos: function() {
                return this.$store.state.manager.clientVideos ? this.$store.state.manager.clientVideos.filter(x => this.client.id == x.ads_client) : []
            },
            videos: function() {
                let videos = []
                let selectedVideos = this.selectedVideos
                let allVideos = this.allVideos
                allVideos.forEach(x => {
                    let video = {}
                    video.src = x
                    video.selected = selectedVideos.includes(x)
                    videos.push(video)
                })
                return videos
            }
        },
        methods: {
            ...mapActions(['postModel']),
            changeSelect: async function(video, selected) {
                if (selected) {
                    if (this.clientChannel.ads_videos.includes(video.id) == false) {
                        this.clientChannel.ads_videos.push(video.id)
                    }
                } else {
                    this.clientChannel.ads_videos = this.clientChannel.ads_videos.filter(x => x != video.id)
                }
                let notify = this.notify
                let client = this.client
                let channel = this.$store.state.manager.channels.find(x => x.id == this.clientChannel.channel)
                let params = {
                    model: this.clientChannel,
                    modelName: 'ClientChannel',
                    callback: function(response) {
                        if (response.status != 200) {
                            params.model.ads_videos = params.model.ads_videos.filter(x => x != video.id)
                            if (selected)
                                notify('Уведомление!', `Видеоролик клиента «${client.name}» не удалось добавить в канал «${channel.name}»!`, 'danger')
                            else
                                notify('Уведомление!', `Видеоролик клиента «${client.name}» не удалось исключить из канала «${channel.name}»!`, 'danger')
                        } else {
                            if (selected)
                                notify('Уведомление!', `Видеоролик клиента «${client.name}» добавлен в канал «${channel.name}»!`, 'success')
                            else
                                notify('Уведомление!', `Видеоролик клиента «${client.name}» исключен из канала «${channel.name}»!`, 'success')
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