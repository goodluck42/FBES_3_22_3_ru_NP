using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


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
        byte[] buffer = new byte[4096];
        
        try
        {
            int read = 0;
            while ((read = client.Receive(buffer)) > 0)
            {
                Console.WriteLine($"read: {read}");
                var message = Encoding.UTF8.GetString(buffer[..read]);
                var users = JsonSerializer.Deserialize<User[]>(message);

                if (users == null)
                {
                    Console.WriteLine("Error!");
                    break;
                }

                Console.WriteLine($"users.Length: {users.Length}");
                
                foreach (var user in users)
                {
                    Console.WriteLine(user.ToString());
                }

                Array.Clear(buffer, 0, read);
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

record User
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}