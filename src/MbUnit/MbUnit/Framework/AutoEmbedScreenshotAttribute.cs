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
using Gallio.Common;
using Gallio.Common.Media;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Decorates a test method and causes a screenshot to be automatically embeded when a trigger event occurs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If screenshots cannot be captured, the attribute will embed a warning message to that effect.
    /// </para>
    /// </remarks>
    /// <seealso cref="Capture.AutoEmbedScreenshot(TriggerEvent, string)"/>
    /// <seealso cref="Capture.AutoEmbedScreenshot(TriggerEvent, string, CaptureParameters)"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class AutoEmbedScreenshotAttribute : TestDecoratorPatternAttribute
    {
        private readonly TriggerEvent triggerEvent;
        private readonly CaptureParameters parameters = new CaptureParameters();

        /// <summary>
        /// Gets or sets the name to give to the image attachment.
        /// (default is null)
        /// </summary>
        /// <remarks>
        /// <para>
        /// The property is <c>null</c> by default, which causes the name of the attachment to be assigned automatically.
        /// </para>
        /// </remarks>
        public string AttachmentName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The zoom factor specifies the degree of magnification or reduction desired.
        /// For example, a zoom factor of 1.0 (the default) is normal size, 0.25 reduces to one quarter the
        /// original size and 2.0 magnifies to twice the original size.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1/16 or more than 16.</exception>
        public double Zoom
        {
            get
            {
                return parameters.Zoom;
            }

            set
            {
                parameters.Zoom = value;
            }
        }

        /// <summary>
        /// Automatically embeds a screenshot when a trigger event occurs.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// [AutoEmbedScreenshot(TriggerEvent.TestPassed, Zoom = 0.25)]
        /// public void Test()
        /// {
        ///     // Code logic here.
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="triggerEvent">The trigger event.</param>
        public AutoEmbedScreenshotAttribute(TriggerEvent triggerEvent)
        {
            this.triggerEvent = triggerEvent;
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.SetUpTestInstanceChain.Before(state =>
                Capture.AutoEmbedScreenshot(triggerEvent, AttachmentName, parameters));
        }
    }
}
