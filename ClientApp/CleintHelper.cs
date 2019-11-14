// <copyright file="CleintHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ClientApp
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using HenE.SocketClient;

    /// <summary>
    /// klas om de cliënt application te helpen.
    /// </summary>
    public static class CleintHelper
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
                ColorConsole.WriteLine(ConsoleColor.Red, "Welkom Bij Vier op een rijn!");

                // krijgt de naam van de speler.
                string naam = string.Empty;
                bool isGeldigValue = true;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Wat is je naam?");
                    Console.WriteLine("Je mag alleen letters gebruiken.");
                    naam = Console.ReadLine();
                    char[] letters = naam.ToCharArray();
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

                    if (!isGeldigValue)
                    {
                        // De naam is ongeldig.
                        Console.WriteLine("Ongeldig naam.");
                    }
                }
                while (!isGeldigValue);

                Console.WriteLine();
                ColorConsole.WriteLine(ConsoleColor.Red, $"Hoi {naam}");

                // Vraag de speler om dimension te geven.
                int dimension = 0;
                string antwoord = string.Empty;
                isGeldigValue = false;
                do
                {
                    Thread.Sleep(2000);
                    Console.WriteLine();
                    Console.WriteLine("Hoegroot is de speelveld?");
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Je mag alleen cijfer gebruiken. De cijfers moeten tussen 4 en 10.");
                    antwoord = Console.ReadLine();
                    if (int.TryParse(antwoord, out dimension))
                    {
                        if (dimension < 0)
                        {
                            Console.WriteLine("De cijfer mag niet mider dan nul zijn.");
                        }
                        else if (dimension > 10)
                        {
                            Console.WriteLine("De cijfer mag niet hoger dan 10 zijn.");
                        }
                        else
                        {
                            isGeldigValue = true;
                        }
                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, "Ongeldige value!");
                    }
                }
                while (!isGeldigValue);

                client.VerzoekOmStartenSpel(naam, dimension, clientSocket);
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
