﻿// <copyright file="GameController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HenE.ConnectionHelper;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// Doet controller op het spel.
    /// </summary>
    public class GameController : Communicate
    {
        private readonly SpeelVlak speelVlak = null;
        private readonly Game gameVierOpEenRij = null;
        private bool magControleer = false;

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
            this.gameVierOpEenRij.InitialiseerHetSpel(this);

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
                // Reset the Speelvalk.
                this.speelVlak.ResetSpeelVlak();

                // get the player.
                Speler speler = this.gameVierOpEenRij.HuidigeSpeler;

                this.StuurBrichtNaarSpelers(Events.Gestart, string.Empty, speler);

                Thread.Sleep(2000);

                // Teken het bord.
                string bord = this.speelVlak.TekenSpeelvlak();

                // send an message  to this player included this bord.
                this.StuurBrichtNaarSpelers(Events.BordGetekend, bord, speler);
                Thread.Sleep(3000);
                this.DoeInzet(speler);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// ask de speler om inzet te doen.
        /// </summary>
        /// <param name="speler">De speler die een vraag heeft gekregen. Dus hij moet een inzet doen.</param>
        public void DoeInzet(Speler speler)
        {
            string bord = string.Empty;
            Speler hudigeSpeler;

            // Vraag de speler om te zetten.
            do
            {
                int inzet;

                if (speler.IsHumanSpeler)
                {
                    if (this.magControleer)
                    {
                        // Krijg de inzet van de speler.
                        inzet = this.gameVierOpEenRij.GetInzet(speler);

                        // Controleer of de inzet mag inzetten of niet.
                        if (!this.speelVlak.MagInzetten(inzet))
                        {
                            // Als het mag niet inzetten dan stuur een bericht naar de speler om nieuwe nummer te kiezen.
                            this.SendEenBericht(Events.OngeldigInzet, string.Empty, speler);
                            break;
                        }
                    }
                    else
                    {
                        // Stuur een bericht naar de speler dat hij moet inzetten.
                        this.StuurBrichtNaarSpelers(Events.JeRol, string.Empty, speler);

                        // Doe mag controleer true.
                        // vanaf nu kan de controller de inzet van de huidige speler controleren
                        this.magControleer = true;
                        break;
                    }
                }
                else
                {
                    // Nu gaat de applicatie met de computer speler behandelen.
                    // Als de speler is geen human speler dan krijgt een nummer van de computer.
                    do
                    {
                        inzet = speler.DoeZet(string.Empty, this.speelVlak, this.gameVierOpEenRij);
                    }
                    while (!this.speelVlak.MagInzetten(inzet));
                }

                // als de inzet geldig is dan teken het op het bord.
                this.gameVierOpEenRij.ZetTekenOpSpeelvlak(inzet, this.speelVlak, speler.GebruikTeken);

                // Teken het bord.
                bord = this.speelVlak.TekenSpeelvlak();
                Thread.Sleep(1500);

                // send an message  to this player included this bord.
                this.StuurBrichtNaarSpelers(Events.BordGetekend, bord, speler);
                hudigeSpeler = speler;

                // Tegen speler.
                speler = this.gameVierOpEenRij.TegenSpeler(speler);
                this.magControleer = false;
            }
            while (!this.gameVierOpEenRij.HeeftGewonnen(this.speelVlak, hudigeSpeler.GebruikTeken)
                && !this.speelVlak.IsSpeelvlakVol());

            if (this.speelVlak.IsSpeelvlakVol())
            {
                this.StuurBrichtNaarSpelers(Events.HetBordVolGeworden, string.Empty, speler);
            }
            else if (this.gameVierOpEenRij.HeeftGewonnen(this.speelVlak, this.gameVierOpEenRij.TegenSpeler(speler).GebruikTeken))
            {
                this.StuurBrichtNaarSpelers(Events.HeeftGewonnen, string.Empty, this.gameVierOpEenRij.TegenSpeler(speler));
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
                msg.AppendFormat($"{events.ToString()}%{speler.Naam}%{message}%{this.gameVierOpEenRij.TegenSpeler(speler).Naam}");

                // omdat de bericht gaat aleen naar de human speler sturen dan maak de speler als een human speler.
                HumanSpeler humanSpeler = speler as HumanSpeler;

                // stuur het bericht naar de speler.
                this.ProcessStream(msg.ToString(), humanSpeler.TcpClient);
            }
        }

        /// <summary>
        /// stuurt een bericht naar elke speler.
        /// </summary>
        /// <param name="events">De event.</param>
        /// <param name="msg">Het berichtje die naar een speler wordt gesturd.</param>
        /// <param name="huidigeSpeler">De huidige speler.</param>
        private void StuurBrichtNaarSpelers(Events events, string msg, Speler huidigeSpeler)
        {
            // Krijg de lijst van de spelers.
            foreach (Speler speler in this.gameVierOpEenRij.GetSpelers())
            {
                // Als de speler human is dan stuur een bericht naar hem.
                if (speler.IsHumanSpeler)
                {
                    HumanSpeler humanSpeler = speler as HumanSpeler;

                    // stuur berichtje naar de huidige speler
                    if (speler == huidigeSpeler || events == Events.BordGetekend)
                    {
                        this.SendEenBericht(events, msg, speler);
                        if (events == Events.HeeftGewonnen || events == Events.HetBordVolGeworden)
                        {
                            Thread.Sleep(3000);
                            this.SendEenBericht(Events.SpelGstopt, msg, huidigeSpeler);
                        }
                    }
                    else if (events == Events.Gestart)
                    {
                        this.SendEenBericht(events, msg, speler);
                    }
                    else if (events == Events.HeeftGewonnen)
                    {
                        this.SendEenBericht(Events.HeeftGewonnenTegen, msg, speler);
                    }
                    else if (events == Events.JeRol)
                    {
                        // stuur deze messag naar de tegen speler.
                        this.SendEenBericht(Events.Wachten, msg, this.gameVierOpEenRij.TegenSpeler(huidigeSpeler));
                    }
                }
            }
        }
    }
}
