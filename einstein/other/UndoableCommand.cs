using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class UndoableCommand
    {
        private Action redo;
        private Action undo;
        
        public UndoableCommand(Action redo, Action undo)
        {
            this.redo = redo;
            this.undo = undo;
        }

        public void Do()
        {
            redo.Invoke();
        }
        public void Undo()
        {
            undo.Invoke();
        }
    }
}
