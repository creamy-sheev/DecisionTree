using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeCsharp
{
    class TreeNode
    {
        public object decription { get; set; }
        public string parentDescription { get; set; }
        public List<TreeNode> child { get; set; }

        public TreeNode()
        {
            decription = null;
            child = new List<TreeNode>();
            parentDescription = null;
        }

        public void PrintTree(string indent)
        {
            Console.WriteLine(indent);
            Console.Write(indent);
            if (child.Count == 0)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine($"{parentDescription} : {decription}");
            for (int i = 0; i < child.Count; i++)
            {
                child[i].PrintTree(indent);
            }
        }
    }
}
