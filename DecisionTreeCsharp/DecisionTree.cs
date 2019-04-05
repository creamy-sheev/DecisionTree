using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeCsharp
{
    class DecisionTree
    {
        private List<Attribute> AttributeList;
        private Attribute DecisionValues { get; set; }
        private double SetEntropy { get; set; }
        public TreeNode Root;

        public DecisionTree(string filePath)
        {
            AttributeList = SetDecisionMatrix(filePath);
            DecisionValues = AttributeList[AttributeList.Count - 1];
            AttributeList.Remove(DecisionValues);
        }

        private List<Attribute> SetDecisionMatrix(string filePath)
        {
            List<Attribute> attributeList = new List<Attribute>();
            try
            {
                var data = File.ReadAllLines(filePath);
                var temp = data.Select(x => (x.Split(',').Select(y => Convert.ChangeType(y, TypeCode.Object)).ToList())).ToList();
                //transpose data for easier handling
                var decisionMatrix = Enumerable.Range(0, temp[0].Count).Select(i => temp.Select(list => list[i]).ToList()).ToList();
                while (true)
                {
                    Console.WriteLine("Load first row as labels? (y)/(n)");
                    var answer = Console.ReadLine();
                    if (answer.Equals("y"))
                    {
                        for (int i=0; i < decisionMatrix.Count; i++)
                        {
                            List<object> atrLs = decisionMatrix[i];
                            string name = (string)atrLs[0];
                            atrLs.Remove(name);
                            Attribute atr = new Attribute(name, atrLs);
                            attributeList.Add(atr);
                        }
                        return attributeList;
                    }

                    if (answer.Equals("n"))
                    {
                        char name = '@';
                        for (int i = 0; i < decisionMatrix.Count; i++)
                        {
                            name++;
                            Attribute atr = new Attribute(name.ToString(), decisionMatrix[i]);
                            attributeList.Add(atr);
                        }
                        return attributeList;
                    }

                    else
                    {
                        Console.WriteLine("Wrong answer. Try again");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not find or read file. {ex.Message}");
                Console.ReadKey();
                Environment.Exit(0);
            }
            return attributeList;
        }

        public List<List<int>> DataProcessing(Attribute atr)
        {
            List<List<int>> atrData = new List<List<int>>();
            List<object> atrValues = atr.Values;
            var distinctAtrVal = atrValues.Distinct().OrderBy(x => x);
            var distinctDecisionVal = DecisionValues.Values.Distinct().OrderBy(x => x);

            foreach (object atrObj in distinctAtrVal)
            {
                List<int> atrValue = new List<int>();
                foreach (object decObj in distinctDecisionVal)
                {
                    int counter = 0;
                    for (int j = 0; j < atrValues.Count; j++)
                    {
                        if (atrValues[j].Equals(atrObj) && DecisionValues.Values[j].Equals(decObj))
                            counter++;
                    }
                    atrValue.Add(counter);
                }
                atrData.Add(atrValue);
            }
            return atrData;
        }

        public double AttributeInformation(List<List<int>> atrData)
        {
            double atrInformation = 0;
            foreach (List<int> valList in atrData)
            {
                atrInformation += ((double)valList.Sum() / DecisionValues.Values.Count) * ID3Helper.CalculateEntropy(valList);
            }
            return atrInformation;
        }

        public double SplitInfo(List<List<int>> atrData)
        {
            double splitInfo = 0;
            foreach (List<int> valList in atrData)
            {
                splitInfo += ID3Helper.CalculateSplitInfo(valList.Sum(), DecisionValues.Values.Count);
            }
            if (splitInfo != 0)
            {
                return splitInfo;
            }
            else
                return 1.0;
            
        }

        public Attribute SelectBestAtr(List<Attribute> attributeList)
        {
            List<double> atrGainRatio = new List<double>();
            for (int i = 0; i < attributeList.Count; i++)
            {
                atrGainRatio.Add(ID3Helper.CalculateGain(AttributeInformation(DataProcessing(attributeList[i])), SetEntropy) / SplitInfo(DataProcessing(attributeList[i])));
            }
            int maxGainRatio = atrGainRatio.
               Select((value, index) => new { Value = value, Index = index }).
                   Aggregate((a, b) => (a.Value > b.Value) ? a : b).
                       Index;
            return attributeList[maxGainRatio];
        }

        public TreeNode ID3Algorithm(List<Attribute> attributeList, Attribute decisionValues)
        {
            TreeNode tn = new TreeNode();
            List<object> dv = decisionValues.Values;
            if (attributeList.Count == 0)
            {
                tn.decription = "Failure";
                return tn;
            }

            if (dv.Distinct().Count().Equals(1))
            {
                tn.decription = dv[0];
                return tn;
            }

            #region ATTRIBUTE SELECTION, SUBSET CREATION, RECURRENCE
            DecisionValues = decisionValues;
            SetEntropy = ID3Helper.InitSetEntropy(decisionValues);
            Attribute selectedAtr = SelectBestAtr(attributeList);
            List<object> distinctSelectedAtrVal = selectedAtr.Values.Distinct().OrderBy(x => x).ToList();
            attributeList.Remove(selectedAtr);

            for (int dObj = 0; dObj < distinctSelectedAtrVal.Count(); dObj++)
            {
                List<Attribute> subsetAtrLs = new List<Attribute>();
                for (int i = 0; i < attributeList.Count; i++)
                {
                    List<object> subsetVal = new List<object>();
                    for (int j = 0; j < attributeList[i].Values.Count; j++)
                    {
                        if (selectedAtr.Values[j].Equals(distinctSelectedAtrVal[dObj]))
                        {
                            subsetVal.Add(attributeList[i].Values[j]);
                        }
                    }
                    subsetAtrLs.Add(new Attribute(attributeList[i].Name, subsetVal));
                }

                Attribute subsetDec = new Attribute();
                List<object> subsetDecVal = new List<object>();
                for (int j = 0; j < decisionValues.Values.Count; j++)
                {
                    if (selectedAtr.Values[j].Equals(distinctSelectedAtrVal[dObj]))
                    {
                        subsetDecVal.Add(decisionValues.Values[j]);
                    }
                }
                subsetDec.Name = decisionValues.Name;
                subsetDec.Values = subsetDecVal;

                tn.decription = selectedAtr.Name.ToString();
                //tn.child.Add(ID3Algorithm(subsetAtrLs, subsetDec));
                TreeNode childNode = ID3Algorithm(subsetAtrLs, subsetDec);
                childNode.parentDescription = distinctSelectedAtrVal[dObj].ToString();
                tn.child.Add(childNode);

                
            }
            #endregion
            return tn;
        }

        public void Run()
        {
            Root = ID3Algorithm(AttributeList, DecisionValues);
        }
    }
}
