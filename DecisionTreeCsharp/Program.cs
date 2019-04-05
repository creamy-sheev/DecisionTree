using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            DecisionTree dt = new DecisionTree("C:\\Users\\Tajner\\Desktop\\playTennis.txt");
            dt.Run();
            dt.Root.PrintTree("");
            Console.ReadKey();
        }
    }
}
