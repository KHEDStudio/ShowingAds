<template>
    <div class="card p-2 mb-0 mt-1">
        <div class="align-items-center pt-2 pr-2 pl-2">
            <button v-if="deleteCallback" type="button" class="close float-right">
                <b-icon-x variant="danger" @click="deleteCallback(device)" />
            </button>
            <button v-if="editCallback" type="button" class="close mr-3 float-right">
                <b-icon-sliders @click="editCallback(device)" />
            </button>
            <button class="card-text float-left btn" :class="device.status & 1 ? 'btn-danger' : 'btn-success'" type="button" @click="clickCallback(device)">
                <h5 class="m-0">{{ device.name }}</h5>
            </button>
        </div>
        <p v-if="device.address" class="card-text ml-2 mb-0">Адрес: {{ device.address }}</p>
        <p class="card-text ml-2 mb-0">Видеоролики: {{ device.info.ReadyVideosCount }} шт.</p>
        <p class="card-text ml-2 mb-0">Прогресс скачивания: {{ device.info.DownloadProgress }}%</p>
        <p class="card-text ml-2 mb-0">Скорость скачивания: {{ Math.round(device.info.DownloadSpeed / 1024) }} кбайт/с</p>
        <p class="card-text ml-2 mb-0">Версия ПО: {{ device.info.Version }}</p>
        <div v-if="device.status & 1" class="card-body p-0 m-0">
            <p class="card-text ml-2 p-0 mb-1">Статус: Offline</p>
        </div>
        <div v-else class="card-body p-0 m-0">
            <p class="card-text ml-2 p-0 mb-1">Статус: Online</p>
            <p v-if="device.status != 0" class="card-text ml-2 mb-0 mt-1">Ошибки:</p>
            <ul class="ml-2 mb-0 mt-0">
                <li v-if="device.status >> 1 & 1">Не удается скачать видеоролик</li>
                <li v-if="device.status >> 2 & 1">Что-то мешает воспроизведению видеоролика</li>
                <li v-if="device.status >> 3 & 1">HDMI кабель не подсоединен</li>
            </ul>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'DeviceCard',
        props: {
            device: Object,
            clickCallback: Function,
            editCallback: Function,
            deleteCallback: Function
        }
    }
</script>

<style scoped>
</style>