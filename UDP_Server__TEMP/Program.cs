using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;


var selfEndpoint = IPEndPoint.Parse("127.0.0.1:13374");
var manualResetEvent = new ManualResetEvent(false);

manualResetEvent.WaitOne();
class Chunk
{
    public required byte[] Buffer { get; init; }
    public required int ActualLength { get; init; }
}

class FileRequest
{
    public required string Filename { get; init; }
    public required IPEndPoint RemoteEndPoint { get; init; }
}

class UdpServer
{
    private const string DefaultPath = @"C:\Users\Alex\Desktop\";
    private UdpClient? _udpClient;
    private bool _listening;
    private Task? _listeningTask;
    private readonly string _basePath;
    private readonly ManualResetEvent _resetEvent;

    public UdpServer(string hostname, short port, string basePath = DefaultPath)
    {
        EndPoint = IPEndPoint.Parse($"{hostname}:{port}");
        _listening = false;
        _basePath = basePath;
        _resetEvent = new ManualResetEvent(true);
    }

    public void Listen()
    {
        if (_listening)
        {
            throw new InvalidOperationException("Already listening");
        }

        _udpClient?.Dispose();
        _udpClient = new UdpClient(EndPoint);
        _listeningTask = Task.Factory.StartNew(ListenHandler, TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        if (!_listening)
        {
            return;
        }

        _listening = false;
        _resetEvent.Reset();
    }

    public IPEndPoint EndPoint { get; }

    private async Task ListenHandler()
    {
        while (_resetEvent.WaitOne())
        {
            var request = await _udpClient!.ReceiveAsync();
            var pathToFile = _basePath + Encoding.UTF8.GetString(request.Buffer);

            Console.WriteLine($"[{request.RemoteEndPoint.Address}]: {pathToFile}");

            _ = Task.Factory.StartNew(SendFile, new FileRequest()
            {
                Filename = Encoding.UTF8.GetString(request.Buffer),
                RemoteEndPoint = request.RemoteEndPoint
            });
        }
    }

    private async void SendFile(object? state)
    {
        if (state is not FileRequest fileRequest)
        {
            return;
        }

        var fullPath = _basePath + fileRequest;
        var file = ReadFile(fullPath);

        foreach (var chunk in file)
        {
            Console.WriteLine($"chunk.ActualLength: {chunk.ActualLength}");
            await _udpClient!.SendAsync(chunk.Buffer, chunk.ActualLength, fileRequest.RemoteEndPoint);
        }

        var endMessage = GetEndMessage();
        await _udpClient!.SendAsync(endMessage, endMessage.Length, fileRequest.RemoteEndPoint);

        Console.WriteLine("Sent end message!");
    }

    private static IEnumerable<Chunk> ReadFile(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var buffer = new byte[ushort.MaxValue];
        var chunkNumber = 0;

        while (true)
        {
            if (fileStream.Position == fileStream.Length) // -1
            {
                Console.WriteLine($"Loop broken: {fileStream.Position} of {fileStream.Length}");
                yield break;
            }

            var offset = 0;
            var chunkSize = 0;
            var chunkNumberBytes = BitConverter.GetBytes(chunkNumber);

            Console.WriteLine($"chunkNumber: {chunkNumber}");

            for (int i = 0; i < sizeof(int); i++, offset++)
            {
                buffer[offset] = chunkNumberBytes[i];
            }

            chunkSize += sizeof(int);

            var read = fileStream.Read(buffer, offset + sizeof(int), ushort.MaxValue - sizeof(int) * 2);
            chunkSize += read;

            var chunkLengthBytes = BitConverter.GetBytes(read);

            Console.WriteLine($"read: {read}");

            for (int i = 0; i < sizeof(int); i++, offset++)
            {
                buffer[offset] = chunkLengthBytes[i];
            }

            chunkSize += sizeof(int);


            chunkNumber++;
            yield return new Chunk()
            {
                Buffer = buffer,
                ActualLength = chunkSize
            };
        }
    }

    public static byte[] GetEndMessage()
    {
        return _endMessage;
    }

    private static readonly byte[] _endMessage;

    static UdpServer()
    {
        _endMessage = new byte[byte.MaxValue];
        Array.Fill(_endMessage, byte.MaxValue);
    }
}

////////////////////////////////////////////////////////////
// var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
// var selfEndpoint = IPEndPoint.Parse("127.0.0.1:13374");
// var manualResetEvent = new ManualResetEvent(false);
//
// socket.Bind(selfEndpoint);
//
// _ = Task.Run(async () =>
// {
//     byte[] buffer = new byte[ushort.MaxValue];
//     IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
//     
//     var request = await socket.ReceiveFromAsync(buffer, remoteEndpoint);
//     var message = Encoding.UTF8.GetString(buffer);
//
//     Console.WriteLine($"{((IPEndPoint)request.RemoteEndPoint).Address}:{message}");
// });
//
// manualResetEvent.WaitOne();