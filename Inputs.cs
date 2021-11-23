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
        static Dictionary<string,List<string>> dict = new();
        static string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;  
        public static Application readApps()
        {

            string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\files\Example\Input\Apps.xml");
            string sFilePath = Path.GetFullPath(sFile);
            Application res = new Application();

            var xs = new XmlSerializer(typeof(Application));
            using (FileStream fileStream =
                new FileStream(sFilePath, FileMode.Open))
            {
                res = (Application) xs.Deserialize(fileStream);
            }

            return res;
        }

        public static Architecture readConfig()
        {
            Architecture res = new Architecture();
            string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\files\Example\Input\Config.xml");
            string sFilePath = Path.GetFullPath(sFile);
            var xs = new XmlSerializer(typeof(Architecture));
            using (FileStream fileStream =
                new FileStream(sFilePath, FileMode.Open))
            {
                res = (Architecture) xs.Deserialize(fileStream);
            }
            mapVertex(res);

            return res;
        }

        public static Output.Report initialSolution(Application apps, Architecture arch)
        {
            return findPath(apps, arch);
        }

        public static Output.Report findPath(Application apps, Architecture arch)
        {
            var random = new Random();
            var solution = new Output.Report();
            var output = new Output();
            solution.Message = new List<Output.Message>();
            int i = 0;
            foreach (var message in apps.Message)
            {
                //Initiate maxE2E at 0 and cycle length, c at 12(microsec) for each message.
                int maxE2E = 0;
                int cycleLength = 12;
                var links = new List<Output.Link>();
                string destination = message.Destination;

                if (dict.ContainsKey(message.Source))
                {
                    // Liste til at tilføje besgøte vertexes så vi ikke ender i et loop og besøger de samme vertexes
                    List<string> visitedNodes = new List<string>();
                    visitedNodes.Add(message.Source);
                    List<string> optionsFromCurrentNode = dict[message.Source];
                    int randomIndex = random.Next(optionsFromCurrentNode.Count);
                    string selectedDestinationFromCurrentNode = optionsFromCurrentNode[randomIndex];
                    var link = new Output.Link();
                    link.Source = message.Source;
                    link.Destination = selectedDestinationFromCurrentNode;
                    link.Qnumber = random.Next(1,4).ToString();

                    //add PropDelay of given edge to maxE2E.
                    /**
                    foreach (var edge in arch.Edge)
                    {
                        if (link.Source == edge.Source && link.Destination == edge.Destination)
                        {
                            maxE2E += Int32.Parse(edge.PropDelay);
                        }
                    }
                    */
                    maxE2E += 10;
                    //add QNumber * c to maxE2E
                    maxE2E += Int32.Parse(link.Qnumber) * cycleLength;
                    links.Add(link);
                    while (selectedDestinationFromCurrentNode != destination)
                    {
                        // tilføjer besøgte vertexes
                        visitedNodes.Add(selectedDestinationFromCurrentNode);
                        if (dict.ContainsKey(selectedDestinationFromCurrentNode))
                        {
                            var source = selectedDestinationFromCurrentNode;
                            List<string> selectedDestinationFromCurrentNode2 = dict[selectedDestinationFromCurrentNode];
                            link = new Output.Link();
                            link.Source = source;

                            randomIndex = random.Next(selectedDestinationFromCurrentNode2.Count);
                            selectedDestinationFromCurrentNode = selectedDestinationFromCurrentNode2[randomIndex];
                            //Make sure selectedDestinationFromCurrentNode is not one which is already within visitedNodes.
                            while (visitedNodes.Contains(selectedDestinationFromCurrentNode))
                            {
                                randomIndex = random.Next(selectedDestinationFromCurrentNode2.Count);
                                selectedDestinationFromCurrentNode = selectedDestinationFromCurrentNode2[randomIndex];
                            }
                            link.Destination = selectedDestinationFromCurrentNode;
                            /**
                            foreach (var edge in arch.Edge)
                            {
                                if (link.Source == edge.Source && link.Destination == edge.Destination)
                                {
                                    maxE2E += Int32.Parse(edge.PropDelay);
                                }
                            }
                            */
                            maxE2E += 10;
                            link.Qnumber = random.Next(1,4).ToString();
                            maxE2E += Int32.Parse(link.Qnumber) * cycleLength;
                            links.Add(link);
                        }
                    }
                }
                var m = new Output.Message();
                m.Link = links;
                m.Name = message.Name;
                m.MaxE2E = maxE2E.ToString();
                solution.Message.Add(m);
                i++;
            }

            output.GiveOutput(solution);

            return solution;
        }

        public static int meanBandWidth(Application apps, Architecture arch)
        {
            int counter = 0;
            int sum = 0;
            foreach (var arc in arch.Edge)
            {
                sum += Int32.Parse(arc.BW);
                counter++;

            }
            return sum / counter;
        }

        public static int meanE2E(Application apps, Architecture arch)
        {
            int counter = 0;
            int sum = 0;
            foreach (var vertex in arch.Vertex)
            {
                sum += Int32.Parse(vertex.MaxE2E);
                counter++;
            }
            return sum / counter;
        }


        public static int deadlineConstraint(Application apps, Architecture arch)
        {
            //Make sure to tie together, the name (e.g. Name="F1", in Aps.xml), with
            //the corresponding maxE2E.
            //The solution is properbly best of, when returning an int, at which the
            //int indicates how many deadlines are not currently being met,
            //and zero meaning that all deadlines are met.
            //In ReportTest.xml, it can be seen that a mesage consists of e.g.
            //(  <Message Name="F1" maxE2E="90">). thus it should be easy to tie together.

            foreach (var app in apps.Message)
            {
                string str = app.Name;
                string deadline = app.Deadline;


            }
        }


        public static void mapVertex(Architecture arch)
        {
            foreach (var vertex in arch.Vertex)
            {
                List<string> sList = new List<string>();
                foreach (var edge in arch.Edge)
                {
                    if (vertex.Name == edge.Source)
                    {
                        sList.Add(edge.Destination);
                    }

                    if (vertex.Name == edge.Destination)
                    {
                        sList.Add(edge.Source);
                    }
                }

                if (sList.Count > 0)
                {
                    dict.Add(vertex.Name, sList);
                }
            }
            Console.WriteLine(dict.Count);
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
            [XmlAttribute(AttributeName="MaxE2E")]
            public string MaxE2E { get; set; }
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
