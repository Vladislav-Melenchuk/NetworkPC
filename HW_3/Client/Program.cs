using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static readonly string[] Currencies = { "USD", "EURO", "UAH", "YEN" };

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Client";

        using TcpClient client = new();
        try
        {
            await client.ConnectAsync("127.0.0.1", 27015);
        }
        catch
        {
            Console.WriteLine("Не удалось подключиться к серверу.");
            return;
        }

        NetworkStream stream = client.GetStream();

        Console.WriteLine("Клиент курсов валют");

        while (true)
        {
            Console.WriteLine("\nВыберите исходную валюту:");
            PrintCurrencyMenu();
            int from = ReadCurrencyIndex();

            Console.WriteLine("\nВыберите целевую валюту:");
            PrintCurrencyMenu();
            int to = ReadCurrencyIndex();

            if (from == to)
            {
                Console.WriteLine("Нельзя конвертировать валюту");
                continue;
            }

            string query = $"{Currencies[from]} {Currencies[to]}";

            byte[] msg = Encoding.UTF8.GetBytes(query);
            await stream.WriteAsync(msg, 0, msg.Length);

            byte[] buffer = new byte[512];
            int bytesRead = await stream.ReadAsync(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Ответ от сервера: " + response);

            if (response.StartsWith("Превышен лимит"))
                break;

            Console.Write("\nПродолжить? (y/n): ");
            string cont = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (cont != "y") break;
        }

        Console.WriteLine("Отключение от сервера...");
    }

    static void PrintCurrencyMenu()
    {
        for (int i = 0; i < Currencies.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {Currencies[i]}");
        }
    }

    static int ReadCurrencyIndex()
    {
        while (true)
        {
            Console.Write("Введите номер валюты: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int index) && index >= 1 && index <= Currencies.Length)
                return index - 1;

            Console.WriteLine("Неверный ввод. Введите число от 1 до " + Currencies.Length);
        }
    }
}
