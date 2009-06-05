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
using MbUnit.Framework;

namespace Gallio.Tests.Common.Reflection
{
    [TestsOn(typeof(AssemblyBinding))]
    public class AssemblyBindingTest
    {
        [Test]
        public void Constructor_WhenAssemblyNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AssemblyBinding((AssemblyName) null));
        }

        [Test]
        public void Constructor_WhenAssemblyNameNotNull_InitializesProperties()
        {
            var assemblyName = new AssemblyName("Gallio");

            var assemblyReference = new AssemblyBinding(assemblyName);

            Assert.Multiple(() =>
            {
                Assert.AreSame(assemblyName, assemblyReference.AssemblyName);
                Assert.IsNull(assemblyReference.CodeBase);
                Assert.IsFalse(assemblyReference.QualifyPartialName);
                Assert.IsEmpty(assemblyReference.BindingRedirects);
                Assert.IsTrue(assemblyReference.ApplyPublisherPolicy);
            });
        }

        [Test]
        public void Constructor_WhenAssemblyIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AssemblyBinding((Assembly)null));
        }

        [Test]
        public void Constructor_WhenAssemblyNotNull_InitializesProperties()
        {
            var assembly = GetType().Assembly;

            var assemblyReference = new AssemblyBinding(assembly);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(assembly.GetName().FullName, assemblyReference.AssemblyName.FullName);
                Assert.AreEqual(assembly.CodeBase, assemblyReference.CodeBase.ToString());
                Assert.IsFalse(assemblyReference.QualifyPartialName);
                Assert.IsEmpty(assemblyReference.BindingRedirects);
                Assert.IsTrue(assemblyReference.ApplyPublisherPolicy);
            });
        }

        [Test]
        public void CodeBase_WhenCodeBaseIsNotAnAbsoluteUri_Throws()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            Assert.Throws<ArgumentException>(() => binding.CodeBase = new Uri("bar/foo.dll", UriKind.Relative));
        }

        [Test]
        public void CodeBase_WhenCodeBaseIsNull_InitializesPropertyToNull()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            binding.CodeBase = null;

            Assert.IsNull(binding.CodeBase);
        }

        [Test]
        public void CodeBase_WhenCodeBaseIsAnAbsoluteUri_InitializesPropertyToUri()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));
            var codeBase = new Uri("file:///c:/foo.dll");

            binding.CodeBase = codeBase;

            Assert.AreEqual(codeBase, binding.CodeBase);
        }

        [Test]
        public void QualifyPartialName_CanGetSet()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            binding.QualifyPartialName = true;
            Assert.IsTrue(binding.QualifyPartialName);

            binding.QualifyPartialName = false;
            Assert.IsFalse(binding.QualifyPartialName);
        }

        [Test]
        public void ApplyPublisherPolicy_CanGetSet()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            binding.ApplyPublisherPolicy = true;
            Assert.IsTrue(binding.ApplyPublisherPolicy);

            binding.ApplyPublisherPolicy = false;
            Assert.IsFalse(binding.ApplyPublisherPolicy);
        }

        [Test]
        public void AddBindingRedirect_WhenBindingRedirectIsNull_Throws()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            Assert.Throws<ArgumentNullException>(() => binding.AddBindingRedirect(null));
        }

        [Test]
        public void AddBindingRedirect_WhenBindingRedirectIsNotNull_AddsTheRedirect()
        {
            var binding = new AssemblyBinding(new AssemblyName("Gallio"));

            binding.AddBindingRedirect(new AssemblyBinding.BindingRedirect("1.2.3.4"));

            Assert.AreElementsEqual(new[] { new AssemblyBinding.BindingRedirect("1.2.3.4") },
                binding.BindingRedirects,
                (x, y) => x.OldVersion == y.OldVersion);
        }

        [TestsOn(typeof(AssemblyBinding.BindingRedirect))]
        public class BindingRedirectTest
        {
            [Test]
            public void Constructor_WhenOldVersionIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new AssemblyBinding.BindingRedirect(null));
            }

            [Test]
            public void Constructor_WhenOldVersionIsInvalid_Throws()
            {
                var ex = Assert.Throws<ArgumentException>(() => new AssemblyBinding.BindingRedirect("abc-def"));
                Assert.Contains(ex.Message, "Old version must be a version number like ;1.2.3.4' or a range like '1.0.0.0-1.1.65535.65535'.");
            }

            [Test]
            public void Constructor_WhenOldVersionIsSingleVersion_InitializesProperties()
            {
                var bindingRedirect = new AssemblyBinding.BindingRedirect("1.2.3.4");

                Assert.AreEqual("1.2.3.4", bindingRedirect.OldVersion);
            }

            [Test]
            public void Constructor_WhenOldVersionIsVersionRange_InitializesProperties()
            {
                var bindingRedirect = new AssemblyBinding.BindingRedirect("1.2.3.4-2.0.0.0");

                Assert.AreEqual("1.2.3.4-2.0.0.0", bindingRedirect.OldVersion);
            }
        }
    }
}