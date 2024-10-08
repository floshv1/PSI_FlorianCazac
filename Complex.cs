using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_SouidiCazac
{
    public class Complex
    {
        #region Fields

        // Partie réelle
        public double Re;

        // Partie imaginaire
        public double Im;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur de la classe Complex
        /// </summary>
        /// <param name="real">Partie réelle</param>
        /// <param name="imaginary">Partie imaginaire</param>
        /// <returns> Un complexe </returns>
        public Complex(double real, double imaginary)
        {
            Re = real;
            Im = imaginary;
        }
        #endregion

        #region Operators

        /// <summary>
        /// Initialisation de l'opérateur +
        /// </summary>
        /// <param name="a">Premier complexe</param>
        /// <param name="b">Deuxième complexe</param>
        /// <returns>La somme des deux complexes</returns>

        public static Complex operator +(Complex a, Complex b)
        {
            return new(a.Re + b.Re, a.Im + b.Im);
        }

        /// <summary>
        /// Initialisation de l'opérateur -
        /// </summary>
        /// <param name="a">Premier complexe</param>
        /// <param name="b">Deuxième complexe</param>
        /// <returns>La différence des deux complexes</returns>

        public static Complex operator -(Complex a, Complex b)
        {
            return new(a.Re - b.Re, a.Im - b.Im);
        }

        /// <summary>
        /// Initialisation de l'opérateur *
        /// </summary>
        /// <param name="a">Premier complexe</param>
        /// <param name="b">Deuxième complexe</param>
        /// <returns>Le produit des deux complexes</returns>

        public static Complex operator *(Complex a, Complex b)
        {
            return new(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
        }

        /// <summary>
        /// Initialisation de l'opérateur /
        /// </summary>
        /// <param name="a">Premier complexe</param>
        /// <param name="b">Deuxième complexe</param>
        /// <returns>Le quotient des deux complexes</returns>

        public static Complex operator /(Complex a, Complex b)
        {
            return new(
                (a.Re * b.Re + a.Im * b.Im) / (b.Re * b.Re + b.Im * b.Im),
                (a.Im * b.Re - a.Re * b.Im) / (b.Re * b.Re + b.Im * b.Im)
            );
        }
        #endregion

        #region Utilitary

        /// <summary>
        /// Méthode qui calcule le module d'un complexe
        /// </summary>
        /// <returns>Le module du complexe</returns>
        public double Modulus()
        {
            return (double)Math.Sqrt(Re * Re + Im * Im);
        }

        /// <summary>
        /// Méthode qui compare deux complexes
        /// </summary>
        /// <param name="other">Deuxième complexe</param>
        /// <returns>True si les deux complexes sont égaux, false sinon</returns>
        public bool Equals(Complex other)
        {
            return Re == other.Re && Im == other.Im;
        }

        /// <summary>
        /// Méthode qui retourne une représentation textuelle du complexe
        /// </summary>
        /// <returns> Le complexe sous forme de texte</returns>
        public override string ToString()
        {
            if (Im == 0)
                return $"{Re}";
            else if (Re == 0)
                return $"{Im}i";
            else if (Im < 0)
                return $"{Re} - {-Im}i";
            else
                return $"{Re} + {Im}i";
        }
        #endregion
    }
}
