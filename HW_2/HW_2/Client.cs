using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client : IDisposable
{
    private TcpClient tcpClient;
    private NetworkStream stream;

    public async Task<bool> ConnectAsync(string host = "127.0.0.1", int port = 27015)
    {
        try
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(host, port);
            stream = tcpClient.GetStream();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> SendRequestAsync(string message)
    {
        if (tcpClient == null || !tcpClient.Connected)
            throw new InvalidOperationException("Не подключен к серверу");

        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);

        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    public void Dispose()
    {
        stream?.Close();
        tcpClient?.Close();
    }
}
