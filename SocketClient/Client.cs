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
    using HenE.ConnectionHelper;
    using HenE.GameVOPR.Protocol;
    using HenE.VierOPEenRij.Enum;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// class om nieuwe client te creëren.
    /// stuurt mesaage naar de server.
    /// Ontvangt mesaage vanuit de server.
    /// </summary>
    public class Client : Communicate
    {
        private readonly byte[] buffer = new byte[1000];
        private Socket clientSocket = null;
        private int dimension = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="client">Een nieuwe client.</param>
        public Client(Socket client)
        {
            this.clientSocket = client;
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
                catch (SocketException e)
                {
                    // Als de cliént niet met de server kan aansluiten.
                    Console.Clear();
                    Console.WriteLine(e.ToString() + attempts.ToString());
                }
            }
        }

        /// <summary>
        /// Handelt de bericht die uit de server komt.
        /// </summary>
        /// <param name="message">het berichtje die uit de server komt.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            // [0] is altijd de event.
            // [1] De naam van de speler.
            // [2] Het berichtje.
            string[] opgeknipt = message.Split(new char[] { '%' });

            // Change this sting to event.
            Events events = EnumHelperl.EnumConvert<Events>(opgeknipt[0]);

            switch (events)
            {
                case Events.Gecreeerd:
                    if (this.WilTegenComputerSpeln())
                    {
                        this.SpeelTegenComputerCommando();
                    }
                    else
                    {
                        Console.WriteLine("Je Moet op andere speler wachten.");
                    }

                    break;

                case Events.SpelerInvoegde:
                    Console.WriteLine();
                    Console.WriteLine("Nu mag je je teken te kiezen.");
                    string teken = this.VraagOmEigenTekenTeKiezen();
                    this.ZetTekenCommando(teken);
                    break;

                case Events.TekenIngezet:
                    Console.WriteLine();
                    Console.WriteLine("we kunnen starten.");
                    this.StartHetSperCommando();
                    break;
                case Events.BordGetekend:
                    Console.WriteLine();
                    Console.WriteLine($"{opgeknipt[1]}");
                    Thread.Sleep(1000);
                    Console.WriteLine($"{opgeknipt[2]}");
                    break;
            }
        }

        /// <summary>
        /// stuurt een berichtje naar de server dat de speler wil spelen.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        /// <param name="dimension">De dimension van het speelvlak.</param>
        /// <param name="socket">De client.</param>
        public void VerzoekOmStartenSpel(string naam, int dimension, Socket socket)
        {
            this.dimension = dimension;
            string message = CommandoHelper.CreëertVerzoekTotDeelnameSpelCommando(naam, dimension);
            this.Send(this.clientSocket, message);
        }

        /// <summary>
        /// Stuurt een bericht naar de server dat de speler wil tegen de computer spelen.
        /// </summary>
        private void SpeelTegenComputerCommando()
        {
            string message = CommandoHelper.CreeertSpeelTegenComputerCommando(this.dimension);
            this.Send(this.clientSocket, message);
        }

        /// <summary>
        /// Stuurt een bericht naar de server er in staat de commando met de teken die de speler heeft gekozen.
        /// </summary>
        /// <param name="teken">De teken die de speler heeft gekozen.</param>
        private void ZetTekenCommando(string teken)
        {
            string message = CommandoHelper.CreeertZetTekenCommando(teken);
            this.Send(this.clientSocket, message);
        }

        private void StartHetSperCommando()
        {
            string msg = CommandoHelper.CreeertStartHetSpelCommando();
            this.Send(this.clientSocket, msg);
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

        /// <summary>
        /// Read the data.
        /// </summary>
        /// <param name="ar">Result ar.</param>
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Wachten op een nieuwe bericht.
            this.clientSocket.BeginReceive(this.buffer, 0, this.buffer.Length, 0, new AsyncCallback(this.ReceiveCallback), this.clientSocket);
        }

        private bool WilTegenComputerSpeln()
        {
            Console.WriteLine("Wil je tegen de computer speler J of N?");
            if (this.ThisCheckAnswer())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Controleert of het anworrd geldig of ongeldig.
        /// </summary>
        /// <returns>Is het antwoord geldig of niet.</returns>
        private bool ThisCheckAnswer()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.J && keyInfo.Key != ConsoleKey.N)
            {
                Console.WriteLine("Je mag alleen j of N gebruiken.");
                keyInfo = Console.ReadKey();
            }

            if (keyInfo.Key == ConsoleKey.J)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stelt een vraag aan de speler om een teken te kiezen.
        /// </summary>
        /// <returns>Wat de speler heeft van teken gekozen.</returns>
        private string VraagOmEigenTekenTeKiezen()
        {
            Console.WriteLine("Welk teken wil je gebruiken O of X?");
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.O && keyInfo.Key != ConsoleKey.X)
            {
                Console.WriteLine("Je mag alleen O of X gebruiken.");
                keyInfo = Console.ReadKey();
            }

            return keyInfo.Key.ToString();
        }
    }
}
