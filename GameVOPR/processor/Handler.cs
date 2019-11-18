// <copyright file="Handler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.GameVierOpEenRij
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using HenE.ServerSocket;
    using HenE.VierOPEenRij;
    using HenE.VierOPEenRij.Interface;

    /// <summary>
    /// gaat de stream die van een speler komt behandlen.
    /// </summary>
    public class Handler : ICanHandelen
    {
        private readonly SpelHandler spelHandler = new SpelHandler();

        /// <inheritdoc/>
        public override string StreamOntvanger(string stream, Socket socket)
        {
            if (stream == string.Empty)
            {
                throw new ArgumentNullException("De stream mag niet empty zijn.");
            }

            return this.SplitsDeStream(stream, socket);
        }

        /// <summary>
        /// Geeft de lijst van de spellen terug.
        /// </summary>
        /// <returns>Lijst van de spellen.</returns>
        public List<Game> GetSpellen()
        {
            return this.spelHandler.GetSpellen();
        }

        /// <inheritdoc/>
        protected override string SplitsDeStream(string stream, Socket socket)
        {
            // De stream komt in een zin.
            // die moet knippen worden.
            string[] opgeknipt = stream.Split(new char[] { '%' });

            if (opgeknipt[0] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[1] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[1] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[2] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            return this.Handel(opgeknipt[1], opgeknipt[2], socket);
        }

        /// <summary>
        /// Hande de data.
        /// Is de dimension tussen grens? Als ja dan stuur de data door.
        /// als de dimenion niet tussen de grens is dan stuur de data niet door.
        /// </summary>
        /// <param name="naam">De naam van de speler.</param>
        /// <param name="dim">De dimension die de speler wil mee doen.</param>
        /// <param name="socket">De tcp client van een speler.</param>
        /// <returns>De event.</returns>
        private string Handel(string naam, string dim, Socket socket)
        {
            string returnMessage;

            // Nu hebben we de naam van de speler en de dimension van het speelvlak.
            // eerst check of de dimension geldig of ongeldig is.
            int dimension = this.ConvertToNumber(dim);

            // Het nummer mag aleen teussen 4 en 10.
            if (dimension < 4 || dimension > 10)
            {
                // de dimension is ongeldig.
                // stuur een bericht naar de speler terug en vraag hem op een nieuwe dimension.
                throw new ArgumentOutOfRangeException("De dimension moet tussen 4 en 10.");
            }
            else
            {
                // dan kan de data naar de spelhandeler sturen.
                returnMessage = this.spelHandler.SpelHandlen(naam, dimension, socket);
            }

            return returnMessage;
        }

        /// <summary>
        /// Change the string to int.
        /// </summary>
        /// <param name="dimension">Dimension als string.</param>
        /// <returns>Dimension.</returns>
        private int ConvertToNumber(string dimension)
        {
            if (dimension == string.Empty)
            {
                throw new ArgumentNullException("Mag niet dimebnsion empty zijn.");
            }

            if (!int.TryParse(dimension, out int number))
            {
                throw new ArgumentNullException("Mag niet dimension nul zijn.");
            }

            return number;
        }
    }
}
