namespace TCP_Server;

internal class Chunk
{
    public required byte[] Buffer { get; init; }
    public required int ActualLength { get; init; }
}