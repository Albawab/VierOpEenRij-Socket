// <copyright file="GameController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Doet controller op het spel.
    /// </summary>
    public class GameController
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
            Speler deWinnaar = null;

            // Reset the Speelvalk.
            this.speelVlak.ResetSpeelVlak();

            // Teken het bord.
            string bord = this.speelVlak.TekenSpeelvlak();
            Console.WriteLine(bord);
            Speler speler = this.gameVierOpEenRij.HuidigeSpeler;

            do
            {
                speler = this.gameVierOpEenRij.TegenSpeler(speler);
                int inzet;
                do
                {
                    inzet = speler.DoeZet(string.Empty, this.speelVlak, this.gameVierOpEenRij);
                }
                while (!this.speelVlak.MagInzetten(inzet));

                this.gameVierOpEenRij.TekentOpSpeelvlak(inzet, this.speelVlak, speler.GebruikTeken);
                bord = this.speelVlak.TekenSpeelvlak();
                Console.WriteLine(bord);
                /*Thread.Sleep(2000);*/
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
    }
}
