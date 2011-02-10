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
using Gallio.Common.Collections;
using Gallio.Framework.Data;

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
    ///     [Impersonate(UserName = "Marcus Brutus", Password = "EtTuBrute?")]
    ///     public void MyTest()
    ///     {
    ///         // Some test logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class ImpersonateAttribute : TestDecoratorPatternAttribute
    {
        private const string ImpersonationDataParameterName = "ImpersonationData";
        private static readonly Key<ImpersonationData> ImpersonationDataKey = new Key<ImpersonationData>(ImpersonationDataParameterName);

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
            EnlistImpersonationData( GetOrCreateImpersonationDataTestParameter(scope), new ImpersonationData 
            {
                UserName = UserName, 
                Password = Password, 
                Domain = Domain
            });
        }

        private static ITestParameterBuilder GetOrCreateImpersonationDataTestParameter(IPatternScope scope)
        {
            ITestParameterBuilder parameterBuilder = scope.TestBuilder.GetParameter(ImpersonationDataParameterName);

            if (parameterBuilder == null)
            {
                // Add a new test parameter for some ImpersonationData. 
                // This makes the test data-driven.
                ITestDataContextBuilder parameterDataContextBuilder = scope.TestDataContextBuilder.CreateChild();
                parameterBuilder = scope.TestBuilder.CreateParameter(ImpersonationDataParameterName, null, parameterDataContextBuilder);

                // When the ImpersonationData is bound to the parameter before the initialization phase
                // of the test, add it to the test instance state so we can access it later.
                parameterBuilder.TestParameterActions.BindTestParameterChain.After((state, obj) =>
                {
                    var data = (ImpersonationData)obj;
                    state.Data.SetValue(ImpersonationDataKey, data);
                    state.AddNameSuffix(data.GetChildTestSuffix());
                });

                scope.TestBuilder.TestInstanceActions.RunTestInstanceBodyChain.Around((state, inner) =>
                {
                    ImpersonationData data = state.Data.GetValue(ImpersonationDataKey);

                    using (new Impersonation(data.UserName, data.Domain ?? String.Empty, data.Password))
                    {
                        return inner(state);
                    }
                });
            }

            return parameterBuilder;
        }

        private static void EnlistImpersonationData(ITestParameterBuilder parameterBuilder, ImpersonationData data)
        {
            DataSource dataSource = parameterBuilder.TestDataContextBuilder.DefineDataSource("");
            dataSource.AddDataSet(new ValueSequenceDataSet(new[] { data }, null, false));
        }

        private struct ImpersonationData
        {
            public string UserName;
            public string Password;
            public string Domain;

            public string GetChildTestSuffix()
            {
                return (String.IsNullOrEmpty(Domain) ? String.Empty : Domain + "_") + UserName;
            }
        }
    }
}
