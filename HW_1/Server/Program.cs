using System.Net; 
using System.Net.Sockets; 
using System.Text; 

class Server 
{
    private const int DEFAULT_BUFLEN = 512; 
    
    private const string DEFAULT_PORT = "27015"; 
    private const int PAUSE = 1000; 

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8; 
        Console.Title = "SERVER SIDE";
        Console.WriteLine("Процесс сервера запущен!");
        Thread.Sleep(PAUSE);

        try
        {
            var ipAddress = IPAddress.Any; 
            var localEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT)); 

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint); 

            Console.WriteLine("Получение адреса и порта сервера прошло успешно!");
            Thread.Sleep(PAUSE);

            listener.Listen(10); 
            Console.WriteLine("Начинается прослушивание информации от клиента.\nПожалуйста, запустите клиентскую программу!");

            var clientSocket = listener.Accept(); 
            Console.WriteLine("Подключение с клиентской программой установлено успешно!");

            listener.Close(); 
            
            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
            int bytesReceived = clientSocket.Receive(buffer);
            if (bytesReceived == 0)
            {
                Console.WriteLine("Клиент отключился.");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Получено: {message}");

            if (int.TryParse(message, out int num))
            {
                int result = num + 1;
                byte[] response = Encoding.UTF8.GetBytes(result.ToString());
                clientSocket.Send(response);
                Console.WriteLine($"Отправлено: {result}");
            }
            else
            {
                byte[] response = Encoding.UTF8.GetBytes("Ошибка: не число");
                clientSocket.Send(response);
            }
            }

            clientSocket.Shutdown(SocketShutdown.Send);
            clientSocket.Close(); 
            Console.WriteLine("Процесс сервера завершает свою работу!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}