// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ClientApp
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Class om de server te starten.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            IPAddress[] iPs = Dns.GetHostAddresses("10.0.0.184");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket clientSocket = socket;
            clientSocket.Connect(iPs, 5000);
            Console.Title = "Client";
            CleintHelper.Start(clientSocket);
        }
    }
}
