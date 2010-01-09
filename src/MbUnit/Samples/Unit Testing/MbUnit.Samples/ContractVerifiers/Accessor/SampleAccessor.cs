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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Runtime.Serialization;

namespace MbUnit.Samples.ContractVerifiers.Accessor
{
    public class SampleAccessor
    {
        public int Magnitude
        {
            get;
            set;
        }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException();
                }

                text = value;
            }
        }

        private DateTime when;
        private bool editing;

        public DateTime When
        {
            get
            {
                return when;
            }

            set
            {
                if ((value == DateTime.MinValue) ||
                    (value == DateTime.MaxValue))
                {
                    throw new ArgumentException();
                }

                if (value >= DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (editing)
                {
                    when = value;
                }
            }
        }

        public void StartEdit()
        {
            editing = true;
        }

        public void StopEdit()
        {
            editing = false;
        }
    }
}
