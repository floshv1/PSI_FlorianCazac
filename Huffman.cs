using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_SouidiCazac
{
    public class Huffman
    {
        /// <summary>
        /// Initialisation d'un root
        /// </summary>
        public Node root { get; set; }

        /// <summary>
        /// Constructeur de la classe Huffman
        /// </summary>
        /// <param name="source">Tableau d'entiers </param>
        public Huffman(int[] source)
        {
            List<Node> nodes = new List<Node>();

            // On ajoute les noeuds qui ont une fréquence supérieure à 0
            // dans la liste des noeuds
            for (int i = 0; i < source.Length; i++)
                if (source[i] > 0)
                    nodes.Add(new Node { value = i, frequency = source[i] });

            // Tant qu'il reste plus d'un noeud
            // On crée un nouveau noeud parent
            while (nodes.Count > 1)
            {
                // On trie les noeuds par fréquence
                List<Node> orderedNodes = nodes.OrderBy(node => node.frequency).ToList();

                
                if (orderedNodes.Count >= 2)
                {
                    // On prend les deux premiers noeuds
                    List<Node> taken = orderedNodes.Take(2).ToList();

                    // On crée un nouveau noeud parent
                    Node parent = new Node()
                    {
                        value = -1,
                        // On ajoute la fréquence des deux noeuds
                        frequency = taken[0].frequency + taken[1].frequency,

                        // On ajoute les deux noeuds comme enfants du parent
                        left = taken[0],
                        right = taken[1]
                    };

                    // On supprime les deux noeuds de la liste des noeuds
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    // On ajoute le parent à la liste des noeuds    
                    nodes.Add(parent);
                }

                // On affecte le premier noeud restant à la racine
                if (nodes.Count > 0)
                {
                    root = nodes[0];
                }
                else
                {
                    root = null;
                }
            }
        }

        /// <summary>
        /// Méthode qui génère les codes de Huffman
        /// </summary>
        /// <returns>Un dictionnaire contenant les codes de Huffman</returns>
        public Dictionary<int, string> GenerateCodes()
        {
            Dictionary<int, string> codes = new Dictionary<int, string>();

            if (root != null)
            {
                GenerateCode(root, "", codes);
            }
            return codes;
        }

        /// <summary>
        /// Méthode qui génère les codes de Huffman
        /// </summary>
        /// <param name="node">Noeud à partir duquel on génère les codes</param>
        /// <param name="code">Code du noeud</param>
        /// <param name="codes">Dictionnaire contenant les codes de Huffman</param>
        private void GenerateCode(Node node, string code, Dictionary<int, string> codes)
        {
            if (node != null)
            {
                if (node.value != -1)
                {
                    // On ajoute le code du noeud à la liste des codes
                    codes.Add(node.value, code);
                }
                else
                {
                    // On génère les codes des enfants du noeud
                    GenerateCode(node.left, code + "0", codes);
                    GenerateCode(node.right, code + "1", codes);
                }
            }
        }
        
    }
}
