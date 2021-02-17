function startTicker() {
    $(document.body).marquee('resume')
    $(document.documentElement).show()
    $(document.body).show()
}

function initTicker(ticker, delay) {
    $(document.documentElement).hide()
    $(document.body).hide()
    clearTimeout(startTicker)
    $(document.body).marquee('destroy')
    $(document.body).text(ticker)
    if (ticker == '')
        return
    $(document.documentElement).show()
    $(document.body).show()
    $(document.body).bind('finished', function () {
        $(this).marquee('pause')
        $(document.documentElement).hide()
        $(document.body).hide()
        setTimeout(startTicker, delay)
    })
        .marquee({
            duration: 30000,
            direction: 'left'
        })
}

initTicker('', 0)