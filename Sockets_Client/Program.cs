using System.Net;
using System.Net.Sockets;
using System.Text;


var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

Console.WriteLine("Enter nickname to connect");

try
{
    client.Connect(IPEndPoint.Parse("127.0.0.1:13374"));
}
catch (SocketException ex)
{
    Console.WriteLine(ex.Message);

    return;
}

Task.Run(() =>
{
    try
    {
        while (true)
        {
            var buffer = new byte[1024];

            client.Receive(buffer);

            var message = Encoding.UTF8.GetString(buffer);

            Console.WriteLine($"Message from server: {message}");
        }
    }
    catch (SocketException ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        client.Dispose();
    }
});

while (true)
{
    string message = Console.ReadLine()!;

    if (message == "0")
    {
        client.Disconnect(false);
        break;
    }

    byte[] bytes = Encoding.UTF8.GetBytes(message);

    client.Send(bytes);
}