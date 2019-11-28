// <copyright file="Events.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Protocol
{
    /// <summary>
    /// Hier staan de events van het spel.
    /// </summary>
    public enum Events
    {
        /// <summary>
        /// Het spel wordt gecreëerd.
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
        /// Bord getekend.
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
        /// Als de speler mag niet deze nummer gebruiken.
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
        /// De tegenspeler heeft het spel verlaten.
        /// </summary>
        TegenSpelerVerlaten,

        /// <summary>
        /// Het spel is gestopt.
        /// </summary>
        SpelGestopt,

        /// <summary>
        /// De tegenspeler heeft gewonnen.
        /// </summary>
        HeeftGewonnenTegenSpeler,

        /// <summary>
        /// Het spel is verwijderd.
        /// </summary>
        SpelVerwijderd,

        /// <summary>
        /// Als de naam van een speler is veranderd dan laat hem dat weten.
        /// </summary>
        NaamVeranderd,

        /// <summary>
        /// Wachten of een reactie.
        /// </summary>
        WachtOPReactie,

        /// <summary>
        /// Als de tegenspeler heeft het huidig spel verlaten en de speler wil nog spelen dan laat hem weten dat
        /// hij moet op nieuwe speler wachten.
        /// </summary>
        WachtenNieuweSpeler,

        /// <summary>
        /// Als er een spel heeft gevonden.
        /// </summary>
        SpelGevonden,

        /// <summary>
        /// De tegenspeler heeft zijn teken ingezet.
        /// </summary>
        TegenSpelerHeeftTekenIngezet,
    }
}
