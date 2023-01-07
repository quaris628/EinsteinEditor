using Einstein.model;
using Einstein.ui.menu;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class DraggableNeuron : Draggable
    {
        public const int SPAWN_X = 250 + NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD;
        public const int SPAWN_Y = 200 + (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3;

        public BaseNeuron Neuron { get; private set; }
        public NeuronDrawable NeuronDrawable { get { return (NeuronDrawable)GetDrawable(); } }

        public DraggableNeuron(BaseNeuron neuron)
            : base(new NeuronDrawable(neuron, SPAWN_X, SPAWN_Y), EditArea.BOUNDS)
        {
            Neuron = neuron;
            NeuronDrawable.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
        }
    }
}
