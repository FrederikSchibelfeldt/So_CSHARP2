using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Linq;


namespace So_CSHARP
{
    public class Output
    {
        static string inputPath = "..\\So_CSHARP2\\files\\Example";

        public void GiveOutput(Report rep)
        {
            string sFilePath =  Path.GetFullPath(inputPath + "\\Output\\ReportTest_SA1.xml");
            string sFilePathMartin = "/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Output/ReportTest.xml";
            var xs2 = new XmlSerializer(typeof(Report));

            //Dont know how to overwrite files in xml so delete and then create :)))))))))))))

            if (File.Exists(sFilePathMartin))
            {
                {
                    File.Delete(sFilePathMartin);
                }
            }

            using (FileStream fileStream =
                new FileStream(sFilePathMartin,
                    FileMode.Create))
            {
                xs2.Serialize(fileStream, rep);
            }
        }

        [XmlRoot(ElementName = "Solution")]
        [Serializable]
        public class Solution : ICloneable
        {
            [XmlAttribute(AttributeName = "Runtime")]
            public long Runtime { get; set; }
            [XmlAttribute(AttributeName = "MeanE2E")]
            public int MeanE2E { get; set; }
            [XmlAttribute(AttributeName = "MeanBW")]
            public long MeanBW { get; set; }
                       [XmlElement(ElementName = "Cost")]
            public long Cost { get; set; }
            public object Clone()
            {
                var item = new Solution()
                {
                    Runtime = Runtime,
                    MeanE2E = MeanE2E,
                    MeanBW = MeanBW
                };
                return item;
            }
    }

    [XmlRoot(ElementName = "Link")]
        public class Link : ICloneable
        {
            [XmlAttribute(AttributeName = "Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName = "Destination")]
            public string Destination { get; set; }
            [XmlAttribute(AttributeName = "Qnumber")]
            public int Qnumber { get; set; }
            public int LinkCycleTurn { get; set; }

            public object Clone()
            {
                var item = new Link()
                {
                    Source = Source,
                    Destination = Destination,
                    Qnumber = Qnumber,
                    LinkCycleTurn = LinkCycleTurn
                };
                return item;
            }
        }

        [XmlRoot(ElementName = "Message")]
        public class Message : ICloneable
        {
            [XmlElement(ElementName = "Link")]
            public List<Link> Links { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "maxE2E")]
            public string MaxE2E { get; set; }
            public string Deadline { get; set; }
            public long Size { get; set; }
            public long BW { get; set; }
            public object Clone()
            {
                var item = new Message()
                {
                    Links = Links.Select(x => x.Clone()).Cast<Link>().ToList(),
                    Name = Name,
                    MaxE2E = MaxE2E,
                    Deadline = Deadline,
                    BW = BW,
                    Size = Size
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
                    Solution = (Solution) Solution.Clone(),
                    Messages = Messages.Select(x => x.Clone()).Cast<Message>().ToList()
                };
                return item;
            }
        }
    }
}
