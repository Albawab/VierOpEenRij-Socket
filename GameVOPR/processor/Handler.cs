// <copyright file="Handler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.GameVierOpEenRij
{
    using System;
    using HenE.VierOPEenRij;
    using HenE.VierOPEenRij.Enum;
    using HenE.VierOPEenRij.Interface;
    using HenE.VierOPEenRij.Protocol;

    /// <summary>
    /// gaat de stream die van een speler komt behandlen.
    /// </summary>
    public class Handler : ICanHandelen
    {
        private readonly Game game = new Game();

        /// <inheritdoc/>
        public override void StreamOntvanger(string stream)
        {
            if (stream == string.Empty)
            {
                throw new ArgumentNullException("De stream mag niet empty zijn.");
            }

            this.SplitsDeStream(stream);
        }

        /// <inheritdoc/>
        protected override void SplitsDeStream(string stream)
        {
            // De stream komt in een zin.
            // die moet knippen worden.
            // hier knip de stream .

            // Todo Maak de split alleen met % symbol.
            string[] opgeknipt = stream.Split(new char[] { '%' });

            if (opgeknipt[0] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[1] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[1] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            if (opgeknipt[2] == null)
            {
                throw new ArgumentNullException("Ietem mag niet null zijn.");
            }

            // nu hebben we de naam van de speler en de situatie als string.
            // We gaan de situatie verandert tot een enum.

            this.game.HandlSpeler(opgeknipt[0], EnumHepler.EnumConvert<Commandos>(opgeknipt[1]), opgeknipt[2]);
            this.HandlHetSpel();
        }

        /// <summary>
        /// Chack of mag spelen.
        /// Als ja dan staart het spel.
        /// als Nee Wacht op andere speler.
        /// </summary>
        protected override void HandlHetSpel()
        {
            if (this.game.KanStarten())
            {
                SpeelVlak speelVlak = new SpeelVlak(4);
                GameController controller = new GameController(speelVlak, this.game);
                controller.GameStart();
                Console.ReadKey();
            }
        }
    }
}
