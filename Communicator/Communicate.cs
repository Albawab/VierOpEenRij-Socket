﻿// <copyright file="Communicate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.ConnectionHelper
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Doe conection tussen de server en een client.
    /// </summary>
    public abstract class Communicate
    {
        /// <summary>
        /// handlt de stream tussen de client en de server.
        /// </summary>
        /// <param name="message">De message die wordt gehandled.</param>
        /// <param name="socket">De Socket.</param>
        public abstract void ProcessStream(string message, Socket socket);

        /// <summary>
        /// Stuurt een bericht tussen de client en de server.
        /// </summary>
        /// <param name="handler">De client.</param>
        /// <param name="data">Het berichtje.</param>
        public void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(this.SendCallback), handler);
        }

        /// <summary>
        /// Laat weten dat de client of de server een bericht heeft ontvangen.
        /// </summary>
        /// <param name="ar">the result.</param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // get the socket from the object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
               handler.EndSend(ar);

                /*                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
