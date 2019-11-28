// <copyright file="ComputerSpeler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij
{
    using System;
    using HenE.Games.VierOpEenRij.Container;

    /// <summary>
    /// Gaat met de computer om.
    /// </summary>
    public class ComputerSpeler : Speler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputerSpeler"/> class.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        public ComputerSpeler(string naam)
            : base(naam)
        {
        }

        /// <inheritdoc/>
        public override bool IsHumanSpeler => false;

        /// <inheritdoc/>
        public override int DoeZet(string inzet, SpeelVlak speelVlak, Game game)
        {
            for (int kolom = 0; kolom < speelVlak.Dimension; kolom++)
            {
                if (!speelVlak.MagInzetten(kolom))
                {
                    continue;
                }

                // Controleert of kan de computer winnen.
                int veldOpKolom = speelVlak.ZetTekenOpSpeelvlak(kolom, this.GebruikTeken);
                if (speelVlak.HeeftGewonnen(this.GebruikTeken))
                {
                    // Als ja, dan reset deze veld en return het nummer van de kolom.
                    speelVlak.ResetVeld(kolom, veldOpKolom);
                    return kolom;
                }

                // als kan niet winnen, dan controleert of de andere speler kan winnen.
                else
                {
                    // reset eerst de oude teken en daarna ga verder.
                    speelVlak.ResetVeld(kolom, veldOpKolom);

                    // Hier breng ik de tegenspeler want ik heb zijn teken nodig om te controleren of kan hij winnen.
                    Speler tegenSpeler = game.TegenSpeler(this);
                    veldOpKolom = speelVlak.ZetTekenOpSpeelvlak(kolom, tegenSpeler.GebruikTeken);
                    if (speelVlak.HeeftGewonnen(tegenSpeler.GebruikTeken))
                    {
                        // Als de tegenspeler kan winnen dan zet je teken dus verkom je dat.
                        speelVlak.ResetVeld(kolom, veldOpKolom);
                        return kolom;
                    }
                }

                speelVlak.ResetVeld(kolom, veldOpKolom);
            }

            // Als de computer speler heeft geen goede veld gevangen, dan geef de eerste kolom terug om te teken te zeten op het speelvlak.
            int random;
            do
            {
                random = this.GeefRandomNummer(speelVlak);
            }
            while (!speelVlak.MagInzetten(random));

            return random;
        }

        /// <summary>
        /// Geeft een random nummer.
        /// </summary>
        /// <param name="speelVlak">Het speelvlak van het huidige spel.</param>
        /// <returns>Een random nummer.</returns>
        private int GeefRandomNummer(SpeelVlak speelVlak)
        {
            Random random = new Random();
            return random.Next(speelVlak.Dimension);
        }
    }
}
