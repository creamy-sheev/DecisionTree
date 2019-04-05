using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeCsharp
{
    static class ID3Helper
    {
        public static double CalculateEntropy(List<int> propability)
        {
            double entropy = 0;
            foreach (int num in propability)
            {
                if (((double)num / propability.Sum()) != 0)
                {
                    entropy += ((double)num / propability.Sum()) * (Math.Log((double)num / propability.Sum(), 2));
                }
                else
                {
                    entropy += ((double)num / propability.Sum()) * (Math.Log(1, 2));
                }
            }
            return -entropy;
        }

        public static double CalculateGain(double AtributeInfo, double SetEntropy)
        {
            return SetEntropy - AtributeInfo;
        }

        public static double CalculateSplitInfo(int propabilitySum, int decisionValuesCount)
        {
            double splitInfo = 0;
            if((double)propabilitySum/decisionValuesCount != 0)
            {
                splitInfo += -((double)propabilitySum / decisionValuesCount) * Math.Log((double)propabilitySum / decisionValuesCount, 2);
            }
            else
            {
                splitInfo += -((double)propabilitySum / decisionValuesCount) * (Math.Log(1, 2));
            }
            return splitInfo;
        }

        public static double InitSetEntropy(Attribute DecisionValues)
        {
            List<object> dv = DecisionValues.Values;
            List<object> distinctDecisionVal = dv.Distinct().OrderBy(x => x).ToList();
            List<int> counter = new List<int>();
            foreach (object item in distinctDecisionVal)
            {
                counter.Add((from val in dv where val.Equals(item) select val).Count());
            }

            return CalculateEntropy(counter);
        }
    }
}
