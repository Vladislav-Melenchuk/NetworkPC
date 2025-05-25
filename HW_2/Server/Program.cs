using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

class Program
{
    private static TcpListener listener;
    private static bool isRunning = false;
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main()
    {
        listener = new TcpListener(IPAddress.Any, 27015);
        listener.Start();
        isRunning = true;
        Console.WriteLine("Сервер запущен.");

        try
        {
            while (isRunning)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }
        catch (SocketException)
        {
            
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        Console.WriteLine("Клиент подключился.");
        var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"Получен запрос: {request}");

                string response = await ProcessRequestAsync(request);

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка с клиентом: " + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Клиент отключился.");
        }
    }

    private static async Task<string> ProcessRequestAsync(string request)
    {
        if (request.Equals("time", StringComparison.OrdinalIgnoreCase))
            return DateTime.Now.ToLongTimeString();

        if (request.Equals("date", StringComparison.OrdinalIgnoreCase))
            return DateTime.Now.ToShortDateString();

        if (request.Equals("eur", StringComparison.OrdinalIgnoreCase))
            return await GetEuroRateAsync();

        if (request.Equals("btc", StringComparison.OrdinalIgnoreCase))
            return await GetBitcoinPriceAsync();

        return "Неизвестная команда";
    }

    private static Task<string> GetEuroRateAsync()
    {
        
        return Task.FromResult("Курс евро: 46.99 грн");
    }


    private static async Task<string> GetBitcoinPriceAsync()
    {
        try
        {
            string url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=uah";
            var response = await httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);

            if (doc.RootElement.TryGetProperty("bitcoin", out var bitcoin) &&
                bitcoin.TryGetProperty("uah", out var price))
            {
                var btcPrice = price.GetDecimal();
                return $"Курс биткоина: {btcPrice} грн";
            }
            else
            {
                return "Не удалось получить курс биткоина";
            }
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

}
