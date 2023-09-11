using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    public class InputVal : Value
    {
        public static Value CONSTANT = new InputVal(0);
        public static Value ENERGY_RATIO = new InputVal(1);
        public static Value MATURITY = new InputVal(2);
        public static Value LIFE_RATIO = new InputVal(3);
        public static Value FULLNESS = new InputVal(4);
        public static Value SPEED = new InputVal(5);
        public static Value ISGRABBING = new InputVal(6);
        public static Value ATTACKED_DAMAGE = new InputVal(7);
        public static Value BIBITE_CLOSENESS = new InputVal(8);
        public static Value BIBITE_ANGLE = new InputVal(9);
        public static Value N_BIBITES = new InputVal(10);
        public static Value PLANT_CLOSENESS = new InputVal(11);
        public static Value PLANT_ANGLE = new InputVal(12);
        public static Value N_PLANTS = new InputVal(13);
        public static Value MEAT_CLOSENESS = new InputVal(14);
        public static Value MEAT_ANGLE = new InputVal(15);
        public static Value N_MEATS = new InputVal(16);
        public static Value RED_BIBITE = new InputVal(17);
        public static Value GREEN_BIBITE = new InputVal(18);
        public static Value BLUE_BIBITE = new InputVal(19);
        public static Value TIC = new InputVal(20);
        public static Value MINUTE = new InputVal(21);
        public static Value TIME_ALIVE = new InputVal(22);
        public static Value PHERO_SENSE_1 = new InputVal(23);
        public static Value PHERO_SENSE_2 = new InputVal(24);
        public static Value PHERO_SENSE_3 = new InputVal(25);
        public static Value PHERO_1_ANGLE = new InputVal(26);
        public static Value PHERO_2_ANGLE = new InputVal(27);
        public static Value PHERO_3_ANGLE = new InputVal(28);
        public static Value PHERO_1_HEADING = new InputVal(29);
        public static Value PHERO_2_HEADING = new InputVal(30);
        public static Value PHERO_3_HEADING = new InputVal(31);
        public static Value INFECTION_RATE = new InputVal(32);

        private Neuron inputNeuron;
        private InputVal(int inputIndex)
        {
            inputNeuron = Inputs.ConcstructInputNeuron(inputIndex);
        }

        public Neuron GetInputNeuron() { return inputNeuron; }

        public override void AddOutputSynapse(Neuron output)
        {
            SynapseFactory.CreateSynapse(inputNeuron, output, 1);
        }
    }
}
