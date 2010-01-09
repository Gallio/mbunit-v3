// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Security;
using System.ComponentModel;
using System.Security.Principal;
using System.Security;
using System.Threading;

namespace MbUnit.Framework
{
    /// <summary>
    /// Run a test or a test fixture under another user account.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>PrincipalAttribute</c> is an abstract class. You must derive your own attribute from this class,
    /// and implement <see cref="CreatePrincipal"/> to return a principal object.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class MyPrincipalAttribute : PrincipalAttribute
    /// {
    ///     protected override IPrincipal CreatePrincipal()
    ///     {
    ///         // Create or retrieve the principal...
    ///     }
    /// }
    /// 
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test, MyPrincipal]
    ///     public void MyTest()
    ///     {
    ///         // Some test logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public abstract class PrincipalAttribute : TestDecoratorPatternAttribute
    {
        /// <summary>
        /// Creates a principal object which represents the security context 
        /// of the user on whose behalf the test is running.
        /// </summary>
        /// <returns>A principal object.</returns>
        protected abstract IPrincipal CreatePrincipal();

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.RunTestInstanceBodyChain.Around((state, inner) =>
            {
                try
                {
                    IPrincipal previous = Thread.CurrentPrincipal;
                    Thread.CurrentPrincipal = CreatePrincipal();

                    try
                    {
                        return inner(state);
                    }
                    finally
                    {
                        Thread.CurrentPrincipal = previous;
                    }
                }
                catch (SecurityException exception)
                {
                    throw new ImpersonationException("Cannot impersonate the specified principal: " + exception.Message, exception);
                }
            });
        }
    }
}
