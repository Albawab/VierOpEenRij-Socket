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
    }
}
