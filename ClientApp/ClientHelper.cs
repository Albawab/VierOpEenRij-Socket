// <copyright file="ClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.ClientApp
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using HenE.Games.VierOpEenRij.SocketClient;

    /// <summary>
    /// klas om de cliënt application te helpen.
    /// </summary>
    public static class ClientHelper
    {
        /// <summary>
        /// Vraagt een speler om zijn naam te geven, ook om de dimension te geven.
        /// </summary><param name="clientSocket">De cleint van de speler.</param>
        public static void Start(Socket clientSocket)
        {
            try
            {
                Client client = new Client(clientSocket);

                // gaat en maakt een contact met de server.
                client.Connect();
                ColorConsole.WriteLine(ConsoleColor.Red, "Welkom Bij Vier op een rij!");

                Console.WriteLine();
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Wat leuk dat je komt spelen.");

                // krijgt de naam van de speler.
                string naam = string.Empty;
                bool isGeldigValue = true;
                do
                {
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine("Wat is je naam?");
                    Console.WriteLine("Je mag alleen letters gebruiken.");

                    /*var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

                    if(regexItem.IsMatch(YOUR_STRING)){..}*/
                    naam = Console.ReadLine();
                    char[] letters = naam.ToCharArray();
                    if (letters.Length < 15)
                    {
                        for (int index = 0; index < letters.Length; index++)
                        {
                            if (!char.IsLetter(letters[index]))
                            {
                                // nu de naam is niet geldig.
                                isGeldigValue = false;
                            }
                            else
                            {
                                // als de naam geldig is dan laat de program doorgaat.
                                isGeldigValue = true;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Je mag niet meer dan 15 letters gebruiken.");
                        isGeldigValue = false;
                    }

                    if (!isGeldigValue || naam == string.Empty)
                    {
                        isGeldigValue = false;
                        Console.WriteLine();

                        // De naam is ongeldig.
                        Console.WriteLine("Ongeldig naam.");
                    }
                }
                while (!isGeldigValue);

                Console.WriteLine();
                ColorConsole.WriteLine(ConsoleColor.Red, $"Hoi {naam}");

                int dimension = client.KrijgDimension();

                bool tegenComputerSpelen = false;
                if (client.WilTegenComputerSpelen())
                {
                    tegenComputerSpelen = true;
                }

                client.SetValue(naam);
                client.VerzoekOmStartenSpel(naam, dimension, tegenComputerSpelen, clientSocket);
                client.RequestLoop();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine("--");
                Console.WriteLine(exp.ToString());
                Console.ReadLine();
            }
        }
    }
}
