using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static So_CSHARP.Inputs;
using static So_CSHARP.Output;
using Google.OrTools;

namespace So_CSHARP
{
    //TODO: Find initial solutions, Find all paths that lead to correct endpoint
    class Program
    {
        static void Main(string[] args)
        {
            var application = readApps();
            var arch = readConfig();
            // populateFields(application,arch);
            // mapVertex(arch);
            // mapVertexNeighbors(arch)
            // findPath(application,arch);
            // FindMessageRoutes(application,arch);
            // simmulatedAnnealing(findPath(application,arch), application, arch)
            GeneticAlgorithms(arch, application, 11);
        }

    }
}

