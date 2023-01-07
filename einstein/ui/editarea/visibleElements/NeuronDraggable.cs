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
    public class NeuronDraggable : Draggable
    {
        public const int SPAWN_X = 250 + NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD;
        public const int SPAWN_Y = 200 + (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3;

        public BaseNeuron Neuron { get; private set; }
        public NeuronDrawable NeuronDrawable { get { return (NeuronDrawable)GetDrawable(); } }
        private List<Action<int, int>> onDrag;

        public NeuronDraggable(BaseNeuron neuron)
            : base(new NeuronDrawable(neuron, SPAWN_X, SPAWN_Y), EditArea.BOUNDS)
        {
            Neuron = neuron;
            NeuronDrawable.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
            onDrag = new List<Action<int, int>>();
        }

        public void SubscribeOnDrag(Action<int, int> action)
        {
            onDrag.Add(action);
        }

        public void UnsubscribeFromDrag(Action<int, int> action)
        {
            onDrag.Remove(action);
        }

        protected override void MyMouseMove(int x, int y)
        {
            foreach (Action<int, int> action in onDrag)
            {
                action.Invoke(x, y);
            }
        }
    }
}
