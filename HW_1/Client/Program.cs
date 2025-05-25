using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";

    static void Main()
    {
        Console.Title = "CLIENT SIDE";
        try
        {
            var ipAddress = IPAddress.Loopback; 
            var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(remoteEndPoint); 
            Console.WriteLine("Подключение к серверу установлено.");

            while (true)
            {
                Console.Write("Введите целое число (или 'exit' для выхода): ");
                string input = Console.ReadLine();
                if (input.ToLower() == "exit") break;

                if (!int.TryParse(input, out int number))
                {
                    Console.WriteLine("Ошибка: нужно ввести целое число.");
                    continue;
                }

                byte[] msg = Encoding.UTF8.GetBytes(number.ToString());
                clientSocket.Send(msg);

                byte[] buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                Console.WriteLine($"Ответ сервера: {response}");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}