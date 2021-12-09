
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

            double T = 20000;
            Random rand = new();
            double coolingRate = 0.003;
            List<Output.Report> solutionSpace = new();
            Output.Report initialRandomSolution = Inputs.GenerateRandomSolution(arch, app);
            int i = 0;
            int ii=0;
            while (T > 1)
            {
                i++;
                double r = rand.NextDouble();
                Output.Report RandomSolution = NewRandomSolution(app, initialRandomSolution);
                long lambda = Inputs.CostFunction(initialRandomSolution, arch) - Inputs.CostFunction(RandomSolution, arch);
                if (lambda > 0 || lambda > Math.Exp(-(1 / T) * lambda))
                {
                    solutionSpace = (List<Output.Report>) solutionSpace.Prepend(RandomSolution).ToList();
                }
                else
                {
                    initialRandomSolution = RandomSolution; 
                    }

                T *= (1 - coolingRate);
            }
            var sortedSolutionSpace = solutionSpace.OrderByDescending(report => report.Solution.Cost).ToList();
            solutionSpace.ForEach(p => Console.Write(p.Solution.Cost + ","));
    
            Console.WriteLine(" \n ********SolutionsSpace Output*******");
            Console.WriteLine(sortedSolutionSpace.Count);
            Console.WriteLine(" \n ********i Output*******");
            Console.WriteLine(i);
            Console.WriteLine(ii);
            Output.Report GlobalMaximaEstimate = sortedSolutionSpace.Last(); 
            GlobalMaximaEstimate.Solution.Runtime = stopwatch.Elapsed.TotalSeconds; // Add runTime to soltion
            Inputs.CreateAReport(GlobalMaximaEstimate);
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
