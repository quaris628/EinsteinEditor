using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class AutoArranger
    {
        private AutoArranger() { }

        public class Arrangement
        {

            
        }

        public class ArrangeableNeuron
        {



        }

        private static BaseBrain brain;
        private static LinkedList<BaseNeuron> inputNeurons = new LinkedList<BaseNeuron>();
        private static LinkedList<BaseNeuron> outputNeurons = new LinkedList<BaseNeuron>();
        private static LinkedList<BaseNeuron> hiddenNeurons = new LinkedList<BaseNeuron>();

        public static Arrangement CalculateArrangement(BaseBrain brain)
        {
            AutoArranger.brain = brain;
            SetNeuronLists();

            foreach (BaseNeuron output in outputNeurons)
            {
                
            }
        }

        private static void SetNeuronLists()
        {
            inputNeurons = new LinkedList<BaseNeuron>();
            outputNeurons = new LinkedList<BaseNeuron>();
            hiddenNeurons = new LinkedList<BaseNeuron>();

            foreach (BaseNeuron neuron in brain.Neurons)
            {
                if (neuron.IsInput())
                {
                    inputNeurons.AddLast(neuron);
                }
                else if (neuron.IsOutput())
                {
                    outputNeurons.AddLast(neuron);
                }
                else
                {
                    hiddenNeurons.AddLast(neuron);
                }
            }
        }
    }
}
