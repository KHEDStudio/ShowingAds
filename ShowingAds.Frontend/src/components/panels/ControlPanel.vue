<template>
    <div id="control-panel" class="card rounded">
        <div class="card-body text-center text-uppercase">
            <h1>{{ Title }}</h1>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'control-panel',
        data() {
            return {
                Title: 'Панель управления'
            }
        }
    }
</script>

<style scoped>
    h1 {
        font-weight: 900;
    }
</style>