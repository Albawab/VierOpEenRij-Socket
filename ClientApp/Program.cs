// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.ClientApp
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Class om de server te starten.
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                IPAddress[] iPs = Dns.GetHostAddresses("10.0.0.184");
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket clientSocket = socket;
                clientSocket.Connect(iPs, 5000);
                Console.Title = "Client";
                ClientHelper.Start(clientSocket);
            }
            catch
            {
                Console.WriteLine("De server is buiten dienst.");
                Console.WriteLine("Probeer het later.");
                Console.WriteLine("Tot ziens!");
            }
        }
    }
}
