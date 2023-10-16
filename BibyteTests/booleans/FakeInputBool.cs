using bibyte.functional.background;
using Bibyte.functional.background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibyteTests.booleans
{
    public class FakeInputBool : Bool
    {
        // TODO figure this whole class out
        public LinkedList<ConnectToRequest> connectToRequests
            = new LinkedList<ConnectToRequest>();
        protected override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            connectToRequests.Concat(outputConns);
        }
    }
}
