using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace So_CSHARP
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.WriteLine(rnd.Next(0, 2) == 1 ? "Mo er adem" : "Abdi er adem");

            Model res = new Model();

            var xs = new XmlSerializer(typeof(Model));
            using (FileStream fileStream =
                new FileStream("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/small.xml", FileMode.Open))
            {
                res = (Model) xs.Deserialize(fileStream);
            }

            Solution res2 = SimulatedAnnealing(res);
            var xs2 = new XmlSerializer(typeof(Solution));

            //Dont know how to overwrite files in xml so delete and then create :)))))))))))))

            if (File.Exists("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallSolutionExample.xml"))
            {
                {
                    File.Delete("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallSolutionExample.xml");
                }
                using (FileStream fileStream =
                    new FileStream("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallSolutionExample.xml",
                        FileMode.Create))
                {
                    xs2.Serialize(fileStream, res2);
                }

                Solution res3 = NewRandomSolution(res2, res);
                var q = res2.Task2.SequenceEqual(res3.Task2);
                
                if (File.Exists("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallNewSolutionExample.xml"))
                {
                    {
                        File.Delete("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallNewSolutionExample.xml");
                    }
                }

                using (FileStream fileStream =
                    new FileStream("/Users/martindanielnielsen/Projects/SO_CSHARP2/SO_CSHARP2/files/smallNewSolutionExample.xml",
                        FileMode.Create))
                {
                    xs2.Serialize(fileStream, res3);
                }
            }
        }

        /// <summary>
        /// Method to return initial random solution given a Model object
        /// </summary>
        static Solution RandomSolution(Model a)
        {
            //Extract tasks from model a.

            var taskList = a.Application.Task;
            taskList.Sort((x, y) => int.Parse(x.Deadline).CompareTo(int.Parse(y.Deadline)));
            var solution = new Solution();
            List<Task2> task2List = new List<Task2>();
            Random rand = new Random();
            // Fill out at random cores and calculate WCRT

            foreach (var t in taskList)
            {
                Task2 task2 = new Task2();
                task2.Id = t.Id;
                task2.Deadline = t.Deadline;
                task2.MCP = rand.Next(0, 2).ToString();
                task2.Core = a.Platform.MCP[Int32.Parse(task2.MCP)].Core[rand.Next(0, 4)].Id;
                var wcetfactor = a.Platform.MCP[int.Parse(task2.MCP)].Core[int.Parse(task2.Core)].WCETFactor;
                task2.WCRT =
                    Convert.ToInt32(float.Parse(t.WCET, CultureInfo.InvariantCulture) *
                                    float.Parse(wcetfactor, CultureInfo.InvariantCulture)).ToString();
                task2.Priority = taskList.IndexOf(t);
                task2List.Add(task2);
                solution.Task2 = task2List;
            }

            return solution;
        }


        /// <summary>
        /// Method that creates a new solution by swapping Tasks from different cores
        /// <param name="s"> A given solution</param>
        /// <param name="a"> The original model used</param>
        /// <returns> A new random solution</returns>
        /// </summary>
        static Solution NewRandomSolution(Solution s, Model a)
        {
            Random rnd = new Random();
            Solution d = (Solution) s.Clone();
            var s1 = d.Task2.OrderBy(x => rnd.Next()).Take(2).ToList();

            // If task from same core chosen get new ones.
            while (s1[0].MCP == s1[1].MCP && s1[0].Core == s1[1].Core)
            {
                s1 = d.Task2.OrderBy(x => rnd.Next()).Take(2).ToList();

            }

            // temp storage for task properties
            var tempCore = s1[0].Core;
            var tempMcp = s1[0].MCP;

            s1[0].Core = s1[1].Core;
            s1[0].MCP = s1[1].MCP;
            s1[1].Core = tempCore;
            s1[1].MCP = tempMcp;

            //probably not efficient
            var wcetfactor = a.Platform.MCP[int.Parse(s1[0].MCP)].Core[int.Parse(s1[0].Core)].WCETFactor;
            var wcet = a.Application.Task[a.Application.Task.FindIndex(x => x.Id == s1[0].Id)].WCET;
            var wcetfactor2 = a.Platform.MCP[int.Parse(s1[1].MCP)].Core[int.Parse(s1[1].Core)].WCETFactor;
            var wcet2 = a.Application.Task[a.Application.Task.FindIndex(x => x.Id == s1[1].Id)].WCET;
            s1[0].WCRT = Convert.ToInt32(float.Parse(wcet, CultureInfo.InvariantCulture) *
                                         float.Parse(wcetfactor, CultureInfo.InvariantCulture)).ToString();
            s1[1].WCRT = Convert.ToInt32(float.Parse(wcet2, CultureInfo.InvariantCulture) *
                                         float.Parse(wcetfactor2, CultureInfo.InvariantCulture)).ToString();
            return d;
        }

        static Solution SimulatedAnnealing(Model a)
        {
            Stopwatch time = new Stopwatch();
            double t = 100000;
            var alpha = 0.0003;
            var c = RandomSolution(a);

            List<Solution> solutionSpace = new List<Solution>();
            time.Start();
            while (time.Elapsed.TotalMinutes < 5)
            {
               Solution d = NewRandomSolution(c, a);
               while (solutionSpace.Contains(d))
               {
                   d = NewRandomSolution(c, a);
               }
               var q = c.Task2.SequenceEqual(d.Task2);
                int lambda = costFunction(c) - costFunction(d);
                if (lambda > 0 || lambda > Math.Exp(-(1 / t) * lambda))
                {
                    solutionSpace.Add(c);
                    c.Laxity = lax(c);
                    c.Scheduleable = doesTasksMeetsDeadline(c);
                }
                else
                {
                    c = d;
                }

                t *= (1 - alpha);
            }

            return solutionSpace.Last();
        }

        static int costFunction(Solution s1)
        {
            int c = 0;
            List<Task2> task2List = s1.Task2;
            foreach (Task2 t in task2List)
            {
                int d = int.Parse(t.Deadline) - int.Parse(t.WCRT);
                if (d < 0)
                {
                    c += 1000;
                }

                c += d;
            }

            return c;
        }

        static int lax(Solution s1)
        {
            int avglax = 0;
            foreach (Task2 t in s1.Task2)
            {
                avglax += int.Parse(t.Deadline) - int.Parse(t.WCRT);
            }
            return avglax/s1.Task2.Count;
        }

        static bool doesTasksMeetsDeadline(Solution s)
        {
            {

                int u = (int) (s.Task2.Count*(Math.Pow(2,(1/s.Task2.Count)-1)));
                int r = 0;
                foreach (var t in s.Task2)
                {
                    r += int.Parse(t.WCRT) / int.Parse(t.Deadline);
                }

                if (r <= u)
                {
                    return true;
                }
                return false;
            }
        }
    }


    [XmlRoot(ElementName = "Task")]
        public class Task
        {
            [XmlAttribute(AttributeName = "Deadline")]
            public string Deadline { get; set; }

            [XmlAttribute(AttributeName = "Id")] public string Id { get; set; }

            [XmlAttribute(AttributeName = "Period")]
            public string Period { get; set; }

            [XmlAttribute(AttributeName = "WCET")] public string WCET { get; set; }
        }

        [XmlRoot(ElementName = "Application")]
        public class Application
        {
            [XmlElement(ElementName = "Task")] public List<Task> Task { get; set; }
        }

        [XmlRoot(ElementName = "Core")]
        public class Core
        {
            [XmlAttribute(AttributeName = "Id")] public string Id { get; set; }

            [XmlAttribute(AttributeName = "WCETFactor")]
            public string WCETFactor { get; set; }
        }

        [XmlRoot(ElementName = "MCP")]
        public class MCP
        {
            [XmlElement(ElementName = "Core")] public List<Core> Core { get; set; }
            [XmlAttribute(AttributeName = "Id")] public string Id { get; set; }
        }

        [XmlRoot(ElementName = "Platform")]
        public class Platform
        {
            [XmlElement(ElementName = "MCP")] public List<MCP> MCP { get; set; }
        }

        [XmlRoot(ElementName = "Model")]
        public class Model
        {
            [XmlElement(ElementName = "Application")]
            public Application Application { get; set; }

            [XmlElement(ElementName = "Platform")] public Platform Platform { get; set; }
        }

        [XmlRoot(ElementName = "Task2")]
        public class Task2 : ICloneable
        {
            [XmlAttribute(AttributeName = "Id")] public string Id { get; set; }
            [XmlAttribute(AttributeName = "MCP")] public string MCP { get; set; }
            [XmlAttribute(AttributeName = "Core")] public string Core { get; set; }
            [XmlAttribute(AttributeName = "WCRT")] public string WCRT { get; set; }
            [XmlIgnoreAttribute] public string Deadline { get; set; }
            [XmlIgnoreAttribute] public int Priority { get; set; }
            
            [XmlIgnoreAttribute] public int ResponseTime { get; set; }

            public object Clone()
            {
                var item = new Task2()
                {
                    Id = Id,
                    MCP = MCP,
                    Core = Core,
                    Deadline = Deadline,
                    Priority = Priority,
                    ResponseTime = ResponseTime,
                    WCRT = WCRT
                };
                return item;
            }
        }

        [XmlRoot(ElementName = "solution")]
        [Serializable]
        public class Solution : ICloneable
        {
            [XmlElement(ElementName = "Task2")] public List<Task2> Task2 { get; set; }
            
            public int Laxity { get; set; }
            public bool Scheduleable { get; set; }
            public object Clone()
            {
                var item = new Solution()
                {
                    Laxity = Laxity,
                    Scheduleable = Scheduleable,
                    Task2 = Task2.Select(x => x.Clone()).Cast<Task2>().ToList()
                };
                return item;
            }
        }
        
    }

