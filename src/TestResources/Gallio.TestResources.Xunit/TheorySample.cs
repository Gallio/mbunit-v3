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
using System.Data;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Gallio.TestResources.Xunit
{
    /// <summary>
    /// A simple sample with a Theory attribute.
    /// </summary>
    public class TheorySample
    {
        // (This isn't nearly as convenient as in-line Data attributes...)
        public static DataTable Triangles
        {
            get
            {
                DataTable triangles = new DataTable();
                triangles.Columns.Add("a", typeof(int));
                triangles.Columns.Add("b", typeof(int));
                triangles.Columns.Add("c", typeof(int));

                triangles.Rows.Add(3, 4, 5);
                triangles.Rows.Add(6, 8, 10);
                triangles.Rows.Add(1, 1, 1); // should fail
                return triangles;
            }
        }

        [Theory]
        [DataViaProperty("Triangles")]
        public void Pythagoras(int a, int b, int c)
        {
            Assert.Equal(c * c, a * a + b * b);
        }
    }
}
