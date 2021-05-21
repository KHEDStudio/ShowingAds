<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <DeviceCard v-for="device in devices" :key="device.id" :device="device" :clickCallback="addDevice" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"
    import DeviceCard from '../cards/DeviceCard.vue'

    export default {
        name: 'AddDeviceModal',
        props: {
            channel: Object
        },
        components: {
            DeviceCard
        },
        computed: {
            devices: function() {
                return this.$store.state.manager.devices ? this.$store.state.manager.devices.filter(x => x.channel == '00000000-0000-0000-0000-000000000000') : []
            }
        },
        methods: {
            ...mapActions(['postModel']),
            addDevice: async function(device) {
                let notify = this.notify
                let channel = this.channel
                device.channel = channel.id
                let params = {
                    model: device,
                    modelName: 'DeviceState',
                    callback: function(response) {
                        if (response.status != 200) {
                            device.channel = '00000000-0000-0000-0000-000000000000'
                            notify('Уведомление!', `Приставку «${device.name}» не удалось подписать на канал «${channel.name}»!`, 'danger')
                        } else {
                            notify('Уведомление!', `Приставка «${device.name}» подписана на канал «${channel.name}»!`, 'success')
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