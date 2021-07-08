<template>
    <div class="card p-2 mb-0 mt-1">
        <div class="card-title">
            <div class="align-items-center">
                <button type="button" class="close float-right">
                    <b-icon-x variant="danger" @click="deleteUpload" />
                </button>
                <h5>«{{ upload.collection.name }}»</h5>
            </div>
            <p>Видеоролик -> {{ video ? video.name : '?' }}</p>
            <b-progress :value="uploaded" :max="videos" show-progress animated />
        </div>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    export default {
        name: 'UploadCard',
        props: {
            upload: Object
        },
        computed: {
            videos: function() {
                return this.upload.videos.length
            }
        },
        data: function() {
            return {
                uploaded: 0,
                video: null,
                uploading: true
            }
        },
        methods: {
            ...mapActions(['postModel', 'uploadVideo']),
            uploadVideos: async function() {
                let that = this
                let notify = this.notify
                for (let i = 0; i < this.videos && this.uploading; i++) {
                    this.uploaded = i
                    this.video = this.upload.videos[i]

                    let params = {
                        video: this.video,
                        callback: async function(response) {
                                console.log(response)
                            if (response.status == 200) {
                                that.uploading = await that.upload.uploaded(response.data.file, response.data.duration)
                            } else {
                                that.uploading = false
                            }
                        }
                    }
                    await this.uploadVideo(params)
                }

                this.$store.state.manager.uploads = this.$store.state.manager.uploads.filter(x => x.id != this.upload.id)
                if (this.uploading == false) {
                    setTimeout(() => notify('Уведомление!', `Не удалось загрузить видеоролики «${this.upload.collection.name}»!`, 'danger'), 0)
                } else {
                    setTimeout(() => notify('Уведомление!', `Видеоролики «${this.upload.collection.name}» успешно загружены!`, 'success'), 0)
                }
            },
            deleteUpload: function() {
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Остановить загрузку видеороликов «${this.upload.collection.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: () => {
                                this.uploading = false
                                this.$modal.hide('dialog')
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
        },
        created: function() {
            this.video = this.upload.videos[0]
            setTimeout(this.uploadVideos, 1000)
        }
    }
</script>

<style scoped>
</style>