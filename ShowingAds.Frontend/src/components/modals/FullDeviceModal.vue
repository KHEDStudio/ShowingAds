<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <div class="d-flex align-items-center">
            <b-icon v-if="device.info.DeviceStatus & 1" icon="link45deg" font-scale="3" variant="danger"></b-icon>
            <b-icon v-else icon="link45deg" font-scale="3" variant="success"></b-icon>
            <h4>{{ device.name }}</h4>
            <small class="text-muted ml-2">({{ device.id }})</small>
        </div>
        <p v-if="device.address" class="ml-2 mb-0">Адрес: {{ device.address }}</p>
        <p class="ml-2 mb-0">Версия ПО: {{ device.info.Version }}</p>
        <p class="ml-2 mb-0">Альтернативный контент: {{ device.info.ReadyVideosCount }} видеороликов</p>
        <div v-if="clients.length">
            <p class="ml-2 mb-0">Клиенты:</p>
            <ul class="ml-2 mb-0 mt-0">
                <li v-for="client in clients" :key="client.name">«{{ client.name }}» скачано {{ client.count }} из {{ client.all }} видеороликов</li>
            </ul>
        </div>
        <div v-if="device.info.DownloadType != -1">
            <p class="ml-2 mb-0">Скачивание:</p>
            <ul class="ml-2 mb-0 mt-0">
                <li v-if="device.info.DownloadType == 0">Логотип</li>
                <li v-if="device.info.DownloadType == 1">Видеоролик клиента</li>
                <li v-if="device.info.DownloadType == 2">Видеоролик альтернативного контента</li>
                <li>Прогресс {{ device.info.DownloadProgress }}%</li>
                <li>Скорость {{ Math.round(device.info.DownloadSpeed / 1024) }} кбайт/с</li>
            </ul>
        </div>
        <div v-if="device.info.Screenshot != '00000000-0000-0000-0000-000000000000'" class="mt-2">
            <img class="rounded image-link" style="max-width: 100%" :href="`http://31.184.219.123:4000/screenshot?screenshot=${device.info.Screenshot}`" :src="`http://31.184.219.123:4000/screenshot?screenshot=${device.info.Screenshot}`" />
        </div>
        <button class="btn btn-outline-primary mt-2 mb-2 float-right" v-on:click="sendScreenshot">Сделать скриншот</button>
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    export default {
        name: 'FullDeviceModal',
        props: {
            device: Object
        },
        computed: {
            clients: function() {
                let clients = []
                let clientChannels = this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels.filter(x => x.channel == this.device.channel) : []
                for (let clientChannel in clientChannels) {
                    let obj = clientChannels[clientChannel]
                    clients.push({
                        name: this.$store.state.manager.clients ? this.$store.state.manager.clients.find(x => x.id == obj.ads_client).name : '',
                        count: this.device.info.ClientVideos ? this.device.info.ClientVideos[obj.id] ? this.device.info.ClientVideos[obj.id] : 0 : 0,
                        all: this.$store.state.manager.clientVideos ? this.$store.state.manager.clientVideos.filter(x => obj.ads_videos.includes(x.id)).length : 0
                    })
                }
                return clients
            }
        },
        methods: {
            ...mapActions(['postModel']),
            sendScreenshot: async function() {
                let tasks = this.device.tasks
                tasks.take_screenshot = true
                let params = {
                    model: tasks,
                    modelName: 'DeviceTasks',
                    callback: function(response) {}
                }
                await this.postModel(params)
            }
        }
    }
</script>

<style scoped>
</style>