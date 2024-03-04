using System.Net;
using System.Net.Sockets;

namespace TCP_Server;

internal class FileRequest
{
    public required string Filename { get; init; }
    public required IPEndPoint RemoteEndPoint { get; init; }
    public required TcpClient TcpClient { get; init; }
}