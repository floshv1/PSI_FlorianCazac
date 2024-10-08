using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConsoleAppVisuals;
using ConsoleAppVisuals.InteractiveElements;

namespace PSI_SouidiCazac
{
    public class Transformation
    {
        /// <summary>
        /// Convertit une image en niveaux de gris
        /// </summary>
        /// <param name="img">Image à convertir</param>
        /// <returns>Une image en niveaux de gris</returns>
        public static MyImage GreyScale(MyImage img)
        {
            MyImage newImage = new MyImage(img.GetLength(0), img.GetLength(1));
            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    Pixel pixel = img[x, y];
                    newImage[x, y] = pixel.GreyAverage();
                }
            }
            return newImage;
        }

        /// <summary>
        /// Convertit une image en noir et blanc
        /// </summary>
        /// <param name="img">Image à convertir</param>
        /// <returns>Une image en noir et blanc</returns>
        public static MyImage BlackAndWhite(MyImage img)
        {
            MyImage newImage = new MyImage(img.GetLength(0), img.GetLength(1));
            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    Pixel pixel = img[x, y];
                    pixel = pixel.GreyAverage();
                    // Si la valeur de la couleur est supérieure à 128, on met le pixel en blanc
                    // Sinon, on le met en noir
                    if (pixel.Blue > 128)
                    {
                        newImage[x, y] = new Pixel(255, 255, 255);
                    }
                    else
                    {
                        newImage[x, y] = new Pixel(0, 0, 0);
                    }
                }
            }
            return newImage;
        }

        /// <summary>
        /// Applique un filtre négatif à une image
        /// </summary>
        /// <param name="img">Image à modifier</param>
        /// <returns>Une image avec un filtre négatif</returns>
        public static MyImage Negative(MyImage img)
        {
            MyImage newImage = new MyImage(img.GetLength(0), img.GetLength(1));
            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    Pixel pixel = img[x, y];
                    // On soustrait la valeur de chaque couleur à 255 pour obtenir le négatif
                    newImage[x, y] = new Pixel(
                        (byte)(255 - pixel.Red),
                        (byte)(255 - pixel.Green),
                        (byte)(255 - pixel.Blue)
                    );
                }
            }
            return newImage;
        }
        /// <summary>
        /// Manipulation de l'image pour le tourner
        /// </summary>
        /// <param name="img">Image à manipuler</param>
        /// <returns>Une image tournée</returns>
        public static MyImage Rotation(MyImage img)
        {
            double degree;
            bool isValid = false;
            do
            {
                Prompt prompt = new Prompt("Entrer l'angle de rotation en degres: ");
                Window.AddElement(prompt);
                Window.ActivateElement(prompt);
                isValid = double.TryParse(prompt.GetResponse().Value, out degree);
            } while (degree < 0 || degree > 360 || !isValid);

            // Conversion de l'angle en radians
            double angle = degree * Math.PI / 180;
            // Calcul des dimensions de la nouvelle image
            int newWidth = (int)(
                img.GetLength(0) * Math.Abs(Math.Cos(angle))
                + img.GetLength(1) * Math.Abs(Math.Sin(angle))
            );
            int newHeight = (int)(
                img.GetLength(0) * Math.Abs(Math.Sin(angle))
                + img.GetLength(1) * Math.Abs(Math.Cos(angle))
            );

            MyImage newImage = new MyImage(newWidth, newHeight);

            // On parcourt chaque pixel de la nouvelle image
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    // On calcule les coordonnées du pixel dans l'image d'origine
                    // en fonction de l'angle de rotation
                    double x0 =
                        (x - newWidth / 2) * Math.Cos(angle)
                        - (y - newHeight / 2) * Math.Sin(angle)
                        + img.GetLength(0) / 2;
                    double y0 =
                        (x - newWidth / 2) * Math.Sin(angle)
                        + (y - newHeight / 2) * Math.Cos(angle)
                        + img.GetLength(1) / 2;

                    // Si les coordonnées sont dans l'image d'origine, on copie le pixel
                    if (x0 >= 0 && x0 < img.GetLength(0) && y0 >= 0 && y0 < img.GetLength(1))
                    {
                        newImage[x, y] = img[(int)x0, (int)y0];
                    }
                }
            }
            return newImage;
        }

        /// <summary>
        /// Redimensionne une image
        /// </summary>
        /// <param name="img">Image à redimensionner</param>
        /// <param name="scale">Facteur de redimensionnement</param>
        /// <returns>Une image redimensionnée</returns>
        public static MyImage Resize(MyImage img, float scale = 1.0f)
        {
            bool isValid = false;
            if (scale == 1.0f)
            {
                do
                {
                    Prompt prompt = new Prompt("Entrer le facteur de redimensionnement : ");
                    Window.AddElement(prompt);
                    Window.ActivateElement(prompt);
                    isValid = float.TryParse(prompt.GetResponse().Value, out scale);
                } while (scale <= 0 || !isValid);
            }

            // Calcul des dimensions de la nouvelle image
            int height = (int)(img.GetLength(0) * scale);
            int width = (int)(img.GetLength(1) * scale);

            MyImage newImage = new MyImage(height, width);

            for (int x = 0; x < newImage.GetLength(0); x++)
            {
                for (int y = 0; y < newImage.GetLength(1); y++)
                {
                    // On copie le pixel de l'image d'origine à la position correspondante
                    newImage[x, y] = img[(int)(x / scale), (int)(y / scale)];
                }
            }
            return newImage;
        }

        /// <summary>
        /// Applique un filtre de convolution à une image
        /// </summary>
        /// <param name="img">Image à modifier</param>
        /// <param name="kernel">Matrice de convolution</param>
        /// <returns>Une image modifiée</returns>
        public static MyImage ApplyConvolution(MyImage img, float[,] kernel)
        {
            MyImage newImage = new MyImage(img.GetLength(0), img.GetLength(1));

            // On parcourt chaque pixel de l'image
            for (int y = 0; y < img.GetLength(1); y++)
            {
                for (int x = 0; x < img.GetLength(0); x++)
                {
                    int red = 0;
                    int green = 0;
                    int blue = 0;

                    // On parcourt chaque élément de la matrice de convolution
                    for (int i = 0; i < kernel.GetLength(0); i++)
                    {
                        for (int j = 0; j < kernel.GetLength(1); j++)
                        {
                            // On calcule les coordonnées du pixel à traiter
                            int xIndex = x + i - kernel.GetLength(1) / 2;
                            int yIndex = y + j - kernel.GetLength(0) / 2;

                            if (
                                xIndex >= 0
                                && xIndex < img.GetLength(0)
                                && yIndex >= 0
                                && yIndex < img.GetLength(1)
                            )
                            {
                                red += (int)(img[xIndex, yIndex].Red * kernel[i, j]);
                                green += (int)(img[xIndex, yIndex].Green * kernel[i, j]);
                                blue += (int)(img[xIndex, yIndex].Blue * kernel[i, j]);
                            }
                        }
                    }
                    // On s'assure que les valeurs sont comprises entre 0 et 255
                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    newImage[x, y] = new Pixel((byte)red, (byte)green, (byte)blue);
                }
            }
            return newImage;
        }

        // matrices de convolution : detection de contours, flou, netteté, Repoussage
        public static float[,] edgeDetectionKernel = new float[,]
        {
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
        };

        public static float[,] blurKernel = new float[,]
        {
            { 1.0f / 16, 2.0f / 16, 1.0f / 16 },
            { 2.0f / 16, 4.0f / 16, 2.0f / 16 },
            { 1.0f / 16, 2.0f / 16, 1.0f / 16 }
        };

        public static float[,] sharpenKernel = new float[,]
        {
            { 0, -1, 0 },
            { -1, 5, -1 },
            { 0, -1, 0 }
        };

        public static float[,] embossKernel = new float[,]
        {
            { -2, -1, 0 },
            { -1, 1, 1 },
            { 0, 1, 2 }
        };

        /// <summary>
        /// Convertit une image en pixel art
        /// </summary>
        /// <param name="originalImage">Image à convertir</param>
        /// <param name="spacing">Taille des blocs de pixels</param>
        /// <returns>Une image en pixel art</returns>
        public static MyImage ConvertToPixelArt(MyImage originalImage, int spacing)
        {
            MyImage pixelArtImage = new MyImage(
                originalImage.GetLength(0),
                originalImage.GetLength(1)
            );

            // On parcourt l'image originale en sautant des blocs de pixels
            for (int i = 0; i < originalImage.GetLength(0); i += spacing)
            {
                for (int j = 0; j < originalImage.GetLength(1); j += spacing)
                {
                    // On calcule la couleur moyenne du bloc
                    Pixel averageColor = CalculateAverageColor(originalImage, i, j, spacing);

                    // On remplit le bloc avec la couleur moyenne
                    for (int x = i; x < i + spacing && x < originalImage.GetLength(0); x++)
                    {
                        for (int y = j; y < j + spacing && y < originalImage.GetLength(1); y++)
                        {
                            pixelArtImage[x, y] = averageColor;
                        }
                    }
                }
            }

            return pixelArtImage;
        }

        /// <summary>
        /// Calcule la couleur moyenne d'un bloc de pixels
        /// </summary>
        /// <param name="image">Image à traiter</param>
        /// <param name="startX">Coordonnée x du coin supérieur gauche du bloc</param>
        /// <param name="startY">Coordonnée y du coin supérieur gauche du bloc</param>
        /// <param name="spacing">Taille du bloc</param>
        /// <returns>La couleur moyenne du bloc</returns>
        private static Pixel CalculateAverageColor(
            MyImage image,
            int startX,
            int startY,
            int spacing
        )
        {
            int totalR = 0,
                totalG = 0,
                totalB = 0;
            int pixelCount = 0;

            // On parcourt le bloc de pixels
            for (int i = startX; i < startX + spacing && i < image.GetLength(0); i++)
            {
                for (int j = startY; j < startY + spacing && j < image.GetLength(1); j++)
                {
                    // On ajoute les valeurs de chaque couleur
                    Pixel pixel = image[i, j];
                    totalR += pixel.Red;
                    totalG += pixel.Green;
                    totalB += pixel.Blue;
                    pixelCount++;
                }
            }
            
            // On calcule la moyenne des couleurs
            int avgR = totalR / pixelCount;
            int avgG = totalG / pixelCount;
            int avgB = totalB / pixelCount;

            return new Pixel((byte)avgR, (byte)avgG, (byte)avgB);
        }
    }
}
