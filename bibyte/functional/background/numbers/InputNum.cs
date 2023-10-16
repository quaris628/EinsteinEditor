using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    /// <summary>
    /// A number from one of the bibite's input neurons.
    /// </summary>
    public class InputNum : Number
    {
        public static InputNum CONSTANT = new InputNum(0);
        public static InputNum ENERGY_RATIO = new InputNum(1);
        public static InputNum MATURITY = new InputNum(2);
        public static InputNum LIFE_RATIO = new InputNum(3);
        public static InputNum FULLNESS = new InputNum(4);
        public static InputNum SPEED = new InputNum(5);
        public static InputNum ISGRABBING = new InputNum(6);
        public static InputNum ATTACKED_DAMAGE = new InputNum(7);
        public static InputNum BIBITE_CLOSENESS = new InputNum(8);
        public static InputNum BIBITE_ANGLE = new InputNum(9);
        public static InputNum N_BIBITES = new InputNum(10);
        public static InputNum PLANT_CLOSENESS = new InputNum(11);
        public static InputNum PLANT_ANGLE = new InputNum(12);
        public static InputNum N_PLANTS = new InputNum(13);
        public static InputNum MEAT_CLOSENESS = new InputNum(14);
        public static InputNum MEAT_ANGLE = new InputNum(15);
        public static InputNum N_MEATS = new InputNum(16);
        public static InputNum RED_BIBITE = new InputNum(17);
        public static InputNum GREEN_BIBITE = new InputNum(18);
        public static InputNum BLUE_BIBITE = new InputNum(19);
        public static InputNum TIC = new InputNum(20);
        public static InputNum MINUTE = new InputNum(21);
        public static InputNum TIME_ALIVE = new InputNum(22);
        public static InputNum PHERO_SENSE_1 = new InputNum(23);
        public static InputNum PHERO_SENSE_2 = new InputNum(24);
        public static InputNum PHERO_SENSE_3 = new InputNum(25);
        public static InputNum PHERO_1_ANGLE = new InputNum(26);
        public static InputNum PHERO_2_ANGLE = new InputNum(27);
        public static InputNum PHERO_3_ANGLE = new InputNum(28);
        public static InputNum PHERO_1_HEADING = new InputNum(29);
        public static InputNum PHERO_2_HEADING = new InputNum(30);
        public static InputNum PHERO_3_HEADING = new InputNum(31);
        public static InputNum INFECTION_RATE = new InputNum(32);

        private Neuron inputNeuron;

        private InputNum(int inputIndex)
        {
            inputNeuron = Inputs.ConcstructInputNeuron(inputIndex);
        }

        public Neuron GetInputNeuron() { return inputNeuron; }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            foreach (Neuron output in outputs)
            {
                SynapseFactory.CreateSynapse(inputNeuron, output, 1);
            }
        }
    }
}
