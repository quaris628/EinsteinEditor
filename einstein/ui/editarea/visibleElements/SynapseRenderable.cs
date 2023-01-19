using Einstein.model;
using Einstein.model.json;
using Einstein.ui.editarea.visibleElements;
using phi.graphics;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* removing a neuron doesn't remove any linked synapse MRs
(including synapses coming from the neuron but not linked to any To neuron yet)

TODO: create another child class that extends SelectableEditableText specificially
for the neuron editable text, and be sure to make it resposition right during backspaces,
and to allow a value to be set if it's deselected while blank (though maybe actually put
that as an optional default-value-if-deselected-while-blank parameter to SelectableEditableText?)
*/

namespace Einstein.ui.editarea
{
    public class SynapseRenderable : MultiRenderable
    {
        public const int LAYER = 5;
        public const int LINE_WIDTH = 2 * HALF_LINE_WIDTH;
        public const int HALF_LINE_WIDTH = 3;
        public static readonly Color LINE_COLOR = Color.DarkGray;
        public const float INITIAL_STRENGTH = 1f;
        public const string DEFAULT_STRENGTH = "0";

        public BaseSynapse Synapse { get; private set; }
        public NeuronRenderable From { get; private set; }
        public NeuronRenderable To { get; private set; }

        // drawables
        private Line line;
        private SynapseArrow arrow;
        private SelectableEditableText text; // technically a renderable

        private EditArea editArea;

        public SynapseRenderable(EditArea editArea, NeuronRenderable from, int mouseX, int mouseY)
        {
            From = from;
            From.SubscribeOnDrag(UpdateBasePosition);
            line = new Line(
                from.NeuronDrawable.GetCircleCenterX(),
                from.NeuronDrawable.GetCircleCenterY(),
                mouseX, mouseY);
            arrow = new SynapseArrow(mouseX, mouseY,
                mouseX - from.NeuronDrawable.GetCircleCenterX(),
                mouseY - from.NeuronDrawable.GetCircleCenterY());
            line.SetPen(new Pen(LINE_COLOR, LINE_WIDTH));
            arrow.SetPen(new Pen(LINE_COLOR, LINE_WIDTH));
            this.editArea = editArea;
        }

        public void Initialize()
        {
            IO.RENDERER.Add(this, LAYER);
            IO.MOUSE.MOVE.Subscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Subscribe(FinalizeTipPosition);
        }
        public void Uninitialize()
        {
            IO.RENDERER.Remove(this);
            IO.MOUSE.MOVE.Unsubscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(FinalizeTipPosition);
        }

        // sort of a second step of initialization (or will uninitialize)
        public void Finalize(NeuronRenderable to)
        {
            // if user cancels creation, or if would connect the same neuron to itself
            if (to == null || to.Neuron.Index == From.Neuron.Index)
            {
                editArea.ClearStartedSynapse();
                Uninitialize();
                return;
            }
            Finalize(to, new JsonSynapse(
                (JsonNeuron)From.Neuron,
                (JsonNeuron)to.Neuron,
                INITIAL_STRENGTH));
        }
        public void Finalize(NeuronRenderable to, BaseSynapse synapse)
        {
            // set data/properties
            To = to;
            Synapse = synapse;

            // set up strength editing text
            text = new SelectableEditableText(
                new SynapseStrengthET(Synapse),
                DEFAULT_STRENGTH);
            text.Initialize();

            // complete initialization
            IO.MOUSE.MOVE.Unsubscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(FinalizeTipPosition);
            IO.MOUSE.MID_CLICK_UP.SubscribeOnDrawable(RemoveIfExactlyContainsClick, line);
            To.SubscribeOnDrag(UpdateTipPosition);
            
            UpdateTipPosition();

            editArea.FinishSynapse(Synapse);
        }

        private void UpdateBasePosition()
        {
            int x = From.NeuronDrawable.GetCircleCenterX();
            int y = From.NeuronDrawable.GetCircleCenterY();
            line.SetXY1(x, y);
            arrow.SetDirection(
                line.GetX2() - x,
                line.GetY2() - y);

            if (To != null)
            {
                UpdateTipPosition();
            }
            UpdateTextPosition();
        }

        // Only use after running Finalize
        private void UpdateTipPosition()
        {
            int circleCenterX = To.NeuronDrawable.GetCircleCenterX();
            int circleCenterY = To.NeuronDrawable.GetCircleCenterY();
            float slopeDeltaX = circleCenterX - From.NeuronDrawable.GetCircleCenterX();
            float slopeDeltaY = circleCenterY - From.NeuronDrawable.GetCircleCenterY();
            if (slopeDeltaX == 0 && slopeDeltaY == 0)
            {
                // neurons are exactly on top of each other
                UpdateTipPositionXY(circleCenterX, circleCenterY);
                return;
            }
            float inputSlopeLength = (float)Math.Sqrt(
                slopeDeltaX * slopeDeltaX + slopeDeltaY * slopeDeltaY);
            float dX = -slopeDeltaX / inputSlopeLength;
            float dY = -slopeDeltaY / inputSlopeLength;
            int arrowTipX = (int)(circleCenterX + dX * NeuronDrawable.CIRCLE_RADIUS);
            int arrowTipY = (int)(circleCenterY + dY * NeuronDrawable.CIRCLE_RADIUS);
            UpdateTipPositionXY(arrowTipX, arrowTipY);
            UpdateTextPosition();
        }

        // intended only for use before Finalize has been run
        private void UpdateTipPositionXY(int x, int y)
        {
            line.SetXY2(x, y);
            arrow.SetXY(x, y);
            arrow.SetDirection(
                x - From.NeuronDrawable.GetCircleCenterX(),
                y - From.NeuronDrawable.GetCircleCenterY());
        }

        private void UpdateTextPosition()
        {
            text?.GetDrawable().SetCenterXY(line.GetCenterX(), line.GetCenterY());
        }

        private void FinalizeTipPosition(int x, int y)
        {
            Finalize(editArea.HasNeuronAtPosition(x, y));
        }

        private void RemoveIfExactlyContainsClick(int x, int y)
        {
            // We only know this click was inside the rectangle that
            // perfectly contains the line, but we only want to remove
            // the synapse if the click was on that line.

            // calculate click's distance to line
            // (derived from solving series of equations for the line and the
            //  perpendicular line that contains the click point)
            float m = line.GetHeight() / (float)line.GetWidth();
            float invM = -1 / m;
            float intersectX = ((line.GetY() - m * line.GetX()) - (y - invM * x)) / (invM - m);
            float intersectY = y + invM * (intersectX - x);
            float dx = x - intersectX;
            float dy = y - intersectY;
            double sqDistanceToLine = dx * dx + dy * dy;
            // if the distance is less than half of the line's width,
            // or if the click is inside the triangle of the arrow,
            if (sqDistanceToLine <= HALF_LINE_WIDTH * HALF_LINE_WIDTH
                || arrow.TriangleContainsPoint(x, y))
            {
                IO.MOUSE.MID_CLICK_UP.UnsubscribeFromDrawable(RemoveIfExactlyContainsClick, line);
                editArea.RemoveSynapse(Synapse);
            }
        }

        public IEnumerable<Drawable> GetDrawables()
        {
            yield return line;
            yield return arrow;
            if (text != null)
            {
                yield return text.GetDrawable();
            }
        }

        public override string ToString()
        {
            return "SynapseRenderable: " + string.Format(
                "Synapse = {{0}} From = {{1}} To = {{2}}",
                Synapse, From, To);
        }
    }
}
