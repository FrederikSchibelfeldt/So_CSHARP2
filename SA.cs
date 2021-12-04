
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace So_CSHARP
{
    public class SimulatedAnnealing
    {

        public DateTime endTime;
        public DateTime startTime;
        private Cycle Cycle;
        private static Application app;
        private int iteration = 0;
        private int IterationBuffer;
        private static Architecture arch;
        private Inputs Input;

        public static Output.Report runSimulatedAnnealing(Architecture arch, Application app)
        {
            Stopwatch stopwatch = new Stopwatch();
            double T = 200000;
            double coolingRate = 0.003;
            List<Output.Report> solutionSpace = new();
            Output.Report initialRandomSolution = Inputs.GenerateRandomSolution(arch, app);
            //solutionSpace.Add(initialRandomSolution);
            ///var counter = 0;
            while (T > 1)
            {
                Output.Report RandomSolution = newRandomSolution(app, initialRandomSolution);

                // TODO: check if RandomSolution is already contained in solution space. (very unlikely) // can be done by comparing BW for now ref: previous sol line 199  
                long lambda = Inputs.costFunction(initialRandomSolution, arch) - Inputs.costFunction(RandomSolution, arch);
                long a = Inputs.costFunction(initialRandomSolution, arch);
                long b = Inputs.costFunction(RandomSolution, arch);
                Console.WriteLine("-------------------------------------------------lambda");
                Console.WriteLine(lambda);
                if (lambda > 0 || lambda > Math.Exp(-(1 / T) * lambda))
                {

                    solutionSpace.Add(RandomSolution);
                }
                else
                {
                    initialRandomSolution = RandomSolution; 
                }
                T *= (1 - coolingRate);
            }
            var l = solutionSpace.ToList();
            Console.WriteLine("********SolutionsSpace Output*******");
            Console.WriteLine(l.Count);
            Inputs.CreateAReport(solutionSpace.Last());
            return solutionSpace.Last();
        }

        public static Output.Report newRandomSolution(Application app, Output.Report currentRandomSolution)
        {

            Random rand = new Random();
            int maxE2E = 0;
            int cycleLength = 12;
            var links = new List<Output.Link>();
            Output.Report report = (Output.Report)currentRandomSolution.Clone();
   
            // ensure messages are sorted by name
            report.Messages.Sort((x, y) => x.Name.CompareTo(y.Name));

            // Choose a random message name from solution messages
            var randomMessageFromSolution = report.Messages[rand.Next(0, report.Messages.Count)];
            Console.WriteLine("Chosen random message");
            Console.WriteLine(randomMessageFromSolution.Name);

            // Use chosen randomMessageFromSolution name to find a message in APP 
            // For extraction of values such as edgepath. 
            var message = app.Messages.Find(message => message.Name == randomMessageFromSolution.Name);

            List<Edge> chosenPath = message.PossibleEdgePaths[rand.Next(0, message.PossibleEdgePaths.Count)];
            long BWForRandomMessage = Inputs.CalculateMeanBWforCurrentMessage(message, chosenPath);
               foreach (Edge edge in chosenPath)
                {
                    //  add cycleTurn for links
                    var link = new Output.Link();
                    link.Qnumber = rand.Next(1, 4);
                    maxE2E += link.Qnumber * cycleLength + Int32.Parse(edge.PropDelay);
                    link.Source = edge.Source;
                    link.Destination = edge.Destination;
                    links.Add(link);
                }

            randomMessageFromSolution.Links = links;
            randomMessageFromSolution.Name = message.Name;
            randomMessageFromSolution.BW = BWForRandomMessage;
            randomMessageFromSolution.MaxE2E = maxE2E.ToString();
        
            return CalculateNewReportSolution(report);
        }

        public static Output.Report CalculateNewReportSolution(Output.Report report)
        {
                long meanBW = 0;
                int meanE2E = 0;
            
            foreach (Output.Message message in report.Messages)
            {
                meanBW += message.BW;
                meanE2E += Int32.Parse(message.MaxE2E);
        
            }
                report.Solution.MeanE2E = meanE2E / report.Messages.Count;
                report.Solution.MeanBW = meanBW / report.Messages.Count;
                
            return report;
        }

        // Transform to a cycle system from a time-based system
        private void TransformationToCycle()
        {
            foreach (Edge edge in arch.Edges)
            {
                // the project description: S = s * |c|
                edge.BWCylceTransferCapacity = (int)(Int32.Parse(edge.BW) * Cycle.Length);

                // project description: D = d / |c|
                edge.BWCycleDelay = (int)(Int32.Parse(edge.PropDelay) / Cycle.Length);
            }
        }

    }
}
