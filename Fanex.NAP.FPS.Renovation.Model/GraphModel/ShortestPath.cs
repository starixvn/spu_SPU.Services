using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Model.GraphModel
{
    public class ShortestPath
    {
        public Node Start { get; set; }

        public Node End { get; set; }

        public Node[] Nodes { get; set; }

        public Relationship[] Relationships { get; set; }
    }
}
