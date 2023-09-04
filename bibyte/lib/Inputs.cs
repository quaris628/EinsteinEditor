using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;

namespace Bibyte
{
    public class Inputs
    {
        public static Neuron CONSTANT         = ConcstructInputNeuron(0);
        public static Neuron ENERGY_RATIO     = ConcstructInputNeuron(1);
        public static Neuron MATURITY         = ConcstructInputNeuron(2);
        public static Neuron LIFE_RATIO       = ConcstructInputNeuron(3);
        public static Neuron FULLNESS         = ConcstructInputNeuron(4);
        public static Neuron SPEED            = ConcstructInputNeuron(5);
        public static Neuron ISGRABBING       = ConcstructInputNeuron(6);
        public static Neuron ATTACKED_DAMAGE  = ConcstructInputNeuron(7);
        public static Neuron BIBITE_CLOSENESS = ConcstructInputNeuron(8);
        public static Neuron BIBITE_ANGLE     = ConcstructInputNeuron(9);
        public static Neuron N_BIBITES        = ConcstructInputNeuron(10);
        public static Neuron PLANT_CLOSENESS  = ConcstructInputNeuron(11);
        public static Neuron PLANT_ANGLE      = ConcstructInputNeuron(12);
        public static Neuron N_PLANTS         = ConcstructInputNeuron(13);
        public static Neuron MEAT_CLOSENESS   = ConcstructInputNeuron(14);
        public static Neuron MEAT_ANGLE       = ConcstructInputNeuron(15);
        public static Neuron N_MEATS          = ConcstructInputNeuron(16);
        public static Neuron RED_BIBITE       = ConcstructInputNeuron(17);
        public static Neuron GREEN_BIBITE     = ConcstructInputNeuron(18);
        public static Neuron BLUE_BIBITE      = ConcstructInputNeuron(19);
        public static Neuron TIC              = ConcstructInputNeuron(20);
        public static Neuron MINUTE           = ConcstructInputNeuron(21);
        public static Neuron TIME_ALIVE       = ConcstructInputNeuron(22);
        public static Neuron PHERO_SENSE_1    = ConcstructInputNeuron(23);
        public static Neuron PHERO_SENSE_2    = ConcstructInputNeuron(24);
        public static Neuron PHERO_SENSE_3    = ConcstructInputNeuron(25);
        public static Neuron PHERO_1_ANGLE    = ConcstructInputNeuron(26);
        public static Neuron PHERO_2_ANGLE    = ConcstructInputNeuron(27);
        public static Neuron PHERO_3_ANGLE    = ConcstructInputNeuron(28);
        public static Neuron PHERO_1_HEADING  = ConcstructInputNeuron(29);
        public static Neuron PHERO_2_HEADING  = ConcstructInputNeuron(30);
        public static Neuron PHERO_3_HEADING  = ConcstructInputNeuron(31);
        public static Neuron INFECTION_RATE   = ConcstructInputNeuron(32);
      
        private static Neuron ConcstructInputNeuron(int index)
        {
            if (index < BibiteVersionConfig.INPUT_NODES_INDEX_MIN
            || BibiteVersionConfig.INPUT_NODES_INDEX_MAX < index)
            {
                throw new ArgumentException("bad index");
            }
            return new Neuron(index, NeuronType.Input,
            BibiteVersionConfig.DESCRIPTIONS[index]);
        }

    }
}
