<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <ContentCard v-for="content in contents" :key="content.id" :content="content" :clickCallback="addContent" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import ContentCard from '../cards/ContentCard.vue'

    export default {
        name: 'AddContentModal',
        props: {
            channel: Object
        },
        components: {
            ContentCard
        },
        computed: {
            contents: function() {
                return this.$store.state.manager.contents ? this.$store.state.manager.contents.filter(x => this.channel.contents.includes(x.id) == false) : []
            }
        },
        methods: {
            ...mapActions(['postModel']),
            addContent: async function(content) {
                let notify = this.notify
                let channel = this.channel
                if (channel.contents.includes(content.id) == false) {
                    channel.contents.push(content.id)
                    let params = {
                        model: channel,
                        modelName: 'Channel',
                        callback: function(response) {
                            if (response.status != 200) {
                                channel.contents = channel.contents.filter(x => x != content.id)
                                notify('Уведомление!', `Альтернативный контент «${content.name}» не удалось добавить в канал «${channel.name}»!`, 'danger')
                            } else {
                                notify('Уведомление!', `Альтернативный контент «${content.name}» добавлен в канал «${channel.name}»!`, 'success')
                            }
                        }
                    }
                    await this.postModel(params)
                }
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