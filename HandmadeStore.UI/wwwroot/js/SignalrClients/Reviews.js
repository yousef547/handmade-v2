$(() => {
    var connection = new signalR.HubConnectionBuilder().withUrl("/Reviews").build();
    connection.start();
    connection.on("loadReviews", (id) => {
        if (window.location.search == `?productId=${id}`)
            location.reload();
    })
})