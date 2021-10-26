using System.Collections.Generic;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Output
    {
        [XmlRoot(ElementName="Solution")]
        public class Solution {
            [XmlAttribute(AttributeName="Runtime")]
            public string Runtime { get; set; }
            [XmlAttribute(AttributeName="MeanE2E")]
            public string MeanE2E { get; set; }
            [XmlAttribute(AttributeName="MeanBW")]
            public string MeanBW { get; set; }
        }

        [XmlRoot(ElementName="Link")]
        public class Link {
            [XmlAttribute(AttributeName="Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName="Destination")]
            public string Destination { get; set; }
            [XmlAttribute(AttributeName="Qnumber")]
            public string Qnumber { get; set; }
        }

        [XmlRoot(ElementName="Message")]
        public  class Message2 {
            [XmlElement(ElementName="Link")]
            public List<Link> Link { get; set; }
            [XmlAttribute(AttributeName="Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName="maxE2E")]
            public string MaxE2E { get; set; }
        }

        [XmlRoot(ElementName="Report")]
        public class Report {
            [XmlElement(ElementName="Solution")]
            public Solution Solution { get; set; }
            [XmlElement(ElementName="Message")]
            public List<Inputs.Message> Message { get; set; }
        }
    }
}