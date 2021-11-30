using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static So_CSHARP.Inputs;
using static So_CSHARP.Output;
using static So_CSHARP.SimulatedAnnealing;
using Google.OrTools;

namespace So_CSHARP
{
    //TODO: Find initial solutions, Find all paths that lead to correct endpoint
    class Program
    {
        static void Main(string[] args)
        {
            // Initialization line 19-24
            var application = readApps();
            var arch = readConfig();
            PopulateFields(application,arch);
            mapVertexNeighbors(arch);
            FindMessageRoutes(application,arch);
            //It might be a problem that the outputtet source and destination of the
            //particular links within a message looks wierd. But it should work.

            //Whenever we want we can do a if condition somewhere that flips source
            //and destination for a particular link if the destination of the
            //previous link is not equal to the source of the current link.
       //     GenerateRandomSolution(arch, application);
      //      GeneticAlgorithms(arch, application, 2000, 200);

            runSimulatedAnnealing(arch,application);
        }

    }
}

