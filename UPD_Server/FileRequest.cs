using System.Net;

namespace UPD_Server;

internal class FileRequest
{
    public required string Filename { get; init; }
    public required IPEndPoint RemoteEndPoint { get; init; }
}