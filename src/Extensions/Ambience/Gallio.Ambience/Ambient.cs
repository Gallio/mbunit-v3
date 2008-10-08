// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// The Ambient object store is a shared lightweight repository for intermediate
    /// test data.  It is used to pass information from one test to another or to store
    /// it for subsequent analysis.
    /// </para>
    /// <para>
    /// The Ambient object store may be used to model the persistent state of the
    /// testing environment for end-to-end black-box integration testing.  It is particularly
    /// useful for decoupling tests that incorporate stateful components such as
    /// databases (that are not wiped and restored each time) or time-sensitive processes
    /// such as asynchronous jobs.
    /// </para>
    /// </summary>
    /// <example>
    /// <para>
    /// Suppose we are testing a periodic invoicing process.  The business requirement
    /// dictates that on the last day of each month, an invoice must be generated
    /// and delivered electronically to all customers with an outstanding balance
    /// in their account.
    /// </para>
    /// <para>
    /// To verify this requirement, we might choose to adopt a combination of unit
    /// testing and integration testing methodologies.
    /// </para>
    /// <para>
    /// First, we will write unit tests for each component involved in the invoicing
    /// process including the invoice generator, data access code, electronic delivery apparatus,
    /// and the periodic job scheduler.  During unit testing, we will probably
    /// replace the periodic job scheduler with a mock or stub implementation that simulates
    /// the end-of-month trigger mechanism.  This is fast and quite effective.
    /// </para>
    /// <para>
    /// Next, for additional confidence we will want to verify that the system works
    /// completely as an integrated whole.  Perhaps the job scheduling mechanism is built
    /// on 3rd party tools that must be correctly configured for the environment.  In
    /// this situation, we might consider implementing an end-to-end system test
    /// with nothing mocked out.
    /// </para>
    /// <para>
    /// We can implement an end-to-end test like this:
    /// <list type="bullet">
    /// <item>Write a few test scripts that are scheduled to run on a regular basis
    /// and that generate transactions that will be reflected in the invoices.</item>
    /// <item>For each transaction, such as creating a new account or purchasing a service,
    /// the scripts should make a record in the <see cref="Ambient"/> object store.</item>
    /// <item>Write a few more test scripts that are scheduled to run after the invoicing
    /// process occurs.  They should verify the contents of the invoices based on previously
    /// stored records in the <see cref="Ambient"/> object store.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Of course, in this example it can take up to a month to obtain confirmation that
    /// the invoicing process functioned as expected.  Consequently, we should not rely
    /// on this approach exclusively for our testing purposes.  Nevertheless, it may be
    /// useful as part of an on-going Quality Assurance audit.
    /// </para>
    /// </example>
    public static class Ambient
    {
        private static readonly object syncRoot = new object();
        private static AmbienceClient defaultClient;

        /// <summary>
        /// <para>
        /// Gets the default ambient data container.
        /// </para>
        /// </summary>
        public static IAmbientDataContainer Data
        {
            get
            {
                lock (syncRoot)
                {
                    if (defaultClient == null)
                        defaultClient = ConnectDefaultClient();
                    return defaultClient.Container;
                }
            }
        }

        /// <summary>
        /// Gets the default client configuration.
        /// </summary>
        /// <returns>The default client configuration</returns>
        public static AmbienceClientConfiguration GetDefaultClientConfiguration()
        {
            // TODO
            return new AmbienceClientConfiguration();
        }

        private static AmbienceClient ConnectDefaultClient()
        {
            return AmbienceClient.Connect(GetDefaultClientConfiguration());
        }
    }
}
