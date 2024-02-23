using System.Net;
using System.Net.Sockets;
using System.Text;

using var client = new TcpClient(IPEndPoint.Parse("127.0.0.1:13375")); // my endpoint


client.Connect("127.0.0.1", 13374); // remote endpoint

var stream = client.GetStream();
var buffer = new byte[1024];

while (true)
{
    Console.WriteLine("Enter a message: ");
    var message = Console.ReadLine()!;
    var request = Encoding.UTF8.GetBytes(message);
    
    stream.Write(request);
    stream.Read(buffer, 0, buffer.Length);

    var response = Encoding.UTF8.GetString(buffer);

    Console.WriteLine(response);
}