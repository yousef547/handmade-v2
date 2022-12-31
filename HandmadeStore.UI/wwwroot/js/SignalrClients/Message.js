
$(() => {
    var connection = new signalR.HubConnectionBuilder().withUrl("/Massage").build();
    connection.start();
    connection.on("receiverMassage", (message) => {
        toastr.success(`${message.sender} send (${message.body})`, 'Handmade Store', { timeOut: 2000 })
    })
})