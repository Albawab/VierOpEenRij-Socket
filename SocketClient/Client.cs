// <copyright file="Client.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.SocketClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HenE.Games.VierOpEenRij.ConnectionHelper;
    using HenE.Games.VierOpEenRij.Enum;
    using HenE.Games.VierOpEenRij.Protocol;

    /// <summary>
    /// class om nieuwe client te creëren.
    /// stuurt mesaage naar de server.
    /// Ontvangt mesaage vanuit de server.
    /// </summary>
    public class Client : Communicate
    {
        private readonly byte[] buffer = new byte[10000];
        private readonly Socket clientSocket = null;
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
            while (!this.clientSocket.Connected)
            {
                // probeert de cliënt met de server om te aansluiten.
                try
                {
                    this.clientSocket.Connect(IPAddress.Loopback, 5000);
                }
                catch (SocketException e)
                {
                    // Als de cliént niet met de server kan aansluiten.
                    throw new ArgumentException(e.Message);
                }
            }
        }

        /// <summary>
        /// Laat de klant doorgaan. Mag hier niet sluiten.
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
        /// <param name="message">het bericht die uit de server komt.</param>
        /// <param name="socket">De client.</param>
        public override void ProcessStream(string message, Socket socket)
        {
            // [0] is altijd de event.
            // [1] De naam van de speler.
            // [2] Het berichtje.
            // [3] De naam van de tegen speler.
            string[] opgeknipt = message.Split(new char[] { '%' });

            // Change this string to event.
            Events events = EnumHelperl.EnumConvert<Events>(opgeknipt[0]);

            switch (events)
            {
                // Het spel is gecreeerd.
                case Events.GecreeerdSpel:
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine("Je moet op een speler wachten.");
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

                case Events.TegenSpelerHeeftTekenIngezet:
                    Console.WriteLine();
                    Console.WriteLine($"De tegenspeler heeft {opgeknipt[2]} gekozen");
                    Console.WriteLine();
                    Console.Write($"Jij gaat");
                    ColorConsole.Write(ConsoleColor.Red, $" {opgeknipt[4]}");
                    Console.WriteLine(" gbruiken.");
                    Thread.Sleep(500);
                    break;

                case Events.NaamVeranderd:
                    Console.WriteLine();
                    Console.WriteLine("Omdat je tegenspeler dezelfde naam heeft, wordt je naam automatisch verandert.");
                    Console.Write($"Je nieuwe naam is: ");
                    ColorConsole.WriteLine(ConsoleColor.Green, $"{opgeknipt[1]}");
                    break;

                case Events.Gestart:
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    ColorConsole.WriteLine(ConsoleColor.Green, "Na twee seconden wordt het spel gestart.");
                    Thread.Sleep(2000);
                    break;

                case Events.BordGetekend:
                    Console.Clear();
                    Thread.Sleep(500);
                    Console.WriteLine();
                    Console.WriteLine($"{opgeknipt[2]}");
                    break;

                case Events.JeRol:
                    Console.WriteLine();
                    int inzet = this.Doezet();
                    if (inzet > 0 || inzet <= this.dimension)
                    {
                        this.DoeZetCommando(inzet);
                    }

                    break;

                case Events.OngeldigInzet:
                    Console.WriteLine();
                    Console.WriteLine("Deze kolom is vol. Je mag een andere kolom kiezen.");
                    inzet = this.Doezet();
                    if (inzet > 0 || inzet <= this.dimension)
                    {
                        this.DoeZetCommando(inzet);
                    }

                    break;
                case Events.Wachten:
                    Console.WriteLine();
                    Console.WriteLine($"{opgeknipt[3]} is aan het spelen. je moet op hem/haar nu wachten.");
                    break;
                case Events.HeeftGewonnen:
                case Events.HetBordVolGeworden:
                    if (events == Events.HeeftGewonnen)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"{opgeknipt[1]}: je hebt gewonnen.");
                    }
                    else if (events == Events.HetBordVolGeworden)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Het speelvlak is vol.");
                    }

                    break;

                case Events.SpelGevonden:
                    Console.WriteLine();
                    Console.WriteLine("Er is een spel gevonden.");
                    break;

                case Events.HeeftGewonnenTegenSpeler:
                    Console.WriteLine();
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

                case Events.WachtOPReactie:
                    Console.WriteLine();
                    Console.WriteLine("Wacht op hem/haar.");
                    break;

                case Events.TegenSpelerVerlaten:
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("De andere speler heeft het spel verlaten.");
                    if (this.WilNieuweSpel())
                    {
                        this.NieuweSpel();
                    }
                    else
                    {
                        this.DichtHetWindow();
                    }

                    break;

                case Events.SpelGestopt:
                    Console.WriteLine();
                    this.UitVoerNieuwRonde();
                    break;

                case Events.WachtenNieuweSpeler:
                    Console.WriteLine();
                    Console.WriteLine("Je moet op een nieuwe Speler wachten.");

                    break;
                case Events.SpelVerwijderd:
                    Console.WriteLine();
                    Console.WriteLine("Het spel is gestopt!");
                    Console.WriteLine("De speler heeft het spel verlaten. Het spel wordt verwijderd.");
                    if (this.WilNieuweSpel())
                    {
                        this.NieuweSpel();
                    }
                    else
                    {
                        this.DichtHetWindow();
                    }

                    break;
            }
        }

        /// <summary>
        /// Stelt een vraag aan een speler of hij wil tegen computer spelen.
        /// </summary>
        /// <returns>Wil een speler tegen computer spelen of niet.</returns>
        public bool WilTegenComputerSpelen()
        {
            Console.WriteLine();
            Console.WriteLine("Wil je tegen de computer speler J of N?");
            if (this.CheckAnswer())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// stuurt een berichtje naar de server dat de speler wil spelen.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        /// <param name="dimension">De dimensies van het speelvlak.</param>
        /// <param name="wilTegenComputerSpelen">Wil tegen computer spelen of niet.</param>
        /// <param name="socket">De client.</param>
        public void VerzoekOmStartenSpel(string naam, int dimension, bool wilTegenComputerSpelen, Socket socket)
        {
            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

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
        /// <returns>De dimensies van het speelvlak.</returns>
        public int KrijgDimension()
        {
            bool isGeldigValue = false;

            // Vraag de speler om dimension te geven.
            int dimensie;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Hoe groot is het speelveld(Vier kanten)?");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Je mag alleen een cijfer gebruiken. Het cijfer ligt tussen de 4 en de 10.");
                Thread.Sleep(2000);
                string antwoord = Console.ReadLine();
                if (int.TryParse(antwoord, out dimensie))
                {
                    if (dimensie > 10)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Het cijfer mag niet hoger dan 10 zijn.");
                    }
                    else if (dimensie < 4)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Het cijfer mag niet lager dan 4 zijn.");
                    }
                    else
                    {
                        isGeldigValue = true;
                    }
                }
                else
                {
                    Console.WriteLine();
                    ColorConsole.WriteLine(ConsoleColor.Red, "Ongeldige waarde!");
                }
            }
            while (!isGeldigValue);
            this.dimension = dimensie;
            return dimensie;
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
        /// Ontvangt een bericht van de server.
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
        /// Controleert of het antwoord geldig of ongeldig.
        /// </summary>
        /// <returns>Is het antwoord geldig of niet.</returns>
        private bool CheckAnswer()
        {
            Console.WriteLine();
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.J && keyInfo.Key != ConsoleKey.N)
            {
                Console.WriteLine();
                Console.WriteLine("Je mag alleen J of N gebruiken.");
                keyInfo = Console.ReadKey();
                Console.ReadKey();
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
            Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.O && keyInfo.Key != ConsoleKey.X)
            {
                Console.WriteLine();
                Console.WriteLine("Je mag alleen O of X gebruiken.");
                keyInfo = Console.ReadKey();
                Console.ReadKey();
            }

            return keyInfo.Key.ToString();
        }

        /// <summary>
        /// Vraagt een speler om een nummer te kiezen.
        /// </summary>
        /// <returns>Het nummer die de speler heeft gekozen.</returns>
        private int Doezet()
        {
            bool isGeldigValue = false;
            int dimen;
            do
            {
                Console.WriteLine();
                ColorConsole.WriteLine(ConsoleColor.Green, "Je mag een nummer Kiezen.");
                string antwoord = Console.ReadLine();
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
            return this.CheckAnswer();
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
                Console.WriteLine();
                ColorConsole.WriteLine(ConsoleColor.Red, "Tot ziens!");
                Thread.Sleep(3000);

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
            Console.WriteLine();
            Console.WriteLine("Wil je nieuw spel doen J of N?");
            if (this.CheckAnswer())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Vraagt de speler of hij een nieuw spel wil doen.
        /// Als je stuur dan de conmmonds naar de server.
        /// </summary>
        private void NieuweSpel()
        {
            bool tegenComputerSpelen;
            this.dimension = this.KrijgDimension();
            tegenComputerSpelen = false;
            if (this.WilTegenComputerSpelen())
            {
                tegenComputerSpelen = true;
            }

            this.VerzoekOmStartenSpel(this.naam, this.dimension, tegenComputerSpelen, this.clientSocket);
        }

        /// <summary>
        /// Doe het window van de speler dicht, want de speler wil niet meer spelen.
        /// </summary>
        private void DichtHetWindow()
        {
            Console.WriteLine();
            ColorConsole.WriteLine(ConsoleColor.Red, "Tot ziens!");
            Thread.Sleep(4000);
            Environment.Exit(0);
        }
    }
}
