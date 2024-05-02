using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using System;

namespace Bibyte.neural
{
    public class Inputs0_5
    {
        public static JsonNeuron CONSTANT         = ConcstructInputNeuron(0);
        public static JsonNeuron ENERGY_RATIO     = ConcstructInputNeuron(1);
        public static JsonNeuron MATURITY         = ConcstructInputNeuron(2);
        public static JsonNeuron LIFE_RATIO       = ConcstructInputNeuron(3);
        public static JsonNeuron FULLNESS         = ConcstructInputNeuron(4);
        public static JsonNeuron SPEED            = ConcstructInputNeuron(5);
        public static JsonNeuron ISGRABBING       = ConcstructInputNeuron(6);
        public static JsonNeuron ATTACKED_DAMAGE  = ConcstructInputNeuron(7);
        public static JsonNeuron BIBITE_CLOSENESS = ConcstructInputNeuron(8);
        public static JsonNeuron BIBITE_ANGLE     = ConcstructInputNeuron(9);
        public static JsonNeuron N_BIBITES        = ConcstructInputNeuron(10);
        public static JsonNeuron PLANT_CLOSENESS  = ConcstructInputNeuron(11);
        public static JsonNeuron PLANT_ANGLE      = ConcstructInputNeuron(12);
        public static JsonNeuron N_PLANTS         = ConcstructInputNeuron(13);
        public static JsonNeuron MEAT_CLOSENESS   = ConcstructInputNeuron(14);
        public static JsonNeuron MEAT_ANGLE       = ConcstructInputNeuron(15);
        public static JsonNeuron N_MEATS          = ConcstructInputNeuron(16);
        public static JsonNeuron RED_BIBITE       = ConcstructInputNeuron(17);
        public static JsonNeuron GREEN_BIBITE     = ConcstructInputNeuron(18);
        public static JsonNeuron BLUE_BIBITE      = ConcstructInputNeuron(19);
        public static JsonNeuron TIC              = ConcstructInputNeuron(20);
        public static JsonNeuron MINUTE           = ConcstructInputNeuron(21);
        public static JsonNeuron TIME_ALIVE       = ConcstructInputNeuron(22);
        public static JsonNeuron PHERO_SENSE_1    = ConcstructInputNeuron(23);
        public static JsonNeuron PHERO_SENSE_2    = ConcstructInputNeuron(24);
        public static JsonNeuron PHERO_SENSE_3    = ConcstructInputNeuron(25);
        public static JsonNeuron PHERO_1_ANGLE    = ConcstructInputNeuron(26);
        public static JsonNeuron PHERO_2_ANGLE    = ConcstructInputNeuron(27);
        public static JsonNeuron PHERO_3_ANGLE    = ConcstructInputNeuron(28);
        public static JsonNeuron PHERO_1_HEADING  = ConcstructInputNeuron(29);
        public static JsonNeuron PHERO_2_HEADING  = ConcstructInputNeuron(30);
        public static JsonNeuron PHERO_3_HEADING  = ConcstructInputNeuron(31);
        public static JsonNeuron INFECTION_RATE   = ConcstructInputNeuron(32);
      
        public static JsonNeuron ConcstructInputNeuron(int index)
        {
            if (index < BibiteVersion.V0_5.INPUT_NODES_INDEX_MIN
                || BibiteVersion.V0_5.INPUT_NODES_INDEX_MAX < index)
            {
                throw new ArgumentException("bad index");
            }
            return new JsonNeuron(index, NeuronType.Input,
            BibiteVersion.V0_5.DESCRIPTIONS[index], BibiteVersion.V0_5);
        }

    }
}
