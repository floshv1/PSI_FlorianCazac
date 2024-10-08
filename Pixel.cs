using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_SouidiCazac
{
    public class Pixel
    {
        #region Fields

        // Variables qui stockent les valeurs des couleurs
        public byte Red;
        public byte Green;
        public byte Blue;
        #endregion

        #region Constructors

        // <summary>
        // Constructeur de la classe Pixel
        // </summary>
        // <param name="red">Valeur de la couleur rouge</param>
        // <param name="green">Valeur de la couleur verte</param>
        // <param name="blue">Valeur de la couleur bleue</param>
        public Pixel(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        // <summary>
        // Constructeur de la classe Pixel
        // </summary>
        // <param name="red">Valeur de la couleur rouge</param>
        // <param name="green">Valeur de la couleur verte</param>
        // <param name="blue">Valeur de la couleur bleue</param>
        public Pixel(int red, int green, int blue)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        #endregion

        #region Methods

        // <summary>
        // Méthode qui retourne un pixel en niveaux de gris
        // </summary>
        // <returns>Pixel en niveaux de gris</returns>
        public Pixel GreyAverage()
        {
            byte grey = (byte)((Red + Green + Blue) / 3);
            return new Pixel(grey, grey, grey);
        }

        // <summary>
        // Methode qui crée un pixel selon la teinte
        // </summary>
        // <param name="hue">Teinte recherchée</param>
        // <returns>Pixel de la teinte donnée</returns>
        public static Pixel Hue(int hue)
        {
            hue %= 360;
            double x = 1 * (1 - Math.Abs((hue / 60.0) % 2 - 1));
            double r,
                g,
                b;

            if (hue < 60)
                (r, g, b) = (1, x, 0);
            else if (hue < 120)
                (r, g, b) = (x, 1, 0);
            else if (hue < 180)
                (r, g, b) = (0, 1, x);
            else if (hue < 240)
                (r, g, b) = (0, x, 1);
            else if (hue < 300)
                (r, g, b) = (x, 0, 1);
            else if (hue < 360)
                (r, g, b) = (1, 0, x);
            else
                (r, g, b) = (0, 0, 0);

            return new Pixel((byte)(b * 255), (byte)(g * 255), (byte)(r * 255));
        }

        #endregion
    }
}
