using System;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Maakt verbinding tussen de cliënt en de server.
/// </summary>
namespace ConnectionHelper
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class Conaction
    {
        public abstract void ProcessStream(string message, Socket socket);
        public void OntvangEenMessage(Socket socket)
        {
            // ontvang het berichtje als string.
            byte[] receivedBuffer = new byte[1024];
            int rec = socket.Receive(receivedBuffer);

            // change the message to string.
            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);
            Console.WriteLine("Received" + Encoding.ASCII.GetString(data));
            ProcessStream(Encoding.ASCII.GetString(data), socket);
        }
    }
}
