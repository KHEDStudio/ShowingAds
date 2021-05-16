<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <b-icon-plus font-scale="5" class="card plus mb-1" />
        <VideoCard v-for="contentVideo in contentVideos" :key="contentVideo.id" :video="contentVideo" :deleteCallback="deleteVideo" />
    </div>
</template>

<script>
    import VideoCard from '../cards/VideoCard.vue'

    export default {
        name: 'ContentVideosModal',
        props: {
            content: Object
        },
        components: {
            VideoCard
        },
        computed: {
            contentVideos: function() {
                return this.$store.state.manager.contentVideos ? this.$store.state.manager.contentVideos.filter(x => x.content == this.content.id) : []
            }
        },
        methods: {
            deleteVideo: function(video) {
                console.log(video)
            }
        }
    }
</script>

<style scoped>
</style>