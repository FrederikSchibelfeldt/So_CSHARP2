using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace So_CSHARP
{
    class Program
    {
        static void Main(string[] args)
        {

            var xs = new XmlSerializer(typeof(Model));
            using (FileStream fileStream = new FileStream("/SO_CSHARP/Files/small.xml", FileMode.Open)) 
            {
                Model result = (Model) xs.Deserialize(fileStream);
            }
        }
    }
    
    [XmlRoot(ElementName="Task")]
    public class Task {
        [XmlAttribute(AttributeName="Deadline5")]
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
    
    

}