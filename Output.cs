using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;

namespace So_CSHARP
{
    public class Output
    {
        public void GiveOutput(Report rep)
        {

            var xs2 = new XmlSerializer(typeof(Report));

            //Dont know how to overwrite files in xml so delete and then create :)))))))))))))

            if (File.Exists("/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Output/ReportTest.xml"))
            {
                {
                    File.Delete("/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Output/ReportTest.xml");
                }
            }

            using (FileStream fileStream =
                new FileStream("/Users/martindanielnielsen/Projects/ExamProject/So_CSHARP2/files/Example/Output/ReportTest.xml",
                    FileMode.Create))
            {
                xs2.Serialize(fileStream, rep);
            }
        }
        [XmlRoot(ElementName = "Solution")]
        public class Solution
        {
            [XmlAttribute(AttributeName = "Runtime")]
            public string Runtime { get; set; }
            [XmlAttribute(AttributeName = "MeanE2E")]
            public string MeanE2E { get; set; }
            [XmlAttribute(AttributeName = "MeanBW")]
            public string MeanBW { get; set; }
        }

        [XmlRoot(ElementName = "Link")]
        public class Link
        {
            [XmlAttribute(AttributeName = "Source")]
            public string Source { get; set; }
            [XmlAttribute(AttributeName = "Destination")]
            public string Destination { get; set; }
            [XmlAttribute(AttributeName = "Qnumber")]
            public string Qnumber { get; set; }
        }

        [XmlRoot(ElementName = "Message")]
        public class Message
        {
            [XmlElement(ElementName = "Link")]
            public List<Link> Link { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "maxE2E")]
            public string MaxE2E { get; set; }
        }

        [XmlRoot(ElementName = "Report")]
        public class Report
        {
            [XmlElement(ElementName = "Solution")]
            public Solution Solution { get; set; }
            [XmlElement(ElementName = "Message")]
            public List<Message> Message { get; set; }
        }
    }
}
