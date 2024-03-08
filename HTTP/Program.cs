using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using HTTP;

var httpClient = new HttpClient()
{
    DefaultRequestVersion = HttpVersion.Version20
};

string action = Console.ReadLine()!;

switch (action)
{
    case "GET":
    {
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
        
        break;
    }

    case "POST":
    {
        var product = new Product
        {
            Quantity = 15,
            Name = "Xiyar"
        };

        var json = JsonSerializer.Serialize(product);
        var content = new StringContent(json, Encoding.UTF8);
        
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var responseMessage = await httpClient.PostAsync(@"http://localhost:5014/product", content);

        Console.WriteLine($"StatusCode: {responseMessage.StatusCode}");
        
        break;
    }

    case "PUT":
    {
        var product = new Product
        {
            Quantity = 0,
            Name = "BadFish"
        };
        var json = JsonSerializer.Serialize(product);
        var content = new StringContent(json, Encoding.UTF8);

        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        var response = await httpClient.PutAsync(@$"http://localhost:5014/product/3", content);
        
        Console.WriteLine($"StatusCode: {(int)response.StatusCode}");
        
        break;
    }
    case "DELETE":
    {
        var response = await httpClient.DeleteAsync(@"http://localhost:5014/product/3");
        
        Console.WriteLine($"StatusCode: {(int)response.StatusCode}");
        
        break;
    }
}



// shop.com - site
// shop-api.com - webapi
// https://127.0.0.1/Users/Add; POST // C
// https://127.0.0.1/Users/2; GET // R
// https://127.0.0.1/Users/Update/3; PUT // U
// https://127.0.0.1/Users/Delete/3; DELETE // D