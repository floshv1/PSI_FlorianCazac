using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_SouidiCazac
{
    public class Node
    {
        // Valeur du noeud
        public int value { get; set; }
        // Frequence d'occurence de la valeur 
        public int frequency { get; set; }
        // Noeud gauche 
        public Node? left { get; set; }
        // Noeud droit
        public Node? right { get; set; }
    }
}
