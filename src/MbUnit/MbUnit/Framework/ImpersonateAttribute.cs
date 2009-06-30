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
using Gallio.Model;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using Gallio.Runtime.Security;
using System.ComponentModel;

namespace MbUnit.Framework
{
    /// <summary>
    /// Run a test or a test fixture under another user account.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     [Impersonate(UserName = "Julius Caesar", Password = "VeniVidiVici")]
    ///     public void MyTest()
    ///     {
    ///         // Some test logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ImpersonateAttribute : TestDecoratorPatternAttribute
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain where the user is defined.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If not specified, the domain is supposed to be the local machine.
        /// </para>
        /// </remarks>
        public string Domain
        {
            get;
            set;
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.RunTestInstanceBodyChain.Around((state, inner) =>
            {
                try
                {
                    using (new Impersonation(UserName, Domain ?? String.Empty, Password))
                    {
                        return inner(state);
                    }
                }
                catch (Win32Exception exception)
                {
                    throw new ImpersonationException(String.Format("Cannot impersonate the specified user ({0})", exception.Message), exception);
                }
            });
        }
    }
}
