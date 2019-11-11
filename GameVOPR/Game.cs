// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Collections.Generic;
    using HenE.GameVOPR;
    using HenE.VierOPEenRij.Enum;
    using HenE.VierOPEenRij.Interface;

    /// <summary>
    /// Gaat over het spel.
    /// </summary>
    public class Game
    {
        private readonly List<Speler> spelers = new List<Speler>();

        /// <summary>
        /// Gets hudige speler.
        /// </summary>
        public Speler HuidigeSpeler { get; private set; }

        /// <summary>
        /// Initialiseert Het Spel.
        /// change the status of the speler.
        /// </summary>
        public void InitialiseerHetSpel()
        {
            foreach (Speler speler in this.spelers)
            {
                speler.ChangeStatus(Status.Gestart);
            }
        }

        /// <summary>
        /// Voeg een nieuwe speler aan het spel toe.
        /// </summary>
        /// <param name="naam">De naam van de nieuwe speler.</param>
        /// <returns>De nieuwe speler.</returns>
        public Speler AddHumanSpeler(string naam)
        {
            Speler speler = new HumanSpeler(naam);
            this.spelers.Add(speler);
            return speler;
        }

        /// <summary>
        /// Voeg een nieuwe computer speler aan het spel toe.
        /// </summary>
        /// <param name="naam">De naam van de nieuwe speler.</param>
        /// <returns>De nieuwe speler.</returns>
        public Speler AddComputerSpeler(string naam)
        {
            Speler speler = new ComputerSpeler(naam);
            this.spelers.Add(speler);
            return speler;
        }

        /// <summary>
        /// Geef de tegen huidige speler terug.
        /// </summary>
        /// <param name="huidigeSpeler">De huidige speler.</param>
        /// <returns>Tegen huidige speler.</returns>
        public Speler TegenSpeler(Speler huidigeSpeler)
        {
            if (huidigeSpeler == null)
            {
                throw new ArgumentNullException("Er zijn geen speler.");
            }

            foreach (Speler speler in this.spelers)
            {
                if (huidigeSpeler != speler)
                {
                    return speler;
                }
            }

            return null;
        }

        /// <summary>
        /// Voeg een nieuwe speler toe.
        /// Verandert de situatie van een speler.
        /// </summary>
        /// <param name="naam">De naam van een nieuwe speler.</param>
        /// <param name="status">De situatie van een speler.</param>
        /// <param name="teken">Het teken die de speler zal gepruiken.</param>
        public void HandlSpeler(string naam, Status status, string teken)
        {
            Speler speler;
            if (naam == "Computer")
            {
                speler = this.AddComputerSpeler(naam);
            }
            else
            {
                // Voeg een speler aan het spel toe.
                speler = this.AddHumanSpeler(naam);
            }

            Teken tekenVanSpeler = EnumHepler.EnumConvert<Teken>(teken);
            this.GeeftTeken(speler, tekenVanSpeler);
            this.HuidigeSpeler = speler;

            // Chang the status of the speler.
            speler.ChangeStatus(status);
        }

        /// <summary>
        /// Chack of de speler mag spelen.
        /// </summary>
        /// <returns>Mag spelen of niet.</returns>
        public bool KanStarten() => this.spelers.Count == 2;

        /// <summary>
        /// Geeft de spelers van het speler terug geven.
        /// </summary>
        /// <returns>Lijst van spelers.</returns>
        public List<Speler> GeefSpeler()
        {
            List<Speler> spelers = new List<Speler>();
            foreach (Speler speler in this.spelers)
            {
                spelers.Add(speler);
            }

            return spelers;
        }

        /// <summary>
        /// tekent op het speelvlak het teken die de speler wil inzetten op het speelvlak.
        /// </summary>
        /// <param name="inzet">De keuze van de speler.</param>
        /// <param name="speelVlak">Het speelvalk.</param>
        /// <param name="teken">Het teken van de speler.</param>
        public void TekentOpSpeelvlak(int inzet, SpeelVlak speelVlak, Teken teken)
        {
            if (teken == Teken.Undefined)
            {
                // throw new ArgumentOutOfRangeException("Mag niet de teken Umdefined zijn of null.");
            }

            if (speelVlak == null)
            {
                throw new ArgumentNullException("mag niet het speelvlak null zijn.");
            }

            if (inzet < 0)
            {
                throw new ArgumentOutOfRangeException("Mag niet het inzet nul of minder zijn.");
            }

            speelVlak.ZetTekenOpSpeelvlak(inzet, teken);
        }

        /// <summary>
        /// Check of de speler heeft gewonnen of niet.
        /// </summary>
        /// <param name="speelVlak">Het speelvlak.</param>
        /// <param name="teken">De teken van de speler die zal nagaan.</param>
        /// <returns>Heeft vier op een rij of niet.</returns>
        public bool HeeftGewonnen(SpeelVlak speelVlak, Teken teken)
        {
            return speelVlak.HeeftGewonnen(teken) ? true : false;
        }

        /// <summary>
        /// Vraagt de speler of hij wil een nieuwe ronde doen of niet.
        /// Als hij wil een nieuw rondje doen  dan geeft dat terug.
        /// </summary>
        /// <returns>Wil de speler mee doen of niet.</returns>
        public bool VraagNieuwRonde()
        {
            foreach (Speler speler in this.spelers)
            {
                if (speler.IsHumanSpeler)
                {
                    // Doe contact met de human speler als interface.
                    IMakeContactBetweenSpelerAndGame humanSpeler = speler as IMakeContactBetweenSpelerAndGame;
                    if (humanSpeler.NieuwRonde("Wil je een nieuw Rondje doen?"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Geeft de speler eigen teken die hij het nodig heeft om te spelen.
        /// </summary>
        /// <param name="speler">De speler die een teken wordt gekregen.</param>
        /// <param name="tekenVanSpeler">De teken.</param>
        private void GeeftTeken(Speler speler, Teken tekenVanSpeler)
        {
            if (speler == null)
            {
                throw new ArgumentNullException("Mag speler niet null zijn");
            }

            speler.GeeftTeken(tekenVanSpeler);
        }
    }
}
