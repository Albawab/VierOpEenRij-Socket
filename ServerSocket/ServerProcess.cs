// <copyright file="ServerProcess.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ServerSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using ConnectionHelper;
    /// <summary>
    /// class om nieuwe server te creëren.
    /// stuurt mesaage naar de cliet.
    /// Ontvangt mesaage vanuit de cliet.
    /// </summary>
    public class ServerProcess : Conaction
    {
        private readonly byte[] buffer = new byte[1000];
        private Socket serverSocket = null;
        private List<Socket> clienten = new List<Socket>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerProcess"/> class.
        /// </summary>
        /// <param name="server">De server.</param>
        public ServerProcess(Socket server)
        {
            this.serverSocket = server;
        }

        /// <summary>
        /// Start de server.
        /// Ontvang een cleint.
        /// </summary>
        public void SetupServer()
        {
            Console.WriteLine("Setting up server....");
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
            this.serverSocket.Listen(3);
            this.serverSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.serverSocket);

        }

        /// <summary>
        /// Accepteert een clietn.
        /// </summary>
        /// <param name="aR">De callback result.</param>
        private void AcceptCallback(IAsyncResult aR)
        {
            Socket socket = this.serverSocket.EndAccept(aR);
            this.clienten.Add(socket);
            Console.WriteLine("Client connected.");

            socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;

            Socket socket = (Socket)ar.AsyncState;

            // Read data from the client socket.
            int bytesRead = socket.EndReceive(ar);

            if (bytesRead > 0)
            {

                content = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);
                if (content.IndexOf("Hello") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    // Echo the data back to the client.
                    Send(socket, "bye");
                }
                else
                {
                    // Not all data received. Get more.
                    socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
                }
            }
        }

        private static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

/*                handler.Shutdown(SocketShutdown.Both);
                handler.Close();*/

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override void ProcessStream(string message, Socket socket)
        {
            string[] opgeknipt = message.Split(new char[] { '%' });
            switch (message)
            {
                case "Hello":
                    Console.WriteLine("Hoi");
                    break;
                default:
                    break;
            }
        }
    }
}
