<template>
    <div class="pt-2 pb-2 pr-4 pl-4 text-center">
        <h4>«{{ collection.name }}»</h4>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">Видеоролики</span>
            </div>
            <input type="file" accept="video/mp4, video/x-m4v, video/*" ref="videos" class="form-control" multiple />
        </div>
        <button type="submit" class="btn btn-primary float-right mb-3" @click="save">Сохранить</button>
    </div>
</template>

<script>
    import { v4 as uuidv4 } from 'uuid'

    export default {
        name: 'VideoForm',
        props: {
            collection: Object,
            uploaded: Function
        },
        methods: {
            save: function() {
                if (this.$refs.videos.files.length > 0) {
                    let upload = {
                        id: uuidv4(),
                        collection: this.collection,
                        videos: this.$refs.videos.files,
                        uploaded: this.uploaded
                    }
                    this.$store.state.manager.uploads.push(upload)
                    this.$emit('close')
                }
            }
        }
    }
</script>

<style scoped>
</style>