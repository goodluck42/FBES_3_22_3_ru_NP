using System.Net;
using System.Net.Sockets;
using System.Text;

var endPoint = IPEndPoint.Parse("127.0.0.1:13374");

using TcpListener server = new TcpListener(endPoint);

server.Start();

while (true)
{
    TcpClient client = server.AcceptTcpClient();

    Console.WriteLine("Client accepted");
    Task.Run(() =>
    {
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];
            
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var request = Encoding.UTF8.GetString(buffer);

                Console.WriteLine(request);
                
                var response = request.ToUpper();

                stream.Write(Encoding.UTF8.GetBytes(response));
            }
        }
        catch (SocketException ex)
        {
        }
        finally
        {
            Console.WriteLine("Client disconnected");
            client.Dispose();
        }
    });
}