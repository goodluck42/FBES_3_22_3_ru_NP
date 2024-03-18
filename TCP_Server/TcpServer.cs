using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Server;

internal class TcpServer
{
    private const int PacketSize = 65000;
    private const string DefaultPath = @"C:\Users\Alex\Desktop\";
    private TcpListener? _tcpListener;
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
        _tcpListener?.Dispose();
        _tcpListener = new TcpListener(EndPoint);
        _tcpListener.Start();
        _listeningTask = Task.Factory.StartNew(ListenHandler, TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        if (!_listening)
        {
            return;
        }

        _tcpListener!.Stop();
        _listening = false;
        _resetEvent.Reset();
    }

    public IPEndPoint EndPoint { get; }

    private async Task ListenHandler()
    {
        var buffer = new byte[PacketSize];

        while (_resetEvent.WaitOne())
        {
            TcpClient? tcpClient = null;
            
            try
            {
                tcpClient = await _tcpListener!.AcceptTcpClientAsync();
                var binaryReader = new BinaryReader(tcpClient.GetStream());

                int read = binaryReader.Read(buffer, 0, PacketSize);
                
                if (read <= 0)
                {
                    tcpClient.Dispose();

                    return;
                }
                
                if (read == byte.MaxValue)
                {
                    _ = Task.Factory.StartNew(SendFile, new FileRequest()
                    {
                        Filename = GetNormalizedFileName(buffer),
                        RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint!,
                        TcpClient = tcpClient
                    });
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error: {tcpClient}");
                tcpClient?.Dispose();
                
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
        var binaryWriter = new BinaryWriter(fileRequest.TcpClient.GetStream());
        
        foreach (var chunk in file)
        {
            Console.WriteLine($"chunk.ActualLength: {chunk.ActualLength}");
            
            binaryWriter.Write(chunk.Buffer);
            binaryWriter.Flush();
        }

        var endMessage = GetEndMessage();
        binaryWriter!.Write(endMessage);
        binaryWriter.Flush();
        
        Console.WriteLine("Sent end message!");
    }

    private static IEnumerable<Chunk> ReadFile(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var buffer = new byte[PacketSize];
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

            var read = fileStream.Read(buffer, offset + sizeof(int), PacketSize - sizeof(int) * 2);
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