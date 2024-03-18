using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

try
{
    client.Connect(IPEndPoint.Parse("127.0.0.1:13374"));
}
catch (SocketException ex)
{
    Console.WriteLine(ex.Message);

    return;
}

var users = new List<User>
{
    new()
    {
        Login = "Log1",
        Password = "Pass1"
    },
    new()
    {
        Login = "Log1",
        Password = "Pass1"
    }
};

var message = JsonSerializer.Serialize(users);


byte[] bytes = Encoding.UTF8.GetBytes(message);

client.Send(bytes);



record User
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}