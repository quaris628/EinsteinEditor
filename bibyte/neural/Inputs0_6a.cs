using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using System;

namespace Bibyte.neural
{
    public class Inputs0_6a
    {
        public static JsonNeuron CONSTANT         = ConstructInputNeuron(0);
        public static JsonNeuron ENERGY_RATIO     = ConstructInputNeuron(1);
        public static JsonNeuron MATURITY         = ConstructInputNeuron(2);
        public static JsonNeuron LIFE_RATIO       = ConstructInputNeuron(3);
        public static JsonNeuron FULLNESS         = ConstructInputNeuron(4);
        public static JsonNeuron SPEED            = ConstructInputNeuron(5);
        public static JsonNeuron ISGRABBING       = ConstructInputNeuron(6);
        public static JsonNeuron ATTACKED_DAMAGE  = ConstructInputNeuron(7);
        public static JsonNeuron EGG_STORED       = ConstructInputNeuron(8);
        public static JsonNeuron BIBITE_CLOSENESS = ConstructInputNeuron(9);
        public static JsonNeuron BIBITE_ANGLE     = ConstructInputNeuron(10);
        public static JsonNeuron N_BIBITES        = ConstructInputNeuron(11);
        public static JsonNeuron PLANT_CLOSENESS  = ConstructInputNeuron(12);
        public static JsonNeuron PLANT_ANGLE      = ConstructInputNeuron(13);
        public static JsonNeuron N_PLANTS         = ConstructInputNeuron(14);
        public static JsonNeuron MEAT_CLOSENESS   = ConstructInputNeuron(15);
        public static JsonNeuron MEAT_ANGLE       = ConstructInputNeuron(16);
        public static JsonNeuron N_MEATS          = ConstructInputNeuron(17);
        public static JsonNeuron RED_BIBITE       = ConstructInputNeuron(18);
        public static JsonNeuron GREEN_BIBITE     = ConstructInputNeuron(19);
        public static JsonNeuron BLUE_BIBITE      = ConstructInputNeuron(20);
        public static JsonNeuron TIC              = ConstructInputNeuron(21);
        public static JsonNeuron MINUTE           = ConstructInputNeuron(22);
        public static JsonNeuron TIME_ALIVE       = ConstructInputNeuron(23);
        public static JsonNeuron PHERO_SENSE_1    = ConstructInputNeuron(24);
        public static JsonNeuron PHERO_SENSE_2    = ConstructInputNeuron(25);
        public static JsonNeuron PHERO_SENSE_3    = ConstructInputNeuron(26);
        public static JsonNeuron PHERO_1_ANGLE    = ConstructInputNeuron(27);
        public static JsonNeuron PHERO_2_ANGLE    = ConstructInputNeuron(28);
        public static JsonNeuron PHERO_3_ANGLE    = ConstructInputNeuron(29);
        public static JsonNeuron PHERO_1_HEADING  = ConstructInputNeuron(30);
        public static JsonNeuron PHERO_2_HEADING  = ConstructInputNeuron(31);
        public static JsonNeuron PHERO_3_HEADING  = ConstructInputNeuron(32);
        public static JsonNeuron INFECTION_RATE   = ConstructInputNeuron(33);
     
        public static JsonNeuron ConstructInputNeuron(int index)
        {
            if (index < BibiteVersion.V0_6_0a.INPUT_NODES_INDEX_MIN
            || BibiteVersion.V0_6_0a.INPUT_NODES_INDEX_MAX < index)
            {
                throw new ArgumentException("bad index");
            }
            return new JsonNeuron(index, NeuronType.Input,
            BibiteVersion.V0_6_0a.DESCRIPTIONS[index], BibiteVersion.V0_6_0a);
        }

    }
}
