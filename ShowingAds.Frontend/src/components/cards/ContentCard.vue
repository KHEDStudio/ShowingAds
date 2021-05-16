<template>
    <div class="card p-2 mb-0 mt-1">
        <div class="align-items-center pt-2 pr-2 pl-2">
            <button v-if="deleteCallback" type="button" class="close float-right">
                <b-icon-x variant="danger" @click="deleteCallback(content)" />
            </button>
            <button v-if="editCallback" type="button" class="close mr-3 float-right">
                <b-icon-sliders @click="editCallback(content)" />
            </button>
            <button :style="`background-color: rgb(${red}, ${green}, ${blue})`" class="card-text float-left btn" type="button" @click="clickCallback(content)">
                <h5 class="m-0">{{ content.name }}</h5>
            </button>
        </div>
        <p v-if="content.description != ''" class="card-text ml-2 mb-0">Описание: {{ content.description }}</p>
        <p class="card-text ml-2 mb-0">Количество видеороликов: {{ countVideos }}</p>
        <p class="card-text ml-2 mb-0">Общая продолжительность: {{ durationVideos }}</p>
    </div>
</template>

<script>
    export default {
        name: 'Content',
        props: {
            content: Object,
            clickCallback: Function,
            editCallback: Function,
            deleteCallback: Function
        },
        computed: {
            user: function() {
                return this.$store.state.manager.users ? this.$store.state.manager.users.find(x => x.id == this.content.account) : null
            },
            red: function() {
                let user = this.user
                return user ? user.red : 255
            },
            green: function() {
                let user = this.user
                return user ? user.green : 255
            },
            blue: function() {
                let user = this.user
                return user ? user.blue : 255
            },
            videos: function() {
                return this.$store.state.manager.contentVideos ? this.$store.state.manager.contentVideos.filter(x => x.content == this.content.id) : []
            },
            countVideos: function() {
                return this.videos.length
            },
            durationVideos: function() {
                return this.msToTime(this.videos.reduce((accumulator, currentValue) => accumulator + currentValue.duration, 0) / 10000)
            }
        },
        methods: {
            msToTime: function(milliseconds) {
                let ms = milliseconds % 1000
                milliseconds = (milliseconds - ms) / 1000
                let secs = milliseconds % 60
                milliseconds = (milliseconds - secs) / 60
                let mins = milliseconds % 60
                let hrs = (milliseconds - mins) / 60
                return this.timeFormat(hrs) + ':' + this.timeFormat(mins) + ':' + this.timeFormat(secs)
            },
            timeFormat: function(number) {
                return (number < 10 ? `0${number}` : number)
            }
        }
    }
</script>

<style scoped>
</style>