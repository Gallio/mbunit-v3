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
using System.Linq;
using System.Text;
using Core;
using Gallio.Common.Media;
using Gallio.Framework;
using MbUnit.Framework;
using Repository;

namespace MbUnit.Samples.GUITestingWithWhite.Framework
{
    /// <summary>
    /// Base test fixture for White GUI tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is intended to be used as a superclass for GUI test fixtures using White.
    /// It sets up automatic screen recording and embeds the video in the test report
    /// when the test fails or reports an inconclusive result.
    /// </para>
    /// </remarks>
    public abstract class GUITestFixture
    {
        private Application application;
        private ScreenRepository screenRepository;

        /// <summary>
        /// Gets the application that was launched.
        /// </summary>
        public Application Application
        {
            get
            {
                if (application == null)
                    throw new InvalidOperationException("The application is not available.");
                return application;
            }
        }

        /// <summary>
        /// Gets the screen repository for the application.
        /// </summary>
        public ScreenRepository ScreenRepository
        {
            get
            {
                if (screenRepository == null)
                    throw new InvalidOperationException("The screen repository is not available.");
                return screenRepository;
            }
        }

        /// <summary>
        /// Sets up automatic screen recording.
        /// </summary>
        [SetUp]
        public virtual void SetUpWhite()
        {
            Capture.SetCaptionFontSize(24);
            Capture.SetCaptionAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            Capture.AutoEmbedRecording(TriggerEvent.TestFailedOrInconclusive, "Screen Recording",
                new CaptureParameters() { Zoom = 0.25 }, 5);

            application = LaunchApplication();
            screenRepository = new ScreenRepository(application);
        }

        /// <summary>
        /// Sets up automatic screen recording.
        /// </summary>
        [TearDown]
        public virtual void TearDownWhite()
        {
            screenRepository = null;

            if (application != null)
                application.Kill();
        }

        /// <summary>
        /// Launches the application.
        /// </summary>
        /// <returns>The application that was launched.</returns>
        public abstract Application LaunchApplication();
    }
}
