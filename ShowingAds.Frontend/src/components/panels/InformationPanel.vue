<template>
    <div class="card mt-3 mb-3">
        <div class="card-body">
            <div v-if="existUploads">
                <h4 class="card-title">Загрузка видеороликов</h4>
                <UploadCard v-for="upload in uploads" :key="upload.id" :upload="upload" />
            </div>
            <div v-if="existDevices">
                <h4 class="card-title">Информация о приставках</h4>
                <InfoDeviceCard v-for="record in deviceRecords" :key="record.device.id" :device="record.device" :channel="record.channel" :deleteCallback="deleteDevice" />
            </div>
        </div>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    import UploadCard from '../cards/UploadCard.vue'
    import InfoDeviceCard from '../cards/InfoDeviceCard.vue'

    export default {
        name: 'InformationPanel',
        components: {
            UploadCard,
            InfoDeviceCard
        },
        computed: {
            existUploads: function() {
                return this.$store.state.manager.uploads.length > 0
            },
            uploads: function() {
                return this.$store.state.manager.uploads
            },
            existDevices: function() {
                return this.$store.state.manager.devices != null
            },
            deviceRecords: function() {
                if (this.$store.state.manager.devices) {
                    let records = []
                    this.$store.state.manager.devices.forEach(device => {
                        let channel = this.$store.state.manager.channels ? this.$store.state.manager.channels.find(x => x.id == device.channel) : undefined
                        records.push({
                            device: device,
                            channel: channel
                        })
                    })
                    records = records.sort((a, b) => {
                        if (a.channel == undefined && b.channel != undefined)
                            return 1
                        if (a.channel != undefined && b.channel == undefined)
                            return -1
                        if (a.channel == undefined && b.channel == undefined)
                            return (a.device.name.toLowerCase() > b.device.name.toLowerCase()) ? 1 : ((b.device.name.toLowerCase() > a.device.name.toLowerCase()) ? -1 : 0)
                        if (a.channel != undefined && b.channel != undefined)
                            if (a.channel.name == b.channel.name)
                                return (a.device.name.toLowerCase() > b.device.name.toLowerCase()) ? 1 : ((b.device.name.toLowerCase() > a.device.name.toLowerCase()) ? -1 : 0)
                            else
                                return (a.channel.name.toLowerCase() > b.channel.name.toLowerCase()) ? 1 : ((b.channel.name.toLowerCase() > a.channel.name.toLowerCase()) ? -1 : 0)
                    })
                    return records
                }
                return []
            }
        },
        methods: {
            ...mapActions(['deleteModel']),
            deleteDevice: async function(device) {
                let that = this
                let notify = this.notify
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить приставку «${device.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.devices = this.$store.state.manager.devices.filter(x => x.id != device.id)
                                let params = {
                                    model: device,
                                    modelName: 'DeviceState',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            that.$store.state.manager.devices.push(device)
                                            notify('Уведомление!', `Приставку «${device.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Приставка «${device.name}» удалена!`, 'success')
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