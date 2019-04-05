using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeCsharp
{
    class Attribute
    {
        public string Name { get; set; }
        public List<object> Values { get; set; }

        public Attribute(string Name, List<object> Values)
        {
            this.Name = Name;
            this.Values = Values;
        }

        public Attribute() { }
    }
}
