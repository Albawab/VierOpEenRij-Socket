// <copyright file="HumanSpeler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij
{
    using System;
    using System.Net.Sockets;
    using HenE.Games.VierOpEenRij.Container;

    /// <summary>
    /// Gaat over de Humanspeler.
    /// </summary>
    public class HumanSpeler : Speler
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
        /// Gets inzet van een speler.
        /// </summary>
        public int HuidigeInZet { get; private set; }

        /// <summary>
        /// Gets de clients van de spelrs.
        /// </summary>
        public Socket TcpClient { get; private set; }

        /// <inheritdoc/>
        public override int DoeZet(string inzet, SpeelVlak speelVlak, Game game)
        {
            // het nummer die de speler heeft gekozen.
            int.TryParse(inzet, out int keuzeNummer);

            if (keuzeNummer < 0 || keuzeNummer > speelVlak.Dimension)
            {
                throw new ArgumentException("Mag niet de inzet mider dan nul of hoger dan de dimension van het speelvlak");
            }

            // doet een nummer af want de array start van nummer nul.
            this.HuidigeInZet = keuzeNummer - 1;

            // De speler gaat een nummer tussen een en de dimension kiesen.
            return this.HuidigeInZet;
        }
    }
}
