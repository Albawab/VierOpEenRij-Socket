// <copyright file="GameController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using HenE.ConnectionHelper;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// Doet controller op het spel.
    /// </summary>
    public class GameController : Communicate
    {
        private readonly SpeelVlak speelVlak = null;
        private readonly Game gameVierOpEenRij = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="speelVlak">Het speelval van het spel.</param>
        /// <param name="dimension">Het grootte van het speelvlak.</param>
        /// <param name="gameVierOpEenRij">Game.</param>
        public GameController(Game gameVierOpEenRij)
        {
            this.gameVierOpEenRij = gameVierOpEenRij;
            this.speelVlak = gameVierOpEenRij.SpeelVlak;
        }

        /// <summary>
        /// Start het spel.
        /// </summary>
        public void GameStart()
        {
            // Doe het spel kaar om te spelen.
            this.gameVierOpEenRij.InitialiseerHetSpel();

            // start een nieuw ronde.
            this.NieuwRonde();
        }

        /// <summary>
        /// Start een nieuw Rindje.
        /// </summary>
        public void NieuwRonde()
        {
            try
            {
                Speler deWinnaar = null;

                // Reset the Speelvalk.
                this.speelVlak.ResetSpeelVlak();

                // Teken het bord.
                string bord = this.speelVlak.TekenSpeelvlak();

                // get the player.
                Speler speler = this.gameVierOpEenRij.HuidigeSpeler;

                // send an message to this player.
                this.SendEenBericht(Events.BordGetekend,bord, speler);

                // Vraag de speler om te zetten.
                do
                {
                    // Tegen speler.
                    speler = this.gameVierOpEenRij.TegenSpeler(speler);
                    int inzet;
                    do
                    {
                        // stelt een vraag aan de speler.
                        inzet = speler.DoeZet(string.Empty, this.speelVlak, this.gameVierOpEenRij);
                    }
                    while (!this.speelVlak.MagInzetten(inzet));

                    // als de inzet geldig is dan teken het op het bord.
                    this.gameVierOpEenRij.TekentOpSpeelvlak(inzet, this.speelVlak, speler.GebruikTeken);

                    // Teken het bord met de nieuwe situatie.
                    bord = this.speelVlak.TekenSpeelvlak();

                    // Stuur de nieuwe teken naar de spelers.
                    this.SendEenBericht(Events.BordGetekend, bord, speler);
                    deWinnaar = speler;
                }
                while (!this.gameVierOpEenRij.HeeftGewonnen(this.speelVlak, speler.GebruikTeken)
                    && !this.speelVlak.IsSpeelvlakVol());

                if (this.speelVlak.IsSpeelvlakVol())
                {
                    Console.WriteLine("Het Speelvlak Is vol");
                }
                else
                {
                    Console.WriteLine($"{deWinnaar.Naam} is de winnaar.");
                }

                if (this.gameVierOpEenRij.VraagNieuwRonde())
                {
                    this.NieuwRonde();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public override void ProcessStream(string message, Socket socket)
        {
            this.Send(socket, message);
        }

        /// <summary>
        /// Controleert of de speler is humanspeler.
        /// Als de speler human speler is dan stuur een bericht naar deze speler.
        /// </summary>
        /// <param name="events">De event.</param>
        /// <param name="message">De bericht die naar de humam speler gaat sturen.</param>
        /// <param name="speler">De speler die een bericht zal ontvangen.</param>
        private void SendEenBericht(Events events, string message, Speler speler)
        {
            StringBuilder msg = new StringBuilder();
            if (speler.IsHumanSpeler)
            {
                // verzamel het bericht die naar de speler zal sturen.
                msg.AppendFormat($"{events.ToString()}%{speler.Naam}%{message}");

                // omdat de bericht gaat aleen naar de human speler sturen dan maak de speler als een human speler.
                HumanSpeler humanSpeler = speler as HumanSpeler;

                // stuur het bericht naar de speler.
                this.ProcessStream(msg.ToString(), humanSpeler.TcpClient);
            }
        }

        /// <summary>
        /// stuurt een bericht naar elke speler.
        /// </summary>
        /// <param name="huidigeSpeler">De huidige speler.</param>
        private void StuurBrichtNaarSpelers(Speler huidigeSpeler)
        {
            foreach (Speler speler in this.gameVierOpEenRij.GetSpelers())
            {
                if (speler.IsHumanSpeler)
                {
                    HumanSpeler humanSpeler = speler as HumanSpeler;
                    this.SendEenBericht(Events.BordGetekend, string.Empty, huidigeSpeler);
                }
            }
        }
    }
}
