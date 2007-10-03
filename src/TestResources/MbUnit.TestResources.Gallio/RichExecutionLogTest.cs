// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Drawing;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.TestResources.Gallio.Properties;

namespace MbUnit.TestResources.Gallio
{
    /// <summary>
    /// Generates an execution log with all sorts of rich content.
    /// </summary>
    [TestFixture]
    public class RichExecutionLogTest
    {
        private LogStreamWriter MbUnitRocks
        {
            get { return Log.Writer["MbUnit Rocks"]; }
        }

        [Test]
        public void GenerateRichLog()
        {
            MbUnitRocks.WriteLine("MbUnit Rocks!");
            MbUnitRocks.WriteLine();

            MbUnitRocks.WriteLine("You can embed images.");
            MbUnitRocks.EmbedImage(Resources.MbUnitLogo);
            MbUnitRocks.WriteLine();

            MbUnitRocks.WriteLine("You can write out your own log streams.");

            MbUnitRocks.BeginSection("My section!");

            MbUnitRocks.WriteLine("And break them into sections.");
            MbUnitRocks.WriteLine("So you can see what's happening.");
            MbUnitRocks.WriteLine("Just in here.");

            MbUnitRocks.EndSection();

            Step.Run("Lemmings!", delegate
            {
                MbUnitRocks.WriteLine("You can subdivide a test into nested steps.");
                MbUnitRocks.WriteLine("And run them repeatedly with independent failure conditions.");

                for (int i = 5; i > 0; i--)
                {
                    Step.Run(i.ToString(), delegate { MbUnitRocks.WriteLine("..."); });
                }

                try
                {
                    Step.Run("Uh oh!", delegate
                    {
                        Assert.Fail("*POP*");
                    });
                }
                catch (AssertionException)
                {
                }
            });

            Step.Run("Tag Line", delegate
            {
                MbUnitRocks.WriteLine("And so much more...");
                MbUnitRocks.WriteLine();

                MbUnitRocks.WriteLine("Welcome to MbUnit Gallio!");
            });
        }
    }
}
