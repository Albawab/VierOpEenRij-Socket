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
        private readonly byte[] buffer = new byte[10000];
        private Socket clientSocket = null;
        private int dimension = 0;
        private string naam = string.Empty;

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
                }
                catch (SocketException e)
                {
                    // Als de cliént niet met de server kan aansluiten.
                    Console.WriteLine(e.ToString() + attempts.ToString());
                }
            }
        }

        /// <summary>
        /// Laat de kleint door gaan. Mag hiet niet sluiten.
        /// </summary>
        public void RequestLoop()
        {
            this.Receive();
            while (true)
            {
                this.Send(this.clientSocket, string.Empty);
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
            // [3] De naam van de tegen speler.
            string[] opgeknipt = message.Split(new char[] { '%' });

            // Change this sting to event.
            Events events = EnumHelperl.EnumConvert<Events>(opgeknipt[0]);

            switch (events)
            {
                case Events.GecreeerdSpel:
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine("Je Moet op een speler wachten.");
                    break;

                case Events.SpelerInvoegde:
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    Console.WriteLine("Nu mag je je teken kiezen.");
                    Console.WriteLine();
                    string teken = this.VraagOmEigenTekenTeKiezen();
                    this.ZetTekenCommando(teken);
                    break;

                case Events.TekenIngezet:
                    this.StartHetSpelCommando();
                    break;

                case Events.Gestart:
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    ColorConsole.WriteLine(ConsoleColor.Green, "Na twee seconds wordt het spel gestart.");
                    Thread.Sleep(2000);
                    break;
                case Events.BordGetekend:
                    Console.Clear();
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine($"{opgeknipt[2]}");
                    break;
                case Events.JeRol:
                    Console.WriteLine();
                    ColorConsole.WriteLine(ConsoleColor.Green, "Je mag een nummer Kiezen.");
                    int inzet = this.Doezet();
                    this.DoeZetCommando(inzet);
                    break;
                case Events.OngeldigInzet:
                    Console.WriteLine();
                    Console.WriteLine("Deze kolom is vol. Je mag een andere kolom kiezen.");
                    inzet = this.Doezet();
                    this.DoeZetCommando(inzet);
                    break;
                case Events.Wachten:
                    Console.WriteLine($"{opgeknipt[3]} is aan het spelen. je moet op hem/haar nu wachten.");
                    break;
                case Events.HeeftGewonnen:
                case Events.HetBordVolGeworden:
                    if (events == Events.HeeftGewonnen)
                    {
                        Console.WriteLine($"{opgeknipt[1]}: je hebt gewonnen.");
                    }
                    else
                    {
                        Console.WriteLine("Het speelvlak is vol.");
                    }

                    break;

                case Events.HeeftGewonnenTegen:
                    Console.WriteLine($"{opgeknipt[3]} heeft gewonnen.");
                    Thread.Sleep(1000);
                    if (opgeknipt[3] == "Computer")
                    {
                        this.UitVoerNieuwRonde();
                    }
                    else
                    {
                        Console.WriteLine("Wacht op hem/haar.");
                    }

                    break;

                case Events.TegenSpelerVerlaten:
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("De andere speler heeft het spel verlaten.");
                    this.UitVoerNieuwRonde();

                    break;

                case Events.SpelGstopt:
                    Console.WriteLine();
                    this.UitVoerNieuwRonde();

                    break;
                case Events.SpelVerwijderd:
                    Console.WriteLine("Het spel is gestopt!");
                    Console.WriteLine("De speler heeft het spel verlaten. Het spel wordt verwijderd.");
                    if (this.WilNieuweSpel())
                    {
                        this.dimension = this.KrijgDimension();
                        string tegenComputerSpelen = string.Empty;
                        if (this.WilTegenComputerSpeln())
                        {
                            tegenComputerSpelen = "true";
                        }

                        this.VerzoekOmStartenSpel(this.naam, this.dimension, tegenComputerSpelen, this.clientSocket);
                    }
                    else
                    {
                        Console.WriteLine();
                        ColorConsole.WriteLine(ConsoleColor.Red, "Tot ziens!");
                        Thread.Sleep(4000);
                        Environment.Exit(0);
                    }

                    break;
            }
        }

        /// <summary>
        /// Stelt een vraag aan een speler of hij wil tegen computer spelen.
        /// </summary>
        /// <returns>Wil een speler tegen computer spelen of niet.</returns>
        public bool WilTegenComputerSpeln()
        {
            Console.WriteLine("Wil je tegen de computer speler J of N?");
            if (this.ThisCheckAnswer())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// stuurt een berichtje naar de server dat de speler wil spelen.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        /// <param name="dimension">De dimension van het speelvlak.</param>
        /// <param name="wilTegenComputerSpelen">Wil tegen computer spelen of niet.</param>
        /// <param name="socket">De client.</param>
        public void VerzoekOmStartenSpel(string naam, int dimension, string wilTegenComputerSpelen, Socket socket)
        {
            string message = CommandoHelper.CreëertVerzoekTotDeelnameSpelCommando(naam, dimension, wilTegenComputerSpelen);
            this.Send(this.clientSocket, message);
        }

        /// <summary>
        /// Zet waarde die we hebben het nodig.De dimension en de naam.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        public void SetValue(string naam)
        {
            this.naam = naam;
        }

        /// <summary>
        /// Vraag de speler om dimension te geven.
        /// </summary>
        /// <returns>De dimension van het speelvalk.</returns>
        public int KrijgDimension()
        {
            // Vraag de speler om dimension te geven.
            int dimension = 0;
            string antwoord = string.Empty;
            bool isGeldigValue = false;
            do
            {
                Thread.Sleep(2000);
                Console.WriteLine();
                Console.WriteLine("Hoe groot is het speelveld?");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Je mag alleen een cijfer gebruiken. Het cijfer ligt tussen d4 4 en de 10.");
                antwoord = Console.ReadLine();
                if (int.TryParse(antwoord, out dimension))
                {
                    if (dimension > 10)
                    {
                        Console.WriteLine("Het cijfer mag niet hoger dan 10 zijn.");
                    }
                    else if (dimension < 4)
                    {
                        Console.WriteLine("Het cijfer mag niet lager dan 4 zijn.");
                    }
                    else
                    {
                        isGeldigValue = true;
                    }
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Ongeldige waarde!");
                }
            }
            while (!isGeldigValue);
            this.dimension = dimension;
            return dimension;
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

        /// <summary>
        /// Maak een bericht dat de speler gaat starten.
        /// stuurt een bericht naar de server dat de speler gaat starten.
        /// </summary>
        private void StartHetSpelCommando()
        {
            string msg = CommandoHelper.CreeertStartHetSpelCommando();
            this.Send(this.clientSocket, msg);
        }

        /// <summary>
        /// Maak een bericht dat de speler een inzet deed.
        /// Stuur het berichtje naar de server.
        /// </summary>
        /// <param name="inzet">De inzet.</param>
        private void DoeZetCommando(int inzet)
        {
            string msg = CommandoHelper.CreeertDoeZetCommando(inzet.ToString());
            this.Send(this.clientSocket, msg);
        }

        /// <summary>
        /// Maakt een bericht dat de speler een nieuw rondje wil doen.
        /// Stuurt het berichtje naar de server.
        /// </summary>
        private void WilNieuweRondjeCommando()
        {
            string msg = CommandoHelper.CreeertWilNieuweRondje();
            this.Send(this.clientSocket, msg);
        }

        /// <summary>
        /// Maakt een nieuwe berichtje dat de speler wil niet mee doen.
        /// Stuurt het berichtje naar de server.
        /// </summary>
        private void WilNietNieuweRondjeCommando()
        {
            string msg = CommandoHelper.CreeertWilNietRonde();
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

        /// <summary>
        /// Controleert of het anworrd geldig of ongeldig.
        /// </summary>
        /// <returns>Is het antwoord geldig of niet.</returns>
        private bool ThisCheckAnswer()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.J && keyInfo.Key != ConsoleKey.N)
            {
                Console.WriteLine("Je mag alleen J of N gebruiken.");
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

        /// <summary>
        /// Vraagt een speler om een nummer te kiezen.
        /// </summary>
        /// <returns>Het nummer die de speler heeft gekozen.</returns>
        private int Doezet()
        {
            int dimen = 0;
            string antwoord = string.Empty;
            bool isGeldigValue = false;
            do
            {
                Console.WriteLine();
                antwoord = Console.ReadLine();
                if (int.TryParse(antwoord, out dimen))
                {
                    if (dimen > this.dimension)
                    {
                        Console.WriteLine($"Het cijfer mag niet hoger dan {this.dimension} zijn.");
                    }
                    else if (dimen < 1)
                    {
                        Console.WriteLine("Het cijfer mag niet lager dan 1 zijn.");
                    }
                    else
                    {
                        isGeldigValue = true;
                    }
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Ongeldige waarde!");
                }
            }
            while (!isGeldigValue);
            return dimen;
        }

        /// <summary>
        /// Vraagt de speler of hij wil een nieuw rondje wil doen.
        /// </summary>
        /// <returns>Wil de speler een nieuwe rondje doen of niet.</returns>
        private bool WilNieuweRonde()
        {
            Console.WriteLine();
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Wil je een nieuwe ronde doen J of N?");
            return this.ThisCheckAnswer();
        }

        /// <summary>
        /// Behande het nieuwe rondje.
        /// </summary>
        private void UitVoerNieuwRonde()
        {
            if (this.WilNieuweRonde())
            {
                // Wil Nieuwe rondje.
                this.WilNieuweRondjeCommando();
            }
            else
            {
                // Niet.
                this.WilNietNieuweRondjeCommando();
                Thread.Sleep(1000);
                ColorConsole.WriteLine(ConsoleColor.Red, "Tot ziens!");
                Thread.Sleep(2000);

                // Dus doe het window dicht.
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Vraag de speler of hij een nieuwe spel wil spelen.
        /// </summary>
        /// <returns>Wil spelen of niet.</returns>
        private bool WilNieuweSpel()
        {
            Console.WriteLine("Wil je nieuew spel doen J of N?");
            if (this.ThisCheckAnswer())
            {
               return true;
            }
            else
            {
                return false;
            }
        }
    }
}
