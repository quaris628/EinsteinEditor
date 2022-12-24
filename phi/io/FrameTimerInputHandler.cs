using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Timers;

// Note: System.Timers.Timer had problems when implemented,
//       seemingly with synchronization of accessing objects
//       System.Windows.Forms.Timer does not have these problems
//        - Nathan

namespace phi.io
{
   public class FrameTimerInputHandler
   {
      private Timer frames;
      private LinkedList<Action> frameActions;
      private Dictionary<int, Action> lockedFrameActions;

      public FrameTimerInputHandler(int fps)
      {
         frames = new Timer();
         frames.Interval = 1000 / fps;
         frames.Tick += new EventHandler(FrameTickEvent);
         //frames = new Timer(1000.0 / Config.RENDER.FPS);
         //frames.Elapsed += new ElapsedEventHandler(FrameTickEvent);
         frameActions = new LinkedList<Action>();
         lockedFrameActions = new Dictionary<int, Action>();
      }
      public FrameTimerInputHandler()
      {
         frames = null;
         frameActions = new LinkedList<Action>();
         lockedFrameActions = new Dictionary<int, Action>();
      }

      public void Subscribe(Action action)
      {
         frameActions.AddFirst(action);
      }

      public void Unsubscribe(Action action)
      {
         frameActions.Remove(action);
      }

      /**
       * Clear() will not removed locked subscriptions
       * locked subscriptions can only be unlocked with the integer key, and
       *    an Action that .Equals() the passed action
       */
      public void LockedSubscribe(int key, Action action)
      {
         lockedFrameActions[key] = action;
      }

      public void LockedUnsubscribe(int key, Action action)
      {
         Action find;
         lockedFrameActions.TryGetValue(key, out find);
         if (find.Equals(action))
         {
            lockedFrameActions.Remove(key);
         }
         // method untested
      }

      public void Clear()
      {
         frameActions.Clear();
      }

      public void Start()
      {
         if (frames == null)
         {
            throw new ArgumentException("Unspecified Frames per Second");
         }
         frames.Enabled = true;
         frames.Start();
      }

      public void SetFPS(int fps)
      {
         if (frames == null)
         {
            frames = new Timer();
            frames.Tick += new EventHandler(FrameTickEvent);
         }
         frames.Interval = 1000 / fps;
      }

      public void FrameTickEvent(object sender, EventArgs e)
      {
         foreach (Action action in lockedFrameActions.Values)
         {
            action.Invoke();
         }
         foreach (Action action in frameActions)
         {
            action.Invoke();
         }
      }
   }
}
