using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Server;

internal class TcpServer
{
    private const int DatagramSize = 65000;
    private const string DefaultPath = @"C:\Users\Alex\Desktop\";
    private UdpClient? _udpClient;
    private bool _listening;
    private Task? _listeningTask;
    private readonly string _basePath;
    private readonly ManualResetEvent _resetEvent;

    public TcpServer(string host, short port, string basePath = DefaultPath)
    {
        EndPoint = IPEndPoint.Parse($"{host}:{port}");
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

        _listening = true;
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

            if (request.Buffer.Length == byte.MaxValue)
            {
                _ = Task.Factory.StartNew(SendFile, new FileRequest()
                {
                    Filename = GetNormalizedFileName(request.Buffer),
                    RemoteEndPoint = request.RemoteEndPoint
                });
            }
        }
    }

    private async void SendFile(object? state)
    {
        if (state is not FileRequest fileRequest)
        {
            return;
        }

        var fullPath = _basePath + fileRequest.Filename;

        Console.WriteLine($"fullPath: {fullPath}");
        
        if (!File.Exists(fullPath))
        {
            Console.WriteLine("File not found");

            return;
        }

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
        var buffer = new byte[DatagramSize];
        var chunkNumber = 0;

        while (true)
        {
            Console.WriteLine($"Loop: {fileStream.Position} of {fileStream.Length}");
            
            if (fileStream.Position == fileStream.Length)
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

            var read = fileStream.Read(buffer, offset + sizeof(int), DatagramSize - sizeof(int) * 2);
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

    static TcpServer()
    {
        _endMessage = new byte[byte.MaxValue];
        Array.Fill(_endMessage, byte.MaxValue);
    }

    private static string GetNormalizedFileName(byte[] bytes) => Encoding.UTF8.GetString(bytes.TakeWhile(b => b != 0).ToArray());
    
}