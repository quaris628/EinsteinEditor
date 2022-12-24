using phi.phisics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhiGraphicsEngineExample.phi.phisics
{
   class Momentum
   {
      private double mass;
      private Vector momentum;
      public Momentum(double mass, Vector velocity)
      {
         this.mass = mass;
         this.momentum = new Vector((mass * (double)velocity), velocity.getDirection());
      }

      public Vector getMomentumUnit()
      {
         return momentum;
      }

      public Vector combineMomentum(Momentum m)
      {
         return new Vector((momentum + m.momentum).getMagnitude(), (momentum + m.momentum).getDirection());
      }
   }
}
