// <copyright file="ServerProcess.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HenE.Games.VierOpEenRij.ConnectionHelper;
    using HenE.Games.VierOpEenRij.Container;
    using HenE.Games.VierOpEenRij.Enum;
    using HenE.Games.VierOpEenRij.Interface;
    using HenE.Games.VierOpEenRij.Protocol;

    /// <summary>
    /// class om nieuwe server te creëren.
    /// stuurt mesaage naar de cliet.
    /// Ontvangt mesaage vanuit de cliet.
    /// </summary>
    public class ServerProcess : Communicate
    {
        private readonly byte[] buffer = new byte[10000];
        private readonly Socket serverSocket = null;
        private readonly List<Socket> clienten = new List<Socket>();
        private readonly CanHandelen handler = new Handler();
        private readonly bool connected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerProcess"/> class.
        /// </summary>
        /// <param name="server">De server.</param>
        public ServerProcess(Socket server)
        {
            this.serverSocket = server;
            this.connected = true;
        }

        /// <summary>
        /// Start de server.
        /// Ontvang een cleint.
        /// </summary>
        public void SetupServer()
        {
            // Bind de server met de client.
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));

            // De hoeveel client mag de server ontvangen.
            this.serverSocket.Listen(2);

            // begint hier de server de cliënt accepteren.
            this.serverSocket.BeginAccept(this.AcceptCallback, null);
            TimeSpan timeout = new TimeSpan(0, 1, 0);
            while (this.connected)
            {
                // Wacht 5 minuten voor dat Gaat door.
                Thread.Sleep(timeout);
            }
        }

        /// <summary>
        /// behanelt het berichtje die uit een cleint wordt gestuurd.
        /// </summary>
        /// <param name="message">De bericht.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            try
            {
                if (message == string.Empty)
                {
                    throw new ArgumentException("Mag niet message empty zijn.");
                }

                if (socket == null)
                {
                    throw new ArgumentException("Mag niet een client null zijn.");
                }

                Game game;

                // [0] is altijd de commando.
                // [1.....] is de rest van de bericht.
                string[] opgeknipt = message.Split(new char[] { '%' });

                // change the string to enum.
                Commandos commando = EnumHelperl.EnumConvert<Commandos>(opgeknipt[0]);
                switch (commando)
                {
                    case Commandos.VerzoekTotDeelnemenSpel:

                        this.Send(socket, this.handler.StreamOntvanger(message, socket));
                        break;

                        // Geef een teken aan een speler.
                        // en geef de andere speler de andere teken.
                    case Commandos.ZetTeken:
                        string msg = string.Empty;
                        Teken teken = EnumHelperl.EnumConvert<Teken>(opgeknipt[1].ToString());
                        game = this.GetGame(socket);
                        game.TekenenBehandl(socket, teken);
                        msg = $"{Events.TegenSpelerHeeftTekenIngezet.ToString()}%%{opgeknipt[1]}%%{game.TegenSpeler(game.GetSpelerViaTcp(socket)).GebruikTeken.ToString()}";
                        this.SendBerichtNaarDeTegenSpeler(game, msg, socket);
                        Thread.Sleep(1000);
                        this.Send(socket, Events.TekenIngezet.ToString());
                        break;

                        // start het spel.
                    case Commandos.Starten:
                        this.GetGame(socket).StartHetSpel();
                        break;

                        // doe zet op het speelvlak.
                    case Commandos.DoeZet:
                        game = this.GetGame(socket);
                        if (game != null)
                        {
                            // eerst gaat het spel de inzet van de speler zetten.
                            // dan gaat de controller dat nummertje uit de properties speler op halen.
                            game.DoeInzet(opgeknipt[1], socket);
                            Speler speler = game.GetSpelerViaTcp(socket);
                            game.GameController.DoeInzet(speler);
                        }

                        break;

                    case Commandos.NieuwRonde:
                        game = this.GetGame(socket);
                        if (game != null)
                        {
                            // Als de speler een nieuw rondje wil doen.
                            // Er staat alleen een speler dan de situatie verandert tot wachten op andere speler.
                            if (game.GetSpelers().Count == 1)
                            {
                                game.ZetSituatie(Status.Wachten);
                                Thread.Sleep(500);
                                this.SendBerichtNaarDeTegenSpeler(game, Events.WachtenNieuweSpeler.ToString(), socket);
                            }
                            else
                            {
                                // Als het spel twee spelers heeft dan gaat het door.
                                game.GameController.NieuwRonde();
                            }
                        }

                        break;
                    case Commandos.WilNietNieuweRonde:
                        // neem de game.
                        game = this.GetGame(socket);
                        if (game != null)
                        {
                                this.VerWijdertHetSpelMetSpeller(socket);
                        }

                        socket.Close();
                        this.clienten.Remove(socket);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Read a message form a client.
        /// </summary>
        /// <param name="ar">result.</param>
        public void ReadCallback(IAsyncResult ar)
        {
            // Get the client.
            Socket socket = (Socket)ar.AsyncState;
            string content = string.Empty;
            try
            {
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
                    socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, this.ReadCallback, socket);
                }
            }
            catch (Exception)
            {
                this.VerWijdertHetSpelMetSpeller(socket);
            }
        }

        /// <summary>
        /// Verwijdert het spel en de spelers.
        /// </summary>
        /// <param name="socket">De client van een speler.Die hebben we nodig om de speler te roepen.</param>
        private void VerWijdertHetSpelMetSpeller(Socket socket)
        {
            // Get the play.
            Game game = this.GetGame(socket);
            if (game != null)
            {
                // Stuur een bericht naar de tegen speler.
                Thread.Sleep(1000);
                this.SendBerichtNaarDeTegenSpeler(game, Events.SpelVerwijderd.ToString(), socket);
                game.VerWijdertEenSpeler(game.TegenSpeler(game.GetSpelerViaTcp(socket)));
                game.VerWijdertEenSpeler(game.GetSpelerViaTcp(socket));
                Handler deHandeler = new Handler();
                deHandeler.DeleteGame(game);

                // verwijdert deze cliënt.
                this.clienten.Remove(socket);
            }
        }

        /// <summary>
        /// Accepteert een clietn.
        /// </summary>
        /// <param name="aR">The result.</param>
        private void AcceptCallback(IAsyncResult aR)
        {
            Socket socket = this.serverSocket.EndAccept(aR);

            try
            {
                // Voeg de nieuwe client in.
                this.clienten.Add(socket);

                // wacht op andere bericht.
                socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, this.ReadCallback, socket);
                this.serverSocket.BeginAccept(this.AcceptCallback, null);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Zoek naar een game via de client.
        /// </summary>
        /// <param name="socket">De client van de speler.</param>
        /// <returns>Het spel waar de speler mee bezig zijn.</returns>
        private Game GetGame(Socket socket)
        {
            if (socket == null)
            {
                throw new ArgumentException("Client mag niet null zijn.");
            }

            Handler deHandler = this.handler as Handler;
            foreach (Game game in deHandler.GetSpellen())
            {
                foreach (Speler speler in game.GetSpelers())
                {
                    if (speler.IsHumanSpeler)
                    {
                        HumanSpeler humanSpeler = speler as HumanSpeler;
                        if (humanSpeler.TcpClient == socket)
                        {
                            return game;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Send een berichtje naar de tegen speler.
        /// </summary>
        /// <param name="game">Het huidige spel.</param>
        /// <param name="message">de tekst die naar een speler wordt gestuurd.</param>
        /// <param name="socket">De tcp client van de huidige speler.</param>
        private void SendBerichtNaarDeTegenSpeler(Game game, string message, Socket socket)
        {
            if (game != null)
            {
                Speler speler = game.GetSpelerViaTcp(socket);
                if (speler != null)
                {
                    Speler tegenSpeler = game.TegenSpeler(speler);
                    if (tegenSpeler != null && tegenSpeler.IsHumanSpeler)
                    {
                        HumanSpeler humanSpeler = tegenSpeler as HumanSpeler;
                        this.Send(humanSpeler.TcpClient, message);
                    }
                }
            }
        }
    }
}
