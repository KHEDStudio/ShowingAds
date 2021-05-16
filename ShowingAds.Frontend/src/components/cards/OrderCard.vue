<template>
    <div class="card p-2 mb-0 mt-1">
        <div class="align-items-center pt-2 pr-2 pl-2">
            <button v-if="deleteCallback" type="button" class="close float-right">
                <b-icon-x variant="danger" @click="deleteCallback(order)" />
            </button>
            <div class="card-text float-left">
                <h5 class="m-0">{{ client.name }}</h5>
            </div>
        </div>
        <p v-if="client.description != ''" class="card-text ml-2 mb-0">Описание: {{ client.description }}</p>
    </div>
</template>

<script>
    export default {
        name: 'Order',
        props: {
            order: Object,
            deleteCallback: Function
        },
        computed: {
            client: function() {
                let clientChannel = this.$store.state.manager.clientChannels ? this.$store.state.manager.clientChannels.find(x => x.id == this.order.ads_client_channel) : null
                if (clientChannel == null)
                    return {}
                return this.$store.state.manager.clients ? this.$store.state.manager.clients.find(x => x.id == clientChannel.ads_client) : {}
            }
        }
    }
</script>

<style scoped>
</style>