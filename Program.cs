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

            if (args.Length == 0)
        {
       
            var application = ReadApps();
            var arch = ReadConfig();
            PopulateFields(application,arch);
            MapVertexNeighbors(arch);
            FindMessageRoutes(application,arch);
           //GenerateRandomSolution(arch, application);
            //GeneticAlgorithms(arch, application, 2000, 200);

            RunSimulatedAnnealing(arch,application);
                
        }
        if(args.Length == 2){
        string app_path = args[0];
       string arch_path = args[1];
       
            var application = ReadApps(app_path);
            var arch = ReadConfig(arch_path);
            PopulateFields(application,arch);
            MapVertexNeighbors(arch);
            FindMessageRoutes(application,arch);
                  //GenerateRandomSolution(arch, application);
            //GeneticAlgorithms(arch, application, 2000, 200);
            RunSimulatedAnnealing(arch,application);
        }

        if(args.Length == 3){
        string app_path = args[0];
       string arch_path = args[1];
       string outputfile = args[2];


            SetOutput(outputfile);

            var application = ReadApps(app_path);
            var arch = ReadConfig(arch_path);
            PopulateFields(application,arch);
            MapVertexNeighbors(arch);
            FindMessageRoutes(application,arch);
                  //GenerateRandomSolution(arch, application);
            //GeneticAlgorithms(arch, application, 2000, 200);
            RunSimulatedAnnealing(arch,application);
        }

        System.Console.WriteLine("Please include both input files or NONE!");
        System.Console.WriteLine("Application file followed by arch file followed by output [OPTIONAL] ");
                System.Console.WriteLine("Example: > dotnet run Apps.xml Config.xml ouput.xml");
        
        }
        

    }
}

