// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using HenE.GameVOPR;
    using HenE.VierOPEenRij.Enum;

    /// <summary>
    /// Gaat over het spel.
    /// </summary>
    public class Game
    {
        private readonly List<Speler> spelers = new List<Speler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="dimension">dimension van het speelvlak.</param>
        public Game(int dimension)
        {
            this.Dimension = dimension;
            this.SpeelVlak = new SpeelVlak(dimension);
        }

        /// <summary>
        /// Gets hudige speler.
        /// </summary>
        public Speler HuidigeSpeler { get; private set; }

        /// <summary>
        /// Gets het speelvalk van het spel.
        /// </summary>
        public SpeelVlak SpeelVlak { get; private set; }

        /// <summary>
        /// Gets the status of the game.
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the dimension of the speelvlak.
        /// </summary>
        public int Dimension { get; private set; }

        /// <summary>
        /// Initialiseert Het Spel.
        /// change the status of the speler.
        /// </summary>
        public void InitialiseerHetSpel()
        {
            // geeft het spel een gestart situatie.
            this.Status = Status.Gestart;

            // dan geef elke speler een gestart situatie.
            foreach (Speler speler in this.spelers)
            {
                speler.ChangeStatus(Status.Gestart);
            }
        }

        /// <summary>
        /// Gaat het spel starten.
        /// </summary>
        public void StartHetSpel()
        {
            GameController gameController = new GameController(this);
            gameController.GameStart();
        }

        /// <summary>
        /// Voeg een nieuwe speler aan het spel toe.
        /// </summary>
        /// <param name="naam">De naam van de nieuwe speler.</param>
        /// <param name="socket">tcp client van de speler.</param>
        /// <returns>De nieuwe speler.</returns>
        public Speler AddHumanSpeler(string naam, Socket socket)
        {
            Speler speler = new HumanSpeler(naam, socket);
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
        /// Zet een situatie van het spel.
        /// </summary>
        /// <param name="status">de nieuwe situatie van het spel.</param>
        public void ZetSitauatie(Status status)
        {
            this.Status = status;
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
        /// Geeft de spelers van het spel terug.
        /// </summary>
        /// <returns>Lijst van speler.</returns>
        public List<Speler> GetSpelers()
        {
            List<Speler> lijstOfSpelers = new List<Speler>();
            foreach (Speler speler in this.spelers)
            {
                lijstOfSpelers.Add(speler);
            }

            return lijstOfSpelers;
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
/*                    // Doe contact met de human speler als interface.
                    if (humanSpeler.NieuwRonde("Wil je een nieuw Rondje doen?"))
                    {
                        return true;
                    }*/
                }
            }

            return false;
        }

        /// <summary>
        /// zet de huidige speler.
        /// </summary>
        /// <param name="speler">De speler die zal starten.</param>
        public void ZetHuidigeSpeler(Speler speler)
        {
            this.HuidigeSpeler = speler;
        }

        /// <summary>
        /// Stuurt het inzet van de inzet die de speler heeft gekozen naar de speler om te handlen.
        /// </summary>
        /// <param name="inzet">de inzet.</param>
        /// <param name="socket">De tcp client van de speler.</param>
        /// <returns>Het nummer die de speler hem heeft gekozen.</returns>
        public int DoeInzet(string inzet, Socket socket)
        {
            Speler speler = this.GetSpelerViaTcp(socket);
            return speler.DoeZet(inzet, this.SpeelVlak, this);
        }

        /// <summary>
        /// Geeft de speler terug via zijn tcp client.
        /// </summary>
        /// <param name="socket">De tcp client van de pseler.</param>
        /// <returns>een speler.</returns>
        private Speler GetSpelerViaTcp(Socket socket)
        {
            foreach (Speler speler in this.spelers)
            {
                if (speler.IsHumanSpeler)
                {
                    HumanSpeler humanSpeler = speler as HumanSpeler;
                    if (humanSpeler.TcpClient == socket)
                    {
                        return speler;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Geeft de speler eigen teken die hij het nodig heeft om te spelen.
        /// </summary>
        /// <param name="socket">De client van de speler.</param>
        /// <param name="tekenVanSpeler">De teken die de speler heeft gekozen.</param>
        /// <returns>De naam van de speler.</returns>
        public string TekenenBehandler(Socket socket, Teken tekenVanSpeler)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("Mag speler niet null zijn");
            }

            Speler deSpeler = null;

            // Zoek naar de speler via zijn tcp.
            foreach (Speler speler in this.spelers)
            {
                // Als de speler is humen speler dan mag zoeken naar zijn tcp.
                if (speler.IsHumanSpeler)
                {
                    HumanSpeler humanSpeler = speler as HumanSpeler;
                    if (humanSpeler.TcpClient == socket)
                    {
                        deSpeler = speler;
                        speler.ZetTeken(tekenVanSpeler);
                    }
                }
            }

            // zoek naar de andere teken.
            Teken teken = this.TegenDezeTeken(tekenVanSpeler);

            // geef de andere teken aan de de andere speler.
            this.ZetEenTekenVoorAnderSpeler(deSpeler, teken);
            return deSpeler.Naam;
        }

        /// <summary>
        /// Geeft de andere speler een teken.
        /// </summary>
        /// <param name="speler">Een speler.</param>
        /// <param name="teken">De teken van de speler.</param>
        private void ZetEenTekenVoorAnderSpeler(Speler speler, Teken teken)
        {
            Speler tegenSpeler = this.TegenSpeler(speler);

            tegenSpeler.ZetTeken(teken);
        }

        /// <summary>
        /// Geeft tegen de huidige teken terug.
        /// </summary>
        /// <param name="teken">De huidige teken.</param>
        /// <returns>De tegen van de huidige teken.</returns>
        private Teken TegenDezeTeken(Teken teken)
        {
            if (teken == Teken.O)
            {
                return Teken.X;
            }
            else
            {
                return Teken.O;
            }
        }
    }
}
