// <copyright file="Events.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij.Protocol
{
    /// <summary>
    /// Hier staan de events van het spel.
    /// </summary>
    public enum Events
    {
        /// <summary>
        /// Het spel wordt gecoreéerd.
        /// </summary>
        GecreeerdSpel,

        /// <summary>
        /// Tweede speler heeft ingevoegd.
        /// </summary>
        SpelerInvoegde,

        /// <summary>
        /// Het spel is gestart.
        /// </summary>
        Gestart,

        /// <summary>
        /// Bord geteken.
        /// </summary>
        BordGetekend,

        /// <summary>
        /// Als de teken heeft ingezet.
        /// </summary>
        TekenIngezet,

        /// <summary>
        /// De speler is in de buurt.
        /// </summary>
        JeRol,

        /// <summary>
        /// Als de speler mag niet deze nummer gpruiken.
        /// </summary>
        OngeldigInzet,

        /// <summary>
        /// Als de speler moet wachten.
        /// </summary>
        Wachten,

        /// <summary>
        /// Het bord vol geworden.
        /// </summary>
        HetBordVolGeworden,

        /// <summary>
        /// Een speler heeft gewonnen.
        /// </summary>
        HeeftGewonnen,

        /// <summary>
        /// De tegen speler heeft het spel verlaten.
        /// </summary>
        TegenSpelerVerlaten,
    }
}
