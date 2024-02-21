using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;


using var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
var endPoint = IPEndPoint.Parse("127.0.0.1:13374");

server.Bind(endPoint);
server.Listen();

var clients = new List<Socket>();

while (true)
{
    var client = server.Accept();

    clients.Add(client);

    Console.WriteLine($"Client accepted {clients.Count}");

    Task.Run(() =>
    {
        byte[] buffer = new byte[1024];

        try
        {
            int byteLength = 0;
            while ((byteLength = client.Receive(buffer)) > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                var reversed = message.Reverse();
                var builder = new StringBuilder();

                foreach (var symbol in reversed)
                {
                    builder.Append(symbol);
                }

                Console.WriteLine($"[{byteLength}]: {message}");

                var response = Encoding.UTF8.GetBytes(builder.ToString());

                client.Send(response);
                Array.Clear(buffer, 0, byteLength);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Client disconnected!");
        }
        finally
        {
            client.Dispose();
        }
    });
}