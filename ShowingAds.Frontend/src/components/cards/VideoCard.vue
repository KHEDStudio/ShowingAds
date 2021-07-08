<template>
    <div class="card p-2 mb-0 mt-1">
        <div class="align-items-center pr-2 pl-2 mb-1">
            <button v-if="deleteCallback" type="button" class="close float-right">
                <b-icon-x variant="danger" @click="deleteCallback(video)" />
            </button>
        </div>
        <video ref="video" controls preload="none" class="img-fluid rounded">
            <source :src="`http://31.184.219.123:3666/video?video=${video.id}`" />
        </video>
    </div>
</template>

<script>
    export default {
        name: 'VideoCard',
        props: {
            video: Object,
            deleteCallback: Function
        }
    }
</script>

<style scoped>
</style>