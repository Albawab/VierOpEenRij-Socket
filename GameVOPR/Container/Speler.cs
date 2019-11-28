// <copyright file="Speler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Container
{
    using HenE.Games.VierOpEenRij.Enum;

    /// <summary>
    /// Vraagt de speler om inzet te doen.
    /// Vraag de speler om naam te geven.
    /// </summary>
    public abstract class Speler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Speler"/> class.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        protected Speler(string naam)
        {
            this.Naam = naam;
        }

        /// <summary>
        /// Gets or sets Teken van de speler.
        /// </summary>
        public Teken GebruikTeken { get; protected set; }

        /// <summary>
        /// Gets de naam van de speler.
        /// </summary>
        public string Naam { get; private set; }

        /// <summary>
        /// Gets a value indicating whether als de speler is human.
        /// </summary>
        public abstract bool IsHumanSpeler { get; }

        /// <summary>
        /// Gets or sets het situatie van een speler.
        /// </summary>
        private Status SpelerSituatie { get; set; }

        /// <summary>
        /// Verandert het situatie van een speler.
        /// </summary>
        /// <param name="status">Het nieuwe situatie.</param>
        public void ChangeStatus(Status status)
        {
            this.SpelerSituatie = status;
        }

        /// <summary>
        /// Vraag de speler om een zet te doen.
        /// </summary>
        /// <param name="inzet">De inzet.</param>
        /// <param name="speelVlak">Speelvlak van het spel.</param>
        /// <param name="game">Game.</param>
        /// <returns>Het nummer die de speler wil inzetten.</returns>
        public abstract int DoeZet(string inzet, SpeelVlak speelVlak, Game game);

        /// <summary>
        /// Zet een teken die de speler het gaat gebruiken.
        /// </summary>
        /// <param name="teken">De teken.</param>
        public void ZetTeken(Teken teken)
        {
            this.GebruikTeken = teken;
        }
    }
}
