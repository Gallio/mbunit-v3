// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gallio.MSTestAdapter.Generator
{
    class Program
    {
        private static string stubFrameworkPath = "../../Microsoft.VisualStudio.QualityTools.UnitTestFramework.Stub/";
        private static string adapterProjectPath = "../../Gallio.MSTestAdapter/";

        static void Main(string[] args)
        {
            CreateStubs();
            CreateAttributesClass();
        }

        private static void CreateAttributesClass()
        {
            Assembly MSTestAssembly = LoadMSTestAssembly();
            if (MSTestAssembly == null)
                return;
            var e = from t in MSTestAssembly.GetTypes()
                    where t.IsPublic
                    && t.Name.EndsWith("Attribute")
                    orderby t.Name
                    select t;
            string fields = String.Empty;
            foreach (var t in e)
            {
                fields += " internal static string " + t.Name + " = \"" + t.FullName + "\";\r\n        ";
            }
            fields = fields.Substring(0, fields.Length - 10);
            FileInfo classTemplateFile = new FileInfo("MSTestAttributes.template");
            string content = null;
            using (StreamReader sr = new StreamReader(classTemplateFile.Open(FileMode.Open)))
            {
                content = sr.ReadToEnd();
            }
            content = content.Replace("// Fields", fields);
            using (StreamWriter sw = new StreamWriter(adapterProjectPath + "MSTestAttributes.cs"))
            {
                sw.Write(content);
            }
        }

        private static void CreateStubs()
        {
            Assembly MSTestAssembly = LoadMSTestAssembly();
            if (MSTestAssembly == null)
                return;
            var e = from t in MSTestAssembly.GetTypes()
                    where t.IsPublic
                    select t;
            foreach (var t in e)
            {
                CreateClassFile(t);
            }
        }

        private static void CreateClassFile(Type t)
        {
            FileInfo classTemplateFile = new FileInfo("ClassTemplate.template");
            string content = null;
            using (StreamReader sr = new StreamReader(classTemplateFile.Open(FileMode.Open)))
            {
                content = sr.ReadToEnd();
            }
            content = content.Replace("@namespace", t.Namespace);
            content = content.Replace("@kind", GetTypeKind(t));
            content = content.Replace("@classname", GetTypeName(t));
            using (StreamWriter sw = new StreamWriter(stubFrameworkPath + t.Name + ".cs"))
            {
                sw.Write(content);
            }
        }

        private static string GetTypeKind(Type t)
        {
            if (t.IsClass)
                return "class";
            if (t.IsEnum)
                return "enum";
            return null;
        }

        private static string GetTypeName(Type t)
        {
            if (t.IsClass)
            {
                if (t.Name.EndsWith("Attribute") || t.Name.EndsWith("Exception"))
                    return t.Name + " : " + t.BaseType.FullName;
                return t.Name;// +" : " + t.BaseType.FullName;
            }
            if (t.IsEnum)
                return t.Name;
            return null;
        }

        private static Assembly LoadMSTestAssembly()
        {
            Assembly MSTestAssembly = Assembly.Load("Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            return MSTestAssembly;
        }
    }
}
