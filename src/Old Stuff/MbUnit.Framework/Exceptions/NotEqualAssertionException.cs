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
using System.Text;
using TestDriven.UnitTesting.Exceptions;

namespace MbUnit.Framework
{
    public class NotEqualAssertionException : AssertExceptionBase
    {

        private string expectedMessage = null;

        public NotEqualAssertionException(
                 Object expected,
                 Object actual
                 )
        {
            this.expectedMessage = String.Format("Equal assertion failed: [[{0}]]!=[[{1}]]", expected, actual);
        }

        public NotEqualAssertionException(
            Object expected,
            Object actual,
            String message
            )
            : base(message)
        {
            this.expectedMessage = String.Format("Equal assertion failed: [[{0}]]!=[[{1}]]", expected, actual);
        }

        public override string Message
        {
            get
            {
                return String.Format("{0} {1}", base.Message, this.expectedMessage);
            }
        }

    }
}
