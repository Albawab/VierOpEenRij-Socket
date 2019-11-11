// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Server
{
    using System;
    using System.Net.Sockets;
    using HenE.ServerSocket;

    /// <summary>
    /// Class om de server te starten.
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerProcess server = new ServerProcess(serverSocket);
            Console.Title = "Server";
            server.SetupServer();
            Console.ReadKey();
        }
    }
}
