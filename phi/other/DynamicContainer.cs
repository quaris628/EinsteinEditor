﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.other
{
    /**
    * A thing that holds objects that can change.
    * Each object can message this container with FlagChange() when
    *    the object itself has changed.
    * @Author Nathan Swartz
    */
    public class DynamicContainer
    {
        private bool changed;
        public delegate void OnChange(DynamicContainer container);
        public event OnChange OnChangeFlagged;

        public DynamicContainer()
        {
            changed = true;
        }
        public void FlagChange()
        {
            changed = true;
            OnChangeFlagged?.Invoke(this);
        }
        protected void UnflagChanges()
        {
            changed = false;
        }
        protected bool HasChanged()
        {
            return changed;
        }
    }
}
