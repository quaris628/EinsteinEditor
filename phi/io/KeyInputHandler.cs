﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace phi.io
{
   public class KeyInputHandler
   {
      private Dictionary<int, LinkedList<Action>> actions;
      private Dictionary<int, LinkedList<Action<KeyStroke>>> keyActions;

      public KeyInputHandler()
      {
         actions = new Dictionary<int, LinkedList<Action>>();
         keyActions = new Dictionary<int, LinkedList<Action<KeyStroke>>>();
      }

      // Subscribe overloads
      public void Subscribe(Action action, Keys key) { Subscribe(action, new KeyStroke(key)); }
      public void Subscribe(Action<KeyStroke> action, Keys key) { Subscribe(action, new KeyStroke(key)); }
      public void Subscribe(Action action, KeyStroke keyStroke)
      {
         if (action != null && keyStroke != null)
         {
            if (!actions.ContainsKey(keyStroke.GetCode()))
            {
               actions[keyStroke.GetCode()] = new LinkedList<Action>();
            }
            actions[keyStroke.GetCode()].AddFirst(action);
         }
      }
      public void Subscribe(Action<KeyStroke> action, KeyStroke keyStroke)
      {
         if (action != null && keyStroke != null)
         {
            if (!keyActions.ContainsKey(keyStroke.GetCode()))
            {
               keyActions[keyStroke.GetCode()] = new LinkedList<Action<KeyStroke>>();
            }
            keyActions[keyStroke.GetCode()].AddFirst(action);
         }
      }

      // Unsubscribe overloads
      public void Unsubscribe(Action action, Keys key) { Unsubscribe(action, new KeyStroke(key)); }
      public void Unsubscribe(Action<KeyStroke> action, Keys key) { Unsubscribe(action, new KeyStroke(key)); }
      public void Unsubscribe(Action action, KeyStroke keyStroke) { actions[keyStroke.GetCode()].Remove(action); }
      public void Unsubscribe(Action<KeyStroke> action, KeyStroke keyStroke) { keyActions[keyStroke.GetCode()].Remove(action); }
      public void UnsubscribeAll(Keys key) { UnsubscribeAll(new KeyStroke(key)); }
      public void UnsubscribeAll(KeyStroke keyStroke) { actions.Remove(keyStroke.GetCode()); keyActions.Remove(keyStroke.GetCode()); }

      public void Clear()
      {
         actions.Clear();
         keyActions.Clear();
      }

      public void KeyInputEvent(object sender, KeyEventArgs e)
      {
         LinkedList<Action> todos = new LinkedList<Action>();
         KeyStroke stroke = new KeyStroke(e.KeyData);
         if (actions.TryGetValue(stroke.GetCode(), out LinkedList<Action> keystrokeActions))
         {
            // make copy of collection b/c otherwise could run into concurrent modification exception
            
            foreach (Action action in keystrokeActions)
            {
               todos.AddLast(action);
            }
         }
         if (keyActions.ContainsKey(stroke.GetCode()))
         {
            foreach (Action<KeyStroke> action in keyActions[stroke.GetCode()])
            {
               todos.AddLast(() => { action.Invoke(stroke); });
            }
         }

         void doLaterKeys()
         {
            IO.FRAME_TIMER.Unsubscribe(doLaterKeys);
            foreach (Action action in todos)
            {
               action.Invoke();
            }
         }
         IO.FRAME_TIMER.Subscribe(doLaterKeys);
      }

      public bool IsModifierKeyDown(Keys key)
      {
         System.Windows.Forms.Keys formsKey = (System.Windows.Forms.Keys)key;
         return formsKey == (Control.ModifierKeys & formsKey);
      }

   }
}
