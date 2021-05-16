import axios from 'axios'
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

const baseURL = 'http://localhost:49158/' //'http://31.184.219.123:63880/'

const models = {
    AdvertisingClient: {
        stateName: 'clients',
        baseURL: 'http://localhost:49158/',
        path: 'advertisingclient'
    },
    AdvertisingVideo: {
        stateName: 'clientVideos',
        baseURL: 'http://localhost:49158/',
        path: 'advertisingvideo'
    },
    Channel: {
        stateName: 'channels',
        baseURL: 'http://localhost:49158/',
        path: 'channel'
    },
    ClientChannel: {
        stateName: 'clientChannels',
        baseURL: 'http://localhost:49158/',
        path: 'clientchannel'
    },
    Content: {
        stateName: 'contents',
        baseURL: 'http://localhost:49158/',
        path: 'content'
    },
    ContentVideo: {
        stateName: 'contentVideos',
        baseURL: 'http://localhost:49158/',
        path: 'contentvideo'
    },
    Order: {
        stateName: 'orders',
        baseURL: 'http://localhost:49158/',
        path: 'order'
    },
    User: {
        stateName: 'users',
        baseURL: 'http://localhost:49158/',
        path: 'user'
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
    users: null
}

const getters = {

}

const actions = {
    async clearModels({ commit }) {
        for (let model in models) {
            commit('setModels', {
                stateName: models[model].stateName,
                models: null
            })
        }
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
            .withUrl('http://localhost:49155/user')
            .configureLogging(LogLevel.Information)
            .build()
        let isReady = true
        userHubConnection.on('Notify', async () => {
            if (isReady) {
                isReady = false
                await getNotifications(that, commit, models, connectionId)
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
            baseURL: 'http://31.184.219.123:3666'
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
        baseURL: 'http://localhost:49155',
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
    let modelInfo = models[notification.type]
    let newModels = that.state.manager[modelInfo.stateName].filter(x => x.id != notification.model.id)
    if (notification.operation == 1 && notification.type == 'ClientChannel')
        commit('setModels', {
            stateName: 'orders',
            models: that.state.manager['orders'].filter(x => x.ads_client_channel != notification.model.id)
        })
    if (notification.operation == 0)
        newModels.push(notification.model)
    commit('setModels', {
        stateName: modelInfo.stateName,
        models: newModels
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
