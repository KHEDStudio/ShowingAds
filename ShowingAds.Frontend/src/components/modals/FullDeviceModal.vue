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
        <p class="ml-2 mb-0">Свободного места: {{ Math.round(device.info.FreeSpaceDisk / 1024 / 1024 / 1024 * 100) / 100 }} гбайт</p>
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
                <li>Скорость {{ Math.round(device.info.DownloadSpeed / 1024 * 100) / 100 }} кбайт/с</li>
            </ul>
        </div>
        <div class="ml-2 mb-0">
            <b-dropdown text="Выбор приоритетного клиента">
                <b-dropdown-item v-on:click="setPriorityClient(client)" v-for="client in clients" :key="client.id" v-bind:active="device.tasks.priority == client.id">
                    <p class="m-0 p-0">{{ client.name }}</p>
                    <p v-if="client.description != ''" class="m-0 p-0 text-muted">{{ client.description }}</p>
                </b-dropdown-item>
                <b-dropdown-divider></b-dropdown-divider>
                <b-dropdown-item v-on:click="setPriorityClient({id: '00000000-0000-0000-0000-000000000000', name: 'Без приоритета'})" v-bind:active="device.tasks.priority == '00000000-0000-0000-0000-000000000000'">
                    <p class="m-0 p-0" :class="(device.tasks.priority == '00000000-0000-0000-0000-000000000000') ? '' : 'text-muted'">Без приоритета</p>
                </b-dropdown-item>
            </b-dropdown>
        </div>
        <div class="mb-0 mt-2">
            <video class="rounded image-link" style="max-width: 100%" autoplay loop controls muted :src="`http://31.184.219.123:3666/video?video=${device.info.CurrentVideo}`"></video>
        </div>
        <div v-if="device.info.Screenshot != '00000000-0000-0000-0000-000000000000'" class="mt-2">
            <img class="rounded image-link" style="max-width: 100%" :href="`http://31.184.219.123:4000/screenshot?screenshot=${device.info.Screenshot}`" :src="`http://31.184.219.123:4000/screenshot?screenshot=${device.info.Screenshot}`" />
        </div>
        <button :disabled="device.tasks.take_screenshot" class="btn btn-outline-primary mt-2 mb-2 float-right" v-on:click="sendScreenshot">Сделать скриншот</button>
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
                        id: obj.id,
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
            },
            setPriorityClient: async function(priority) {
                let notify = this.notify
                let tasks = this.device.tasks
                tasks.priority = priority.id
                let params = {
                    model: tasks,
                    modelName: 'DeviceTasks',
                    callback: function(response) {
                        if (response.status == 200) {
                            setTimeout(() => notify('Уведомление!', `Приоритетный клиент «${priority.name}» установлен!`, 'success'), 0)
                        } else {
                            setTimeout(() => notify('Уведомление!', `Приоритетный клиент «${priority.name}» не установлен!`, 'danger'), 0)
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