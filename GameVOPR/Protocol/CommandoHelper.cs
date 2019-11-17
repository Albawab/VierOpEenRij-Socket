// <copyright file="CommandoHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.GameVOPR.Protocol
{
    using HenE.VierOPEenRij.Protocol;

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
        /// <returns>De zien die naar de server gaat sturen met de commando , de naam en de dimension.</returns>
        public static string CreëertVerzoekTotDeelnameSpelCommando(string naam, int dimension)
        {
            return string.Format($"{Commandos.VerzoekTotDeelnemenSpel.ToString()}%{naam}%{dimension}");
        }

        /// <summary>
        /// Maak een message dat de speler tegen de computer wil spelen.
        /// </summary>
        /// <param name="dimension">De dimension van het speelvalk.</param>
        /// <returns>De zien die naar de server gaat sturen.</returns>
        public static string CreeertSpeelTegenComputerCommando(int dimension)
        {
            return string.Format($"{Commandos.SpeelTegenComputer.ToString()}%Computer%{dimension}");
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
        /// <returns>text die naar het spel gaat met commando en de inzet.</returns>
        public static string CreeertDoeZetCommando(string inzet)
        {
            return string.Format($"{Commandos.DoeZet}%{inzet}");
        }
    }
}
