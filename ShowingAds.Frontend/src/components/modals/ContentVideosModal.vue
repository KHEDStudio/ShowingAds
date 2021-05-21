<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <b-icon-plus font-scale="5" class="card plus mb-1" @click="addContentVideo" />
        <VideoCard v-for="contentVideo in contentVideos" :key="contentVideo.id" :video="contentVideo" :deleteCallback="deleteVideo" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    import VideoCard from '../cards/VideoCard.vue'
    import VideoForm from '../forms/VideoForm.vue'

    export default {
        name: 'ContentVideosModal',
        props: {
            content: Object
        },
        components: {
            VideoCard
        },
        computed: {
            contentVideos: function() {
                return this.$store.state.manager.contentVideos ? this.$store.state.manager.contentVideos.filter(x => x.content == this.content.id) : []
            }
        },
        methods: {
            ...mapActions(['deleteModel', 'postModel']),
            addContentVideo: function() {
                let that = this
                this.$modal.show(
                    VideoForm, 
                    {
                        collection: this.content,
                        uploaded: async function(id, duration) {
                            let saved = null
                            let contentVideo = {
                                id: id,
                                duration: duration,
                                content: that.content.id
                            }
                            let params = {
                                model: contentVideo,
                                modelName: 'ContentVideo',
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
                let content = this.content
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить видеоролик из альтернативного контента «${this.content.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.contentVideos = this.$store.state.manager.contentVideos.filter(x => x.id != video.id)
                                let params = {
                                    model: video,
                                    modelName: 'ContentVideo',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            this.$store.state.manager.contentVideos.push(video)
                                            notify('Уведомление!', `Видеоролик альтернативного контента «${content.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Видеоролик альтернативного контента «${content.name}» успешно удален!`, 'success')
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