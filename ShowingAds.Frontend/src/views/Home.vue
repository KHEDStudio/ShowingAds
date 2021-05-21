<template>
    <div class="home">
        <div class="container-fluid pt-3 pb-3">
            <ControlPanel />
            <div class="row mt-3">
                <div class="col-md">
                    <AccountPanel />
                    <InformationPanel />
                </div>
                <div class="col-md">
                    <b-icon-plus font-scale="5" class="card plus mb-1" @click="addChannel" />
                    <ChannelCard v-for="channel in channels" :channel="channel" :key="channel.id" :editCallback="editChannel" :deleteCallback="deleteChannel" />
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    import ControlPanel from '../components/panels/ControlPanel.vue'
    import AccountPanel from '../components/panels/AccountPanel.vue'
    import InformationPanel from '../components/panels/InformationPanel.vue'

    import ChannelCard from '../components/cards/ChannelCard.vue'

    import ChannelForm from '../components/forms/ChannelForm.vue'

    export default {
        name: 'Home',
        components: {
            ControlPanel,
            AccountPanel,
            InformationPanel,
            ChannelCard
        },
        computed: {
            channels: function() {
                let channels = this.$store.state.manager.channels ? this.$store.state.manager.channels : []
                return channels.sort((a, b) => (a.name.toLowerCase() > b.name.toLowerCase()) ? 1 : ((b.name.toLowerCase() > a.name.toLowerCase()) ? -1 : 0))
            }
        },
        methods: {
            ...mapActions(['getModels', 'deleteModel', 'clearModels', 'connectToNotifyService']),
            addChannel: function() {
                this.$modal.show(
                    ChannelForm,
                    {
                        channel: null
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            editChannel: function(channel) {
                this.$modal.show(
                    ChannelForm,
                    {
                        channel: channel
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            deleteChannel: async function(channel) {
                let that = this
                let notify = this.notify
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить канал «${channel.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.channels = this.$store.state.manager.channels.filter(x => x.id != channel.id)
                                let params = {
                                    model: channel,
                                    modelName: 'Channel',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            that.$store.state.manager.channels.push(channel)
                                            notify('Уведомление!', `Канал «${channel.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Канал «${channel.name}» удален!`, 'success')
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
        },
        async created() {
            this.getModels('AdvertisingClient')
            this.getModels('AdvertisingVideo')
            this.getModels('Channel')
            this.getModels('ClientChannel')
            this.getModels('Content')
            this.getModels('ContentVideo')
            this.getModels('Order')
            this.getModels('User')
            this.getModels('DeviceState')
            this.connectToNotifyService()
            setTimeout(() => {
                console.log(this.$store.state.manager.devices)
            }, 5000)
        },
        async beforeDestroy() {
            await this.clearModels()
        }
    }
</script>

<style scoped>
@import '../assets/home.css';
</style>