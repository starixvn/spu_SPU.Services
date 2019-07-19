using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.SPU.Model
{
    [XmlRoot(ElementName = "ExecutePath")]
    public class ExecutePath
    {
        [XmlAttribute(AttributeName = "InstanceId")]
        public string InstanceId { get; set; }

        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
    }
}
