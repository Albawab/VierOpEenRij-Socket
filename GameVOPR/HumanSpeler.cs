// <copyright file="HumanSpeler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Net.Sockets;
    using HenE.VierOPEenRij.Interface;

    /// <summary>
    /// Gaat over de Humanspeler.
    /// </summary>
    internal class HumanSpeler : Speler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanSpeler"/> class.
        /// </summary>
        /// <param name="naam">De naam van een speler.</param>
        /// <param name="socket">De tcp client van de humen speler.</param>
        public HumanSpeler(string naam, Socket socket)
            : base(naam)
        {
            this.TcpClient = socket;
        }

        /// <inheritdoc/>
        public override bool IsHumanSpeler => true;

        /// <summary>
        /// Gets de clients van de spelrs.
        /// </summary>
        public Socket TcpClient { get; private set; }

        /// <inheritdoc/>
        public override int DoeZet(string vraag, SpeelVlak speelVlak, Game game)
        {
            Console.WriteLine("Kies een nummer");
            string antwoord = Console.ReadLine();

            // het nummer die de speler heeft gekozen.
            int keuzeNummer = 0;
            int.TryParse(antwoord, out keuzeNummer);

            // we doen een nummer af want de array start van nummer nul.
            // De speler gaat een nummer tussen een en de dimension kiesen.
            int zet = keuzeNummer - 1;

            return zet;
        }

        /// <inheritdoc/>
        public bool NieuwRonde(string vraag)
        {
            if (vraag == string.Empty)
            {
                throw new ArgumentNullException("Mag de vraag niet null zijn. Er moet een vrijg.");
            }

            Console.WriteLine(vraag);
            string antwoord = Console.ReadLine().ToUpper();

            return antwoord == "J";
        }

        /// <inheritdoc/>
        public string WilStopen(string vraag)
        {
            throw new NotImplementedException();
        }
    }
}
