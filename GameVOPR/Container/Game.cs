// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using HenE.Games.VierOpEenRij.Container;
    using HenE.Games.VierOpEenRij.Enum;

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
        /// Gets the game controller.
        /// </summary>
        public GameController GameController { get; private set; }

        /// <summary>
        /// Gets huidige speler.
        /// </summary>
        public Speler HuidigeSpeler { get; private set; }

        /// <summary>
        /// Gets het speelveld van het spel.
        /// </summary>
        public SpeelVlak SpeelVlak { get; private set; }

        /// <summary>
        /// Gets het speelveld van het spel.
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Gets het speelveld van het spel.
        /// </summary>
        public int Dimension { get; private set; }

        /// <summary>
        /// Initialisiert  Het Spel.
        /// change the status of the speler.
        /// </summary>
        /// <param name="gameController">De controller van het spel.</param>
        public void InitialiseerHetSpel(GameController gameController)
        {
            this.GameController = gameController;

            // geeft het spel een gestart situatie.
            this.Status = Status.Gestart;

            // dan geef elke speler een gestart situatie.
            foreach (Speler speler in this.spelers)
            {
                speler.ChangeStatus(Status.Gestart);
            }
        }

        /// <summary>
        /// Voeg een nieuwe controller aan het spel toe.
        /// </summary>
        public void VoegEenControllerToe()
        {
            this.GameController = new GameController(this);
        }

        /// <summary>
        /// Gaat het spel starten.
        /// </summary>
        public void StartHetSpel()
        {
            this.GameController.GameStart();
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
        public void ZetSituatie(Status status)
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
        public void ZetTekenOpSpeelvlak(int inzet, SpeelVlak speelVlak, Teken teken)
        {
            if (teken == Teken.Undefined)
            {
                 throw new ArgumentException("Mag niet de teken Undefined zijn of null.");
            }

            if (speelVlak == null)
            {
                throw new ArgumentException("mag niet het speelvlak null zijn.");
            }

            if (inzet < 0)
            {
                throw new ArgumentException("Mag niet het inzet nul of minder zijn.");
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
            return speelVlak.HeeftGewonnen(teken);
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
        /// Krijgt de inzet van een speler.
        /// </summary>
        /// <param name="speler">Een speler.</param>
        /// <returns>De inzet van deze speler.</returns>
        public int GetInzet(Speler speler)
        {
            if (speler == null)
            {
                throw new ArgumentException("Mag niet de speler null zijn.");
            }

            if (speler.IsHumanSpeler)
            {
                HumanSpeler humanSpeler = speler as HumanSpeler;

                return humanSpeler.HuidigeInZet;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Geeft de speler terug via zijn tcp client.
        /// </summary>
        /// <param name="socket">De tcp client van de pseler.</param>
        /// <returns>een speler.</returns>
        public Speler GetSpelerViaTcp(Socket socket)
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
        /// Controleert of de huidige speler al de zelfde naam heeft of niet.
        /// </summary>
        /// <param name="naam">De naam van de nieuwe speler.</param>
        /// <returns>bestaat deze naam al of niet.</returns>
        public bool BestaatDezeNaam(string naam)
        {
            foreach (Speler speler in this.spelers)
            {
                if (speler.Naam == naam)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Geeft de speler eigen teken die hij het nodig heeft om te spelen.
        /// </summary>
        /// <param name="socket">De client van de speler.</param>
        /// <param name="tekenVanSpeler">De teken die de speler heeft gekozen.</param>
        /// <returns>De naam van de speler.</returns>
        public string TekenenBehandl(Socket socket, Teken tekenVanSpeler)
        {
            if (socket == null)
            {
                throw new ArgumentException("Mag speler niet null zijn");
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
            if (deSpeler != null)
            {
                this.ZetEenTekenVoorAnderSpeler(deSpeler, teken);
                return deSpeler.Naam;
            }

            return null;
        }

        /// <summary>
        /// Verwijdert Een speler van het spel.
        /// </summary>
        /// <param name="speler">De speler die wordt verwijderd.</param>
        public void VerWijdertEenSpeler(Speler speler)
        {
            this.spelers.Remove(speler);
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
