using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class UndoRedo
    {
        private UndoRedo() { }

        private static readonly LinkedList<UndoableCommand> history = new LinkedList<UndoableCommand>();
        private static readonly LinkedList<UndoableCommand> future = new LinkedList<UndoableCommand>();

        public static void Do(UndoableCommand command)
        {
            history.AddLast(command);
            future.Clear();
            command.Do();
        }

        public static void Undo()
        {
            if (history.Count == 0)
            {
                return;
            }

            UndoableCommand command = history.Last();
            history.RemoveLast();
            future.AddFirst(command);

            command.Undo();
        }

        public static void Redo()
        {
            if (future.Count == 0)
            {
                return;
            }

            UndoableCommand command = future.First();
            future.RemoveFirst();
            history.AddLast(command);

            command.Do();
        }

        public static bool CanUndo()
        {
            return history.Count > 0;
        }

        public static bool CanRedo()
        {
            return future.Count > 0;
        }

        public static void Clear()
        {

        }
    }
}
