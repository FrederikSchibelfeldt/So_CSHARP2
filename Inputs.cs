using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Inputs
    {
        public static Architecture readApps()
        {
            Architecture res = new Architecture();

            var xs = new XmlSerializer(typeof(Architecture));
            using (FileStream fileStream =
                new FileStream("C:\\Users\\frederik\\RiderProjects\\So_CSHARP2\\files\\Apps.xml", FileMode.Open))
            {
                res = (Architecture) xs.Deserialize(fileStream);
            }

            return res;
        }
        
        public static Application readConfig()
        {
            Application res = new Application();

            var xs = new XmlSerializer(typeof(Application));
            using (FileStream fileStream =
                new FileStream("C:\\Users\\frederik\\RiderProjects\\So_CSHARP2\\files\\Config.xml", FileMode.Open))
            {
                res = (Application) xs.Deserialize(fileStream);
            }

            return res;
        }
        
        [XmlRoot(ElementName="Message")]
        public class Message {
            [XmlAttribute(AttributeName="Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName="Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName="Destination")]
            public string Destination { get; set; }
            [XmlAttribute(AttributeName="Size")]
            public string Size { get; set; }
            [XmlAttribute(AttributeName="Period")]
            public string Period { get; set; }
            [XmlAttribute(AttributeName="Deadline")]
            public string Deadline { get; set; }
        }

        [XmlRoot(ElementName="Application")]
        public class Application {
            [XmlElement(ElementName="Message")]
            public List<Message> Message { get; set; }
        }
        
        [XmlRoot(ElementName="Vertex")]
        public class Vertex {
            [XmlAttribute(AttributeName="Name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName="Edge")]
        public class Edge {
            [XmlAttribute(AttributeName="Id")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName="BW")]
            public string BW { get; set; }
            [XmlAttribute(AttributeName="PropDelay")]
            public string PropDelay { get; set; }
            [XmlAttribute(AttributeName="Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName="Destination")]
            public string Destination { get; set; }
        }

        [XmlRoot(ElementName="Architecture")]
        public class Architecture {
            [XmlElement(ElementName="Vertex")]
            public List<Vertex> Vertex { get; set; }
            [XmlElement(ElementName="Edge")]
            public List<Edge> Edge { get; set; }
        }
        
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
            public List<Message> Message { get; set; }
        }
    }
}