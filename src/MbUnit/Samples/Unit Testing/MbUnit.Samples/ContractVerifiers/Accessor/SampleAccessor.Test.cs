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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace MbUnit.Samples.ContractVerifiers.Accessor
{
    public class SampleAccessorTest
    {
        // Basic usage.
        [VerifyContract]
        public readonly IContract MagnitudeAccessorTests = new AccessorContract<SampleAccessor, int>
        {
            // You can identify the tested property directly by its name 
            // through the contract property "PropertyName'. 
            PropertyName = "Magnitude",

            // Provide some valid values to feed the tested property.
            ValidValues = { 123, 456, 789 }
        };

        // Advanced usage.
        [VerifyContract]
        public readonly IContract TextAccessorTests = new AccessorContract<SampleAccessor, string>
        {
            // A second approach which is more flexible but also more complicated, consists in 
            // indicating explicitly how to access to the tested accessors by settings the contract 
            // properties 'Setter' and 'Getter' appropriately.
            Setter = (o, v) => o.Text = v,
            Getter = o => o.Text,

            // You still need to provide some valid values.
            ValidValues = { "Some Text", "Hello World!" },

            // Specify if the null reference is expected to be considered as a valid input.
            // If set to 'false', an ArgumentNullException is expected to be thrown by the tested property.
            // By default, that contract property is 'true'; but it is always ignored for non-nullable types.
            AcceptNullValue = false,

            // Invalid values are expected to be rejected by the tested property setter.
            // You can provided lists of invalid values with the type of the exception
            // which is expected to be thrown. In the example below, sending an empty string should
            // cause an ArgumentException.
            InvalidValues =
            {
                { typeof(ArgumentException), String.Empty },
            }
        };

        private static IEnumerable<DateTime> GetSomePastDates()
        {
            yield return DateTime.Now.AddDays(-1);
            yield return new DateTime(1975, 4, 21);
        }

        // More advanced usage.
        [VerifyContract]
        public readonly IContract WhenAccessorTests = new AccessorContract<SampleAccessor, DateTime>
        {
            PropertyName = "When",

            // Valid values can be obtained from external data sources such as a static method.
            ValidValues = new DistinctInstanceCollection<DateTime>(GetSomePastDates()),

            // By default, the contract verifier performs the test on an instance of the tested type
            // created by invoking the default constructor. If no default constructor is available, or
            // if you want to customize the way the instance of the tested type is created, use the
            // contract property 'DefaultInstance' and provide your own instance.
            DefaultInstance = () =>
            {
                var o = new SampleAccessor();
                o.StartEdit();
                return o;
            },

            // You can define several sets of invalid values for each type of exception. 
            InvalidValues =
            {
                { typeof(ArgumentException), DateTime.MinValue, DateTime.MaxValue },
                { typeof(ArgumentOutOfRangeException), DateTime.Now.AddDays(1), new DateTime(9000, 1, 1) }
            }
        };
    }
}
