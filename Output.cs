using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Output
    {

        public void GiveOutput(Report rep)
        {

            var xs2 = new XmlSerializer(typeof(Report));

            //Dont know how to overwrite files in xml so delete and then create :)))))))))))))

            if (File.Exists("..\\So_CSHARP2\\files\\Example\\Output\\ReportTest1.xml"))
            {
                {
                    File.Delete("..\\So_CSHARP2\\files\\Example\\Output\\ReportTest1.xml");
                }
            }

            using (FileStream fileStream =
                new FileStream("..\\So_CSHARP2\\files\\Example\\Output\\ReportTest1.xml",
                    FileMode.Create))
            {
                xs2.Serialize(fileStream, rep);
            }
        }

        [XmlRoot(ElementName = "Solution")]
        public class Solution
        {
            [XmlAttribute(AttributeName = "Runtime")]
            public long Runtime { get; set; }
            [XmlAttribute(AttributeName = "MeanE2E")]
            public int MeanE2E { get; set; }
            [XmlAttribute(AttributeName = "MeanBW")]
            public long MeanBW { get; set; }
        }

        [XmlRoot(ElementName = "Link")]
        public class Link: ICloneable
        {
            [XmlAttribute(AttributeName = "Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName = "Destination")]
            public string Destination { get; set; }
            [XmlAttribute(AttributeName = "Qnumber")]
            public int Qnumber { get; set; }
            [XmlIgnore]
            public int LinkCycleTurn { get; set; }

            
            public object Clone()
            {
                var item = new Link()
                {
                    Source = Source,
                    Destination = Destination,
                    Qnumber = Qnumber
                };
                return item;
            }
        }

        [XmlRoot(ElementName = "Message")]
        public class Message: ICloneable
        {
            [XmlElement(ElementName = "Link")]
            public List<Link> Links { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "maxE2E")]
            public string MaxE2E { get; set; }
            [XmlAttribute(AttributeName = "BW")]
            public long BW { get; set; }
            [XmlIgnore]
            public string Deadline { get; set; }
            [XmlIgnore]
            public long Size { get; set; }

            public object Clone()
            {
                var item = new Message()
                {
                    Links = Links.Select(x => x.Clone()).Cast<Link>().ToList(),
                    Name = Name,
                    MaxE2E = MaxE2E,
                    BW = BW
                };
                return item;
            }
        }

        [XmlRoot(ElementName = "Report")]
        public class Report : ICloneable
        {
            [XmlElement(ElementName = "Solution")]
            public Solution Solution { get; set; }
            [XmlElement(ElementName = "Message")]
            public List<Message> Messages { get; set; }
            public object Clone()
            {
                var item = new Report()
                {
                    Solution = Solution,
                    Messages = Messages.Select(x => x.Clone()).Cast<Message>().ToList()
                };
                return item;
            }
        }
    }
}
