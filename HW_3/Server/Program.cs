using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static Dictionary<string, double> rates = new()
    {
        ["USD_EURO"] = 0.92,
        ["EURO_USD"] = 1.09,

        ["USD_UAH"] = 39.1,
        ["UAH_USD"] = 0.026,

        ["EURO_UAH"] = 42.5,
        ["UAH_EURO"] = 0.023,

        ["EURO_UAH"] = 42.5,
        ["UAH_EURO"] = 0.023,

        ["YEN_USD"] = 0.0064,
        ["USD_YEN"] = 156.4,

        ["EURO_YEN"] = 170.2,
        ["YEN_EURO"] = 0.0059,

        ["UAH_YEN"] = 4.0,
        ["YEN_UAH"] = 0.25
    };

    static ConcurrentDictionary<string, (int Count, DateTime LastRequest)> clients = new();
    const int MAX_REQUESTS = 5;
    static TimeSpan COOLDOWN = TimeSpan.FromMinutes(1);

    static async Task Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 27015);
        listener.Start();
        Console.WriteLine("Сервер запущен на порту 27015");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        string ip = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
        Console.WriteLine($"[{DateTime.Now}] Подключение от {ip}");

        var stream = client.GetStream();
        byte[] buffer = new byte[512];

        while (client.Connected)
        {
            int bytesRead;
            try
            {
                bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;
            }
            catch { break; }

            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"[{DateTime.Now}] Запрос от {ip}: {msg}");

            if (!clients.TryGetValue(ip, out var state)) state = (0, DateTime.MinValue);

            if (state.Count >= MAX_REQUESTS && (DateTime.Now - state.LastRequest) < COOLDOWN)
            {
                await Send(stream, "Превышен лимит запросов. Подождите минуту.");
                continue;
            }

            if (state.Count >= MAX_REQUESTS)
            {
                state = (0, DateTime.MinValue);
            }

            string[] parts = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string response = "error";

            if (parts.Length == 2)
            {
                string from = parts[0].ToUpper();
                string to = parts[1].ToUpper();
                string key = $"{from}_{to}";

                if (rates.TryGetValue(key, out double rate))
                    response = $"1 {from} = {rate} {to}";
                else
                    response = "Курс не найден.";
            }

            clients[ip] = (state.Count + 1, DateTime.Now);
            await Send(stream, response);
        }

        Console.WriteLine($"[{DateTime.Now}] Клиент {ip} отключился.");
        client.Close();
    }

    static async Task Send(NetworkStream stream, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }
}
