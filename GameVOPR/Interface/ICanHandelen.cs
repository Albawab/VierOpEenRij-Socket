// <copyright file="ICanHandelen.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// interface waar de handeler de stream gaat handelen.
    /// </summary>
    public abstract class ICanHandelen
    {
        /// <summary>
        /// Ontvangt de stream.
        /// controlleert de stream.
        /// </summary>
        /// <param name="streamk">De stream die van een persoon komt.</param>
        /// <param name="socket">De tcp client van een speler.</param>
        /// <returns>De events.</returns>
        public abstract string StreamOntvanger(string streamk, Socket socket);

        /// <summary>
        /// Functie om de stream te splitsen.
        /// </summary>
        /// <param name="stream">De stream die van een persoon komt.</param>
        /// <param name="socket">De tcp client van een speler.</param>
        /// <returns>De events.</returns>
        protected abstract string SplitsDeStream(string stream, Socket socket);
    }
}
