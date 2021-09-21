using System;
using System.Collections.Generic;
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

            Model res = new Model();

            var xs = new XmlSerializer(typeof(Model));
            using (FileStream fileStream = new FileStream("../So_CSHARP/files/small.xml", FileMode.Open)) 
            {
                res = (Model) xs.Deserialize(fileStream);
            }
            Solution res2 =  RandomSolution(res);
            var xs2 = new XmlSerializer(typeof(Solution));

  
            using (FileStream fileStream = new FileStream("../So_CSHARP/files/smallSolutionExample.xml", FileMode.Create)) 
            {
                xs2.Serialize(fileStream,res2);
            }


        }
        /// <summary>
        /// Method to return initial random solution given a Model object
        /// </summary>
      static  Solution RandomSolution(Model a)
        {
            //Extract tasks and cores from model a.
            //Remake to loop for cores

            var taskList = a.Application.Task;
            var coreList1 = a.Platform.MCP[0].Core;
            var coreList2 = a.Platform.MCP[1].Core;
            Solution solution = new Solution();
            List<Task2> task2List = new List<Task2>();
            Random rand = new Random();
            // Fill out at random cores and calculate WCRT

            foreach (var t in taskList)
            {
                Task2 task2 = new Task2();
                task2.Id = t.Id;
                task2.MCP = rand.Next(0, 2).ToString();
                task2.Core = a.Platform.MCP[Int32.Parse(task2.MCP)].Core[rand.Next(0, 4)].Id;
                var wcetfactor = a.Platform.MCP[int.Parse(task2.MCP)].Core[int.Parse(task2.Core)].WCETFactor;
                task2.WCRT =
                    Convert.ToInt32(float.Parse(t.WCET,CultureInfo.InvariantCulture) * float.Parse(wcetfactor,CultureInfo.InvariantCulture)).ToString();
                task2List.Add(task2);
                solution.Task2 = task2List;
            }

            return solution;
        }
    }

    [XmlRoot(ElementName="Task")]
    public class Task {
        [XmlAttribute(AttributeName="Deadline")]
        public string Deadline { get; set; }
        [XmlAttribute(AttributeName="Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName="Period")]
        public string Period { get; set; }
        [XmlAttribute(AttributeName="WCET")]
        public string WCET { get; set; }
    }

    [XmlRoot(ElementName="Application")]
    public class Application {
        [XmlElement(ElementName="Task")]
        public List<Task> Task { get; set; }
    }

    [XmlRoot(ElementName="Core")]
    public class Core {
        [XmlAttribute(AttributeName="Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName="WCETFactor")]
        public string WCETFactor { get; set; }
    }

    [XmlRoot(ElementName="MCP")]
    public class MCP {
        [XmlElement(ElementName="Core")]
        public List<Core> Core { get; set; }
        [XmlAttribute(AttributeName="Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName="Platform")]
    public class Platform {
        [XmlElement(ElementName="MCP")]
        public List<MCP> MCP { get; set; }
    }

    [XmlRoot(ElementName="Model")]
    public class Model {
        [XmlElement(ElementName="Application")]
        public Application Application { get; set; }
        [XmlElement(ElementName="Platform")]
        public Platform Platform { get; set; }
    }

    [XmlRoot(ElementName="Task2")]
    public class Task2 {
        [XmlAttribute(AttributeName="Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName="MCP")]
        public string MCP { get; set; }
        [XmlAttribute(AttributeName="Core")]
        public string Core { get; set; }
        [XmlAttribute(AttributeName="WCRT")]
        public string WCRT { get; set; }
    }

    [XmlRoot(ElementName="solution")]
    public class Solution {
        [XmlElement(ElementName="Task2")]
        public List<Task2> Task2 { get; set; }
    }
}
