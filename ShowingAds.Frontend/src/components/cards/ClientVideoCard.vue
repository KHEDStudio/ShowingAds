<template>
    <div class="card p-2">
        <b-form-checkbox v-model="videoSelected" @change="changed" />
        <video controls preload="metadata" class="img-fluid rounded">
            <source :src="`http://31.184.219.123:3666/video?video=${video.id}`" />
        </video>
    </div>
</template>

<script>
    export default {
        name: 'ClientVideoCard',
        props: {
            video: Object,
            selected: Boolean,
            callback: Function
        },
        data: function() {
            return {
                videoSelected: false
            }
        },
        methods: {
            changed: async function() {
                await this.callback(this.video, this.videoSelected)
            }
        },
        created() {
            this.videoSelected = this.selected
        }
    }
</script>

<style scoped>
</style>