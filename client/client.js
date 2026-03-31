const WebSocket = require('ws');

const socket = new WebSocket("ws://localhost:8080");

function ligar(){
    socket.addEventListener("open", (event) => {
        console.log("conectado");
        socket.send("ok");
    });

    socket.addEventListener("message", (event) => {
        console.log("server:", event.data);
    });

    socket.addEventListener("error", (event) => {
        console.error("erro", event);
    });

    socket.addEventListener("close", () => {
        console.log("morri");
    });
}

ligar();