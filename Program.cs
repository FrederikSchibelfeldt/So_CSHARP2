using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static So_CSHARP.Inputs;
using static So_CSHARP.Output;

namespace So_CSHARP
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = readApps();
            var arch = readConfig();
            Console.WriteLine("Hey");

        }

    }
}

