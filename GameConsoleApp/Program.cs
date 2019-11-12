// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.GameVierOpEenRij
{
    using System;
    using HenE.VierOPEenRij.Interface;

    /// <summary>
    /// Console Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main Method.
        /// </summary>
        /// <param name="args">arg.</param>
        private static void Main(string[] args)
        {
            ICanHandelen handler = new Handler();

           // handler.StreamOntvanger("A%Wachten%O");
           // handler.StreamOntvanger("Computer%Gestart%X");
            Console.ReadLine();
        }
    }
}
