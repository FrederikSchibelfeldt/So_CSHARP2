﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Inputs
    {
        // Try with inputPath later
        static string inputPath = "..\\So_CSHARP2\\files\\Example";

        static string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static Dictionary<string, List<string>> dict = new();
        public static Application readApps()
        {
            Application res = new Application();
            var xs = new XmlSerializer(typeof(Application));
            string sFilePath = Path.GetFullPath(inputPath + "\\Input\\Apps.xml");
            {
                using (FileStream fileStream =
                new FileStream(sFilePath, FileMode.Open))
                    res = (Application)xs.Deserialize(fileStream);
            }

            return res;
        }

        public static Architecture readConfig()
        {
            Architecture res = new Architecture();
            var xs = new XmlSerializer(typeof(Architecture));
            string sFilePath = Path.GetFullPath(inputPath + "\\Input\\Config.xml");
            {
                using (FileStream fileStream =
                new FileStream(sFilePath, FileMode.Open))
                    res = (Architecture)xs.Deserialize(fileStream);
            }

            return res;
        }

        public static void populateFields(Application apps, Architecture arch)
        {

            foreach (Message message in apps.Messages)
            {
                // Populate message with source and dist Vertex objects
                foreach (Vertex vertex in arch.Vertices)
                {
                    if (vertex.Name == message.Source)
                    {
                        message.SourceVertex = vertex;
                    }

                    if (vertex.Name == message.Destination)
                    {
                        message.DestinationVertex = vertex;
                    }
                }

                if (message.Source == null || message.Destination == null)
                {
                    Console.WriteLine("Source or Destination EMPTY message: " + message);
                    Console.WriteLine("HUH HUH ?? \n");
                    Environment.Exit(0);
                }

            }

            foreach (Edge edge in arch.Edges)
            {
                // Populate message with source and dist Vertex objects
                foreach (Vertex vertex in arch.Vertices)
                {
                    if (vertex.Name == edge.Source)
                    {
                        edge.SourceVertex = vertex;
                    }

                    if (vertex.Name == edge.Destination)
                    {
                        edge.DestinationVertex = vertex;
                    }
                }

                if (edge.Source == null || edge.Destination == null)
                {
                    Console.WriteLine("Source or Destination EMPTY EDGE: " + edge);
                    Console.WriteLine("HUH HUH ?? \n");
                    Environment.Exit(0);
                }

            }
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
            foreach (var message in apps.Messages)
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
                    link.Qnumber = random.Next(1, 4).ToString();

                    //add PropDelay of given edge to maxE2E.
                    foreach (var edge in arch.Edges)
                    {
                        if (link.Source == edge.Source && link.Destination == edge.Destination || link.Source == edge.Destination && link.Destination == edge.Source)
                        {
                            maxE2E += Int32.Parse(edge.PropDelay);
                        }
                    }
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
                            foreach (var edge in arch.Edges)
                            {
                                if (link.Source == edge.Source && link.Destination == edge.Destination || link.Source == edge.Destination && link.Destination == edge.Source)
                                {
                                    maxE2E += Int32.Parse(edge.PropDelay);
                                }
                            }

                            link.Qnumber = random.Next(1, 4).ToString();
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

            return solution;
        }

        //Add functionsimilar to NweRandomSOlution in the master branch. 

        // MeanBW is the average of 

        public static void costFunction()
        {
            //should go through every elem in the solution, and
            //compute whether the solution is better than the original solution
            //by the use of the three constraint functions below.
            //if a particular constraint is not fulfilled -> miss penalty. 
        }




        // INFO: THIS FUNCTION SHOULD BE INCLUDED AS PART OF message loop (the intention is for an outer variable to keep track) 
        // PARAM: "message": the message being sent 
        // PARAM: "edges": the edges the message is going through 
        private static long CalculateMeanBWforCurrentMessage(Message message, List<Edge> edges)
        {
            int sumBW = 0;

            foreach (Edge edge in edges)
            {
                edge.BWConsumption = Int32.Parse(message.Size); // BW_Consumption is just size of message sent through edge
                sumBW += edge.BWConsumption;          // SUM UP for each edge message is going through
            }

            return sumBW / edges.Count;
        }

        public static int meanE2E(Application apps, Architecture arch)
        {
            int counter = 0;
            int sum = 0;
            foreach (var vertex in arch.Vertices)
            {
                sum += Int32.Parse(vertex.MaxE2E);
                counter++;
            }
            return sum / counter;
        }


        public static bool deadlineContraint(int maxE2E, string deadline)
        {
            if (maxE2E < Int32.Parse(deadline))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void mapVertex(Architecture arch)
        {
            foreach (var vertex in arch.Vertices)
            {
                List<string> sList = new List<string>();
                foreach (var edge in arch.Edges)
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

        public static void mapVertexNeighbors(Architecture arch)
        {
            foreach (var vertex in arch.Vertices)

            {


                List<Vertex> neighbors = new();
                List<Edge> edgeNeighbors = new();


                foreach (Edge edge in arch.Edges)
                {
                    if (edge.Destination == vertex.Name || edge.Source == vertex.Name)
                    {
                        edgeNeighbors.Add(edge);
                    }
                }

                foreach (Edge edge in edgeNeighbors)
                {
                    if (edge.Source != vertex.Name)
                    {
                        neighbors.Add(edge.SourceVertex);
                    }

                    if (edge.Destination != vertex.Name)
                    {
                        neighbors.Add(edge.DestinationVertex);
                    }
                }

                vertex.vertexNeighbors = neighbors.ToList();

                // Console.WriteLine("---------------------- vertex ---------------------------------------------");
                //     Console.WriteLine(vertex);  
                //    Console.WriteLine("---------------------- neighbors ---------------------------------------------");
                //    Console.WriteLine(neighbors);
            }
            Console.WriteLine(" ----------------------Vertex Neighbors found---------------------------");
            System.Console.WriteLine();

        }

        public static void FindMessageRoutes(Application apps, Architecture arch)
        {
            int count_test = 0;

            List<string> visited;

            foreach (Message message in apps.Messages)
            {
                visited = new();
                visited.Add(message.Source);


                // Color console "skrift" for testing purposes
                Console.ForegroundColor = count_test % 2 == 0 ? ConsoleColor.Cyan : ConsoleColor.Green;

                Console.WriteLine("count_test");
                Console.WriteLine(count_test);
                Console.WriteLine("Finding Routes for Message: " + message.Name);
                //       Console.WriteLine("Message source to destination: " + message.Source. + " -> " + message.Destination);
                Console.WriteLine("Message source to destination: " + message.SourceVertex.Name + " -> " + message.DestinationVertex.Name);
                Console.WriteLine("------------------------------------------");

                // Call recursive utility
                List<Vertex> isVisited = new();
                List<Vertex> pathList = new();

                // add source to path
                pathList.Add(message.SourceVertex);
                isVisited.Add(message.SourceVertex);

                // Call to find possible 
                Console.WriteLine("--------------------findAllPossiblePaths--------------------");
                findAllPossiblePaths(message.SourceVertex, message.DestinationVertex, message, isVisited, pathList);

                // DEFINE  possible edge paths from possible vertex paths 
                // then add to message 
                message.PossibleEdgePaths = MessageVertexPathsToEdgePaths(message, arch);


                //message.PrintPossiblePaths();

                count_test++;

                Console.WriteLine("------------------------------------------");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void findAllPossiblePaths(Vertex source, Vertex destination, Message message, List<Vertex> isVisited, List<Vertex> localPathList)
        {
            if (source.Equals(destination))
            {
                Console.WriteLine(string.Join("\n PATH:", "\n"));
                localPathList.ForEach(p => Console.Write(p.Name));
                message.PossibleVerticesPath.Add(localPathList);


                return;
            }

            // Mark the current node
            isVisited.Add(source);

            // For all neighbor vertices 
            foreach (var vertex in source.vertexNeighbors)
            {
                if (!isVisited.Contains(vertex))
                {
                    // store current node
                    // in path[]
                    localPathList.Add(vertex);

                    findAllPossiblePaths(vertex, destination, message, isVisited,
                                    localPathList);

                    // remove current node in path
                    localPathList.Remove(vertex);
                }
            }
            isVisited.Remove(source);
        }
        // possibly return List<List<Edge>>
        public static List<List<Edge>> MessageVertexPathsToEdgePaths(Message message, Architecture arch)
        {

            List<List<Edge>> possiblePaths = new();
            List<Edge> path;


            foreach (List<Vertex> route in message.PossibleVerticesPath)
            {
                path = new();
                for (int i = 0; i < route.Count; i++)
                {
                    if (i < route.Count - 1)
                    {
                        path.Add(EdgeFromVerticies(route.ElementAt(i), route.ElementAt(i + 1), arch));
                    }
                }

                possiblePaths.Add(path);
            }

            //Return and add to possible edge paths for a message. 
            // message.PossibleEdgePaths.Add();                 // ADD TO MESSAGES AS WELL
            return possiblePaths.ToList();
        }
        public static Edge EdgeFromVerticies(Vertex source, Vertex destination, Architecture arch)
        {
            foreach (Edge edge in arch.Edges)
            {
                if (String.Equals(edge.Source, source.Name) && String.Equals(edge.Destination, destination.Name) || String.Equals(edge.Source, destination.Name) && String.Equals(edge.Destination, source.Name))
                {
                    return edge;
                }
            }
            return null;
        }
        public static Output.Report GenerateRandomSolution(Architecture arch, Application app)
        {
            var random = new Random();
            var solutionSpace = new Output.Report();
            var output = new Output();
            solutionSpace.Message = new List<Output.Message>();
            int i = 0;

            long sumBWForSolution = 0;
            foreach (Message message in app.Messages)
            {
                int maxE2E = 0;
                int cycleLength = 12;
                var links = new List<Output.Link>();

                List<Edge> chosenPath = message.PossibleEdgePaths[0]; // just take first path for now 
                sumBWForSolution = +CalculateMeanBWforCurrentMessage(message, chosenPath); // ineffienct looping 

                foreach (Edge edge in chosenPath)
                {
                    var link = new Output.Link();
                    link.Qnumber = random.Next(1, 4).ToString();
                    link.Source = message.Source;
                    link.Destination = message.Destination;
                    links.Add(link);
                }
                var m = new Output.Message();
                m.Link = links;
                m.Name = message.Name;
                m.MaxE2E = maxE2E.ToString();
                solutionSpace.Message.Add(m);
                i++;
                // Color console "skrift" for testing purposes
                Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.Cyan : ConsoleColor.Green;

                Console.WriteLine("message solved");
                Console.WriteLine(i);
                Console.WriteLine("Solving message: " + message.Name);
                //       Console.WriteLine("Message source to destination: " + message.Source. + " -> " + message.Destination);
                Console.WriteLine("Message source to destination: " + message.SourceVertex.Name + " -> " + message.DestinationVertex.Name);
                Console.WriteLine("------------------------------------------");
            }
            var solution = new Output.Solution();
            solutionSpace.Solution =solution;
            output.GiveOutput(solutionSpace);
            return solutionSpace;
            }

        }
    }


    [XmlRoot(ElementName = "Application")]
    public class Application
    {
        [XmlElement(ElementName = "Message")]
        public List<Message> Messages { get; set; }
    }

    [XmlRoot(ElementName = "Message")]
    public class Message
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Source")]
        public string Source { get; set; }
        public Vertex SourceVertex { get; set; }
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; }
        public Vertex DestinationVertex { get; set; }
        [XmlAttribute(AttributeName = "Size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "Period")]
        public string Period { get; set; }
        [XmlAttribute(AttributeName = "Deadline")]
        public string Deadline { get; set; }
        public List<List<Vertex>> PossibleVerticesPath { get; set; }

        public List<List<Edge>> PossibleEdgePaths { get; set; } // Caculated after finding all possible paths using vertices.  
    }

    [XmlRoot(ElementName = "Vertex")]
    public class Vertex
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "MaxE2E")]
        public string MaxE2E { get; set; }
        public List<Vertex> vertexNeighbors { get; set; }
    }

    [XmlRoot(ElementName = "Edge")]
    public class Edge
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "BW")]
        public string BW { get; set; }
        [XmlAttribute(AttributeName = "PropDelay")]
        public string PropDelay { get; set; }
        [XmlAttribute(AttributeName = "Source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; }
        public Vertex SourceVertex { get; set; }
        public Vertex DestinationVertex { get; set; }
        public int BWConsumption { get; set; }
        public long BWConsumptionPerCycle { get; set; }
        public int BWCylceTransferCapacity { get; set; }
        public int BWCycleDelay { get; set; }
        public int Latency { get; set; }
    }

    [XmlRoot(ElementName = "Architecture")]
    public class Architecture
    {
        [XmlElement(ElementName = "Vertex")]
        public List<Vertex> Vertices
        {
            get; set;
        }
        [XmlElement(ElementName = "Edge")]
        public List<Edge> Edges { get; set; }
    }
    public class Cycle
    {

        public int Length { get; set; }

    }
}
