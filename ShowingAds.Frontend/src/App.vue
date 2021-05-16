<template>
    <div id="app">
        <notifications position="top center" />
        <v-dialog style="z-index: 1000;" />
        <router-view />
    </div>
</template>

<script>
    export default {
        name: 'app'
    }
</script>

<style>
@import 'assets/main.css';
</style>