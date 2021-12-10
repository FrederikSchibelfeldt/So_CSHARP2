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
                Environment.Exit(0);
                Console.WriteLine("Please choose an option: SA or GA");
                Console.WriteLine("Example: dotnet run SA");

            }
            else if (args.Length == 1)
            {
                var application = ReadApps();
                var arch = ReadConfig();
                PopulateFields(application, arch);
                MapVertexNeighbors(arch);
                FindMessageRoutes(application, arch);
                if (args[0] == "SA")
                {
                    RunSimulatedAnnealing(arch, application);
                }

                else if (args[0] == "GA")
                {

                    GeneticAlgorithms(arch, application, 100, 20);
                   Output.Report GAsolution =  GeneticAlgorithms(arch, application, 1000, 200);
                   RunSimulatedWithSolutionFromGA(GAsolution,application,arch);
                }
                else{
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Please include both input files or NONE!");
                System.Console.WriteLine("Algorithm then Application file followed by arch file followed by output [OPTIONAL] ");
                System.Console.WriteLine("Example for SA with 2 input files and output [optional]: > dotnet run SA Apps.xml Config.xml ouput.xml");
                Console.ResetColor();
                return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n Algorithm run succesfully please refer to ReportTest.xml");
                Console.ResetColor();
                Console.WriteLine("");
            }
            else if (args.Length == 3)
            {
                string app_path = args[1];
                string arch_path = args[2];
                var application2 = ReadApps(app_path);
                var arch2 = ReadConfig(arch_path);
                PopulateFields(application2, arch2);
                MapVertexNeighbors(arch2);
                FindMessageRoutes(application2, arch2);
                if (args[0] == "SA")
                {
                    RunSimulatedAnnealing(arch2, application2);
                }

               else if (args[0] == "GA")
                {
              
                    GeneticAlgorithms(arch2, application2, 100, 20);
                   Output.Report GAsolution =  GeneticAlgorithms(arch2, application2, 1000, 200);
                   RunSimulatedWithSolutionFromGA(GAsolution,application2,arch2);
                }
                else{
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Please include both input files or NONE!");
                System.Console.WriteLine("Algorithm then Application file followed by arch file followed by output [OPTIONAL] ");
                System.Console.WriteLine("Example for SA with 2 input files and output [optional]: > dotnet run SA Apps.xml Config.xml ouput.xml");
                Console.ResetColor();
                return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n Algorithm run succesfully please refer to ReportTest.xml");
                Console.ResetColor();
                Console.WriteLine("");


            }

            else if (args.Length == 4)
            {
                string app_path = args[1];
                string arch_path = args[2];
                string outputfile = args[3];

                SetOutput(outputfile);
                var application2 = ReadApps(app_path);
                var arch2 = ReadConfig(arch_path);
                PopulateFields(application2, arch2);
                MapVertexNeighbors(arch2);
                FindMessageRoutes(application2, arch2);

                if (args[0] == "SA")
                {
                    RunSimulatedAnnealing(arch2, application2);
                }

              else if (args[0] == "GA")
                {

                    GeneticAlgorithms(arch2, application2, 100, 20);
                   Output.Report GAsolution =  GeneticAlgorithms(arch2, application2, 1000, 200);
                   RunSimulatedWithSolutionFromGA(GAsolution,application2,arch2);
                }
                else{
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Please include both input files or NONE!");
                System.Console.WriteLine("Algorithm then Application file followed by arch file followed by output [OPTIONAL] ");
                System.Console.WriteLine("Example for SA with 2 input files and output [optional]: > dotnet run SA Apps.xml Config.xml ouput.xml");
                Console.ResetColor();
                return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n Algorithm run succesfully please refer to {outputfile}");
                Console.ResetColor();
                Console.WriteLine("");

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Please include both input files or NONE!");
                System.Console.WriteLine("Algorithm then Application file followed by arch file followed by output [OPTIONAL] ");
                System.Console.WriteLine("Example for SA with 2 input files and output [optional]: > dotnet run SA Apps.xml Config.xml ouput.xml");
                Console.ResetColor();


            }

        }


    }
}

