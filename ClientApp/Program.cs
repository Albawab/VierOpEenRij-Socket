// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ClientApp
{
    using System;
    using System.Net.Sockets;
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
            CleintHelper.Start(clientSocket);
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
