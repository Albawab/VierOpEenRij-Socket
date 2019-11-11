// <copyright file="Client.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.SocketClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using ConnectionHelper;
    using HenE.GameVOPR.Protocol;

    /// <summary>
    /// class om nieuwe client te creëren.
    /// stuurt mesaage naar de server.
    /// Ontvangt mesaage vanuit de server.
    /// </summary>
    public class Client : Conaction
    {
        private readonly byte[] buffer = new byte[1000];
        private Socket clientSocket = null;
        private ManualResetEvent connectDone =
        new ManualResetEvent(false);

        private ManualResetEvent sendDone =
            new ManualResetEvent(false);

        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="client">Een nieuwe client.</param>
        public Client(Socket client)
        {
            this.clientSocket = client;
        }

        /// <summary>
        /// Stuur een berichtje naar de server.
        /// </summary>
        /// <param name="berichtje">Het berichtje die naar de server wordt gestuurd.</param>
        public void SendMessageToServer(string berichtje)
        {
            // change het berichtje tot een byte array.
            byte[] buffer = Encoding.ASCII.GetBytes(berichtje);

            // stuur het berichtje naar de server.
            this.clientSocket.Send(buffer);
        }

        /// <summary>
        /// Start de client.
        /// </summary>
        public void Connect()
        {
            int attempts = 0;
            while (!this.clientSocket.Connected)
            {
                // probeert de cliënt met de server om te aansluiten.
                try
                {
                    attempts++;

                    this.clientSocket.Connect(IPAddress.Loopback, 5000);
                    this.Receive();
                }
                catch (SocketException)
                {
                    // Als de cliént niet met de server kan aansluiten.
                    Console.Clear();
                    Console.WriteLine("connection attempts :" + attempts.ToString());
                }
            }

            // Als de cliënt met de server wordt geconnect.
            Console.WriteLine("Connected");
        }

        /// <summary>
        /// Handelt de bericht die uit de server komt.
        /// </summary>
        /// <param name="message">het berichtje die uit de server komt.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            switch (message)
            {

            }
        }

        /// <summary>
        /// stuurt een berichtje naar de server dat de speler wil speln.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        /// <param name="dimension">De dimension van het speelvlak.</param>
        /// <param name="socket">De client.</param>
        public void VerzoekOmStartenSpel(string naam, int dimension, Socket socket)
        {
            string message = CommandoHelper.CreëertVerzoekTotDeelnameSpelComando(naam, dimension);
            this.Send(this.clientSocket, message);
        }

        /// <summary>
        /// Stuurt een berichtje naar de server.
        /// </summary>
        /// <param name="client">De client.</param>
        /// <param name="data">De melding.</param>
        public void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(this.SendCallback), client);
        }

        /// <summary>
        /// Ontvangt een bericht.
        /// </summary>
        private void Receive()
        {
            try
            {
                // Begin receiving the data from the remote device.
                this.clientSocket.BeginReceive(this.buffer, 0, this.buffer.Length, 0, new AsyncCallback(this.ReceiveCallback), this.clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Read data from the remote device.
                int bytesRead = this.clientSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    string message = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);
                    this.ProcessStream(message, this.clientSocket);

                    // Get the rest of the data.
                    this.clientSocket.BeginReceive(this.buffer, 0, this.buffer.Length, 0, new AsyncCallback(this.ReceiveCallback), this.clientSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// laat weten dat de server heeft een berichtje ontvangen.
        /// </summary>
        /// <param name="ar">The result.</param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                this.sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
