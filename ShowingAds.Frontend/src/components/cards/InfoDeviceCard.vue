<template>
    <div :class="device.info.DeviceStatus ? 'card mt-1 mb-0 offline-card' : 'card mt-1 mb-0'">
        <div class="card-title p-2 mb-0">
            <div class="mb-0">
                <div class="align-items-center pt-2 pr-2 pl-2">
                    <strong class="float-left">
                        <b-icon v-if="device.info.DeviceStatus & 1" icon="link45deg" font-scale="2" variant="danger"></b-icon>
                        <b-icon v-else icon="link45deg" font-scale="2" variant="success"></b-icon>
                        {{ channel != undefined ? `${channel.name} -> ${device.name}` : `${device.name}` }}
                        <small v-if="device.address" class="text-muted ml-2">({{ device.address }})</small>
                    </strong>
                    <button v-if="deleteCallback" type="button" class="close float-right">
                        <b-icon-x variant="danger" @click="deleteCallback(device)" />
                    </button>
                    <button v-if="editCallback" type="button" class="close mr-2 float-right">
                        <b-icon-sliders @click="editCallback(device)" />
                    </button>
                    <small v-if="device.info.DeviceStatus & 1" class="text-muted float-right mr-2">{{ timePassed }}</small>
                </div>
            </div>
        </div>
        <div v-if="device.info.DeviceStatus & 1" class="card-body p-0 m-0">
            <p class="card-text ml-4 p-0 mb-1">Статус: Offline</p>
        </div>
        <div v-else class="card-body p-0 m-0">
            <p class="card-text ml-4 p-0 mb-1">Статус: Online</p>
            <p v-if="device.info.DeviceStatus != 0" class="card-text ml-4 mb-0 mt-1">Ошибки:</p>
            <ul class="ml-4 mb-0 mt-0">
                <li v-if="device.info.DeviceStatus >> 1 & 1">Не удается скачать видеоролик</li>
                <li v-if="device.info.DeviceStatus >> 2 & 1">Что-то мешает воспроизведению видеоролика</li>
                <li v-if="device.info.DeviceStatus >> 3 & 1">HDMI кабель не подсоединен</li>
            </ul>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'InfoDeviceCard',
        props: {
            device: Object,
            channel: Object,
            editCallback: Function,
            deleteCallback: Function
        },
        data: function() {
            return {
                timeout: null,
                timePassed: ''
            }
        },
        methods: {
            timeTick: function() {
                let today = new Date()
                let time = new Date(Date.parse(this.device.last_online))
                let diff
                if ((diff = DateDiff.inSeconds(time, today)) < 60) {
                    this.timePassed = DateFormate.forSeconds(time, diff)
                    this.timeout = setTimeout(this.timeTick, 1000)
                } else if ((diff = DateDiff.inMinutes(time, today)) < 60) {
                    this.timePassed = DateFormate.forMinutes(time, diff)
                    this.timeout = setTimeout(this.timeTick, 1000)
                } else if ((diff = DateDiff.inHours(time, today)) < 24) {
                    this.timePassed = DateFormate.forHours(time, diff)
                    this.timeout = setTimeout(this.timeTick, 60 * 1000)
                } else if ((diff = DateDiff.inDays(time, today)) < 365) {
                    this.timePassed = DateFormate.forDays(time, diff)
                    this.timeout = setTimeout(this.timeTick, 60 * 60 * 1000)
                } else {
                    this.timePassed = DateFormate.forYears(time, DateDiff.inYears(time, today))
                    this.timeout = setTimeout(this.timeTick, 24 * 60 * 60 * 1000)
                }
            }
        },
        created() {
            this.timeTick()
        },
        beforeDestroy() {
            if (this.timeout)
                clearTimeout(this.timeout)
        }
    }

    var DateDiff = {
        inYears: function (d1, d2) {
            var t2 = d2.getFullYear()
            var t1 = d1.getFullYear()

            return parseInt(t2 - t1)
        },

        inDays: function (d1, d2) {
            var t2 = d2.getTime()
            var t1 = d1.getTime()

            return parseInt((t2 - t1) / (24 * 3600 * 1000))
        },

        inHours: function (d1, d2) {
            var t2 = d2.getTime()
            var t1 = d1.getTime()

            return parseInt((t2 - t1) / (3600 * 1000))
        },

        inMinutes: function (d1, d2) {
            var t2 = d2.getTime()
            var t1 = d1.getTime()

            return parseInt((t2 - t1) / (60 * 1000))
        },

        inSeconds: function (d1, d2) {
            var t2 = d2.getTime()
            var t1 = d1.getTime()

            return parseInt((t2 - t1) / 1000)
        }
    }

    function getShortTime(d1) {
        return ((d1.getHours() < 10) ? '0' + d1.getHours() : d1.getHours()) + ":" + ((d1.getMinutes() < 10) ? '0' + d1.getMinutes() : d1.getMinutes())
    }

    function getFullTime(d1) {
        return d1.toLocaleDateString() + " " + getShortTime(d1)
    }

    var DateFormate = {
        forSeconds: function (d1, diff) {
            return getShortTime(d1) + " (" + diff + " секунд назад)"
        },

        forMinutes: function (d1, diff) {
            return getShortTime(d1) + " (" + diff + " минут назад)"
        },

        forHours: function (d1, diff) {
            return getShortTime(d1) + " (" + diff + " часов назад)"
        },

        forDays: function (d1, diff) {
            return getFullTime(d1) + " (" + diff + " дней назад)"
        },

        forYears: function (d1, diff) {
            return getFullTime(d1) + " (" + diff + " лет назад)"
        }
    }

    var DateFormateNext = {
        forSeconds: function (d1, diff) {
            if (diff < 0)
                return "0 секунд"
            else return diff + " секунд"
        },

        forMinutes: function (d1, diff) {
            return diff + " минут"
        },

        forHours: function (d1, diff) {
            return diff + " часов"
        },

        forDays: function (d1, diff) {
            return diff + " дней"
        },

        forYears: function (d1, diff) {
            return diff + " лет"
        }
    }
</script>

<style scoped>
</style>