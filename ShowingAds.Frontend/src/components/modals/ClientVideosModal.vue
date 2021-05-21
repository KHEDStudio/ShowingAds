<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <b-icon-plus font-scale="5" class="card plus mb-1" @click="addClientVideo" />
        <VideoCard v-for="clientVideo in clientVideos" :key="clientVideo.id" :video="clientVideo" :deleteCallback="deleteVideo" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    import VideoCard from '../cards/VideoCard.vue'
    import VideoForm from '../forms/VideoForm.vue'

    export default {
        name: 'ClientVideosModal',
        props: {
            client: Object
        },
        components: {
            VideoCard
        },
        computed: {
            clientVideos: function() {
                return this.$store.state.manager.clientVideos ? this.$store.state.manager.clientVideos.filter(x => x.ads_client == this.client.id) : []
            }
        },
        methods: {
            ...mapActions(['deleteModel', 'postModel']),
            addClientVideo: function() {
                let that = this
                this.$modal.show(
                    VideoForm,
                    {
                        collection: this.client,
                        uploaded: async function(id, duration) {
                            let saved = null
                            let clientVideo = {
                                id: id,
                                duration: duration,
                                ads_client: that.client.id
                            }
                            let params = {
                                model: clientVideo,
                                modelName: 'AdvertisingVideo',
                                callback: function(response) {
                                    saved = response.status == 200
                                }
                            }
                            await that.postModel(params)
                            return saved
                        }
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            deleteVideo: function(video) {
                let notify = this.notify
                let client = this.client
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить видеоролик из клиента «${this.client.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.clientVideos = this.$store.state.manager.clientVideos.filter(x => x.id != video.id)
                                let params = {
                                    model: video,
                                    modelName: 'AdvertisingVideo',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            this.$store.state.manager.clientVideos.push(video)
                                            notify('Уведомление!', `Видеоролик клиента «${client.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Видеоролик клиента «${client.name}» успешно удален!`, 'success')
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