// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Server
{
    using System.Net.Sockets;

    /// <summary>
    /// Class om de server te starten.
    /// </summary>
    public static class Program
    {
        private static void Main()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerProcess server = new ServerProcess(serverSocket);
            server.SetupServer();
        }
    }
}
