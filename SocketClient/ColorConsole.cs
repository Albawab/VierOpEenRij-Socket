// <copyright file="ColorConsole.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.SocketClient
{
    using System;

    /// <summary>
    /// Klas om de de kleur van de tekst te veranderen.
    /// </summary>
    public static class ColorConsole
    {
        /// <summary>
        /// Write an new line with a color.
        /// </summary>
        /// <param name="color">De nieuwe kleur.</param>
        /// <param name="tekst">De tekst die de kleur wordt verandert.</param>
        public static void WriteLine(ConsoleColor color, string tekst)
        {
            // geeft de tekst deze kleur.
            Console.ForegroundColor = color;

            // Schrijft de tekst op nieuwe lijn.
            Console.WriteLine(tekst);

            // Reset color te basic color.
            Console.ResetColor();
        }

        /// <summary>
        /// Write an line with a color.
        /// </summary>
        /// <param name="color">De nieuwe kleur.</param>
        /// <param name="tekst">De tekst die de kleur wordt verandert.</param>
        public static void Write(ConsoleColor color, string tekst)
        {
            // Geeft de tekst deze kleur.
            Console.ForegroundColor = color;

            // Schrijft de tekst op de zelfde lijn.
            Console.Write(tekst);

            // Reset color te basic color.
            Console.ResetColor();
        }
    }
}
