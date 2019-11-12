// <copyright file="Commandos.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Hier staan de Commandos van het spel.
    /// </summary>
    public enum Commandos
    {
        /// <summary>
        /// Zoek naar andre speler.
        /// </summary>
        VerzoekTotDeelnemenSpel,

        /// <summary>
        /// Als de speler moet wachten.
        /// </summary>
        Wachten,

        /// <summary>
        /// Als de speler wil tegen de computer spelen.
        /// </summary>
        SpeelTegenComputer,
    }
}
