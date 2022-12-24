using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

      public void Clear()
      {
         actions.Clear();
         keyActions.Clear();
      }

      public void KeyInputEvent(object sender, KeyEventArgs e)
      {
         KeyStroke stroke = new KeyStroke(e.KeyData);
         if (actions.ContainsKey(stroke.GetCode()))
         {
            // This extra-verbose while loop iteration is here instead of a simpler foreach
            //    to force iteration to continue even if an element in the collection is updated.
            // If this was replaced by a foreach, an exception would be thrown.
            IEnumerator<Action> todos = actions[stroke.GetCode()].GetEnumerator();
            while(todos.MoveNext())
            {
               Action action = todos.Current;
               action.Invoke();
            }
         }
         if (keyActions.ContainsKey(stroke.GetCode()))
         {
            foreach (Action<KeyStroke> action in keyActions[stroke.GetCode()])
            {
               action.Invoke(stroke);
            }
         }
      }

   }
}
