import axios from 'axios'
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

const baseURL = 'http://84.38.188.128:3690/' //'http://31.184.219.123:63880/'

const models = {
    AdvertisingClient: {
        stateName: 'clients',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'advertisingclient'
    },
    AdvertisingVideo: {
        stateName: 'clientVideos',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'advertisingvideo'
    },
    Channel: {
        stateName: 'channels',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'channel'
    },
    ClientChannel: {
        stateName: 'clientChannels',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'clientchannel'
    },
    Content: {
        stateName: 'contents',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'content'
    },
    ContentVideo: {
        stateName: 'contentVideos',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'contentvideo'
    },
    Order: {
        stateName: 'orders',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'order'
    },
    User: {
        stateName: 'users',
        baseURL: 'http://84.38.188.128:3690/',
        path: 'user'
    },
    DeviceState: {
        stateName: 'devices',
        baseURL: 'http://84.38.188.128:3700/',
        //baseURL: 'http://localhost:49160/',
        path: 'device'
    },
    DeviceTasks: {
        baseURL: 'http://84.38.188.128:3700/',
        path: 'device/tasks'
    }
}

const state = {
    clients: null,
    clientVideos: null,
    channels: null,
    clientChannels: null,
    contents: null,
    contentVideos: null,
    orders: null,
    users: null,
    devices: null,

    uploads: []
}

const actions = {
    async clearModels({ commit }) {
        for (let model in models) {
            commit('setModels', {
                stateName: models[model].stateName,
                models: null
            })
        }

        commit('setModels', {
            stateName: 'uploads',
            models: []
        })
    },
    async getModels({ commit }, modelName) {
        let modelInfo = models[modelName]
        await axios.get(modelInfo.path, {
                baseURL: modelInfo.baseURL,
                headers: {
                    'Authorization': `Bearer ${this.getters.StateToken}`,
                    'Content-Type': 'application/json'
                }
            })
            .then(async response => {
                commit('setModels', {
                    stateName: modelInfo.stateName,
                    models: response.data
                })
            })
    },
    async postModel({ commit }, { model, modelName, callback }) {
        let modelInfo = models[modelName]
        await axios.post(modelInfo.path, model, {
                baseURL: modelInfo.baseURL,
                headers: {
                    'Authorization': `Bearer ${this.getters.StateToken}`,
                    'Content-Type': 'application/json'
                }
            })
            .then(response => {
                callback(response)
            })
            .catch(error => {
                let response = {
                    status: 12002,
                    error: error
                }
                callback(response)
            })
    },
    async deleteModel({ commit }, { model, modelName, callback }) {
        let modelInfo = models[modelName]
        await axios.delete(modelInfo.path, {
                baseURL: modelInfo.baseURL,
                headers: {
                    'Authorization': `Bearer ${this.getters.StateToken}`,
                    'Content-Type': 'application/json'
                },
                data: model
            })
            .then(response => {
                callback(response)
            })
            .catch(error => {
                let response = {
                    status: 12002,
                    error: error
                }
                callback(response)
            })
    },
    connectToNotifyService({ commit }) {
        let that = this
        let connectionId = ''
        let userHubConnectionStart = async function() {
            try {
                await userHubConnection.start()
                connectionId = await userHubConnection.invoke('SetSubscriberIdAsync', that.getters.StateUser.guidId)
            } catch (error) {
                setTimeout(userHubConnectionStart, 5000)
            }
        }
        const userHubConnection = new HubConnectionBuilder()
            .withUrl('http://84.38.188.128:3680/user')
            .configureLogging(LogLevel.Information)
            .build()
        let isReady = true
        let lastNotify = new Date()
        userHubConnection.on('Notify', async () => {
            if (isReady) {
                isReady = false
                await getNotifications(that, commit, models, connectionId)
                lastNotify = new Date()
                isReady = true
            }
        })
        userHubConnection.onclose(userHubConnectionStart)
        userHubConnectionStart()
    },
    async uploadLogo({ commit }, { logo, callback }) {
        let formData = new FormData()
        formData.append('file', logo)

        await axios.post('logo', formData, {
            baseURL: 'http://31.184.219.123:4000'
        })
        .then(response => {
            callback(response)
        })
        .catch(error => {
            let response = {
                status: 12002,
                error: error
            }
            callback(response)
        })
    },
    async uploadVideo({ commit }, { video, callback }) {
        let formData = new FormData()
        formData.append('file', video)

        await axios.post('video', formData, {
            baseURL: 'http://31.184.219.123:4000'
        })
        .then(response => {
            callback(response)
        })
        .catch(error => {
            let response = {
                status: 12002,
                error: error
            }
            callback(response)
        })
    }
}

async function getNotifications(that, commit, models, connectioId) {
    await axios.get(`notifications?connection=${connectioId}`, {
        baseURL: 'http://84.38.188.128:3680',
        headers: {
            'Authorization': `Bearer ${that.getters.StateToken}`,
            'Content-Type': 'application/json'
        }
    })
    .then(async response => {
        response.data.forEach(x => parseNotification(that, x, commit, models))
    })
}

function parseNotification(that, notification, commit, models) {
    notification.model = JSON.parse(notification.model)
    if (notification.type == 'DiagnosticInfo') {
        if (notification.model.Id == '11111111-1111-1111-1111-111111111111')
            console.log(notification)
        let device = that.state.manager['devices'].find(x => x.id == notification.model.Id)
        if (device != undefined)
            device.info = notification.model
        return
    }
    if (notification.type == 'DeviceTasks') {
        let device = that.state.manager['devices'].find(x => x.id == notification.model.id)
        if (device != undefined)
            device.tasks = notification.model
        return
    }
    if (notification.type == 'Device') {
        let device = that.state.manager['devices'].find(x => x.id == notification.model.id)
        if (device != undefined) {
            device.name = notification.model.name
            device.address = notification.model.address
            device.latitude = notification.model.latitude
            device.longitude = notification.model.longitude
            device.last_online = notification.model.last_online
            device.account = notification.model.account
            device.channel = notification.model.channel
        }
        return
    }
    let modelInfo = models[notification.type]
    let newModels = that.state.manager[modelInfo.stateName].filter(x => x.id != notification.model.id)
    if (notification.operation == 1 && notification.type == 'ClientChannel')
        deleteOrders(that, commit, notification.model)
    if (notification.operation == 1 && notification.type == 'AdvertisingClient') {
        let clientChannels = that.state.manager['clientChannels'].filter(x => x.ads_client == notification.model.id)
        clientChannels.forEach(x => deleteOrders(that, commit, x))
        commit('setModels', {
            stateName: 'clientChannels',
            models: that.state.manager['clientChannels'].filter(x => x.ads_client != notification.model.id)
        })
    }
    if (notification.operation == 0)
        newModels.push(notification.model)
    commit('setModels', {
        stateName: modelInfo.stateName,
        models: newModels
    })
}

function deleteOrders(that, commit, clientChannel) {
    commit('setModels', {
        stateName: 'orders',
        models: that.state.manager['orders'].filter(x => x.ads_client_channel != clientChannel.id)
    })
}

const mutations = {
    setModels(state, { stateName, models }) {
        state[stateName] = models
    }
}

export default {
    state,
    actions,
    mutations
}
