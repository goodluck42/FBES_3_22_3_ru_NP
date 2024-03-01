using System.Net;
using System.Net.Sockets;
using System.Text;

using var loader = new FileLoader("10.0.0.198", 13375);

await loader.RequestFile("4kwallpaper.jpg");

Console.Read();

class FileLoader : IDisposable
{
    private FileStream? _stream;
    private readonly UdpClient _udpClient;

    public FileLoader(string host, short port)
    {
        _udpClient = new UdpClient();
        _udpClient.Connect(IPEndPoint.Parse($"{host}:{port}"));
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
        _stream?.Dispose();
    }

    public async Task<bool> RequestFile(string filename)
    {
        _stream = new FileStream(GetSafeFilename(filename), FileMode.OpenOrCreate, FileAccess.Write);
        var fileNameBytes = Encoding.UTF8.GetBytes(filename);
        var message = new byte[byte.MaxValue];

        Array.Copy(fileNameBytes, message, fileNameBytes.Length);

        await _udpClient.SendAsync(message);

        return await Task.Run(LoadFile);
    }

    private async Task<bool> LoadFile()
    {
        bool firstBreak = true;

        int it = 0;
        
        while (true)
        {
            it++;
            var message = await _udpClient.ReceiveAsync();

            if (IsEndMessage(message.Buffer))
            {
                await _stream!.FlushAsync();
                await _stream!.DisposeAsync();

                Console.WriteLine($"Loop broken: {it}");
                
                return !firstBreak;
            }

            firstBreak = false;
            
            Console.WriteLine($"Message Length: {message.Buffer.Length}");
            Console.WriteLine($"Chunk Number: {BitConverter.ToInt32(message.Buffer, 0)}");
            Console.WriteLine($"Chunk Size: {BitConverter.ToInt32(message.Buffer, 4)}");
        }
    }

    private static bool IsEndMessage(byte[] buffer)
    {
        return buffer.Length == byte.MaxValue && buffer.All(x => x == byte.MaxValue);
    }

    private static string GetSafeFilename(string filename)
    {
        return
            $"{filename}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}_{Guid.NewGuid():N}";
    }
}