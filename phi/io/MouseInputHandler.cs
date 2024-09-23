﻿using phi.graphics;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using phi.other;
using System.Reflection;

namespace phi.io
{
   public class MouseInputHandler
   {
      private LinkedList<Action<int, int>> actions;
      private FastClickRegions<Action<int, int>> fastRegions;
      // private Dictionary<Rectangle, LinkedList<Action<int, int>>> regionActions;
      private Dictionary<Drawable, LinkedList<Action<int, int>>> drawableActions;
      private Dictionary<Action, Action<int, int>> wrapIndex;

      public MouseInputHandler()
      {
         actions = new LinkedList<Action<int, int>>();
         fastRegions = new FastClickRegions<Action<int, int>>();
         drawableActions = new Dictionary<Drawable, LinkedList<Action<int, int>>>();
         wrapIndex = new Dictionary<Action, Action<int, int>>();
      }

      public void Subscribe(Action<int, int> action) { actions.AddFirst(action); }
      public void Unsubscribe(Action<int, int> action) { actions.Remove(action); }

      public void SubscribeOnDrawable(Action<int, int> action, Drawable drawable)
      {
         LinkedList<Action<int, int>> existingActions;
         if (!drawableActions.TryGetValue(drawable, out existingActions))
         {
            existingActions = new LinkedList<Action<int, int>>();
            drawableActions.Add(drawable, existingActions);
         }
         existingActions.AddFirst(action);
      }
      public void UnsubscribeFromDrawable(Action<int, int> action, Drawable drawable)
      {
         drawableActions[drawable].Remove(action);
         if (drawableActions[drawable].Count == 0)
         {
            drawableActions.Remove(drawable);
         }
      }
      public void UnsubscribeAllFromDrawable(Drawable drawable)
      {
         drawableActions.Remove(drawable);
      }

      public void SubscribeOnRegion(Action<int, int> action, Rectangle region)
      {
         fastRegions.Add(action, region);
      }

      public void UnsubscribeFromRegion(Action<int, int> action, Rectangle region)
      {
         fastRegions.Remove(action, region);
      }

      // Subscription overloads for no-parameter actions
      private Action<int, int> Wrap(Action action)
      {
         Action<int, int> actionXY;
         if (!wrapIndex.TryGetValue(action, out actionXY))
         {
            actionXY = new Action<int, int>((a, b) => { action.Invoke(); });
            wrapIndex.Add(action, actionXY);
         }
         return actionXY;
      }
      public void Subscribe(Action action) { Subscribe(Wrap(action)); }
      public void Unsubscribe(Action action) { Unsubscribe(Wrap(action)); }
      public void SubscribeOnDrawable(Action action, Drawable drawable) { SubscribeOnDrawable(Wrap(action), drawable); }
      public void UnsubscribeFromDrawable(Action action, Drawable drawable) { UnsubscribeFromDrawable(Wrap(action), drawable); }
      public void SubscribeOnRegion(Action action, Rectangle region) { SubscribeOnRegion(Wrap(action), region); }
      public void UnsubscribeFromRegion(Action action, Rectangle region) { UnsubscribeFromRegion(Wrap(action), region); }

      public void Clear()
      {
         actions.Clear();
         fastRegions = new FastClickRegions<Action<int, int>>();
         drawableActions.Clear();
      }

      public void Event(object sender, MouseEventArgs e)
      {
         // Deep copy actions to do after iteration through collections of actions
         // This resolves what should happen if one of those actions edits one of the
         //   collections. (Throws exception if done during iteration.)
         LinkedList<Action<int, int>> todos = new LinkedList<Action<int, int>>();

         // Actions
         LinkedListNode<Action<int, int>> iter = actions.First;
         if (iter != null)
         {
            while (iter.Next != null)
            {
               todos.AddLast(iter.Value);
               iter = iter.Next;
            }
            todos.AddLast(iter.Value);
         }

         // Fast Regions Actions
         IEnumerable<Action<int, int>> fastRegionsTodos = fastRegions.GetClickItems(e.X, e.Y);

         // Drawable Actions
         // TODO this could be made more efficient by sorting the drawables by their layer in the internal data structure
         // Only one drawable should be able to be clicked at a time.
         // And it should be the drawable in the farthest-front layer.
         int layerOfFurthestFront = -1;
         Drawable furthestFront = null;
         foreach (Drawable d in drawableActions.Keys)
         {
            if (d.IsDisplaying() &&
               layerOfFurthestFront <= IO.RENDERER.GetLayerOf(d) && 
               d.GetBoundaryRectangle().Contains(e.X, e.Y))
            {
               layerOfFurthestFront = IO.RENDERER.GetLayerOf(d);
               furthestFront = d;
            }
         }
         if (furthestFront != null)
         {
            foreach (Action<int, int> action in drawableActions[furthestFront])
            {
               todos.AddLast(action);
            }
         }

         // do all the actions after 'deciding which actions to do' is complete
         // so one action does not change a state that 'deciding which actions to do' depends on
         // in the middle of 'deciding which actions to do'
         /*
         if (fastRegionsTodos != null)
         {
            foreach (Action<int, int> action in fastRegionsTodos)
            {
               action.Invoke(e.X, e.Y);
            }
         }
         // */

         void doLaterMouse()
         {
            IO.FRAME_TIMER.Unsubscribe(doLaterMouse);
            foreach (Action<int, int> action in todos)
            {
               action.Invoke(e.X, e.Y);
            }
         }
         IO.FRAME_TIMER.Subscribe(doLaterMouse);
      }

      public string LogDetailsForCrash()
      {
         string log = "";
         foreach (KeyValuePair<Drawable, LinkedList<Action<int, int>>> kvp in drawableActions)
         {
            log += "\nactions for drawable " + kvp.Key;
            foreach (Action<int, int> actionXY in kvp.Value)
            {
               log += "\n\t";
               MethodInfo method = actionXY.Method;
               if (wrapIndex.ContainsValue(actionXY))
               {
                  // find the wrapped method and log that instead
                  log += "Wrapped: ";
                  method = wrapIndex.FirstOrDefault(kvp1 => kvp1.Value == actionXY).Key.Method;
               }
               log += method.DeclaringType.FullName + "." + method.Name;
            }
         }
         log += "\nactions:";
         foreach (Action<int, int> actionXY in actions)
         {
            log += "\n\t";
            MethodInfo method = actionXY.Method;
            if (wrapIndex.ContainsValue(actionXY))
            {
               // find the wrapped method and log that instead
               log += "Wrapped: ";
               method = wrapIndex.FirstOrDefault(kvp1 => kvp1.Value == actionXY).Key.Method;
            }
            log += method.DeclaringType.FullName + "." + method.Name;
         }
         return log;
      }
   }
}
