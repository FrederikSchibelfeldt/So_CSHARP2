
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

        public static Output.Report RunSimulatedAnnealing(Architecture arch, Application app)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            double T = 20;
            Random rand = new Random();
            double coolingRate = 0.003;
            List<Output.Report> solutionSpace = new();
            Output.Report initialRandomSolution = Inputs.GenerateRandomSolution(arch, app);
            while (T > 1)
            {
                double r = rand.NextDouble();
                Output.Report RandomSolution = NewRandomSolution(app, initialRandomSolution);
                long lambda = Inputs.CostFunction(initialRandomSolution, arch) - Inputs.CostFunction(RandomSolution, arch);
                if (lambda > 0|| r < 0.15)
                {
                    solutionSpace = (List<Output.Report>) solutionSpace.Prepend(RandomSolution).ToList();
                }
                else
                {
                    initialRandomSolution = RandomSolution; 
                }

                T *= (1 - coolingRate);
            }
            solutionSpace.ForEach(p => Console.Write(p.Solution.Cost + ","));
            var sortedSolutionSpace = solutionSpace.OrderByDescending(report => report.Solution.Cost).ToList();

            Output.Report OptimaSolution = sortedSolutionSpace.Last(); 
            OptimaSolution.Solution.Runtime = stopwatch.Elapsed.TotalSeconds; // Add runTime to soltion
            Inputs.CreateAReport(OptimaSolution);
            return sortedSolutionSpace.First();
        }

        public static Output.Report NewRandomSolution(Application app, Output.Report currentRandomSolution)
        {
            Random rand = new();
            int maxE2E = 0;
            int cycleLength = 12;
            var links = new List<Output.Link>();
            Output.Report report = (Output.Report)currentRandomSolution.Clone();
   
            // ensure messages are sorted by name
            report.Messages.Sort((x, y) => x.Name.CompareTo(y.Name));
            // Choose a random message name from solution messages
            var randomMessageFromSolution = report.Messages[rand.Next(0, report.Messages.Count)];
            // Use chosen randomMessageFromSolution name to find a message in APP 
            // For extraction of values such as edgepath. 
            var message = app.Messages.Find(message => message.Name == randomMessageFromSolution.Name);

            List<Edge> chosenPath = message.PossibleEdgePaths[rand.Next(0, message.PossibleEdgePaths.Count)];
            long BWForRandomMessage = Inputs.ComputeMeanBWforMessage(message, chosenPath);
            int tempCycleTurn = 0;
               foreach (Edge edge in chosenPath)
                {
                // add cycleTurn for links
                var link = new Output.Link
                {
                    Qnumber = rand.Next(1, 4)
                };
                maxE2E += link.Qnumber * cycleLength + Int32.Parse(edge.PropDelay);
                    link.Source = edge.Source;
                    link.Destination = edge.Destination;
                    tempCycleTurn += link.Qnumber;
                    link.LinkCycleTurn = tempCycleTurn;
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
    }
}
