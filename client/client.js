const WebSocket = require("ws");

function ligar(){
    const sock = new WebSocket("ws://localhost:8080"); 

    sock.on("open", () => {
        console.log("conectado");
        sock.send("wow");
    });     

    sock.on("message", (data) => {
        console.log("server:", data.toString());
    });

    sock.on("error", (erro) => {
        //console.error("erro", erro.message);
    });

    sock.on("close", () => {
        //console.log("morri");

        setTimeout(() => {
            ligar();
        }, 200);
    });

    return sock;
}

ligar();