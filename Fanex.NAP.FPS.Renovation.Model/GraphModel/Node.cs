using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Model.GraphModel
{
    public class Node
    {
        public string Id { get; set; }

        public string[] Labels { get; set; }

        public Customer Properties { get; set; }
    }
}
