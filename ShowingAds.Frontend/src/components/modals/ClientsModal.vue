<template>
    <div class="pt-2 pb-2 pr-4 pl-4">
        <b-icon-plus font-scale="5" class="card plus mb-1" @click="addClient" />
        <ClientCard v-for="client in clients" :key="client.id" :client="client" :clickCallback="showClientVideos" :editCallback="editClient" :deleteCallback="deleteClient" />
    </div>
</template>

<script>
    import { mapActions } from "vuex"

    import ClientCard from '../cards/ClientCard.vue'
    import ClientVideosModal from '../modals/ClientVideosModal.vue'

    import ClientForm from '../forms/ClientForm.vue'

    export default {
        name: 'ClientsCard',
        components: {
            ClientCard
        },
        computed: {
            clients: function() {
                return this.$store.state.manager.clients ? this.$store.state.manager.clients : []
            }
        },
        methods: {
            ...mapActions(['deleteModel']),
            showClientVideos: function(client) {
                this.$modal.show(
                    ClientVideosModal, 
                    {
                        client: client
                    },
                    {
                        height: "auto",
                        width: "80%",
                        scrollable: true
                    }
                )
            },
            addClient: function() {
                this.$modal.show(
                    ClientForm,
                    {
                        client: null
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            editClient: function(client) {
                this.$modal.show(
                    ClientForm,
                    {
                        client: client
                    },
                    {
                        height: "auto",
                        width: "60%",
                        scrollable: true
                    }
                )
            },
            deleteClient: async function(client) {
                let that = this
                let notify = this.notify
                this.$modal.show('dialog', {
                    title: 'Подтвердите действие',
                    text: `Удалить клиента «${client.name}»?`,
                    buttons: [
                        {
                            title: 'Да',
                            handler: async () => {
                                this.$modal.hide('dialog')
                                this.$store.state.manager.clients = this.$store.state.manager.clients.filter(x => x.id != client.id)
                                let params = {
                                    model: client,
                                    modelName: 'AdvertisingClient',
                                    callback: function(response) {
                                        if (response.status != 200) {
                                            that.$store.state.manager.clients.push(client)
                                            notify('Уведомление!', `Клиента «${client.name}» не удалось удалить!`, 'danger')
                                        } else {
                                            notify('Уведомление!', `Клиент «${client.name}» удален!`, 'success')
                                        }
                                    }
                                }
                                await this.deleteModel(params)
                            }
                        },
                        {
                            title: 'Нет',
                            handler: () => {
                                this.$modal.hide('dialog')
                            }
                        }
                    ]
                })
            },
            notify: function(title, text, variant) {
                this.$bvToast.toast(text, {
                    title: title,
                    autoHideDelay: 5000,
                    toaster: 'b-toaster-top-center',
                    variant: variant,
                    appendToast: false
                })
            }
        }
    }
</script>

<style scoped>
</style>