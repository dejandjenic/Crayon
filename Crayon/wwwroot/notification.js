const host = window.location.protocol + "//" + window.location.host;


const connection = new signalR.HubConnectionBuilder()
    .withUrl(host+ window.location.pathname.replace("/index.html","") + "/notification" + window.location.search, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();


const images = [];


async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        console.log(connection)

        connection.on("OnEvent", (message) => {


            console.log(message)
            const li = document.createElement("li");
            li.textContent = "DATA:" + JSON.stringify(message)
            document.getElementById("messageList").appendChild(li);
        });




    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};


connection.onclose(async () => {
    await start();
});


// Start the connection.
start();


Number.prototype.round = function (places) {
    return +(Math.round(this + "e+" + places) + "e-" + places);
}


async function subscribe(data){
    connection.invoke("subscribe", data);
}


async function unsubscribe(data){
    connection.invoke("unsubscribe", data);
}
