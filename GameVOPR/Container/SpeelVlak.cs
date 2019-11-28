// <copyright file="SpeelVlak.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HenE.Games.VierOpEenRij.Container
{
    using System;
    using System.Text;
    using HenE.Games.VierOpEenRij.Enum;

    /// <summary>
    /// Klas om speelvlak te teken.
    /// Reset het speelvlak.
    /// controleert of heeft speler gewonnen.
    /// </summary>
    public class SpeelVlak
    {
        private readonly Teken[,] veldenInHetSpeelvlak = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeelVlak"/> class.
        /// </summary>
        /// <param name="dimension">De grootte van het speelvlak.</param>
        public SpeelVlak(int dimension)
        {
            this.veldenInHetSpeelvlak = new Teken[dimension, dimension];
            this.Dimension = dimension;
        }

        /// <summary>
        /// Gets dimension of the speelvlak.
        /// </summary>
        public int Dimension { get; private set; }

        /// <summary>
        /// Teken het speelvlak.
        /// </summary>
        /// <returns>de speelvlak.</returns>
        public string TekenSpeelvlak()
        {
            StringBuilder teken = new StringBuilder();

            // Schrijft het eerste de nummer van een kolom.
            for (int columnNummer = 1; columnNummer <= this.Dimension; columnNummer++)
            {
                teken.Append($"    {columnNummer}    ");
            }

            teken.AppendLine();
            teken.AppendLine();

            for (int columnNummer = 0; columnNummer < this.Dimension; columnNummer++)
            {
                string line = string.Empty;
                for (int doorKolom = 0; doorKolom < this.Dimension; doorKolom++)
                {
                    if (this.veldenInHetSpeelvlak[doorKolom, columnNummer] == Teken.Undefined)
                    {
                        if (doorKolom == 0)
                        {
                            teken.Append("      ");
                        }
                        else
                        {
                            teken.Append("   ");
                        }
                    }
                    else
                    {
                        if (doorKolom == 0)
                        {
                            teken.Append("   ");
                        }

                        teken.Append($" {this.veldenInHetSpeelvlak[doorKolom, columnNummer].ToString()} ");
                    }

                    if (columnNummer < this.Dimension)
                    {
                        teken.Append("  |   ");
                        if (doorKolom < this.Dimension)
                        {
                            line += "--------+";
                        }
                    }
                }

                teken.AppendLine();
                teken.Append(line);
                teken.AppendLine();
            }

            return teken.ToString();
        }

        /// <summary>
        /// Reset the table.
        /// </summary>
        public void ResetSpeelVlak()
        {
            for (int rij = 0; rij < this.Dimension; rij++)
            {
                for (int column = 0; column < this.Dimension; column++)
                {
                    this.ResetVeld(rij, column);
                }
            }
        }

        /// <summary>
        /// Reset one cell.
        /// </summary>
        /// <param name="rij">Het nummer  van de rij.</param>
        /// <param name="column">Het nummer van de column.</param>
        public void ResetVeld(int rij, int column)
        {
            this.veldenInHetSpeelvlak[rij, column] = Teken.Undefined;
        }

        /// <summary>
        /// Check of de column nog een vrij veld heeft of niet.
        /// </summary>
        /// <param name="columnNummer">De nummer van de column waar de method door loopt.</param>
        /// <returns>Heeft deze column een vrij veld of niet.</returns>
        public bool MagInzetten(int columnNummer)
        {
            if (columnNummer > this.Dimension)
            {
                throw new ArgumentException("Het nummer mag niet hoger dan de dimensie  van het speelvlak.");
            }

            if (columnNummer < 0)
            {
                throw new ArgumentException("Het nummer mag niet minder dan de dimensie  van het speelvlak.");
            }

            for (int kolom = 0; kolom < this.Dimension; kolom++)
            {
                if (this.veldenInHetSpeelvlak[columnNummer, kolom] == Teken.Undefined)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Check of het speelvlak vol of nog niet helemaal is.
        /// </summary>
        /// <returns>Is het speelvlak vol of nog niet.</returns>
        public bool IsSpeelvlakVol()
        {
            for (int rij = 0; rij < this.Dimension; rij++)
            {
                for (int column = 0; column < this.Dimension; column++)
                {
                    if (this.veldenInHetSpeelvlak[rij, column] == Teken.Undefined)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Zet de teken op het speelvlak.
        /// </summary>
        /// <param name="inzet">Waar de speler wil zetten.</param>
        /// <param name="teken">De teken van de speler.</param>
        /// <returns>Het nummer in de kolom die wordt gebruiken om teken te zetten.</returns>
        public int ZetTekenOpSpeelvlak(int inzet, Teken teken)
        {
            int inzetNummer = 0;
            for (int kolom = 0; kolom < this.Dimension; kolom++)
            {
                // als de veld vrij is.
                if (this.veldenInHetSpeelvlak[inzet, kolom] == Teken.Undefined)
                {
                    inzetNummer = kolom;
                    if (kolom == this.Dimension - 1)
                    {
                        // zet teken als de veld vrij is.
                        this.veldenInHetSpeelvlak[inzet, kolom] = teken;
                        return inzetNummer;
                    }
                }
                else
                {
                    if (this.veldenInHetSpeelvlak[inzet, inzetNummer] == Teken.Undefined)
                    {
                        this.veldenInHetSpeelvlak[inzet, inzetNummer] = teken;
                        return inzetNummer;
                    }
                }
            }

            return inzetNummer;
        }

        /// <summary>
        /// controleert of het speelvlak heeft het zelfde teken op vier velden.
        /// </summary>
        /// <param name="teken">De teken.</param>
        /// <returns>Heeft de speler gewonnen of niet.</returns>
        public bool HeeftGewonnen(Teken teken)
        {
            if (teken == Teken.Undefined)
            {
                throw new ArgumentException("Mag teken hier niet undefined.");
            }

            bool heeftIemandGewonnen = false;

            // wanneer heeft een teken gewonnen?
            // horizontaal een hele rij
            // roep voor elke row op het bord de functie AreAllFieldsInTheRowEqual aan
            for (int rij = 0; rij < this.Dimension && !heeftIemandGewonnen; rij++)
            {
                heeftIemandGewonnen = this.ZijnErVierTekenGelijkInEenKolom(rij, teken);
                if (heeftIemandGewonnen)
                {
                    return true;
                }
            }

            // verticaal een hele rij
            for (int col = 0; col < this.Dimension && !heeftIemandGewonnen; col++)
            {
                heeftIemandGewonnen = this.ZijnErVierTekenGelijkInEenRij(col, teken);

                if (heeftIemandGewonnen)
                {
                    return true;
                }
            }

            // richtsboven naar linksonder.
            if (this.ControleertRichtsbovenNaarLinksOnder(teken))
            {
                return true;
            }

            // Linksboven naar richtsonder.
            if (this.ControleertLinksBovenNaarRichtsOnder(teken))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// controleer of de rij heeft het zelfde teken.
        /// </summary>
        /// <param name="rij">Het loopt door dezen nummer.</param>
        /// <param name="teken">teken.</param>
        /// <returns>Heeft de functie vier vilden zijn gelijk of niet.</returns>
        private bool ZijnErVierTekenGelijkInEenRij(int rij, Teken teken)
        {
            int aantalTekenInEenrij = 0;
            for (int kolomNummer = 0; kolomNummer < this.Dimension; kolomNummer++)
            {
                if (aantalTekenInEenrij <= 4)
                {
                    if (this.veldenInHetSpeelvlak[rij, kolomNummer] == teken)
                    {
                        aantalTekenInEenrij++;
                    }
                    else
                    {
                        aantalTekenInEenrij = 0;
                    }
                }
            }

            if (aantalTekenInEenrij >= 4)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Controleert of de er zijn vier teken in de kolom zijn gelijk.
        /// </summary>
        /// <param name="kolomNummer">Het nummer van de kolom.</param>
        /// <param name="teken">De teken die wordt gecontroleerd.</param>
        /// <returns>Heeft de functie vier vilden zijn gelijk of niet.</returns>
        private bool ZijnErVierTekenGelijkInEenKolom(int kolomNummer, Teken teken)
        {
            // Eigenlijk is door Kolom is de rij.
            int aantalTekenInEenrij = 0;

            for (int doorKolom = 0; doorKolom < this.Dimension; doorKolom++)
            {
                if (aantalTekenInEenrij <= 4)
                {
                    if (this.veldenInHetSpeelvlak[doorKolom, kolomNummer] == teken)
                    {
                        aantalTekenInEenrij++;
                    }
                    else
                    {
                        aantalTekenInEenrij = 0;
                    }
                }

                if (aantalTekenInEenrij >= 4)
                {
                    return true;
                }
            }

            return false;
        }

        // richtboven naar links onder.

        /// <summary>
        /// Controleert of de speler heeft vier op een rij vanaf richtboven naar links onder.
        /// </summary>
        /// <param name="teken">De teken die wordt gecontroleerd.</param>
        /// <returns>Heeft vier op een rij of niet.</returns>
        private bool ControleertRichtsbovenNaarLinksOnder(Teken teken)
        {
            int aantalTekenOpEenRij = 0;

            // start van af kolom 3 t/m de grootte.
            // Hier gaat |"""". boven naar links. boven - - >
            // linksboven naar beneden.
            for (int kolom = 3; kolom < this.Dimension; kolom++)
            {
                int kolomNummer = kolom;
                int rijNummer = 0;
                for (int rij = 0; rij < kolom + 1; rij++)
                {
                    // Als de speler een teken op een rij heeft dan voeg een nummer aan de anntalTeken toe.
                    if (this.veldenInHetSpeelvlak[kolomNummer--, rijNummer++] == teken)
                    {
                        aantalTekenOpEenRij++;
                    }

                    // Als de speler niet meer teken op een rij heeft dan doe de aantalTeken nul.
                    else
                    {
                        aantalTekenOpEenRij = 0;
                    }

                    // Als de teken vier op een rij heeft dan geef true terug.
                    if (aantalTekenOpEenRij >= 4)
                    {
                        return true;
                    }
                }

                // We gaan naar een nieuwe kolom en rij dus maak de aantalTeken nul.
                aantalTekenOpEenRij = 0;
            }

            // Hier gaat de speelvlak naar onder __|.
            // vanaf boven naar beneden.
            // links --> naar richt.
            int doorKolomLopen = this.Dimension;
            for (int rij = 0; rij < this.Dimension - 1; rij++)
            {
                doorKolomLopen--;
                int rijNummer = rij;
                int kolomNummer = this.Dimension - 1;
                for (int kolom = doorKolomLopen; kolom >= 0; kolom--)
                {
                    // als de speler een teken op een rij heeft dan voeg een nummer aan de aantalTeken toe.
                    if (this.veldenInHetSpeelvlak[kolomNummer--, rijNummer++] == teken)
                    {
                        aantalTekenOpEenRij++;
                    }

                    // Als de speler niet meer teken op een rij heeft dan doe de aantal Teken nul.
                    else
                    {
                        aantalTekenOpEenRij = 0;
                    }

                    // Als de teken vier op een rij heeft dan geef true terug.
                    if (aantalTekenOpEenRij >= 4)
                    {
                        return true;
                    }
                }

                aantalTekenOpEenRij = 0;
            }

            // Als de teken vier op een rij heeft dan geef true terug.
            return aantalTekenOpEenRij == 4;
        }

        // links boven naar richt onder

        /// <summary>
        /// Controleert of de speler heeft vier op een rij van af links boven naar richt onder.
        /// </summary>
        /// <param name="teken">De teken die wordt gecontroleerd.</param>
        /// <returns>Heeft vier op een rij of niet.</returns>
        private bool ControleertLinksBovenNaarRichtsOnder(Teken teken)
        {
            int aantalTekenOpEenRij = 0;
            int doorRijLopen = this.Dimension;

            // start vanaf linksboven naar richtsonder.
            // Linksboven .
            for (int kolom = 0; kolom < this.Dimension - 3; kolom++)
            {
                doorRijLopen--;
                int kolomNummer = kolom;
                int rijNummer = 0;
                for (int rij = doorRijLopen; rij >= 0; rij--)
                {
                    // Als de speler een teken op een rij heeft dan voeg een nummer aan de aantal Teken toe.
                    if (this.veldenInHetSpeelvlak[kolomNummer++, rijNummer++] == teken)
                    {
                        aantalTekenOpEenRij++;
                    }

                    // Als de speler niet meer teken op een rij heeft dan doe de aantal Teken nul.
                    else
                    {
                        aantalTekenOpEenRij = 0;
                    }

                    if (aantalTekenOpEenRij >= 4)
                    {
                        return true;
                    }
                }

                aantalTekenOpEenRij = 0;
            }

            // links start.
            doorRijLopen = this.Dimension;
            for (int kolom = 0; kolom < this.Dimension - 3; kolom++)
            {
                doorRijLopen--;
                int kolomNummer = 0;
                int rijNummer = kolom;
                for (int rij = doorRijLopen; rij >= 0; rij--)
                {
                    // Als de speler een teken op een rij heeft dan voeg een nummer aan de aantal Teken toe.
                    if (this.veldenInHetSpeelvlak[kolomNummer++, rijNummer++] == teken)
                    {
                        aantalTekenOpEenRij++;
                    }

                    // Als de speler niet meer teken op een rij heeft dan doe de aantal Teken nul.
                    else
                    {
                        aantalTekenOpEenRij = 0;
                    }

                    if (aantalTekenOpEenRij >= 4)
                    {
                        return true;
                    }
                }

                aantalTekenOpEenRij = 0;
            }

            return false;
        }
    }
}
