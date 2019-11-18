// <copyright file="SpelHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ServerSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using HenE.VierOPEenRij;
    using HenE.VierOPEenRij.Enum;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// class om het spel te behandelen.
    /// </summary>
    public class SpelHandler
    {
        private List<Game> spellen = new List<Game>();

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
        /// <param name="socket">De tcp client van de speler.</param>
        /// <returns>Mag de speler starten of wachte.</returns>
        public string SpelHandlen(string naam, int dimension, Socket socket)
        {
            try
            {
                Game game;
                string returnMessage = string.Empty;

                if (naam == string.Empty)
                {
                    throw new ArgumentNullException("Mag niet de naam leeg zijn.");
                }

                game = null;
                if (naam == "Computer")
                {
                    // zoek naar het zelfde spel van de human speler.
                    game = this.GetGameByTcpClient(socket);
                }
                else
                {
                    // als de speler wil tegen andre speler spelen, dan gaat hier naar ander spel zoeken.
                    // Zoek naar een spel die de zelfde dimension heeft.
                    game = this.GetGameByDimensionAndStatus(dimension);
                    .Er zijn geen human speler op wachten. Wil je tegen combuter speler ?
                }

                // Is er een game of niet.
                // als niet dan laat de speler weten dat hij moet wachten.
                if (game == null)
                {
                    // als geen game heeft gevonden dan creeert een nieuwe game.
                    game = this.CreerenEenSpel(dimension);
                    game.ZetSitauatie(Status.Wachten);

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
                    // kan het spel starten.
                    // als de speler is computer dus voeg een computer speler toe.
                    // anders voeg een human speler toe.
                    if (naam == "Computer")
                    {
                        game.AddComputerSpeler(naam);
                    }
                    else
                    {
                        // Controleert of tegen speler heeft de zelfde naam.
                        // als ja, Dan komt een nummer achter de naam van de nieuwe speler.
                        string nieuweNaam = naam;

                        if (game.BestaalDezeNaam(naam))
                        {
                            nieuweNaam += "1";
                        }

                        game.AddHumanSpeler(nieuweNaam, socket);
                    }

                    return Events.SpelerInvoegde.ToString();
                }

                return returnMessage;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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

        /// <summary>
        /// Zoekt naar een game van de zelfde tcpclient heeft.
        /// Die is nodig omdat de speler tegen de computer wil spelen, Nu moet de computer tegen de speler spelen op zelefde spel.
        /// </summary>
        /// <param name="socket">De tcp client van de speler.</param>
        /// <returns>Het game van de speler.</returns>
        private Game GetGameByTcpClient(Socket socket)
        {
            foreach (Game game in this.spellen)
            {
                foreach (Speler speler in game.GetSpelers())
                {
                    HumanSpeler human = speler as HumanSpeler;
                    if (human.TcpClient == socket)
                    {
                        return game;
                    }
                }
            }

            return null;
        }
    }
}
