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
        public static Number CONSTANT = new InputNum(0);
        public static Number ENERGY_RATIO = new InputNum(1);
        public static Number MATURITY = new InputNum(2);
        public static Number LIFE_RATIO = new InputNum(3);
        public static Number FULLNESS = new InputNum(4);
        public static Number SPEED = new InputNum(5);
        public static Number ISGRABBING = new InputNum(6);
        public static Number ATTACKED_DAMAGE = new InputNum(7);
        public static Number BIBITE_CLOSENESS = new InputNum(8);
        public static Number BIBITE_ANGLE = new InputNum(9);
        public static Number N_BIBITES = new InputNum(10);
        public static Number PLANT_CLOSENESS = new InputNum(11);
        public static Number PLANT_ANGLE = new InputNum(12);
        public static Number N_PLANTS = new InputNum(13);
        public static Number MEAT_CLOSENESS = new InputNum(14);
        public static Number MEAT_ANGLE = new InputNum(15);
        public static Number N_MEATS = new InputNum(16);
        public static Number RED_BIBITE = new InputNum(17);
        public static Number GREEN_BIBITE = new InputNum(18);
        public static Number BLUE_BIBITE = new InputNum(19);
        public static Number TIC = new InputNum(20);
        public static Number MINUTE = new InputNum(21);
        public static Number TIME_ALIVE = new InputNum(22);
        public static Number PHERO_SENSE_1 = new InputNum(23);
        public static Number PHERO_SENSE_2 = new InputNum(24);
        public static Number PHERO_SENSE_3 = new InputNum(25);
        public static Number PHERO_1_ANGLE = new InputNum(26);
        public static Number PHERO_2_ANGLE = new InputNum(27);
        public static Number PHERO_3_ANGLE = new InputNum(28);
        public static Number PHERO_1_HEADING = new InputNum(29);
        public static Number PHERO_2_HEADING = new InputNum(30);
        public static Number PHERO_3_HEADING = new InputNum(31);
        public static Number INFECTION_RATE = new InputNum(32);

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
