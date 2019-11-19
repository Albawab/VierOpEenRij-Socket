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
    using System.Threading;
    using HenE.ConnectionHelper;
    using HenE.GameVierOpEenRij;
    using HenE.VierOPEenRij;
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
        private readonly byte[] buffer = new byte[10000];
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

            Game game = null;

            // [0] is altijd de commando.
            string[] opgeknipt = message.Split(new char[] { '%' });

            // change the string to enum.
            Commandos commando = EnumHelperl.EnumConvert<Commandos>(opgeknipt[0]);
            switch (commando)
            {
                case Commandos.VerzoekTotDeelnemenSpel:
                    // this.Send(socket, message);
                    this.Send(socket, this.handler.StreamOntvanger(message, socket));
                    break;

                case Commandos.ZetTeken:
                    Teken teken = EnumHelperl.EnumConvert<Teken>(opgeknipt[1].ToString());
                    this.GetGame(socket).TekenenBehandler(socket, teken);
                    this.Send(socket, Events.TekenIngezet.ToString());
                    break;

                case Commandos.Starten:
                    this.GetGame(socket).StartHetSpel();
                    break;
                case Commandos.DoeZet:
                    game = this.GetGame(socket);
                    game.DoeInzet(opgeknipt[1], socket);
                    Speler speler = game.GetSpelerViaTcp(socket);
                    game.GameController.DoeInzet(speler);
                    break;
                default:
                    break;

                case Commandos.NieuwRonde:
                    game = this.GetGame(socket);
                    game.GameController.NieuwRonde();

                    break;
                case Commandos.WilNietNieuweRonde:
                    // neem de game.
                    game = this.GetGame(socket);
                    if (game.GetSpelers().Count == 1)
                    {
                        Handler handler = new Handler();
                        handler.DeleteGame(game);
                    }
                    else
                    {
                        // stuur een bericht naar de tegen speler dat deze speler wil niet meer spelen.
                        this.SendBerichtNaarDeTegenSpeler(game, Events.TegenSpelerVerlaten.ToString(), socket);

                        // verwijder de speler.
                        game.VerWijdertEenSpeler(socket);

                        // Zet het situatie van het spel op een speler wachten.
                        game.ZetSitauatie(Status.Wachten);
                    }

                    socket.Close();
                    this.clienten.Remove(socket);
                    break;
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
                    socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
                }
            }
            catch (Exception)
            {
                // Get the play.
                Game game = this.GetGame(socket);
                if (game != null)
                {
                    // Stuur een bericht naar de tegen speler.
                    // this.SendBerichtNaarDeTegenSpeler(game, Events.TegenSpelerVerlaten.ToString(), socket);
                    Thread.Sleep(1000);
                    this.SendBerichtNaarDeTegenSpeler(game, Events.SpelVerwijderd.ToString(), socket);
                    game.VerWijdertEenSpeler(socket);
                    Handler handler = new Handler();
                    handler.DeleteGame(game);

                    // verwijdert deze cliënt.
                    this.clienten.Remove(socket);
                }
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
                socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.ReadCallback), socket);
                this.serverSocket.BeginAccept(this.AcceptCallback, null);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                throw new ArgumentNullException("Client mag niet null zijn.");
            }

            Handler handler = this.handler as Handler;
            foreach (Game game in handler.GetSpellen())
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
        /// <param name="socket">De tcp client van de huidige speler.</param>
        private void SendBerichtNaarDeTegenSpeler(Game game, string events, Socket socket)
        {
            if (game != null)
            {
                Speler speler = game.GetSpelerViaTcp(socket);
                if (speler != null)
                {
                    Speler tegenSpeler = game.TegenSpeler(speler);
                    if (tegenSpeler != null)
                    {
                        if (tegenSpeler.IsHumanSpeler)
                        {
                            HumanSpeler humanSpeler = tegenSpeler as HumanSpeler;
                            this.Send(humanSpeler.TcpClient, events);
                        }
                    }
                }
            }
        }
    }
}
