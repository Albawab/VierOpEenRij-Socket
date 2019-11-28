// <copyright file="SpelHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.ServerSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using HenE.Games.VierOpEenRij.Container;
    using HenE.Games.VierOpEenRij.Enum;
    using HenE.Games.VierOpEenRij.Protocol;

    /// <summary>
    /// class om het spel te behandelen.
    /// </summary>
    public class SpelHandler
    {
        private readonly List<Game> spellen = new List<Game>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpelHandler"/> class.
        /// </summary>
        public SpelHandler()
        {
        }

        /// <summary>
        /// Ceéren een nieuwe spel.
        /// </summary>
        /// <param name="dimension">De dimension van de nieuwe spel.</param>
        /// <returns>Een nieuwe spel.</returns>
        public Game CreerenEenSpel(int dimension)
        {
            Game game = new Game(dimension);
            this.spellen.Add(game);
            return game;
        }

        /// <summary>
        /// Voeg een nieuwe speler toe.
        /// Verandert de situatie van een speler.
        /// </summary>
        /// <param name="naam">De naam van een nieuwe speler.</param>
        /// <param name="dimension">De dimension die de speler mee wil spelen.</param>
        /// <param name="wilTegenComputerSpelen">Als de speler wil tegen computer spelen of niet.</param>
        /// <param name="socket">De tcp client van de speler.</param>
        /// <returns>Mag de speler starten of wachte.</returns>
        public string SpelHandlen(string naam, int dimension, bool wilTegenComputerSpelen, Socket socket)
        {
            try
            {
                Game game;
                string returnMessage = string.Empty;

                if (naam == string.Empty)
                {
                    throw new ArgumentException("Mag niet de naam leeg zijn.");
                }

                // Als de speler tegen computer wil spelen.
                if (wilTegenComputerSpelen)
                {
                    // een nieuw spel gemaakt.
                    game = this.CreerenEenSpel(dimension);

                    // Voeg een nieuwe controller aan het spel.
                    game.VoegEenControllerToe();

                    // Geef een situatie aan die nieuwe spel.
                    game.ZetSituatie(Status.InHetSpel);

                    // Voeg de human speler toe.
                    Speler speler = game.AddHumanSpeler(naam, socket);

                    game.ZetHuidigeSpeler(speler);

                    // Voeg de computer speler toe.
                    game.AddComputerSpeler("Computer");
                    return Events.SpelerInvoegde.ToString();
                }
                else
                {
                    // Als de computer tegen een andere human spelen dan zoek of er staat een spel.
                    game = this.GetGameByDimensionAndStatus(dimension);

                    // Is er een game of niet.
                    // als niet dan laat de speler weten dat hij moet wachten.
                    if (game == null)
                    {
                        // als geen game heeft gevonden dan creeert een nieuwe game.
                        game = this.CreerenEenSpel(dimension);
                        game.ZetSituatie(Status.Wachten);

                        // voeg de speler toe.
                        Speler speler = game.AddHumanSpeler(naam, socket);

                        // Omdat hij de eerste speler mag hij starten.
                        game.ZetHuidigeSpeler(speler);

                        // Change the status of the speler.
                        speler.ChangeStatus(Status.Wachten);

                        // geeft de event terug sturen.
                        return Events.GecreeerdSpel.ToString();
                    }
                    else
                    {
                        // Controleert of tegen speler heeft dezelfde naam.
                        // als ja, Dan komt een nummer achter de naam van de nieuwe speler.
                        string nieuweNaam = naam;
                        game.VoegEenControllerToe();

                        if (game.BestaatDezeNaam(naam))
                        {
                            nieuweNaam += "1";
                            game.AddHumanSpeler(nieuweNaam, socket);
                            game.GameController.SendEenBericht(Events.NaamVeranderd, nieuweNaam, game.GetSpelerViaTcp(socket));
                        }
                        else
                        {
                            game.AddHumanSpeler(naam, socket);
                        }

                        game.GameController.SendEenBericht(Events.SpelGevonden, nieuweNaam, game.TegenSpeler(game.GetSpelerViaTcp(socket)));

                        return Events.SpelerInvoegde.ToString();
                    }
                }

                return returnMessage;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Geeft de lijst van de spellen terug geven.
        /// </summary>
        /// <returns>De list van de spellen.</returns>
        public List<Game> GetSpellen()
        {
            List<Game> deSpellen = new List<Game>();

            foreach (Game game in this.spellen)
            {
                deSpellen.Add(game);
            }

            return deSpellen;
        }

        /// <summary>
        /// Verwijdert een spel.
        /// </summary>
        /// <param name="game">Het game die zal verwijderen.</param>
        public void DeleteGame(Game game)
        {
            this.spellen.Remove(game);
        }

        /// <summary>
        /// Zoekt naar een spel die de zelfde dimension en wachte situatie heeft.
        /// </summary>
        /// <param name="dimension">De dimension van de nieuwe spel. zoekt bij de spellen of er zijn een spel die de zelfde dimension heeft.</param>
        /// <returns>De spel die een speler er mee mag starten.</returns>
        private Game GetGameByDimensionAndStatus(int dimension)
        {
            Game nieuweSpel = null;

            // Zoek in de lijst van de spellen.
            foreach (Game spel in this.spellen)
            {
                // Als een spel met de gelijk dimension en de situatie is wachten dan laat het terugsturen.
                if (spel.Status == Status.Wachten && spel.Dimension == dimension)
                {
                    nieuweSpel = spel;
                }
            }

            return nieuweSpel;
        }
    }
}
