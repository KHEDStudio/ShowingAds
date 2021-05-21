<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <b-icon-plus font-scale="5" class="card plus mb-1" @click="addContent" />
        <ContentCard v-for="content in contents" :key="content.id" :content="content" :clickCallback="showContentVideos" :editCallback="editContent" :deleteCallback="deleteContent" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    
    import ContentCard from '../cards/ContentCard.vue'
    
    import ContentVideosModal from '../modals/ContentVideosModal.vue'

    import ContentForm from '../forms/ContentForm.vue'

    export default {
        name: 'ContentsCard',
        components: {
            ContentCard
        },
        computed: {
            contents: function() {
                return this.$store.state.manager.contents ? this.$store.state.manager.contents : []
            }
        },
        methods: {
            ...mapActions(['deleteModel']),
            showContentVideos: function(content) {
                this.$modal.show(
                    ContentVideosModal, 
                    {
                        content: content
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            addContent: function() {
                this.$modal.show(
                    ContentForm,
                    {
                        content: null
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            editContent: function(content) {
                this.$modal.show(
                    ContentForm,
                    {
                        content: content
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            deleteContent: async function(content) {
                let that = this
                let notify = this.notify
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить альтернативный контент «${content.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.contents = this.$store.state.manager.contents.filter(x => x.id != content.id)
                                let params = {
                                    model: content,
                                    modelName: 'Content',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            that.$store.state.manager.contents.push(content)
                                            notify('Уведомление!', `Альтернативный контент «${content.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Альтернативный контент «${content.name}» удален!`, 'success')
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