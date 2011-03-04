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
using System.Text;
using NVelocity;
using NVelocity.App;
using Gallio.Runner.Reports;
using System.Text.RegularExpressions;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Collections;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Markup;
using System.Xml;
using System.IO;
using Gallio.Model.Schema;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Provides helper methods to sort and prepare the contents of the report.
    /// </summary>
    internal class FormatContentsHelper
    {
        /// <summary>
        /// Returns the value of the specified attribute in a marker tag.
        /// </summary>
        /// <param name="markerTag">The marker tag.</param>
        /// <param name="name">The name of the searched attribute.</param>
        /// <returns>The value of the attribute, or an empty string if not found.</returns>
        public static string GetMarkerAttributeValue(MarkerTag markerTag, string name)
        {
            int index = markerTag.Attributes.FindIndex(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return (index < 0) ? String.Empty : markerTag.Attributes[index].Value;
        }

        public static List<AnnotationData> GetAnnotations(Report report)
        {
            var list = new List<AnnotationData>();
            list.AddRange(GetAnnotationsWithType(report, AnnotationType.Error));
            list.AddRange(GetAnnotationsWithType(report, AnnotationType.Warning));
            list.AddRange(GetAnnotationsWithType(report, AnnotationType.Info));
            return list;
        }

        private static IEnumerable<AnnotationData> GetAnnotationsWithType(Report report, AnnotationType type)
        {
            foreach (AnnotationData annotation in report.TestModel.Annotations)
            {
                if (annotation.Type == type)
                    yield return annotation;
            }
        }
    }
}