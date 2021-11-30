
using System.Collections.Generic;
using System;
using System.IO;

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
 

        /*
        public SimulatedAnnealing(Architecture arch, Application app, int iterationBuffer, Cycle cycle)
        {
            var random = new Random();
            var solution = new Output.Report();
            var finalsolution = new Output.Report();
            var output = new Output();
            var SolutionSpace = new List<Output.Report>();

            this.IterationBuffer = iterationBuffer;
            this.Cycle = cycle;
            this.startTime = DateTime.Now;

        }
*/
        // lets run SA baby!
        public static void runSimulatedAnnealing(Architecture arch,Application app){
        //    Stopwatch stopwatch = new Stopwatch();
            double T = 1000;
            double coolingRate = 0.003;
            List<Output.Report> solutionsSpace = new();
            Output.Report initialRandomSolution = Inputs.GenerateRandomSolution(arch, app);
            while(T>1){
                Output.Report RandomSolution = newRandomSolution(arch,app,initialRandomSolution);

                // TODO: check if RandomSolution is already contained in solution space. (very unlikely) // can be done by comparing BW for now ref: previous sol line 199

                return;
                int lambda = costFunction(RandomSolution) - costFunction(initialRandomSolution); // TODO: which rækkefølge?
                if (lambda > 0 || lambda > Math.Exp(-(1 / T) * lambda))
                {
                  //  solutionSpace.Add(c);
              //      c.Laxity = lax(c);
               //     c.Scheduleable = doesTasksMeetsDeadline(c);
                }


                T = T *( 1 - coolingRate);
            }
                ;
        } 

        public static int costFunction(Output.Report report){ // param: 
                // Return/calcukat cost function for report/solution --- Look for it in inputs.cs (made already)

                return 1; 

        }
        public static Output.Report newRandomSolution(Architecture arch, Application app, Output.Report currentRandomSolution){
            
// remember to calculate new sumBWForSolution and to subtract older results (for changed messages solution )
// Or just calculate the whole thing/sumBWForSolution again lol. 
            Random rand = new Random();
            int maxE2E = 0;
            int cycleLength = 12;
            var links = new List<Output.Link>();
            Output.Report report = (Output.Report) currentRandomSolution.Clone();
            int sumBWForSolution = 0;  
            
            // ensure messages are sorted by name
            report.Messages.Sort((x, y) => x.Name.CompareTo(y.Name));

            // Choose a random message name from solution messages
            var randomMessageFromSolution = report.Messages[rand.Next(0,report.Messages.Count)]; 
            Console.WriteLine("Chosen random message");
            Console.WriteLine(randomMessageFromSolution.Name);

            // Use chosen randomMessageFromSolution name to find a message in APP 
            // For extraction of values such as edgepath. 
            var message = app.Messages.Find( message => message.Name == randomMessageFromSolution.Name);
            
            List<Edge> chosenPath = message.PossibleEdgePaths[rand.Next(0,message.PossibleEdgePaths.Count)]; // just take first path for now 

       //   sumBWForSolution = sumBWForSolution + CalculateMeanBWforCurrentMessage(message, chosenPath); // ignore for now 

                foreach (Edge edge in chosenPath)
                {
                    var link = new Output.Link();
                    link.Qnumber = rand.Next(1, 4).ToString();
                    link.Source = edge.Source;
                    link.Destination = edge.Destination;
                    links.Add(link);
                }
            var m = new Output.Message();
            m.Link = links;
            m.Name = message.Name;
            m.MaxE2E = maxE2E.ToString();
            report.Messages.Add(m);
           Inputs.CreateAReport(report);   // uncomment if you want to see current solution as XML format
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
