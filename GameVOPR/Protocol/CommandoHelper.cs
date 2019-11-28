// <copyright file="CommandoHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Protocol
{
    /// <summary>
    /// Stelt alle delen in een zin samen.
    /// </summary>
    public static class CommandoHelper
    {
        /// <summary>
        /// creëert een message dat de speler wil spelen.
        /// </summary>
        /// <param name="naam">De naam van een speler.</param>
        /// <param name="dimension">De dimension van het speelvlak die de speler wil mee spelen.</param>
        /// <param name="wilTegenComputerSpelen">Wil Tegen computer spelen of niet.</param>
        /// <returns>De zien die naar de server gaat sturen met de commando , de naam en de dimension.</returns>
        public static string CreëertVerzoekTotDeelnameSpelCommando(string naam, int dimension, bool wilTegenComputerSpelen)
        {
            return string.Format($"{Commandos.VerzoekTotDeelnemenSpel.ToString()}%{naam}%{dimension}%{wilTegenComputerSpelen}");
        }

        /// <summary>
        /// Maak een tekst met de commandos en de teken die de speler heeft gekozen.
        /// </summary>
        /// <param name="teken">De teken die de speler heeft gekozen.</param>
        /// <returns>De tekst die naar de server wil sturen.</returns>
        public static string CreeertZetTekenCommando(string teken)
        {
            return string.Format($"{Commandos.ZetTeken}%{teken}");
        }

        /// <summary>
        /// Maakt een tekst met de commando.
        /// </summary>
        /// <returns>De tekst met de start commando.</returns>
        public static string CreeertStartHetSpelCommando()
        {
            return string.Format($"{Commandos.Starten}");
        }

        /// <summary>
        /// Creeert een commando met een "Doe zet" value.
        /// </summary>
        /// <param name="inzet">inzet.</param>
        /// <returns>tekst die naar het spel gaat met commando en de inzet.</returns>
        public static string CreeertDoeZetCommando(string inzet)
        {
            return string.Format($"{Commandos.DoeZet}%{inzet}");
        }

        /// <summary>
        /// Maak een berichtje dat de speler wil een nieuwe rondje doen.
        /// </summary>
        /// <returns>Het berichtje.</returns>
        public static string CreeertWilNieuweRondje()
        {
            return string.Format($"{Commandos.NieuwRonde}");
        }

        /// <summary>
        /// Maakt een nieuw bericht dat de speler geen nieuwe rondje wil doen.
        /// </summary>
        /// <returns>Het berichtje.</returns>
        public static string CreeertWilNietRonde()
        {
            return string.Format($"{Commandos.WilNietNieuweRonde}");
        }
    }
}
