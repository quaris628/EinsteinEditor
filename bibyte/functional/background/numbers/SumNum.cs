using bibyte.functional.background;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background
{
    /// <summary>
    /// A number composed from a sum of 2 input numbers
    /// </summary>
    public class SumNum : Number
    {
        private Number left;
        private Number right;
        private JsonNeuron linear;

        public SumNum(Number left, Number right)
        {
            this.left = left;
            this.right = right;
            linear = null;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (containsMults(outputConns) || linear != null)
            {
                // create a linear node in between the inputs and outputs
                if (linear == null)
                {
                    linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Sum");
                    Dictionary<Number, int> leafNumsToSum = new Dictionary<Number, int>();
                    CalculateLeafSumsToSum(this, leafNumsToSum);
                    foreach (Number leafNum in leafNumsToSum.Keys)
                    {
                        leafNum.ConnectTo(new[] { new ConnectToRequest(linear, leafNumsToSum[leafNum]) });
                    }
                }
                connectAndHandleLargeScalars(linear, outputConns);
            }
            else
            {
                Dictionary<Number, int> leafNumsToSum = new Dictionary<Number, int>();
                CalculateLeafSumsToSum(this, leafNumsToSum);
                foreach (Number leafNum in leafNumsToSum.Keys)
                {
                    leafNum.ConnectTo(multiplyAllConnsBy(outputConns, leafNumsToSum[leafNum]));
                }
            }
        }

        private static void CalculateLeafSumsToSum(Number num, Dictionary<Number, int> leafNumsToSum)
        {
            if (num is SumNum leftSumNum)
            {
                CalculateLeafSumsToSum(leftSumNum.left, leafNumsToSum);
                CalculateLeafSumsToSum(leftSumNum.right, leafNumsToSum);
            }
            else if (leafNumsToSum.ContainsKey(num))
            {
                leafNumsToSum[num]++;
            }
            else
            {
                leafNumsToSum[num] = 1;
            }
        }
    }
}
