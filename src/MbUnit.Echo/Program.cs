using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MbUnit.Echo
{
    class Program
    {
        static void Main(string[] args)
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            string applicationTitle = string.Format("MbUnit Console Application - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build);
            
            Console.Title = applicationTitle;
            Console.WriteLine(applicationTitle);
            Console.Write("Get the latest version at ");
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("http://www.mbunit.com/");
            Console.ResetColor();
            
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "/?": // Help Menu
                        Console.WriteLine("\tMbUnit Echo");
                        Console.WriteLine(string.Format("\tVersion {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build));
                        Console.WriteLine();
                        Console.WriteLine("\tProject Lead: Andrew Stopford");
                        Console.WriteLine("\tContributors: Ben Hall, Graham Hay, Johan Appelgren, Joey Calisay,");
                        Console.WriteLine("\t              David Parkinson, Jeff Brown, Marc Stober, Mark Haley");
                        Console.WriteLine();
                        break;

                    default:
                        Console.WriteLine(string.Format("Unrecognised command line argument '{0}'", args[0]));
                        break;
                }
            }

            Console.WriteLine("[Warning] Application not yet implemented.");
        }
    }
}
