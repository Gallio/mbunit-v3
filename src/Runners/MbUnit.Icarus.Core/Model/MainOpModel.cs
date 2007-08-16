
using System.Collections.Generic;
using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Serialization;
using MbUnit.Icarus.Core.Interfaces;

namespace MbUnit.Icarus.Core.Model
{
    public class MainOpModel : IMainOpModel
    {

        private readonly List<string> _progressData = new List<string>();

        public TestModel LoadUpAssembly(ITestRunner runner)
        {

                TestPackage testp = new TestPackage();
                testp.AssemblyFiles.Add("C:\\MbUnit\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll");
                // "C:\\MbUnit\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.Gallio\\bin\\MbUnit.TestResources.Gallio.dll");

                ProgressMonitorModel p1 = new ProgressMonitorModel(new List<string>());
                runner.LoadPackage(p1, testp);

                _progressData.AddRange(p1.InfoList);

                ProgressMonitorModel p2 = new ProgressMonitorModel(new List<string>());
                LoadUpTemplates(p2, runner);
                _progressData.AddRange(p2.InfoList);

                ProgressMonitorModel p3 = new ProgressMonitorModel(new List<string>());
                LoadUpTests(p3, runner);
                _progressData.AddRange(p3.InfoList);
            
    

            return runner.TestModel;
     
        }

        private static void LoadUpTemplates(IProgressMonitor p, ITestRunner runner)
        {
            runner.BuildTemplates(p);
        }

        private static void LoadUpTests(IProgressMonitor p, ITestRunner runner)
        {
            runner.BuildTests(p);
        }

    }
}
