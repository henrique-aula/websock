using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

class Program
{
    
    private static ConcurrentDictionary<string, WebSocket> _clients = new();


    static async Task HandleClientAsync(string id, WebSocket webSocket)
    {
        var buffer = new byte[1024];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Close) break;


                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"{id}: {message}");

                byte[] response = Encoding.UTF8.GetBytes(message);

                await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error com{id}: {ex.Message}");
        }
        finally
        {
            _clients.TryRemove(id, out _);
            if (webSocket.State != WebSocketState.Aborted)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }


    
    static async Task Main()
    {
        Console.Write("SERVIDOR ");

        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        
        Console.WriteLine("ligado");


        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                string clientID = Guid.NewGuid().ToString();

                _clients.TryAdd(clientID, wsContext.WebSocket);
                Task.Run(() => HandleClientAsync(clientID, wsContext.WebSocket));
                Console.WriteLine($"novo cliente: {clientID}");
            }
        }
    }

    



}

