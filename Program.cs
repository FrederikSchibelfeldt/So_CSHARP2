using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace So_CSHARP
{
    class Program
    {
        static void Main(string[] args)
        {

            Model res = new Model();

            var xs = new XmlSerializer(typeof(Model));
            using (FileStream fileStream = new FileStream("../So_CSHARP2/files/small.xml", FileMode.Open)) 
            {
                res = (Model) xs.Deserialize(fileStream);
            }


        }
        /// <summary>
        /// Method to return initial random solution given a Model object
        /// </summary>
        Solution RandomSolution(Model a)
        {
            //Extract tasks and cores from model a.
            //Remake to loop for cores

            var taskList = a.Application.Task;
            var coreList1 = a.Platform.MCP[0].Core;
            var coreList2 = a.Platform.MCP[1].Core;
            Solution solution = new Solution();
            // Fill out at random cores and calculate WCRT

            foreach (var t in taskList)
            {


            }




            return null;
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
