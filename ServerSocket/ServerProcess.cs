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
    using HenE.ConnectionHelper;
    using HenE.GameVierOpEenRij;
    using HenE.VierOPEenRij.Enum;
    using HenE.VierOPEenRij.Interface;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// class om nieuwe server te creëren.
    /// stuurt mesaage naar de cliet.
    /// Ontvangt mesaage vanuit de cliet.
    /// </summary>
    public class ServerProcess : Communicate
    {
        private readonly byte[] buffer = new byte[1000];
        private Socket serverSocket = null;
        private List<Socket> clienten = new List<Socket>();
        private ICanHandelen handler = new Handler();

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

            // Bind de server met de client.
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));

            // De hoeveel client mag de server ontvangen.
            this.serverSocket.Listen(100);

            // begin hier de server de cliënt accepteren.
            this.serverSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.serverSocket);
        }

        /// <summary>
        /// behanelt het berichtje die uit een cleint wordt gestuurd.
        /// </summary>
        /// <param name="message">De bericht.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            if (message == string.Empty)
            {
                throw new ArgumentException("Mag niet messagen empty zijn.");
            }
            else if (socket == null)
            {
                throw new ArgumentNullException("Mag niet een client null zijn.");
            }

            string[] opgeknipt = message.Split(new char[] { '%' });

            // change the string to enum.
            Commandos commando = EnumHelperl.EnumConvert<Commandos>(opgeknipt[0]);
            switch (commando)
            {
                case Commandos.VerzoekTotDeelnemenSpel:
                    // this.Send(socket, message);
                   this.Send(socket, this.handler.StreamOntvanger(message, socket));
                   break;
                case Commandos.SpeelTegenComputer:
                    this.handler.StreamOntvanger(message, socket);
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

            // Get the client.
            Socket socket = (Socket)ar.AsyncState;

            // Read data from the client socket.
            int bytesRead = socket.EndReceive(ar);

            // start read.
            if (bytesRead > 0)
            {
                content = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);
                if (content.IndexOf(content) > -1)
                {
                    // All the data has been read from the
                    // client.
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

            // Voeg de nieuwe client in.
            this.clienten.Add(socket);
            Console.WriteLine("Client connected.");

            // wacht op andere bericht.
            socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
            this.serverSocket.BeginAccept(this.AcceptCallback, null);
        }
    }
}
