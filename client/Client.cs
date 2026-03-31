using System.Net.WebSockets;
using System.Text;


Console.Write("CLIENTE ");

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://10.62.206.38:8080/"), CancellationToken.None);
Console.WriteLine("conectado");

var sendTask = Task.Run(async () =>
    {
        while (ws.State == WebSocketState.Open)
        {
            var message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message))
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
);

var receiveTask = Task.Run(async () =>
    {
        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"server: {message}");
        }
    }
);

await Task.WhenAll(sendTask, receiveTask);