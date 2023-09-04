using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;

namespace Bibyte
{
   public static partial class BrainCreator
   {
      // these functions are optional to use,
      // but should reduce the amount of code you need to write

      private static int hiddenNeuronIndex = BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN;
      
      private static Neuron createNeuron(NeuronType type)
      {
         return createNeuron(type, "Hidden" + hiddenNeuronIndex);
      }

      private static Neuron createNeuron(NeuronType type, string description)
      {
         if (type == NeuronType.Input)
         {
            throw new ArgumentException("You tried to create a new input neuron. That's not allowed, silly!");
         }
         return new Neuron(hiddenNeuronIndex++, type, description);
      }

      private static Brain buildBrainFrom(params Synapse[][] synapseArrays)
      {
         Brain brain = new Brain();

         foreach (Synapse[] synapses in synapseArrays)
         {
            foreach (Synapse synapse in synapses)
            {
               AddSynapseAndItsNeurons(brain, synapse);
            }
         }
         return brain;
      }

      private static Brain buildBrainFrom(params Synapse[] synapses)
      {
         Brain brain = new Brain();

         foreach (Synapse synapse in synapses)
         {
            AddSynapseAndItsNeurons(brain, synapse);
         }
         return brain;
      }

      private static void AddSynapseAndItsNeurons(Brain brain, Synapse synapse)
      {
         if (!brain.ContainsNeuron(synapse.From))
         {
            brain.Add(synapse.From);
         }
         if (!brain.ContainsNeuron(synapse.To))
         {
            brain.Add(synapse.To);
         }
         brain.Add(synapse);
      }
   }
}
