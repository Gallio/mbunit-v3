// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using System.IO;
using Rhino.Mocks;
using Gallio.Common;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(PluginRegistration))]
    public class PluginRegistrationTest
    {
        [Test]
        public void Constructor_WhenPluginIdIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration(null, new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
            });
        }

        [Test]
        public void Constructor_WhenPluginTypeNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration("pluginId", null, new DirectoryInfo(@"C:\"));
            });
        }

        [Test]
        public void Constructor_WhenBaseDirectoryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), null);
            });
        }

        [Test]
        public void PluginId_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual("pluginId", registration.PluginId);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginId = null; });

            registration.PluginId = "differentPluginId";

            Assert.AreEqual("differentPluginId", registration.PluginId);
        }

        [Test]
        public void PluginTypeName_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual(new TypeName("Plugin, Assembly"), registration.PluginTypeName);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginTypeName = null; });

            registration.PluginTypeName = new TypeName("DifferentPlugin, Assembly");

            Assert.AreEqual(new TypeName("DifferentPlugin, Assembly"), registration.PluginTypeName);
        }

        [Test]
        public void BaseDirectory_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual(@"C:\", registration.BaseDirectory.ToString());
            Assert.Throws<ArgumentNullException>(() => { registration.BaseDirectory = null; });

            registration.BaseDirectory = new DirectoryInfo(@"D:\");

            Assert.AreEqual(@"D:\", registration.BaseDirectory.ToString());
        }

        [Test]
        public void PluginProperties_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.PluginProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginProperties = null; });

            var differentProperties = new PropertySet();
            registration.PluginProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.PluginProperties);
        }

        [Test]
        public void TraitsProperties_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.TraitsProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.TraitsProperties = null; });

            var differentProperties = new PropertySet();
            registration.TraitsProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.TraitsProperties);
        }

        [Test]
        public void PluginHandlerFactory_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsInstanceOfType<SingletonHandlerFactory>(registration.PluginHandlerFactory);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginHandlerFactory = null; });

            var differentHandlerFactory = MockRepository.GenerateStub<IHandlerFactory>();
            registration.PluginHandlerFactory = differentHandlerFactory;

            Assert.AreSame(differentHandlerFactory, registration.PluginHandlerFactory);
        }

        [Test]
        public void AssemblyBindings_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.AssemblyBindings);
            Assert.Throws<ArgumentNullException>(() => { registration.AssemblyBindings = null; });

            var differentReferences = new[] { new AssemblyBinding(new AssemblyName("Gallio")) };
            registration.AssemblyBindings = differentReferences;

            Assert.AreSame(differentReferences, registration.AssemblyBindings);
        }

        [Test]
        public void PluginDependencies_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.PluginDependencies);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginDependencies = null; });

            var differentDependencies = new[] { MockRepository.GenerateStub<IPluginDescriptor>() };
            registration.PluginDependencies = differentDependencies;

            Assert.AreSame(differentDependencies, registration.PluginDependencies);
        }

        [Test]
        public void ProbingPaths_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.ProbingPaths);
            Assert.Throws<ArgumentNullException>(() => { registration.ProbingPaths = null; });

            var differentPaths = new[] { "privateBin", "publicBin" };
            registration.ProbingPaths = differentPaths;

            Assert.AreSame(differentPaths, registration.ProbingPaths);
        }

        [Test]
        public void RecommendedInstallationPath_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsNull(registration.RecommendedInstallationPath);

            registration.RecommendedInstallationPath = "MyPlugin";
            Assert.AreEqual("MyPlugin", registration.RecommendedInstallationPath);

            registration.RecommendedInstallationPath = null;
            Assert.IsNull(registration.RecommendedInstallationPath);
        }

        [Test]
        public void EnableCondition_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
            var condition = Condition.Parse("${minFramework:NET35}");

            Assert.IsNull(registration.EnableCondition);

            registration.EnableCondition = condition;
            Assert.AreEqual(condition, registration.EnableCondition);

            registration.EnableCondition = null;
            Assert.IsNull(registration.EnableCondition);
        }

        [Test]
        public void FilePaths_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.FilePaths);
            Assert.Throws<ArgumentNullException>(() => { registration.FilePaths = null; });

            var differentPaths = new[] { "file1.txt", "file2.dll" };
            registration.FilePaths = differentPaths;

            Assert.AreSame(differentPaths, registration.FilePaths);
        }
    }
}
