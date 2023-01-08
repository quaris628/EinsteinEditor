using Einstein.model;
using Einstein.ui.editarea.visibleElements;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class SynapseMultiRenderable
    {
        public const float DEFAULT_STRENGTH = 1f;

        public const int LAYER = 5;
        public const int LINE_WIDTH = 2 * HALF_LINE_WIDTH;
        public const int HALF_LINE_WIDTH = 3;
        public static readonly Color LINE_COLOR = Color.Black;

        public BaseSynapse Synapse { get; private set; }
        public NeuronDraggable From { get; private set; }
        public NeuronDraggable To { get; private set; }

        private Line line;
        private SynapseArrow arrow;
        private EditArea editArea;

        public SynapseMultiRenderable(EditArea editArea, NeuronDraggable from, int mouseX, int mouseY)
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
            IO.RENDERER.Add(line, LAYER);
            IO.RENDERER.Add(arrow, LAYER);
            IO.MOUSE.MOVE.Subscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Subscribe(FinalizeTipPosition);
        }
        public void Uninitialize()
        {
            IO.RENDERER.Remove(line);
            IO.RENDERER.Remove(arrow);
            IO.MOUSE.MOVE.Unsubscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(FinalizeTipPosition);
        }

        // sort of a second step of initialization (or will uninitialize)
        public void SetTipNeuron(NeuronDraggable dragNeuron)
        {
            To = dragNeuron;
            if (To == null)
            {
                Uninitialize();
                return;
            }

            IO.MOUSE.MOVE.Unsubscribe(UpdateTipPositionXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(FinalizeTipPosition);
            IO.MOUSE.MID_CLICK_UP.SubscribeOnDrawable(RemoveIfExactlyContainsClick, line);
            To.SubscribeOnDrag(UpdateTipPosition);
            UpdateTipPosition();

            // TODO allow setting a different strength besides the default
            Synapse = new BaseSynapse(From.Neuron, To.Neuron, DEFAULT_STRENGTH);
            editArea.FinishSynapse(Synapse);
        }

        private void UpdateBasePosition()
        {
            UpdateBasePositionXY(From.NeuronDrawable.GetCircleCenterX(),
                               From.NeuronDrawable.GetCircleCenterY());
            if (To != null)
            {
                UpdateTipPosition();
            }
        }
        private void UpdateBasePositionXY(int x, int y)
        {
            line.SetXY1(x, y);
            arrow.SetDirection(
                line.GetX2() - x,
                line.GetY2() - y);
        }

        // Only use if To is set
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
        }
        private void UpdateTipPositionXY(int x, int y)
        {
            line.SetXY2(x, y);
            arrow.SetXY(x, y);
            arrow.SetDirection(
                x - From.NeuronDrawable.GetCircleCenterX(),
                y - From.NeuronDrawable.GetCircleCenterY());
        }

        private void FinalizeTipPosition(int x, int y)
        {
            SetTipNeuron(editArea.HasNeuronAtPosition(x, y));
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
    }
}
