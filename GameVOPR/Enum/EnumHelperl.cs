// <copyright file="EnumHelperl.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Enum
{
    using System;

    /// <summary>
    /// helpt de situatie.
    /// </summary>
    public static class EnumHelperl
    {
        /// <summary>
        /// Omzetten de message van string tot een enum.
        /// </summary>
        /// <param name="value">De situatie als string.</param>
        /// <returns>Geeft de situatie terug.</returns>
        public static T EnumConvert<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
