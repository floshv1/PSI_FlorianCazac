using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppVisuals;
using ConsoleAppVisuals.AnimatedElements;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;

namespace PSI_SouidiCazac
{
    public class MyImage
    {
        #region Fields
        /// <summary>
        /// Contient les metadonnées de l'image
        /// </summary>
        byte[] info;

        /// <summary>
        /// Contient les données de l'image
        /// </summary>
        byte[] data;

        /// <summary>
        /// Chemin de l'image
        /// </summary>
        string imagePath;
        #endregion

        /// <summary>
        /// Taille du fichier
        /// </summary>
        uint sizeFile => ConvertToUInt(info, 2);

        /// <summary>
        /// Taille des métadonnées
        /// </summary>
        uint sizeOffset => ConvertToUInt(info, 10);

        /// <summary>
        /// Nombre de bits par pixel
        /// </summary>
        ushort bytesPerPixel => ConvertToUShort(info, 28);

        /// <summary>
        /// Taille d'une ligne de l'image en pixels
        /// </summary>
        int stride => (GetLength(0) * bytesPerPixel / 8 + 3) / 4 * 4;

        /// <summary>
        /// Récupère la longueur ou la largeur de l'image
        /// </summary>
        /// <param name="dimension">0 pour la largeur, 1 pour la hauteur</param>
        /// <returns>La longueur ou la largeur de l'image</returns>
        public int GetLength(int dimension) =>
            dimension switch
            {
                // Width
                0 => ConvertToInt(info, 18),
                // Height
                1 => ConvertToInt(info, 22),
                // default
                _
                    => throw new ArgumentOutOfRangeException(
                        nameof(dimension),
                        dimension,
                        "Dimension must be 0 or 1"
                    )
            };

        #region Constructors

        /// <summary>
        /// Constructeur de la classe MyImage
        /// </summary>
        /// <param name="file"> Chemin de l'image </param>
        public MyImage(string? file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using (FileStream stream = File.OpenRead(file))
            {
                info = new byte[54];
                for (int i = 0; i < 54; i++)
                    info[i] = (byte)stream.ReadByte();

                data = new byte[sizeFile - sizeOffset];
                for (int i = 0; i < data.Length; i++)
                    data[i] = (byte)stream.ReadByte();
            }

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            imagePath = file;
        }

        /// <summary>
        /// Constructeur de la classe MyImage
        /// </summary>
        /// <param name="width"> Largeur de l'image </param>
        /// <param name="height"> Hauteur de l'image </param>
        public MyImage(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Width and height must be positive integers.");
            }

            info = new byte[54];

            // Signature
            Array.Copy(new byte[] { 66, 77 }, 0, info, 0, 2);
            // Taille des métadonnées
            Array.Copy(ConvertToEndian(info.Length), 0, info, 10, 4);
            // Taille des informations de l'entête
            Array.Copy(ConvertToEndian(40), 0, info, 14, 4);
            // Largeur
            Array.Copy(ConvertToEndian(width), 0, info, 18, 4);
            // Hauteur
            Array.Copy(ConvertToEndian(height), 0, info, 22, 4);
            // Plan
            Array.Copy(ConvertToEndian(1), 0, info, 26, 2);
            // Bits par pixel
            Array.Copy(ConvertToEndian(24), 0, info, 28, 2);

            data = new byte[height * stride];

            // Taille du fichier
            Array.Copy(ConvertToEndian(info.Length + data.Length), 0, info, 2, 4);
        }
        #endregion

        #region Conversion

        /// <summary>
        /// Convertit un tableau de bytes en entier non signé
        /// </summary>
        /// <param name="array"> Tableau de bytes </param>
        /// <param name="offset"> Décalage </param>
        /// <returns> Entier non signé </returns>
        public uint ConvertToUInt(byte[] array, int offset = 0)
        {
            uint result = 0;
            for (int i = 0; i < 4; i++)
                result += array[offset + i] * (uint)Math.Pow(256, i);
            return result;
        }

        /// <summary>
        /// Convertit un tableau de bytes en entier non signé sur 2 octets
        /// </summary>
        /// <param name="array"> Tableau de bytes </param>
        /// <param name="offset"> Décalage </param>
        /// <returns> Entier non signé </returns>
        public ushort ConvertToUShort(byte[] array, int offset = 0)
        {
            ushort result = 0;
            for (int i = 0; i < 2; i++)
                result += (ushort)(array[offset + i] * Math.Pow(256, i));
            return result;
        }

