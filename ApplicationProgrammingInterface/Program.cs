using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

// var client = new WebClient();

// try
// {
//     var response = client.DownloadString(
//         "https://api.openweathermap.org/data/2.5/weather?q=Baku&appid={apiKey}");
//
//     var data = JsonSerializer.Deserialize<WeatherData>(response);
//
//     
//     // (32°F − 32) × 5/9
//     // Console.WriteLine((data.Main.Temp - 32)  / 1.8);
//     Console.WriteLine(data.Main.Temp - 273.15);
//     Console.WriteLine(data.Sys.Country);
//     Console.WriteLine(data.Sys.Sunrise);
//     Console.WriteLine(data.Sys.Sunset);
// }
// catch (WebException ex)
// {
//     Console.WriteLine(ex.Message);
// }

var httpClient = new HttpClient();
var result = httpClient.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=Baku&appid={apiKey}").GetAwaiter().GetResult();
var data = result.Content.ReadFromJsonAsync<WeatherData>().GetAwaiter().GetResult();

Console.WriteLine(data.Main.Temp - 273.15);