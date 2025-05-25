using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class Order
{
    public string Name { get; set; }
    public string Dish { get; set; }
}

class Program
{
    static async Task Main()
    {
        Console.Title = "CLIENT";
        Console.OutputEncoding = Encoding.UTF8;

        var client = new UdpClient();
        var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);

        Console.Write("Введите ваше имя: ");
        string name = Console.ReadLine();

        while (true)
        {
            Console.Write("Введите название блюда (или 'exit'): ");
            string dish = Console.ReadLine();
            if (dish.ToLower() == "exit") break;

            var order = new Order { Name = name, Dish = dish };
            string json = JsonSerializer.Serialize(order);
            byte[] data = Encoding.UTF8.GetBytes(json);

            await client.SendAsync(data, data.Length, serverEndPoint);

            var result1 = await client.ReceiveAsync();
            string queueInfo = Encoding.UTF8.GetString(result1.Buffer);
            Console.WriteLine("\n" + queueInfo);

            var result2 = await client.ReceiveAsync();
            string response = Encoding.UTF8.GetString(result2.Buffer);
            Console.WriteLine("\n" + response);
            Console.WriteLine();
        }

        Console.WriteLine("Вы вышли.");
    }
}
