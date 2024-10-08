using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_SouidiCazac
{
    public class ConvertToJpeg
    {
        // Matrice de quantification pour la luminance
        byte[,] quntificationMatrixLuminance = new byte[8, 8]
        {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }
        };

        // Matrice de quantification pour la chrominance
        byte[,] quntificationMatrixChrominance = new byte[8, 8]
        {
            { 17, 18, 24, 47, 99, 99, 99, 99 },
            { 18, 21, 26, 66, 99, 99, 99, 99 },
            { 24, 26, 56, 99, 99, 99, 99, 99 },
            { 47, 66, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 }
        };

        /// <summary>
        /// Convertie l'image RGB en YCbCr
        /// </summary>
        /// <param name="img">Image RGB</param>
        /// <returns>Image YCbCr</returns>
        public MyImage CST(MyImage img)
        {
            MyImage newImage = new MyImage(img.GetLength(0), img.GetLength(1));

            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    Pixel pix = img[x, y];
                    byte Y = (byte)(0.299 * pix.Red + 0.587 * pix.Green + 0.114 * pix.Blue);
                    byte Cb = (byte)(
                        128 - 0.168736 * pix.Red - 0.331264 * pix.Green + 0.5 * pix.Blue
                    );
                    byte Cr = (byte)(
                        128 + 0.5 * pix.Red - 0.418688 * pix.Green - 0.081312 * pix.Blue
                    );

                    newImage[x, y] = new Pixel(Y, Cb, Cr);
                }
            }
            return newImage;
        }

        /// <summary>
        /// Méthode qui divise l'image en blocs de 8x8
        /// </summary>
        /// <param name="img">Image à diviser</param>
        /// <returns>Une liste de blocs de 8x8</returns>
        public List<MyImage> BlockSplitting(MyImage img)
        {
            int blockWidth = 8;
            int blockHeight = 8;

            List<MyImage> listBlock = new List<MyImage>();

            for (int x = 0; x < img.GetLength(0); x += blockHeight)
            {
                for (int y = 0; y < img.GetLength(1); y += blockWidth)
                {
                    MyImage block = new MyImage(blockHeight, blockWidth);

                    for (int i = 0; i < blockHeight; i++)
                    {
                        for (int j = 0; j < blockWidth; j++)
                        {
                            block[i, j] = img[x + i, y + j];
                        }
                    }
                    listBlock.Add(block);
                }
            }
            return listBlock;
        }

        /// <summary>
        /// Méthode qui effectue la DCT sur une liste de blocs
        /// </summary>
        /// <param name="ListBlocks">Liste de blocs</param>
        /// <returns>Une liste de blocs DCT</returns>
        public List<MyImage> ForwardDCT(List<MyImage> ListBlocks)
        {
            int DCTheight = 8;
            int DCTwidth = 8;
            double ci,
                cj;

            List<MyImage> listDCT = new List<MyImage>();

            foreach (MyImage block in ListBlocks)
            {
                MyImage DCTblock = new MyImage(DCTheight, DCTwidth);

                // Parcours des 3 composantes de l'image (Y, Cb, Cr)
                for (int YCbCr = 0; YCbCr < 3; YCbCr++)
                {
                    // Matrice des coefficients DCT
                    double[,] DCTCoef = new double[DCTheight, DCTwidth];

                    for (int i = 0; i < DCTheight; i++)
                    {
                        for (int j = 0; j < DCTwidth; j++)
                        {

                            // Initialisation de la somme
                            double sum = 0;

                            // Calcul des coefficients ci et cj
                            if (i == 0)
                                ci = 1 / Math.Sqrt(2);
                            else
                                ci = 1;

                            if (j == 0)
                                cj = 1 / Math.Sqrt(2);
                            else
                                cj = 1;

                            // Calcul de la somme pour chaque composante (Y, Cb, Cr)
                            for (int x = 0; x < DCTheight; x++)
                            {
                                for (int y = 0; y < DCTwidth; y++)
                                {
                                    if (YCbCr == 0)
                                    {
                                        double pixelValue = block[x, y].Red;
                                        double cos1 = Math.Cos(
                                            (2 * x + 1) * i * Math.PI / 8.0 * 2.0
                                        );
                                        double cos2 = Math.Cos(
                                            (2 * y + 1) * j * Math.PI / 8.0 * 2.0
                                        );
                                        sum += pixelValue * cos1 * cos2;
                                    }
                                    else if (YCbCr == 1)
                                    {
                                        double pixelValue = block[x, y].Green;
                                        double cos1 = Math.Cos(
                                            (2 * x + 1) * i * Math.PI / 8.0 * 2.0
                                        );
                                        double cos2 = Math.Cos(
                                            (2 * y + 1) * j * Math.PI / 8.0 * 2.0
                                        );
                                        sum += pixelValue * cos1 * cos2;
                                    }
                                    else
                                    {
                                        double pixelValue = block[x, y].Blue;
                                        double cos1 = Math.Cos(
                                            (2 * x + 1) * i * Math.PI / 8.0 * 2.0
                                        );
                                        double cos2 = Math.Cos(
                                            (2 * y + 1) * j * Math.PI / 8.0 * 2.0
                                        );
                                        sum += pixelValue * cos1 * cos2;
                                    }
                                }
                            }

                            // Calcul du coefficient DCT
                            // 2/N * ci * cj * sum
                            DCTCoef[i, j] = (2.0 / 8.0) * ci * cj * sum;
                        }
                    }

                    // Remplissage de l'image DCT
                    for (int x = 0; x < DCTheight; x++)
                    {
                        for (int y = 0; y < DCTwidth; y++)
                        {
                            if (YCbCr == 0)
                            {
                                DCTblock[x, y].Red = (byte)DCTCoef[x, y];
                            }
                            else if (YCbCr == 1)
                            {
                                DCTblock[x, y].Green = (byte)DCTCoef[x, y];
                            }
                            else
                            {
                                DCTblock[x, y].Blue = (byte)DCTCoef[x, y];
                            }
                        }
                    }
                }
                listDCT.Add(DCTblock);
            }
            return listDCT;
        }

        /// <summary>
        /// Méthode qui effectue la quantification sur une liste de blocs DCT
        /// </summary>
        /// <param name="ListDCT">Liste de blocs DCT</param>
        /// <returns>Une liste de blocs quantifiés</returns>
        public List<MyImage> Quantization(List<MyImage> ListDCT)
        {
            List<MyImage> listQuantization = new List<MyImage>();

            foreach (MyImage DCTblock in ListDCT)
            {
                MyImage QuantizationBlock = new MyImage(8, 8);

                // Parcours des 3 composantes de l'image (Y, Cb, Cr)
                for (int YCbCr = 0; YCbCr < 3; YCbCr++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (YCbCr == 0)
                            {
                                // Quantification de la luminance
                                QuantizationBlock[x, y].Red = (byte)(
                                    Math.Round(
                                        DCTblock[x, y].Red
                                            / (double)quntificationMatrixLuminance[x, y]
                                    )
                                );
                            }
                            else
                            {
                                // Quantification de la chrominance
                                QuantizationBlock[x, y].Green = (byte)(
                                    Math.Round(
                                        DCTblock[x, y].Green
                                            / (double)quntificationMatrixChrominance[x, y]
                                    )
                                );
                                QuantizationBlock[x, y].Blue = (byte)(
                                    Math.Round(
                                        DCTblock[x, y].Blue
                                            / (double)quntificationMatrixChrominance[x, y]
                                    )
                                );
                            }
                        }
                    }
                }
                listQuantization.Add(QuantizationBlock);
            }
            return listQuantization;
        }

        /// <summary>
        /// Méthode qui effectue la zigzag sur une matrice carrée
        /// </summary>
        /// <param name="cols">Nombre de colonnes</param>
        /// <param name="rows">Nombre de lignes</param>
        /// <returns>Un tableau de tuples</returns>
        private Tuple<int, int>[] ZigZag(int rows, int cols)
        {
            Tuple<int, int>[] a = new Tuple<int, int>[rows * cols];
            int p = 0;
            int i = 0;
            int j = 0;
            a[p++] = Tuple.Create(i, j);
            while (p < rows * cols)
            {
                if (j < cols - 1)
                {
                    j++;
                }
                else
                {
                    i++;
                }

                while (i < rows && j >= 0)
                {
                    a[p++] = Tuple.Create(i, j);
                    i++;
                    j--;
                }
                i--;
                j++;

                if (i < rows - 1)
                {
                    i++;
                }
                else
                {
                    j++;
                }

                while (i >= 0 && j < cols)
                {
                    a[p++] = Tuple.Create(i, j);
                    i--;
                    j++;
                }
                i++;
                j--;
            }
            return a;
        }
    }
}
