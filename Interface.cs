using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleAppVisuals;
using ConsoleAppVisuals.AnimatedElements;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;

namespace PSI_SouidiCazac
{
    public class Interface
    {
        /// <summary>
        /// Méthode pour afficher le menu principal
        /// </summary>
        public static void MainMenu()
        {
            Window.Render();
            string[] options = new string[]
            {
                "Transformer une image",
                "Créer une Fractale",
                "Stéganographie",
                "Huffman test",
                "Innovation",
                "Afficher une image",
                "Quitter"
            };

            ScrollingMenu menu = new ScrollingMenu(
                "Choisissez parmi les options suivantes : ",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            while (true)
            {
                switch (response!.Status)
                {
                    case Status.Selected:
                        switch (response.Value)
                        {
                            case 0:
                                TransformationMenu();
                                break;
                            case 1:
                                FractaleMenu();
                                break;
                            case 2:
                                SteganographyMenu();
                                break;
                            case 3:
                                HuffmanMenu();
                                break;
                            case 4:
                                InnovationMenu();
                                break;
                            case 5:
                                DisplayImageMenu();
                                break;
                            case 6:
                                // Quit the app
                                CloseSession();
                                break;
                        }
                        break;
                    case Status.Escaped:
                    case Status.Deleted:
                        // Quit the app anyway
                        CloseSession();
                        break;
                }
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de transformation
        /// </summary>
        public static void TransformationMenu()
        {
            Window.Render();

            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Dans cette partie, vous pouvez transformer une image.",
                    "Pour cela, vous pouvez choisir d'appliquer un filtre ou de manipuler l'image."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string[] options = new string[] { "Appliquer un filtre", "Manipuler", "Retour" };

            ScrollingMenu menu = new ScrollingMenu(
                "Choisissez parmi les options suivantes : ",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0:
                            Window.RemoveElement(text);
                            FilterMenu();
                            break;
                        case 1:
                            Window.RemoveElement(text);
                            ManipulationMenu();
                            break;

                        case 2:
                            Window.RemoveElement(text);
                            MainMenu();
                            break;
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    Window.RemoveElement(text);
                    MainMenu();
                    break;
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de filtre
        /// </summary>
        public static void FilterMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Dans cette partie, vous pouvez appliquer des filtres sur une image.",
                    "Pour cela, vous pouvez choisir de transformer l'image en nuance de gris, en noir et blanc,",
                    "de détecter les bords, de flouter, de rendre l'image plus nette ou de repousser les bords."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string imagePath = ChooseImageMenu();

            if (imagePath == "")
            {
                Window.RemoveElement(text);
                TransformationMenu();
            }
            else
            {
                MyImage image = new MyImage(imagePath);
                string imageName = imagePath.Split("/").Last().Split(".").First();

                string[] options = new string[]
                {
                    "Nuance de Gris",
                    "Noir et Blanc",
                    "Detection de bord",
                    "Flou",
                    "Netteté",
                    "Repoussage",
                    "Negatif",
                    "Retour"
                };

                ScrollingMenu menu = new ScrollingMenu(
                    "Choisissez parmi les options suivantes : ",
                    0,
                    Placement.TopCenter,
                    options
                );

                Window.AddElement(menu);
                Window.ActivateElement(menu);

                var response = menu.GetResponse();
                switch (response!.Status)
                {
                    case Status.Selected:
                        switch (response.Value)
                        {
                            case 0:

                                image = Transformation.GreyScale(image);
                                image.Save($"{imageName}_GreyScale");
                                Window.RemoveElement(text);
                                break;
                            case 1:

                                image = Transformation.BlackAndWhite(image);

                                image.Save($"{imageName}_BlackAndWhite");
                                Window.RemoveElement(text);
                                break;
                            case 2:

                                image = Transformation.ApplyConvolution(
                                    image,
                                    Transformation.edgeDetectionKernel
                                );
                                image.Save($"{imageName}_EdgeDetection");
                                Window.RemoveElement(text);
                                break;
                            case 3:

                                image = Transformation.ApplyConvolution(
                                    image,
                                    Transformation.blurKernel
                                );

                                image.Save($"{imageName}_Blur");
                                Window.RemoveElement(text);
                                break;
                            case 4:

                                image = Transformation.ApplyConvolution(
                                    image,
                                    Transformation.sharpenKernel
                                );

                                image.Save($"{imageName}_Sharpness");

                                Window.RemoveElement(text);
                                break;
                            case 5:

                                image = Transformation.ApplyConvolution(
                                    image,
                                    Transformation.embossKernel
                                );

                                image.Save($"{imageName}_Emboss");
                                Window.RemoveElement(text);
                                break;
                            case 6:
                                image = Transformation.Negative(image);
                                image.Save($"{imageName}_Negative");
                                Window.RemoveElement(text);
                                break;
                            case 7:
                                Window.RemoveElement(text);
                                TransformationMenu();
                                break;
                        }
                        break;
                    case Status.Escaped:
                    case Status.Deleted:
                        Window.RemoveElement(text);
                        TransformationMenu();
                        break;
                }
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de manipulation
        /// </summary>
        public static void ManipulationMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Dans cette partie, vous pouvez effectuer des transformations sur une image.",
                    "Pour cela, vous pouvez choisir de faire une rotation ou de redimensionner l'image."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string imagePath = ChooseImageMenu();
            if (imagePath == "")
            {
                Window.RemoveElement(text);
                TransformationMenu();
            }
            else
            {
                MyImage image = new MyImage(imagePath);
                string imageName = imagePath.Split("/").Last().Split(".").First();

                string[] options = new string[] { "Rotation", "Redimensionner", "Retour" };

                ScrollingMenu menu = new ScrollingMenu(
                    "Choisissez parmi les options suivantes : ",
                    0,
                    Placement.TopCenter,
                    options
                );

                Window.AddElement(menu);
                Window.ActivateElement(menu);

                var response = menu.GetResponse();
                switch (response!.Status)
                {
                    case Status.Selected:
                        switch (response.Value)
                        {
                            case 0:
                                image = Transformation.Rotation(image);
                                image.Save($"{imageName}_Rotated");
                                Window.RemoveElement(text);
                                break;
                            case 1:
                                image = Transformation.Resize(image);
                                image.Save($"{imageName}_Resized");
                                Window.RemoveElement(text);
                                break;
                            case 2:
                                Window.RemoveElement(text);
                                TransformationMenu();
                                break;
                        }
                        break;
                    case Status.Escaped:
                    case Status.Deleted:
                        Window.RemoveElement(text);
                        TransformationMenu();
                        break;
                }
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de fractale
        /// </summary>
        public static void FractaleMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Une fractale est une figure géométrique complexe qui peut être divisée en parties, chacune d'elles étant une réplique réduite de l'ensemble.",
                    "Pour cela, vous pouvez choisir de créer une fractale de Mandelbrot ou une fractale de Julia.",
                    "La création d'une fractale peut prendre du temps en fonction de la taille de l'image et de la complexité de la fractale.",
                    "Soyez patient et attendez la fin de la création avant de continuer."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string[] options = new string[]
            {
                "Ensemble de Mandelbrot",
                "Ensemble de Julia",
                "Retour"
            };

            ScrollingMenu menu = new ScrollingMenu(
                "Choisissez parmi les options suivantes : ",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0:
                            MyImage mandelbrot = Program.MandelbrotSet();
                            mandelbrot.Save("Mandelbrot");
                            Window.RemoveElement(text);
                            break;
                        case 1:
                            Window.RemoveElement(text);
                            JuliaMenu();
                            break;
                        case 2:
                            Window.RemoveElement(text);
                            MainMenu();
                            break;
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    Window.RemoveElement(text);
                    MainMenu();
                    break;
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de Julia
        /// </summary>
        public static void JuliaMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Il existe plusieurs valeurs de c pour lesquelles on peut créer une fractale de Julia.",
                    "En fonction de la valeur de c, la fractale sera différente.",
                    "La création d'une fractale peut prendre du temps en fonction de la taille de l'image et de la complexité de la fractale.",
                    "Soyez patient et attendez la fin de la création avant de continuer."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string[] options = new string[]
            {
                "-0.4 + 0.6i",
                "0.285 + 0.01i",
                "-0.70176 - 0.3842i",
                "-0.835 - 0.2321i",
                "-0.8 + 0.156i",
                "-0.7269 + 0.1889i",
                " 0.35 + 0.35i",
                "0.4 + 0.4i",
                "Retour"
            };

            ScrollingMenu menu = new ScrollingMenu(
                "Creer une fractale de Julia pour c =",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();

            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0:
                            MyImage julia1 = Program.JuliaSet(-0.4, 0.6);
                            julia1.Save("Julia1");
                            Window.RemoveElement(text);
                            break;
                        case 1:
                            MyImage julia2 = Program.JuliaSet(0.285, 0.01);
                            julia2.Save("Julia2");
                            Window.RemoveElement(text);
                            break;
                        case 2:
                            MyImage julia3 = Program.JuliaSet(-0.70176, -0.3842);
                            julia3.Save("Julia3");
                            Window.RemoveElement(text);
                            break;
                        case 3:
                            MyImage julia4 = Program.JuliaSet(-0.835, -0.2321);
                            julia4.Save("Julia4");
                            Window.RemoveElement(text);
                            break;
                        case 4:
                            MyImage julia5 = Program.JuliaSet(-0.8, 0.156);
                            julia5.Save("Julia5");
                            Window.RemoveElement(text);
                            break;
                        case 5:
                            MyImage julia6 = Program.JuliaSet(-0.7269, 0.1889);
                            julia6.Save("Julia6");
                            Window.RemoveElement(text);
                            break;
                        case 6:
                            MyImage julia7 = Program.JuliaSet(0.35, 0.35);
                            julia7.Save("Julia7");
                            Window.RemoveElement(text);
                            break;
                        case 7:
                            MyImage julia8 = Program.JuliaSet(0.4, 0.4);
                            julia8.Save("Julia8");
                            Window.RemoveElement(text);
                            break;
                        case 8:
                            Window.RemoveElement(text);
                            FractaleMenu();
                            break;
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    Window.RemoveElement(text);
                    FractaleMenu();
                    break;
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de stéganographie
        /// </summary>
        public static void SteganographyMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "La stéganographie est une technique qui consiste à cacher une image dans une autre.",
                    "Pour cela, vous devez choisir une image de couverture et une image à cacher.",
                    "Vous pouvez également extraire une image cachée dans une image."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string[] options = new string[]
            {
                "Cacher une image dans une image",
                "Extraire une image cachée",
                "Retour"
            };

            ScrollingMenu menu = new ScrollingMenu(
                "Choisissez parmi les options suivantes : ",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0:

                            string imagePath = ChooseFile(
                                txt: "Choisissez l'image de couverture : "
                            );

                            string toHidePath = ChooseFile(txt: "Choisissez l'image à cacher : ");

                            if (toHidePath == "" || imagePath == "")
                            {
                                Window.RemoveElement(text);
                                SteganographyMenu();
                            }
                            else
                            {
                                MyImage cover = new MyImage(imagePath);
                                MyImage toHide = new MyImage(toHidePath);

                                MyImage.Hide(cover, toHide);
                                Window.RemoveElement(text);
                            }
                            break;
                        case 1:
                            string stegStr = ChooseFile(
                                Sortie: true,
                                Steganography: true,
                                code: true,
                                txt: "Choisissez l'image à extraire : "
                            );
                            if (stegStr == "")
                            {
                                Window.RemoveElement(text);
                                SteganographyMenu();
                            }
                            else
                            {
                                MyImage steganography = new MyImage(stegStr);
                                MyImage.Extract(steganography);
                                Window.RemoveElement(text);
                            }
                            break;
                        case 2:
                            Window.RemoveElement(text);
                            MainMenu();
                            break;
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    Window.RemoveElement(text);
                    MainMenu();
                    break;
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de Huffman
        /// </summary>
        public static void HuffmanMenu()
        {
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "Dnas cette partie, on va utiliser l'algorithme de Huffman.",
                    "Pour cela, vous devez choisir une image à compresser.",
                    "L'image sera compressée puis décompressée pour vérifier l'efficacité de l'algorithme."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string imagePath = ChooseFile();
            if (imagePath == "")
            {
                Window.RemoveElement(text);
                MainMenu();
            }
            else
            {
                MyImage image = new MyImage(imagePath);
                string imageName = imagePath.Split("/").Last().Split(".").First();

                image.Decompress(image.Compress());
                image.Save($"{imageName}_Compressed_Decompressed");
                Window.RemoveElement(text);
            }
        }

        /// <summary>
        /// Méthode pour afficher le menu de l'innovation
        /// </summary>
        public static void InnovationMenu()
        {
            Window.Render();
            EmbedText text = new EmbedText(
                new List<string>()
                {
                    "L'innovation est une fonctionnalité qui permet de transformer une image en pixel art.",
                    "Pour cela, vous devez choisir une image et la taille des pixels.",
                    "Plus la taille des pixels est grande, plus l'image sera simplifiée."
                },
                TextAlignment.Center
            );

            Window.AddElement(text);
            Window.ActivateElement(text);

            string imagePath = ChooseImageMenu();
            if (imagePath == "")
            {
                Window.RemoveElement(text);
                MainMenu();
            }
            else
            {
                MyImage image = new MyImage(imagePath);
                string imageName = imagePath.Split("/").Last().Split(".").First();
                int spacing;
                bool isValid = false;

                do
                {
                    Prompt prompt = new Prompt("Entrer la taille des pixels entre 1 et 32 : ");
                    Window.AddElement(prompt);
                    Window.ActivateElement(prompt);
                    isValid = int.TryParse(prompt.GetResponse().Value, out spacing);
                } while ((spacing < 1 || spacing > 32) || !isValid);

                Transformation.ConvertToPixelArt(image, spacing).Save($"{imageName}_PixelArt");
                Window.RemoveElement(text);
            }
        }

        /// <summary>
        /// Méthode pour afficher une image
        /// </summary>
        public static void DisplayImageMenu()
        {
            string imagePath = ChooseImageMenu();
            if (imagePath == "")
            {
                MainMenu();
            }
            else
            {
                MyImage image = new MyImage(imagePath);
                string imageName = imagePath.Split("/").Last().Split(".").First();

                Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });

                Dialog dialog = new Dialog(
                    new List<string>()
                    {
                        $"L'image {imageName} a été ouverte dans votre visionneuse d'image."
                    },
                    null,
                    "Fermer"
                );

                Window.AddElement(dialog);
                Window.ActivateElement(dialog);
            }
        }

        public static string ChooseFile(
            bool Sortie = false,
            bool Steganography = false,
            bool code = false,
            string txt = "Choisissez une images parmi les fichiers suivants : "
        )
        {
            string beforePath = Directory.GetCurrentDirectory();
            string folderPath = "Images/Default/";
            string[] files = null;
            string[] options;
            string result = "";

            if (Sortie && Steganography && code)
            {
                folderPath = "Images/Sortie/Steganography/Code/";
            }
            else if (Sortie && Steganography)
            {
                folderPath = "Images/Sortie/Steganography/Decode/";
            }
            else if (Sortie)
            {
                folderPath = "Images/Sortie/Bitmap/";
            }

            string path = beforePath + "/" + folderPath;

            if (Directory.Exists(path))
            {
                files = Directory.GetFiles(path);
                options = new string[files.Length + 1];
                for (int i = 0; i < files.Length; i++)
                {
                    options[i] = Path.GetFileName(files[i]);
                }
                options[files.Length] = "Retour";
            }
            else
            {
                options = new string[] { "Aucun fichier trouvé" };
            }

            ScrollingMenu menu = new ScrollingMenu(txt, 0, Placement.TopCenter, options);

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            switch (response!.Status)
            {
                case Status.Selected:
                    if (response.Value == files.Length)
                    {
                        result = "";
                    }
                    else
                    {
                        result = files[response.Value];
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    result = "";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Méthode pour choisir une image
        /// </summary>
        public static string ChooseImageMenu()
        {
            string result = "";
            string[] options = new string[]
            {
                "Par defaut",
                "Sortie Bitmap",
                "Sortie Steganography code",
                "Sortie Steganography decode",
                "Retour"
            };

            ScrollingMenu menu = new ScrollingMenu(
                "Dans quel dossier se trouve l'image : ",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();

            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0:
                            result = ChooseFile();
                            break;
                        case 1:
                            result = ChooseFile(true);
                            break;
                        case 2:
                            result = ChooseFile(true, true, true);
                            break;
                        case 3:
                            result = ChooseFile(true, true);
                            break;
                        case 4:
                            result = "";
                            break;
                    }
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    result = "";
                    break;
            }

            return result;
        }       

        /// <summary>
        /// Méthode pour fermer la session
        /// </summary>
        public static void CloseSession()
        {
            FakeLoadingBar loadingBar = new FakeLoadingBar("Fermeture de la session...");
            Window.AddElement(loadingBar);
            Window.ActivateElement(loadingBar);
            Window.Close();
        }
    }
}
