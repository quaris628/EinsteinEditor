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
    public class NeuronRenderable : Draggable
    {
        public const int SPAWN_X = 250 + NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD;
        public const int SPAWN_Y = 200 + (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3;

        public BaseNeuron Neuron { get; private set; }
        public NeuronDrawable NeuronDrawable { get { return (NeuronDrawable)GetDrawable(); } }
        private List<Action> onDrag;

        private EditArea editArea;

        public NeuronRenderable(EditArea editArea, BaseNeuron neuron)
            : base(new NeuronDrawable(neuron, SPAWN_X, SPAWN_Y), EditArea.BOUNDS)
        {
            Neuron = neuron;
            NeuronDrawable.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
            onDrag = new List<Action>();
            this.editArea = editArea;
        }

        public override void Initialize()
        {
            base.Initialize();
            IO.RENDERER.Add(this);
            IO.MOUSE.MID_CLICK.SubscribeOnDrawable(() => {
                editArea.RemoveNeuron(Neuron);
            }, GetDrawable());
            IO.MOUSE.RIGHT_UP.SubscribeOnDrawable((x, y) => {
                editArea.StartSynapse(this, x, y);
            }, GetDrawable());
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            IO.RENDERER.Remove(this);
            IO.MOUSE.MID_CLICK.UnsubscribeAllFromDrawable(GetDrawable());
            IO.MOUSE.RIGHT_UP.UnsubscribeAllFromDrawable(GetDrawable());
        }

        public void SubscribeOnDrag(Action action)
        {
            onDrag.Add(action);
        }

        public void UnsubscribeFromDrag(Action action)
        {
            onDrag.Remove(action);
        }

        protected override void MyMouseMove(int x, int y)
        {
            foreach (Action action in onDrag)
            {
                action.Invoke();
            }
        }
    }
}
