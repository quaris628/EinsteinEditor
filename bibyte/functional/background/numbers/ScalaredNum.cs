using bibyte.functional.background;
using Bibyte.neural;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    /// <summary>
    /// A number that's a scalar multiple of an input number
    /// </summary>
    public class ScalaredNum : Number
    {
        private Number num;
        private float scalar;

        public ScalaredNum(Number num, float scalar)
        {
            this.num = num;
            this.scalar = scalar;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            num.ConnectTo(multiplyAllConnsBy(outputConns, scalar));
        }
    }
}
