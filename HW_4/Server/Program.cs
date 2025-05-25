using System.Collections.Concurrent;
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
    private static UdpClient server;
    private static int port = 9000;
    private static ConcurrentQueue<(Order Order, IPEndPoint Client, DateTime ReceivedTime)> orders = new();
    private static List<string> orderLog = new();
    private static Random random = new();

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        server = new UdpClient(port);
        Console.Title = "SERVER";
        Console.WriteLine($"Сервер запущен на порту {port}");

        _ = Task.Run(ReceiveMessagesAsync);
        _ = Task.Run(ProcessOrdersAsync);

        await Task.Delay(-1);
    }

    private static async Task ReceiveMessagesAsync()
    {
        while (true)
        {
            var result = await server.ReceiveAsync();
            var message = Encoding.UTF8.GetString(result.Buffer);

            try
            {
                var order = JsonSerializer.Deserialize<Order>(message);
                if (order != null)
                {
                    orders.Enqueue((order, result.RemoteEndPoint, DateTime.Now));

                    string logEntry = $"{DateTime.Now:HH:mm:ss} {order.Name} заказал: {order.Dish}";
                    orderLog.Add(logEntry);
                    Console.WriteLine(logEntry);

                    var queueText = string.Join("\n", orderLog);
                    var info = $"Текущая очередь заказов:\n{queueText}\nОжидайте...";
                    byte[] queueData = Encoding.UTF8.GetBytes(info);
                    await server.SendAsync(queueData, queueData.Length, result.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static async Task ProcessOrdersAsync()
    {
        while (true)
        {
            if (orders.TryDequeue(out var item))
            {
                int prepTime = random.Next(10, 20);
                await Task.Delay(prepTime * 1000);

                var response = $"Ваша еда '{item.Order.Dish}' готова за {prepTime} сек. Приятного аппетита!";
                var responseData = Encoding.UTF8.GetBytes(response);
                await server.SendAsync(responseData, responseData.Length, item.Client);

                var elapsed = DateTime.Now - item.ReceivedTime;
                Console.WriteLine($"{item.Order.Name} получил '{item.Order.Dish}' спустя {elapsed.TotalSeconds:F1} сек.");
            }
            else
            {
                await Task.Delay(500);
            }
        }
    }
}
