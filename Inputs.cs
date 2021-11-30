﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Inputs
    {

        static Dictionary<string, List<string>> dict = new();
        public static Application readApps()
        {
            Application res = new Application();

            var xs = new XmlSerializer(typeof(Application));
            {
                using (FileStream fileStream =
                new FileStream("/Users/mo/02229/So_CSHARP2/files/Example/Input/Apps.xml", FileMode.Open))
                    res = (Application)xs.Deserialize(fileStream);
            }

            return res;
        }

        public static Architecture readConfig()
        {
            Architecture res = new Architecture();

            var xs = new XmlSerializer(typeof(Architecture));
            {
                using (FileStream fileStream =
                new FileStream("/Users/mo/02229/So_CSHARP2/files/Example/Input/Config.xml", FileMode.Open))
                    res = (Architecture)xs.Deserialize(fileStream);
            }
            return res;
        }

        /// <summary>
        /// Creates properties SourceVertex and DestinationVertex for message and edge object.
        /// This is used in generating the initial solution.
        /// </summary>
        /// <param name="apps"></param>
        /// <param name="arch"></param>
        public static void PopulateFields(Application apps, Architecture arch)
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
                    //Invalid input
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
                    //invalid input
                    Environment.Exit(0);
                }

            }
        }

        /// <summary>
        /// Adds property vertexneighbors to all vertex objects. 
        /// </summary>
        /// <param name="arch"></param>
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


        public static Output.Report GenerateRandomSolution(Architecture arch, Application app)
        {
            var random = new Random();
            var report = new Output.Report();
            report.Message = new List<Output.Message>();
            int i = 0;

            long sumBWForSolution = 0;
            foreach (Message message in app.Messages)
            {
                int maxE2E = 0;
                int cycleLength = 12;
                var links = new List<Output.Link>();

                List<Edge> chosenPath = message.PossibleEdgePaths[random.Next(0,message.PossibleEdgePaths.Count)]; // just take first path for now 

                sumBWForSolution = sumBWForSolution + CalculateMeanBWforCurrentMessage(message, chosenPath); // ineffienct looping 

                foreach (Edge edge in chosenPath)
                {
                    var link = new Output.Link();
                    link.Qnumber = random.Next(1, 4).ToString();
                    link.Source = edge.Source;
                    link.Destination = edge.Destination;
                    links.Add(link);
                }
                var m = new Output.Message();
                m.Link = links;
                m.Name = message.Name;
                m.MaxE2E = maxE2E.ToString();
                report.Message.Add(m);
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
            solution.MeanBW = sumBWForSolution;
            solution.Runtime = 0;
            solution.MeanE2E = 0;
            Console.WriteLine("---Solution---");
            Console.WriteLine(solution);
            report.Solution = solution;
            return report;
        }

        public static void CreateAReport(Output.Report report){
            var output = new Output();
            output.GiveOutput(report);
        }




    //Add functionsimilar to NweRandomSOlution in the master branch. 

    // MeanBW is the average of 

    public static void costFunction()
        {
            //should go through every elem in the solution, and
            //compute whether the solution is better than the original solution
            //by the use of the three constraint functions below.
            //if a particular constraint is not fulfilled -> miss penalty.

            //Hvis alle edges har Quenumber 1 bliver der ofte sendt for meget over et link medium
            //på en gang (bandwidth constraint).
            //Derfor skal vi sørge for at minimere meanE2E, mens vi samtidig sikre os at der ikke bliver overført for meget
            //på et link medium i løbetr af en cycle.

        }




        // INFO: THIS FUNCTION SHOULD BE INCLUDED AS PART OF message loop (the intention is for an outer variable to keep track) 
        // PARAM: "message": the message being sent 
        // PARAM: "edges": the edges the message is going through 
        private static long CalculateMeanBWforCurrentMessage(Message message, List<Edge> edges)
        {
            int sumBW = 0;
            Console.WriteLine("CalculateMeanBWforCurrentMessage");
            Console.WriteLine(message.Name);

            foreach (Edge edge in edges)
            {
                Console.WriteLine("CalculateMeanBWforCurrentMessage LOOP");
                edge.BWConsumption = Int32.Parse(message.Size); // BW_Consumption is just size of message sent through edge
                Console.WriteLine(message.Size);
                 sumBW = sumBW + edge.BWConsumption;          // SUM UP for each edge message is going through
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


        /// <summary>
        /// Finds all possible paths a message can take. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        /// <param name="isVisited"></param>
        /// <param name="localPathList"></param>
        private static void findAllPossiblePaths(Vertex source, Vertex destination, Message message, List<Vertex> isVisited, List<Vertex> localPathList)
        {
            if (source.Equals(destination))
            {
                Console.WriteLine(string.Join("\n PATH:", "\n"));
                localPathList.ForEach(p => Console.Write(p.Name));
                message.PossibleVerticesPath.Add(localPathList.ToList());
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
        
        public static List<Output.Report> InitPopulation(int size, Architecture arch, Application apps)
        {
            List<Output.Report> population = new();
            for (int i = 0; i < size; i++) 
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(i);
                population.Add(GenerateRandomSolution(arch, apps));
            }

            return population;
        }
        public static int meanE2E(Output.Report report)
        {
            int counter = 0;
            int sum = 0;
            foreach (Output.Message message in report.Message)
            {
                sum += Int32.Parse(message.MaxE2E);
                counter++;
            }
            return sum / counter;
        }
        public static double ObjectiveFunction(Output.Report report)
        {
            int meane2e = meanE2E(report);
            return meane2e;
        }
        
        public static List<double> EvaluationList(List<Output.Report> population)
        {
            List<double> evaluations = new();
            foreach (Output.Report report in population)
            {
                evaluations.Add(ObjectiveFunction(report));
            }
            return evaluations;
        }
        
        public static List<Output.Report> SelectedPopulation(int selectedSize,List<Output.Report> population, List<double> evaluations)
        {
            List<Output.Report> selectedPopulation = new List<Output.Report>();
            var nums = evaluations.OrderBy(x => x).Take(selectedSize).ToList();
            for(int i = 0; i<nums.Count();i++){
                selectedPopulation.Add(population[evaluations.FindIndex(x => x == nums[i])]);
            }
            return selectedPopulation;
        }

        public static List<Output.Report> RecombinedPopulation(List<Output.Report> selectedPopulation)
        {
            List<Output.Report> recombinedPopulation = new List<Output.Report>();
            int m = selectedPopulation[0].Message.Count/2;
            int j = (3*selectedPopulation[0].Message.Count)/4;
            Output.Report tempReport = selectedPopulation[0];

            for(int i = 0;i < selectedPopulation.Count;i++){
                Output.Report tmp = new Output.Report();
                while(m<= j){
                if(i == selectedPopulation.Count-1){
                selectedPopulation[i].Message[m] = tempReport.Message[m];
                }
                else{
                selectedPopulation[i].Message[m] = selectedPopulation[i+1].Message[m];
                tmp = selectedPopulation[i];
                }
                m++;
                }
                recombinedPopulation.Add(tmp);
            }
            Console.WriteLine(selectedPopulation.SequenceEqual(recombinedPopulation));
            return recombinedPopulation;
        }

        public static List<Output.Report> MutatePopulation(int populationSize, List<Output.Report> recombinedPopulation)
        {
            List<Output.Report> mutatedPopulation = recombinedPopulation;
            var random = new Random();
            for (int i = 0; i < populationSize - recombinedPopulation.Count; i++)
            {
                int j = i % recombinedPopulation.Count;
                Output.Report currentSolution = recombinedPopulation[j];
                // currentSolution.Message[random.Next(0, currentSolution.Message.Count)] == Abdi skadet funktion;
                mutatedPopulation.Add(currentSolution);
            }
            return mutatedPopulation;
        }
        
        public static void GeneticAlgorithms(Architecture arch, Application apps, int populationSize, int selectedSize)
        {
            // Initialize population
            List<Output.Report> population = InitPopulation(populationSize, arch, apps);
            // Evaluate population
            List<double> evaluations = EvaluationList(population);
            // Return best solution if stopping criteria met
            
            // Selection using elitism (fitness proportionate and tournament selection)
            List<Output.Report> selectedPopulation = SelectedPopulation(selectedSize, population, evaluations);
            // Recombine 
            List<Output.Report> recombinedPopulation = RecombinedPopulation(selectedPopulation);
            // Mutation to create new generation
            List<Output.Report> mutatedPopulation = MutatePopulation(populationSize, recombinedPopulation);

        }
    
        


        // possibly return List<List<Edge>>
        public static List<List<Edge>> MessageVertexPathsToEdgePaths(Message message, Architecture arch)
        {
            Console.WriteLine(" \n --------------------MessageVertexPathsToEdgePaths--------------------");

            List<List<Edge>> possiblePaths = new();
            List<Edge> path;
            foreach (List<Vertex> route in message.PossibleVerticesPath)
            {
                path =  new();
                for (int i = 0; i < route.Count; i++)
                {

                    if (i < route.Count - 1)
                    {
                        Console.WriteLine(route.ElementAt(i).Name);
                        Console.WriteLine(route.ElementAt(i + 1).Name);
                        path.Add(EdgeFromVerticies(route.ElementAt(i), route.ElementAt(i + 1), arch));
                    }
                }
                Console.WriteLine(string.Join("PATH TO EDGES edges:", "\n"));
                //path.ForEach(p => Console.Write("source: " + p.Source + " destination: " + p.Destination + "\n"    ));
                Console.WriteLine("\n");
                Console.WriteLine(string.Join("PATH TO EDGES edges:", "\n"));
                path.ForEach(p => Console.Write("source: " + p.Source + " destination: " + p.Destination + "\n"));
                possiblePaths.Add(path);
                Console.WriteLine("\n");
            }

            //Return and add to possible edge paths for a message. 
            // message.PossibleEdgePaths.Add();                 // ADD TO MESSAGES AS WELL
            return possiblePaths.ToList();
        }
        public static Edge EdgeFromVerticies(Vertex source, Vertex destination, Architecture arch)
        {
            foreach (Edge edge in arch.Edges)
            {
                // maybe???
                // We should check direction before invoking this function thus, the deleted part of the if statement. 
                // since edges are biderctional we can possibly recieve a wrong direction 
                if ( (String.Equals(edge.Source, source.Name) && String.Equals(edge.Destination, destination.Name)) || (String.Equals(edge.Source, destination.Name) && String.Equals(edge.Destination, source.Name)))
                {
                    return edge;
                }
            }
            return null;
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
