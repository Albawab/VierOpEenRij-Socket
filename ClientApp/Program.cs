// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ClientApp
{
    using System;
    using System.Net.Sockets;
    using ConnectionHelper;
    using HenE.SocketClient;

    /// <summary>
    /// Class om de server te starten.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.Title = "Client";
            Client client = new Client(clientSocket);
            client.Connect();

/*            string naam = "A";
            int dimension = 4;
            client.VerzoekOmStartenSpel(naam, dimension, clientSocket);*/
            Console.ReadLine();
        }
    }
}
