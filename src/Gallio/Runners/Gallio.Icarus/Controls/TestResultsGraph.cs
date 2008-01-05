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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using ZedGraph;

namespace Gallio.Icarus.Controls
{
    public class TestResultsGraph : ZedGraphControl
    {
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> testResults;
        private List<string> labels;
        private string mode = "Test results by Type";

        public string Mode
        {
            set
            {
                if (mode != value)
                {
                    mode = value;
                    DisplayGraph();
                }
            }
        }

        public TestResultsGraph()
        {
            testResults = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            labels = new List<string>();
            GraphPane.IsFontsScaled = false;
            GraphPane.XAxis.Title.Text = "Number of tests";
        }

        public void UpdateTestResults(string testOutcome, string typeName, string namespaceName, string assemblyName)
        {
            //int index = typeName.LastIndexOf('.');
            //if (index != -1)
            //    typeName = typeName.Substring(index + 1);
            UpdateTestResults(testOutcome, "Type", typeName);
            UpdateTestResults(testOutcome, "Namespace", namespaceName);
            //index = assemblyName.IndexOf(',');
            //if (index != -1)
            //    assemblyName = assemblyName.Substring(0, index);
            UpdateTestResults(testOutcome, "Assembly", assemblyName);

            DisplayGraph();
        }

        private void UpdateTestResults(string testOutcome, string category, string key)
        {
            if (testResults.ContainsKey(testOutcome))
            {
                if (testResults[testOutcome].ContainsKey(category))
                {
                    if (testResults[testOutcome][category].ContainsKey(key))
                        testResults[testOutcome][category][key] += 1;
                    else
                        testResults[testOutcome][category].Add(key, 1);
                }
                else
                {
                    testResults[testOutcome].Add(category, new Dictionary<string, double>());
                    UpdateTestResults(testOutcome, category, key);
                }
            }
            else
            {
                testResults.Add(testOutcome, new Dictionary<string, Dictionary<string, double>>());
                UpdateTestResults(testOutcome, category, key);
            }
        }

        public void DisplayGraph()
        {
            // clear any existing curves
            GraphPane.CurveList.Clear();
            labels.Clear();

            // set title
            GraphPane.Title.Text = mode;
            switch (mode)
            {
                case "Test results by Type":
                    GraphPane.YAxis.Title.Text = "Type name";
                    DisplayBarGraph("Type");
                    break;

                case "Test results by Namespace":
                    GraphPane.YAxis.Title.Text = "Namespace name";
                    DisplayBarGraph("Namespace");
                    break;

                case "Test results by Assembly":
                    GraphPane.YAxis.Title.Text = "Assembly name";
                    DisplayBarGraph("Assembly");
                    break;
            }

            // resize axes and refresh
            AxisChange();
            Refresh();
        }

        private void DisplayBarGraph(string category)
        {
            // colour background
            GraphPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 45.0F);
            // rotate graph
            GraphPane.BarSettings.Base = BarBase.Y;
            // stack bars
            GraphPane.BarSettings.Type = BarType.Stack;

            // passed results
            GraphPane.AddBar("Passed", GetValues("Passed", category).ToArray(), null, Color.Green);
            // failed results
            GraphPane.AddBar("Failed", GetValues("Failed", category).ToArray(), null, Color.Red);
            // inconclusive results
            GraphPane.AddBar("Inconclusive", GetValues("Inconclusive", category).ToArray(), null, Color.Yellow);

            // add bar labels
            GraphPane.YAxis.Scale.TextLabels = labels.ToArray();
            GraphPane.YAxis.Type = AxisType.Text;
        }

        private List<double> GetValues(string testOutcome, string category)
        {
            // inconclusive results
            List<double> list = new List<double>();
            if (testResults.ContainsKey(testOutcome) && testResults[testOutcome].ContainsKey(category))
            {
                foreach (string type in testResults[testOutcome][category].Keys)
                {
                    if (!labels.Contains(type))
                        labels.Add(type);
                    list.Add(testResults[testOutcome][category][type]);
                }
            }
            return list;
        }
    }
}
