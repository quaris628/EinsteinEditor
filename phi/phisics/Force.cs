using phi.phisics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics
{
   class Force
   {
      double mass;
      Vector acceleration;
      public Force(double mass, Vector acceleration)
      {
         this.mass = mass;
         this.acceleration = acceleration;
      }
      public Vector getNewtons()
      {
         return new Vector(acceleration * mass, acceleration.getDirection());
      }
   }
}
