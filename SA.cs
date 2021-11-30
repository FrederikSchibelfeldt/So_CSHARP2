
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
        private bool debug = true;

        private List<Edge> edges;

        private int iteration = 0;
        private int IterationBuffer;
        private List<Message> messages;
        private int T = 1000;
        public SimulatedAnnealing(List<Message> messages, List<Edge> edges, int iterationBuffer, Cycle cycle)
        {


            var random = new Random();
            var solution = new Output.Report();
            var finalsolution = new Output.Report();
            var output = new Output();
            var SolutionSpace = new List<Output.Report>();


            while (T > 1)
                this.messages = messages;
            this.edges = edges;
            this.IterationBuffer = iterationBuffer;
            // this.Cycle = cycle;

            this.startTime = DateTime.Now;

        }
        // Transform to a cycle system from a time-based system
        private void TransformationToCycle()
        {
            foreach (Edge edge in edges)
            {
                // the project description: S = s * |c|
                edge.BWCylceTransferCapacity = (int)(Int32.Parse(edge.BW) * Cycle.Length);

                // project description: D = d / |c|
                edge.BWCycleDelay = (int)(Int32.Parse(edge.PropDelay) / Cycle.Length);
            }
        }
    }
}