        /// <summary>
        /// Convertit un tableau de bytes en entier signé
        /// </summary>
        /// <param name="array"> Tableau de bytes </param>
        /// <param name="offset"> Décalage </param>
        /// <returns> Entier signé </returns>
        public static int ConvertToInt(byte[] array, int offset = 0)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
                result += array[offset + i] * (int)Math.Pow(256, i);
            return result;
        }

        /// <summary>
        /// Convertit un entier en tableau de bytes
        /// </summary>
        /// <param name="val"> Entier </param>
        /// <returns> Tableau de bytes </returns>
        public static byte[] ConvertToEndian(int val)
        {
            byte[] tab = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                tab[i] = Convert.ToByte(val % 256);
                val = val / 256;
            }
            return tab;
        }

        #endregion

        #region Steganography

        /// <summary>
        /// Cache une image dans une autre
        /// </summary>
        /// <param name="cover"> Image Couverture </param>
        /// <param name="toHide"> Image à cacher </param>
        /// <returns> Image cachée dans une image </returns>
        public static MyImage Hide(MyImage cover, MyImage toHide)
        {
            MyImage newImage = cover.Copy();
            string coverPath = cover.imagePath;
            string toHidePath = toHide.imagePath;

            if (
                newImage.GetLength(0) < toHide.GetLength(0)
                || newImage.GetLength(1) < toHide.GetLength(1)
            )
            {
                // Redimensionne l'image à cacher pour qu'elle rentre dans l'image de couverture
                float scale = Math.Min(
                    (float)newImage.GetLength(0) / (float)toHide.GetLength(0),
                    (float)newImage.GetLength(1) / (float)toHide.GetLength(1)
                );
                toHide = Transformation.Resize(toHide, scale);
            }

            for (int x = 0; x < toHide.GetLength(0); x++)
            {
                for (int y = 0; y < toHide.GetLength(1); y++)
                {
                    Pixel pixel = newImage[x, y];
                    Pixel hidden = toHide[x, y];
                    newImage[x, y] = new Pixel(
                        fusion(pixel.Red, hidden.Red),
                        fusion(pixel.Green, hidden.Green),
                        fusion(pixel.Blue, hidden.Blue)
                    );
                }
            }

            // Fusion des pixels
            // Fusionne les 4 bits de poids fort de la couleur de la couverture
            // avec les 4 bits de poids fort de la couleur à cacher
            byte fusion(byte cover, byte toHide)
            {
                return (byte)((cover & 0xF0) | (toHide >> 4) & 0x0F);
            }

            string coverName = coverPath.Split('/').Last().Split('.').First();
            string toHideName = toHidePath.Split('/').Last().Split('.').First();
            newImage.Save($"{coverName}_hidden_{toHideName}", "Images/Sortie/Steganography/Code/");

            return newImage;
        }

        /// <summary>
        /// Extrait une image cachée dans une autre
        /// </summary>
        /// <param name="cover"> Image Couverture </param>
        /// <returns> Image extraite de l'image de couverture </returns>
        public static MyImage Extract(MyImage cover)
        {
            MyImage newImage = new MyImage(cover.GetLength(0), cover.GetLength(1));

            for (int x = 0; x < newImage.GetLength(0); x++)
            {
                for (int y = 0; y < newImage.GetLength(1); y++)
                {
                    Pixel pixel = cover[x, y];
                    newImage[x, y] = new Pixel(
                        fission(pixel.Red),
                        fission(pixel.Green),
                        fission(pixel.Blue)
                    );
                }
            }

            // Fission des pixels
            byte fission(byte cover)
            {
                return (byte)((cover << 4) & 0xF0);
            }

            string coverName = cover
                .imagePath.Split('/')
                .Last()
                .Split('_')
                .Last()
                .Split('.')
                .First();
            newImage.Save($"{coverName}_extracted", "Images/Sortie/Steganography/Decode/");

            return newImage;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Enregistre l'image
        /// </summary>
        /// <param name="imageName"> Nom de l'image </param>
        /// <param name="path"> Chemin de l'image </param>
        /// <returns> Image enregistrée </returns>
        public void Save(string imageName, string path = "Images/Sortie/Bitmap/")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Chemin de l'image
            string beforePath = Directory.GetCurrentDirectory();
            path = beforePath + "/" + path;

            imagePath = path + imageName + ".bmp";
            using (FileStream stream = File.OpenWrite(imagePath))
            {
                stream.Write(info, 0, info.Length);
                stream.Write(data, 0, data.Length);
            }

            Dialog text = new Dialog(
                new List<string>() { "L'image a été modifiée et enregistrée avec succès !", },
                null,
                "Ok"
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string[] options = { "Oui", "Non" };
            ScrollingMenu menu = new ScrollingMenu(
                "Voulez-vous ouvrir l'image ?",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();

            switch (response.Value)
            {
                case 0:
                    Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
                    Dialog text2 = new Dialog(
                        new List<string>()
                        {
                            $"L'image {imageName} a été ouverte dans votre visionneuse d'image."
                        },
                        null,
                        "Fermer"
                    );

                    Window.AddElement(text2);
                    Window.ActivateElement(text2);
                    break;
                case 1:
                    break;
            }
        }

        /// <summary>
        /// Récupère la position d'un pixel
        /// </summary>
        /// <param name="x"> Position en x </param>
        /// <param name="y"> Position en y </param>
        /// <returns> Position du pixel </returns>
        private int pos(int x, int y) => x * 3 + (GetLength(1) - y - 1) * stride;

        /// <summary>
        /// Indexeur de la classe MyImage
        /// </summary>
        /// <param name="x"> Position en x </param>
        /// <param name="y"> Position en y </param>
        /// <returns> Pixel à la position (x, y) </returns>
        public Pixel this[int x, int y]
        {
            get
            {
                int position = pos(x, y);
                return new(data[position + 2], data[position + 1], data[position + 0]);
            }
            set
            {
                int position = pos(x, y);
                data[position + 2] = value.Red;
                data[position + 1] = value.Green;
                data[position + 0] = value.Blue;
            }
        }

        /// <summary>
        /// Copie l'image
        /// </summary>
        /// <returns> Copie de l'image </returns>
        public MyImage Copy()
        {
            MyImage newImage = new MyImage(GetLength(0), GetLength(1));
            for (int x = 0; x < GetLength(0); x++)
            {
                for (int y = 0; y < GetLength(1); y++)
                {
                    newImage[x, y] = this[x, y];
                }
            }
            return newImage;
        }

        #endregion

        #region Compression

        /// <summary>
        /// Compresse l'image
        /// </summary>
        /// <returns> Image compressée </returns>
        public Huffman Compress()
        {
            int[] values = Array.ConvertAll(data, b => (int)b);
            int[] frequency = CountFrequency(values);
            var huffman = new Huffman(frequency);
            var CompressedBits = ConvertToCompressedBits(huffman, values);
            var CompressedBytes = ConvertToCompressedBytes(CompressedBits);

            data = CompressedBytes;
            return huffman;
        }

        /// <summary>
        /// Compte la fréquence des valeurs
        /// </summary>
        /// <param name="data"> Données à compresser </param>
        /// <returns> Fréquence des valeurs </returns>
        private int[] CountFrequency(int[] data)
        {
            int[] frequency = new int[256];

            foreach (int bytes in data)
            {
                frequency[bytes]++;
            }

            return frequency;
        }

        /// <summary>
        /// Convertit les données en bits compressés
        /// </summary>
        /// <param name="tree"> Arbre de Huffman </param>
        /// <param name="data"> Données à compresser </param>
        /// <returns> Bits compressés </returns>
        private List<bool> ConvertToCompressedBits(Huffman tree, int[] data)
        {
            Dictionary<int, string> codes = tree.GenerateCodes();
            List<bool> result = new List<bool>();

            foreach (int b in data)
            {
                string code = codes[b];

                foreach (char bit in code)
                {
                    if (bit == '0')
                    {
                        result.Add(false);
                    }
                    else if (bit == '1')
                    {
                        result.Add(true);
                    }
                }
            }
            AddPadding(result);

            return result;
        }

        /// <summary>
        /// Ajoute un padding
        /// </summary>
        /// <param name="bits"> Bits à compresser </param>
        /// <returns> Bits compressés avec padding </returns>
        private void AddPadding(List<bool> bits)
        {
            // Ajoute un padding pour que la taille des bits soit un multiple de 8
            int padding = 8 - bits.Count % 8;
            for (int i = 0; i < padding; i++)
            {
                bits.Add(false);
            }
        }

        /// <summary>
        /// Convertit les bits en tableau de bytes compressés
        /// </summary>
        /// <param name="bits"> Bits compressés </param>
        /// <returns> Tableau de bytes compressés </returns>
        private byte[] ConvertToCompressedBytes(List<bool> bits)
        {
            // Convertit les bits en tableau de bytes
            int numBytes = (bits.Count + 7) / 8;

            byte[] bytes = new byte[numBytes];

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    // Calcule l'index du byte et le décalage du bit
                    int byteIndex = i / 8;
                    int bitOffset = 7 - (i % 8);

                    // Crée un masque pour le bit
                    byte mask = (byte)(1 << bitOffset);
                    bytes[byteIndex] |= mask;
                }
            }

            return bytes;
        }

        #endregion

        #region Decompression

        /// <summary>
        /// Décompresse l'image
        /// </summary>
        /// <param name="tree"> Arbre de Huffman </param>
        /// <returns> Image décompressée </returns>
        public void Decompress(Huffman tree)
        {
            List<bool> bits = new List<bool>();

            foreach (byte b in this.data)
            {
                // Convertit le byte en binaire
                string binary = Convert.ToString(b, 2).PadLeft(8, '0');
                bits.AddRange(binary.Select(bit => bit == '1'));
            }

            int index = 0;
            List<int> result = new List<int>();

            while (index < bits.Count)
            {
                // Parcourt l'arbre de Huffman
                Node node = tree.root;
                while (node.left != null && node.right != null && index < bits.Count)
                {
                    if (bits[index])
                    {
                        // Si le bit est 1, on va à droite
                        node = node.right;
                    }
                    else
                    {
                        // Si le bit est 0, on va à gauche
                        node = node.left;
                    }
                    index++;
                }
                result.Add(node.value);
            }

            // Convertit les valeurs en tableau de bytes
            // et remplace les données de l'image
            this.data = result.Select(i => (byte)i).ToArray();
        }
        #endregion
    }
}
