// <copyright file="CanHandelen.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Interface
{
    using System.Net.Sockets;

    /// <summary>
    /// interface waar de handeler de stream gaat handelen.
    /// </summary>
    public abstract class CanHandelen
    {
        /// <summary>
        /// Ontvangt de stream.
        /// controleert de stream.
        /// </summary>
        /// <param name="stream">De stream die van een persoon komt.</param>
        /// <param name="socket">De tcp client van een speler.</param>
        /// <returns>De events.</returns>
        public abstract string StreamOntvanger(string stream, Socket socket);

        /// <summary>
        /// Functie om de stream te splitsen.
        /// </summary>
        /// <param name="stream">De stream die van een persoon komt.</param>
        /// <param name="socket">De tcp client van een speler.</param>
        /// <returns>De events.</returns>
        protected abstract string SplitsDeStream(string stream, Socket socket);
    }
}
