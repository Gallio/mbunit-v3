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
using System.Collections.Generic;
using System.Text;
using Gallio.Data.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Data.Conversions
{
    /// <summary>
    /// Abstract base class for <see cref="IConversionRule" /> tests.
    /// Automatically sets up a <see cref="RuleBasedConverter" /> populated with
    /// the rule and a <see cref="ConvertibleToConvertibleConversionRule"/>
    /// </summary>
    public abstract class BaseConversionRuleTest<T>
        where T : IConversionRule, new()
    {
        private IConverter converter;

        public IConverter Converter
        {
            get { return converter; }
        }

        [SetUp]
        public void SetUpConverter()
        {
            converter = new RuleBasedConverter(new IConversionRule[]
            {
                new T(),
                new ConvertibleToConvertibleConversionRule()
            });
        }
    }
}
