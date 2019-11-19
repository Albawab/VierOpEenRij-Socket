﻿// <copyright file="Commandos.cs" company="PlaceholderCompany">
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
        /// Als het spelen mag starten.
        /// </summary>
        Starten,

        /// <summary>
        /// Doe de teken.
        /// </summary>
        ZetTeken,

        /// <summary>
        /// Commando om de inzet van humenspeler naar het spel te sturen.
        /// </summary>
        DoeZet,

        /// <summary>
        /// De speler wil een nieuw ronde doen.
        /// </summary>
        NieuwRonde,

        /// <summary>
        /// De speler wil niet een nieuwe rondje doen.
        /// </summary>
        WilNietNieuweRonde,
    }
}
