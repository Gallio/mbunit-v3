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
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Gallio.Model.Environments
{
    /// <summary>
    /// Ensures Windows Forms applications can be tested safely.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This environment disables <see cref="WindowsFormsSynchronizationContext.AutoInstall" />
    /// because it can cause problems with tests that use Windows Forms and that require COM
    /// message pumping.  Just because a thread happens to instantiate a Control does
    /// not imply that it should always be enlisted into the Windows Forms synchronization context.
    /// </para>
    /// <para>
    /// Specifically this setting change resolves an obscure hangs that can occur.  Creating
    /// a <see cref="Form" /> will auto-register the WindowsFormsSynchronizationContext.  This
    /// is fine as long as a message pump is running in the thread.  However as is typical with
    /// tests, such message pumps are short-lived so soon enough there is no pump.
    /// Now when a system event like UserPreferenceChanged occurs, the <see cref="SystemEvents" />
    /// class will try to synchronize with the registered handler using a WindowsFormsSynchronizationContext
    /// but there is no pump running.  The call hangs indefinitely.  What's more, the test
    /// runner will not be able to unload the AppDomain because it will remain stuck in
    /// native code!
    /// </para>
    /// <para>
    /// As a work-around, we disable auto-installation of the Windows Forms synchronization context.
    /// </para>
    /// <para>
    /// For more information, see the following:
    /// http://ikriv.com/en/prog/info/dotnet/MysteriousHang.html
    /// http://www.aaronlerch.com/blog/2008/12/15/debugging-ui/
    /// </para>
    /// </remarks>
    public class WindowsFormsTestEnvironment : BaseTestEnvironment
    {
        /// <inheritdoc />
        public override IDisposable SetUpThread()
        {
            return new State();
        }

        private sealed class State : IDisposable
        {
            private readonly bool oldAutoInstall;

            public State()
            {
                oldAutoInstall = WindowsFormsSynchronizationContext.AutoInstall;

                WindowsFormsSynchronizationContext.AutoInstall = false;
            }

            public void Dispose()
            {
                WindowsFormsSynchronizationContext.AutoInstall = oldAutoInstall;
            }
        }
    }
}
