using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main()
    {
        string apiKey = "9f246c9c94e1449592185841252605";

        Console.Write("Введите название города: ");
        string city = Console.ReadLine();

        var client = new RestClient("https://api.weatherapi.com");

        var request = new RestRequest($"/v1/forecast.json", Method.Get);
        request.AddHeader("User-Agent", "RestSharpClient");
        request.AddParameter("key", apiKey);
        request.AddParameter("q", city);
        request.AddParameter("days", "2");

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            Console.WriteLine("Ошибка запроса: " + response.StatusCode);
            Console.WriteLine("Проверьте правильность названия города.");
            return;
        }

        var data = JObject.Parse(response.Content);

        Console.WriteLine($"\n=== Погода в {city} ===");

        
        var current = data["current"];
        Console.WriteLine("\nСегодня:");
        Console.WriteLine($"Температура: {current["temp_c"]}°C");
        Console.WriteLine($"Состояние: {current["condition"]["text"]}");
        Console.WriteLine($"Влажность: {current["humidity"]}%");
        Console.WriteLine($"Скорость ветра: {current["wind_kph"]} км/ч");

        var forecastDay = data["forecast"]["forecastday"][1]; 
        var day = forecastDay["day"];

        Console.WriteLine("\nЗавтра:");
        Console.WriteLine($"Температура: {day["avgtemp_c"]}°C");
        Console.WriteLine($"Состояние: {day["condition"]["text"]}");
        Console.WriteLine($"Влажность: {day["avghumidity"]}%");
        Console.WriteLine($"Скорость ветра: {day["maxwind_kph"]} км/ч");
    }
}
