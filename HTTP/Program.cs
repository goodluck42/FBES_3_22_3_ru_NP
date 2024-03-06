

using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

var httpClient = new HttpClient()
{
    DefaultRequestVersion = HttpVersion.Version20
};

var response = await httpClient.GetAsync(@"http://localhost:5014/product");

using var streamReader = new StreamReader(response.Content.ReadAsStream());


var products = JsonSerializer.Deserialize<Product[]>(streamReader.ReadToEnd());

foreach (var product in products!)
{
    Console.WriteLine(product.Id);
    Console.WriteLine(product.Quantity);
    Console.WriteLine(product.Name);
    Console.WriteLine("-----------------");
}


// https://127.0.0.1/Users/Add; POST // C
// https://127.0.0.1/Users/2; GET // R
// https://127.0.0.1/Users/Update/3; PUT // U
// https://127.0.0.1/Users/Delete/3; DELETE // D


class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}