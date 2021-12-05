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

namespace So_CSHARP
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = ReadApps();
            var arch = ReadConfig();
            PopulateFields(application,arch);
            MapVertexNeighbors(arch);
            FindMessageRoutes(application,arch);
            //Whenever we want we can do a if condition somewhere that flips source
            //and destination for a particular link if the destination of the
            //previous link is not equal to the source of the current link.
            //GenerateRandomSolution(arch, application);
            //GeneticAlgorithms(arch, application, 2000, 200);

            RunSimulatedAnnealing(arch,application);
        }

    }
}

