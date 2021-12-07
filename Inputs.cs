using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                new FileStream("..\\So_CSHARP2\\files\\Example\\Input\\Apps.xml", FileMode.Open))
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
                new FileStream("..\\So_CSHARP2\\files\\Example\\Input\\Config.xml", FileMode.Open))
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
         
        }


        public static Output.Report GenerateRandomSolution(Architecture arch, Application app)
        {
            var random = new Random();
            var report = new Output.Report();
            report.Messages = new List<Output.Message>();
            int i = 0;

            long sumBWForSolution = 0;
            foreach (Message message in app.Messages)
            {
                int maxE2E = 0;
                int cycleLength = 12;
                var links = new List<Output.Link>();

                List<Edge> chosenPath = message.PossibleEdgePaths[random.Next(0,message.PossibleEdgePaths.Count)]; // just take first path for now 
                long mBW = CalculateMeanBWforCurrentMessage(message, chosenPath);
                sumBWForSolution = sumBWForSolution + mBW;
                var tempCycleTurn = 0;
 // ineffienct looping 

                foreach (Edge edge in chosenPath)
                {
                    var link = new Output.Link();
                    link.Qnumber = random.Next(1, 4);
                    link.Source = edge.Source;
                    maxE2E += link.Qnumber * cycleLength + Int32.Parse(edge.PropDelay);
                    link.Destination = edge.Destination;
                    tempCycleTurn += link.Qnumber;  
                    link.LinkCycleTurn = tempCycleTurn;
                    links.Add(link);
                }
                var m = new Output.Message();
                m.Links = links;
                m.Name = message.Name;
                m.MaxE2E = maxE2E.ToString();
                m.Deadline = message.Deadline;
                m.BW = mBW;
                m.Size = Int32.Parse(message.Size);
                report.Messages.Add(m);
                i++;
                // Color console "skrift" for testing purposes
               
            }
            var solution = new Output.Solution();
            solution.MeanBW = sumBWForSolution/report.Messages.Count;
            solution.Runtime = 0;
            solution.MeanE2E = 0;
          
            report.Solution = solution;
            return report;
        }
        
        public static Output.Report newRandomSolution(Application app, Output.Report currentRandomSolution)
        {
            // remember to calculate new sumBWForSolution and to subtract older results (for changed messages solution )
            // Or just calculate the whole thing/sumBWForSolution again lol. 
            Random rand = new Random();
            int maxE2E = 0;
            int cycleLength = 12;
            var links = new List<Output.Link>();
            Output.Report report = (Output.Report) currentRandomSolution.Clone();
            
            // Choose a random message name from solution messages
            var randomMessageFromSolution = report.Messages[rand.Next(0,report.Messages.Count)]; 
       

            // Use chosen randomMessageFromSolution name to find a message in APP 
            // For extraction of values such as edgepath. 
            var message = app.Messages.Find( message => message.Name == randomMessageFromSolution.Name);
            
            List<Edge> chosenPath = message.PossibleEdgePaths[rand.Next(0,message.PossibleEdgePaths.Count)];
            var tempCycleTurn = 0; // just take first path for now 
            foreach (Edge edge in chosenPath)
                {
                    var link = new Output.Link();
                    link.Qnumber = rand.Next(1, 4);
                    link.Source = edge.Source;
                    maxE2E += link.Qnumber * cycleLength + Int32.Parse(edge.PropDelay);
                    link.Destination = edge.Destination;
                    tempCycleTurn += link.Qnumber;  
                    link.LinkCycleTurn = tempCycleTurn;
                    links.Add(link);
                }
            randomMessageFromSolution.Links = links;
            randomMessageFromSolution.Name = message.Name;
            randomMessageFromSolution.Deadline = message.Deadline;
            randomMessageFromSolution.MaxE2E = maxE2E.ToString();
            randomMessageFromSolution.BW = CalculateMeanBWforCurrentMessage(message, chosenPath);
            randomMessageFromSolution.Size = Int32.Parse(message.Size);
            return report;
        }

        public static void CreateAReport(Output.Report report,int i){
            var output = new Output();
            output.GiveOutput(report,i);
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
           

            foreach (Edge edge in edges)
            {
                edge.BWConsumption = Int32.Parse(message.Size); // BW_Consumption is just size of message sent through edge
                 sumBW = sumBW + edge.BWConsumption;          // SUM UP for each edge message is going through
            }

            return sumBW;
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
                

                // Call recursive utility
                List<Vertex> isVisited = new();
                List<Vertex> pathList = new();

                // add source to path
                pathList.Add(message.SourceVertex);
                isVisited.Add(message.SourceVertex);

                // Call to find possible 
                findAllPossiblePaths(message.SourceVertex, message.DestinationVertex, message, isVisited, pathList);

                // DEFINE  possible edge paths from possible vertex paths 
                // then add to message 
                message.PossibleEdgePaths = MessageVertexPathsToEdgePaths(message, arch);


                //message.PrintPossiblePaths();

                count_test++;

            }

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
               
                population.Add(GenerateRandomSolution(arch, apps));
            }

            return population;
        }
        public static int meanE2E(Output.Report report)
        {
            int counter = 0;
            int sum = 0;
            foreach (Output.Message message in report.Messages)
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
        
        public static List<long> EvaluationList(List<Output.Report> population, Architecture arch, Application apps)
        {
            List<long> evaluations = new();
            
            foreach (Output.Report report in population)
            {
                long meanBW = new();
                int meanE2E = new();
                foreach (Output.Message message in report.Messages)
                {
                    meanE2E += Int32.Parse(message.MaxE2E);
                    meanBW += message.BW;
                }
                if(!deadlineContraint(report, apps)){
                    meanBW += 500;
                }
                if(!linkCapacityContraint(report,arch)){
                    meanBW += 500;
                }

                report.Solution.MeanE2E = meanE2E / report.Messages.Count;
                report.Solution.MeanBW = meanBW / report.Messages.Count;
                evaluations.Add(meanBW / report.Messages.Count);
            }
            return evaluations;
        }
        
        public static List<Output.Report> SelectedPopulation(int selectedSize,List<Output.Report> population, List<long> evaluations)
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
            int m = selectedPopulation[0].Messages.Count/2;
            int j = (3*selectedPopulation[0].Messages.Count)/4;
            Output.Report tempReport = selectedPopulation[0];

            for(int i = 0;i < selectedPopulation.Count;i++){
                Output.Report tmp = new Output.Report();
                while(m<= j){
                    if(i == selectedPopulation.Count-1){
                        selectedPopulation[i].Messages[m] = tempReport.Messages[m];
                    }
                    else{
                        selectedPopulation[i].Messages[m] = selectedPopulation[i+1].Messages[m];
                    }
                    m++;
                }
                tmp = selectedPopulation[i];
                recombinedPopulation.Add(tmp);
            }
            return recombinedPopulation;
        }

        public static List<Output.Report> MutatePopulation(int populationSize, List<Output.Report> recombinedPopulation, Application apps)
        {
            List<Output.Report> mutatedPopulation = new List<Output.Report>(recombinedPopulation);
            var random = new Random();
            for (int i = 0; i < populationSize - recombinedPopulation.Count; i++)
            {
                int j = i % recombinedPopulation.Count;
                Output.Report currentSolution = (Output.Report) recombinedPopulation[j].Clone();
                mutatedPopulation.Add(newRandomSolution(apps, currentSolution));
            }
            return mutatedPopulation;
        }

        
        public static void GeneticAlgorithms(Architecture arch, Application apps, int populationSize, int selectedSize)
        {
            // Initialize population
            string filename = $"..\\So_CSHARP2\\files\\Example\\Output\\APPSGA{populationSize}-{selectedSize}.csv";
            List<Output.Report> population = InitPopulation(populationSize, arch, apps);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<long> evaluations = new List<long>();
            List<int> meanEval = new List<int>();
            while (sw.Elapsed.TotalMinutes < 2)
            {
                Console.WriteLine(sw.Elapsed.TotalSeconds);
                // Evaluate population
                evaluations = EvaluationList(population,arch, apps);
                meanEval.Add((int) evaluations.Average());
                // Selection using elitism (fitness proportionate and tournament selection)
                List<Output.Report> selectedPopulation = SelectedPopulation(selectedSize, population, evaluations);
                // Recombine 
                List<Output.Report> recombinedPopulation = RecombinedPopulation(selectedPopulation);
                // Mutation to create new generation
                population = MutatePopulation(populationSize, recombinedPopulation,apps);


                if(meanEval.Count > 5){

                if(meanEval.TakeLast(5).ToList().Sum()/5 - meanEval.TakeLast(1).Sum() < 1){
                    break;
                }}
            }
            // Write best solution to XML
            meanEval.ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Generation:" + meanEval.Count);
            string csv = String.Join(",", meanEval.Select(x => x.ToString()).ToArray());
            File.WriteAllText(filename, csv);
            CreateAReport(population[evaluations.IndexOf(evaluations.Min())],selectedSize);
        }

         public static bool deadlineContraint(Output.Report report, Application apps)
        {
            foreach (Output.Message message in report.Messages)
            {
                if(message.Deadline == null){findDeadline(message,apps);}
                if(message.Deadline == null){Console.WriteLine($"Null deadline for message {message.Name}");}
                if (!(Int32.Parse(message.MaxE2E) < Int32.Parse(message.Deadline)))
                {
                    return false;
                }
            }
            return true;
        }

        public static void findDeadline(Output.Message message, Application apps){
        var s =  apps.Messages.Find(x => x.Name == message.Name);
        message.Deadline = s.Deadline;
        }

        public static bool linkCapacityContraint(Output.Report report, Architecture arch)
        {

         //   Console.WriteLine("--------------------linkCapacityContraint--------------------");

            for (int i = 1; i < (arch.Edges.Count() - 1) * 3; i++)
            {
                List<(Output.Link, Output.Message)> links = new();
                foreach (Output.Message message in report.Messages)
                {
                    foreach (Output.Link link in message.Links)
                    {

                        if (link.LinkCycleTurn == i)
                        {
                            links.Add((link, message));

                        }
                    }

                }
            //    Console.WriteLine("--------------------message--------------------");
             //   links.ForEach(p => Console.Write("Source: " + p.Item1.Source + " Destination: " + p.Item1.Destination + " LinkCycleTurn: " + p.Item1.LinkCycleTurn + " FOR MESSAGE: " + p.Item2.Name + "\n"));
              //  Console.WriteLine("");

                // loops over (links,message) list
                foreach ((Output.Link, Output.Message) linkandMessage in links)
                {  

                    var linkandMessages = links.FindAll(link2 => linkandMessage.Item1.Source == link2.Item1.Source && linkandMessage.Item1.Destination == link2.Item1.Destination);

                //    Console.WriteLine("--------------------linkandMessage--------------------");
                 //   linkandMessages.ForEach(p => Console.Write("Source: " + p.Item1.Source + " Destination: " + p.Item1.Destination + " LinkCycleTurn: " + p.Item1.LinkCycleTurn + " FOR MESSAGE: " + p.Item2.Name + "\n"));
                    var currentSource = linkandMessages[0].Item1.Source;
                    var currentDestination = linkandMessages[0].Item1.Destination;
                    var bw = arch.Edges.FindLast(edge => edge.Source == currentSource && edge.Destination == currentDestination).BW;

                    var currentSize = linkandMessages.Sum(p => p.Item2.Size);
                  //  Console.WriteLine("--------------------BW--------------------");
                   // Console.WriteLine(bw);
                   // Console.WriteLine(currentSize);
                    if (currentSize > Int32.Parse(bw)) { return false; }
                }

            }

            return true;
        }
    
        


        // possibly return List<List<Edge>>
        public static List<List<Edge>> MessageVertexPathsToEdgePaths(Message message, Architecture arch)
        {

            List<List<Edge>> possiblePaths = new();
            List<Edge> path;
            foreach (List<Vertex> route in message.PossibleVerticesPath)
            {
                path =  new();
                for (int i = 0; i < route.Count; i++)
                {

                    if (i < route.Count - 1)
                    {
                        
                        path.Add(EdgeFromVerticies(route.ElementAt(i), route.ElementAt(i + 1), arch));
                    }
                }
                //path.ForEach(p => Console.Write("source: " + p.Source + " destination: " + p.Destination + "\n"    ));
                
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
