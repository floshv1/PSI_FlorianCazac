using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using ConsoleAppVisuals;
using ConsoleAppVisuals.AnimatedElements;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;

namespace PSI_SouidiCazac
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Window.Open();
            Title title = new Title("Image-In");
            Window.AddElement(title);
            Window.Render();
            Interface.MainMenu();
        }

       
        /// <summary>
        /// Méthode pour afficher le menu principal
        /// </summary>
        /// <param name="width">Largeur de l'image</param>
        /// <param name="height">Hauteur de l'image</param>
        /// <param name="depth">Nombre d"itération</param>
        /// <returns>Une image de la fractale de Mandelbrot</returns>
        public static MyImage MandelbrotSet(int width = 1000, int height = 1000, int depth = 200)
        {
            MyImage img = new MyImage(height, width);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Complex c = new Complex(2 * i / (double)width - 1, 2 * j / (double)height - 1);
                    Complex z = new Complex(0, 0);
                    for (int cpt = 0; cpt < depth && z.Modulus() < 2; cpt++)
                    {
                        z = z * z + c;
                        if (cpt == depth)
                        {
                            img[i, j] = new Pixel(0, 0, 0);
                        }
                        else
                        {
                            img[i, j] = Pixel.Hue((int)(360 * cpt / depth));
                        }
                    }
                }
            }
            return img;
        }

        /// <summary>
        /// Méthode qui génère une image de la fractale de Julia
        /// </summary>
        /// <param name="re">Partie réelle de la constante c</param>
        /// <param name="im">Partie imaginaire de la constante c</param>
        /// <param name="height">Hauteur de l'image</param>
        /// <param name="width">Largeur de l'image</param>
        /// <param name="depth">Nombre d'itération </param>
        /// <returns>Une image de la fractale de Julia</returns>
        public static MyImage JuliaSet(
            double re = -0.4,
            double im = 0.6,
            int width = 1000,
            int height = 1000,
            int depth = 200
        )
        {
            MyImage img = new MyImage(height, width);
            Complex c = new Complex(re, im);
            for (int i = 0; i < img.GetLength(0); i++)
            {
                for (int j = 0; j < img.GetLength(1); j++)
                {
                    Complex z = new Complex(2 * i / (double)width - 1, 2 * j / (double)height - 1);

                    for (int cpt = 0; cpt < depth && z.Modulus() < 2; cpt++)
                    {
                        z = z * z + c;

                        if (cpt == depth)
                        {
                            img[i, j] = new Pixel(0, 0, 0);
                        }
                        else
                        {
                            img[i, j] = Pixel.Hue((int)(360 * cpt / depth));
                        }
                    }
                }
            }
            return img;
        }
    }
}
