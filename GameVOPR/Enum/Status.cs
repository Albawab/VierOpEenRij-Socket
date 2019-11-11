// <copyright file="Status.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.VierOPEenRij.Enum
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Situatie van een speler.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// als de speler heeft nog geen situatie.
        /// </summary>
        Undefined,

        /// <summary>
        /// Als de speler wel gestart.
        /// </summary>
        Gestart,

        /// <summary>
        /// Als de speler wacht op andere speler.
        /// </summary>
        Wachten,

        /// <summary>
        /// Als de speler is gestopt.
        /// </summary>
        Gestopt,
    }
}
