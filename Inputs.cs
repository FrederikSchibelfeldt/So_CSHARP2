using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

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

        public static void populateFields(Application apps, Architecture arch){
            
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
                    link.Qnumber = random.Next(1,4).ToString();

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

            return solution;
        }

        

        public static void costFunction()
        {
            //should go through every elem in the solution, and
            //compute whether the solution is better than the original solution
            //by the use of the three constraint functions below.
            //if a particular constraint is not fulfilled -> miss penalty. 
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
            }
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
                Console.ForegroundColor =  count_test % 2 ==  0 ? ConsoleColor.Cyan :  ConsoleColor.Green; 
                

            List<Vertex> isVisited =new();
            List<Vertex> pathList = new();
    
            // add source to path
            pathList.Add(message.SourceVertex);
            isVisited.Add(message.SourceVertex);
    
            Console.WriteLine("--------------------findAPath--------------------");
            findAPath(message.SourceVertex, message.DestinationVertex, message, isVisited, pathList);

                count_test++;

                Console.WriteLine("------------------------------------------");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void findAPath(Vertex source, Vertex destination,Message message,List<Vertex> isVisited, List<Vertex> localPathList)
        {
            Console.WriteLine(string.Join(" start FIND A PATH", ""));
            if (source.Equals(destination)) {
                message.PossibleVerticesPath.Add(localPathList);

                return;
            }
    
            // Mark the current node
            isVisited.Add(source);
    
            // For all neighbor vertices 
            foreach(var vertex in source.vertexNeighbors)
            {
                                    Console.WriteLine(string.Join(" ", vertex));
                if (!isVisited.Contains(vertex)) {
                    // store current node
                    // in path[]
                    localPathList.Add(vertex);

                    findAPath(vertex, destination, message, isVisited,
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
                population.Add(findPath(apps, arch));
            }

            return population;
        }

        public static int meanBandWidth(Application apps, Architecture arch)
        {
            int counter = 0;
            int sum = 0;
            foreach (var arc in arch.Edges)
            {
                sum += Int32.Parse(arc.BW);
                counter++;
            }
            return sum / counter;
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

            for(int i = 0;i <= selectedPopulation.Count;i++){
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
        }

        [XmlRoot(ElementName = "Architecture")]
        public class Architecture
        {
            [XmlElement(ElementName = "Vertex")]
            public List<Vertex> Vertices { get; set; 
            }
            [XmlElement(ElementName = "Edge")]
            public List<Edge> Edges { get; set; }
        }
    }
}
