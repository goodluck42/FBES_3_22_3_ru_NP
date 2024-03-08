using System.Text.Json.Serialization;

namespace HTTP;

class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}