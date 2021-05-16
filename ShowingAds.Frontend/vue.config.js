module.exports = {
    configureWebpack: {
      devtool: 'source-map',
    },
    devServer: {
        proxy: 'http://31.184.219.123:63880/',
    }
}