using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

namespace So_CSHARP
{
    public class Inputs
    {
        static Dictionary<Vertex,List<string>> dict = new();
        public static Application readApps()
        {
            Application res = new Application();

            var xs = new XmlSerializer(typeof(Application));
            using (FileStream fileStream =
                new FileStream("/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Input/Apps.xml", FileMode.Open))
            {
                res = (Application) xs.Deserialize(fileStream);
            }

            return res;
        }
        
        public static Architecture readConfig()
        {
            Architecture res = new Architecture();

            var xs = new XmlSerializer(typeof(Architecture));
            using (FileStream fileStream =
                new FileStream("/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Input/Config.xml", FileMode.Open))
            {
                res = (Architecture) xs.Deserialize(fileStream);
            }

            return res;
        }

        public static Output.Solution initialSolution(Application apps)
        {
            return null;
        }

        public static void mapVertex(Architecture arch)
        {
            List<string> sList = new List<string>();
            foreach (var vertex in arch.Vertex)
            {
                foreach (var ed in arch.Edge)
                {
                    if (vertex.Name == ed.Source)
                    {
                        sList.Add(ed.Destination);
                    }
                }
                dict.Add(vertex,sList);
                sList.Clear();
            }
            Console.WriteLine(dict.Count);
        }

        public void AssignQueNumber()
        {

        }

        public void CSQF(Dictionary<Vertex, List<string>> dict)
        {
            int counter = 0;
            int cycleLen = 12; // 12 micro-seconds
            //every time we go across a link in a possible route, a local counter is incremented.
            //depending on the the AssignQueue function, this counter corresponds to 1, 2, or 3 multipled by cycleLen.
            //
            AssignQueNumber(); //when traversing from one vertex to another, assign queue number(1-3) to link object
            
        }

        
        [XmlRoot(ElementName="Application")]
        public class Application {
            [XmlElement(ElementName="Message")]
            public List<Message> Message { get; set; }
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
    }
}