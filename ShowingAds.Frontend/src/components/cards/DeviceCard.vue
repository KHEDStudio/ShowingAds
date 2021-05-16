<template>
    <div id="device" class="card p-2 mb-0 mt-1">
        <div class="align-items-center pt-2 pr-2 pl-2">
            <button type="button" class="close float-right">
                <b-icon-x />
            </button>
            <button class="card-text float-left btn btn-success" type="button">
                <h5 class="m-0">{{ Device.Name }}</h5>
            </button>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'device',
        props: {
            Device: Object
        }
    }
</script>

<style scoped>
</style>