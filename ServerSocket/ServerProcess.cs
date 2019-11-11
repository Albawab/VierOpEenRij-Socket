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
    using HenE.GameVierOpEenRij;
    using HenE.VierOPEenRij.Interface;

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
        ICanHandelen handler = new Handler();

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
            this.serverSocket.Listen(100);
            this.serverSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.serverSocket);
        }

        /// <summary>
        /// behanelt het berichtje die uit een cleint wordt gestuurd.
        /// </summary>
        /// <param name="message">De bericht.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            string[] opgeknipt = message.Split(new char[] { '%' });
            switch (opgeknipt[1])
            {
                case "VerzoekTotDeelnemenSpel":
                   // this.Send(socket, message);
                    this.handler.StreamOntvanger(message);
                    break;
                case "Lolo":
                    Console.WriteLine("Lolo");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Read a message form a client.
        /// </summary>
        /// <param name="ar">result.</param>
        public void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;

            Socket socket = (Socket)ar.AsyncState;

            // Read data from the client socket.
            int bytesRead = socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                content = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);
                if (content.IndexOf(content) > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.
                    this.ProcessStream(content, socket);
                }

                // Not all data received. Get more.
                socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
            }
        }

        /// <summary>
        /// Accepteert een clietn.
        /// </summary>
        /// <param name="aR">The result.</param>
        private void AcceptCallback(IAsyncResult aR)
        {
            Socket socket = this.serverSocket.EndAccept(aR);
            this.clienten.Add(socket);
            Console.WriteLine("Client connected.");

            socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
            this.serverSocket.BeginAccept(this.AcceptCallback, null);
        }

        /// <summary>
        /// Stuurt een bericht naar een client.
        /// </summary>
        /// <param name="handler">De client.</param>
        /// <param name="data">Het berichtje.</param>
        private void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(this.SendCallback), handler);
        }

        /// <summary>
        /// Laat weten dat de client heeft een bericht ontvang.
        /// </summary>
        /// <param name="ar">the result.</param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the object.
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
    }
}
