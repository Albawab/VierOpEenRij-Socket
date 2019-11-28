// <copyright file="Commandos.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Protocol
{
    /// <summary>
    /// Hier staan de Commandos van het spel.
    /// </summary>
    public enum Commandos
    {
        /// <summary>
        /// Zoek naar andere speler.
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
        /// Commando om de inzet van human speler naar het spel te sturen.
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
