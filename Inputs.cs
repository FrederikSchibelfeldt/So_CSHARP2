﻿using System;
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
                //int maxE2E = 0;
                //int cycleLength = 12;
                var links = new List<Output.Link>();
                string destination = message.Destination;
                //add propDelay of given edge to maxE2E. 
                //maxE2E = maxE2E + arch.Edge.

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
                    links.Add(link);
                    //add QNumber * c to maxE2E
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
                            link.Qnumber = random.Next(1,4).ToString();
                            links.Add(link);
                            //add Qnumber * c to maxE2E
                        }
                    }
                }
                var m = new Output.Message();
                m.Link = links;
                m.Name = message.Name;
                //set m.MaxE2E to a string convertet value of the calculated maxE2E. 
                //m.MaxE2E = maxE2E
                // indsæt formel for maxe2e etc.
                solution.Message.Add(m);
                i++;
            }

            output.GiveOutput(solution);

            return solution;
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