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
using System.IO;
using System.Text;

namespace MbUnit.Core.ConsoleSupport
{
    /// <summary>
    /// <para>
    /// A rich console provides a number of services that enable a console to be
    /// shared among several cooperating tasks that are independently updating
    /// different portions of the display.  One task can be writing to the main
    /// body of text while another updates a footer.
    /// </para>
    /// <para>
    /// The rich console also supports intelligent cancelation of tasks.  When
    /// the user presses control-C, a cancelation flag is set and an event handler
    /// is invoked.  Tasks can check for the presence of the cancelation flag even if
    /// they started execution after the control-C itself occurred.  This provides
    /// protection against dropped cancelation requests due to race conditions.
    /// If the user presses control-C 3 times within a short interval
    /// the rich console assumes that the program has become non-responsive and
    /// terminates it.
    /// </para>
    /// <para>
    /// Finally, the rich console interface can be mocked for testing purposes
    /// unlike the standard <see cref="Console" /> API.
    /// </para>
    /// </summary>
    public interface IRichConsole
    {
        /// <summary>
        /// Gets a synchronization object that a task can lock to ensure
        /// that it is the only thread currently accessing the console.
        /// </summary>
        object SyncRoot
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether the cancelation function is enabled.
        /// If false, cancelation events will not be sent.
        /// </summary>
        bool IsCancelationEnabled { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets whether cancelation has occurred.
        /// </para>
        /// <para>
        /// The <see cref="NativeConsole.Cancel" /> event handlers will be called
        /// when the value of <see cref="NativeConsole.IsCanceled" /> transitions from
        /// false to true.  The value will remain true unless it is reset.
        /// </para>
        /// </summary>
        bool IsCanceled
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if the console is being redirected and therefore the
        /// output should be as simple as possible.  In particular, it may not be
        /// possible to set the cursor position, console color or other properties.
        /// </summary>
        /// <remarks>
        /// Trying to set the console color while output is redirected is tolerated
        /// by the system and won't cause an error; but it is pointless.  On the other
        /// hand cursor positioning or attempting to listen to Ctrl-C will fail with an
        /// <see cref="IOException" />.
        /// </remarks>
        bool IsRedirected
        {
            get;
        }

        /// <summary>
        /// Gets the error stream writer.
        /// </summary>
        TextWriter Error
        {
            get;
        }

        /// <summary>
        /// Gets the output stream writer.
        /// </summary>
        TextWriter Out
        {
            get;
        }

        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        ConsoleColor ForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the zero-based column index the cursor is located at.
        /// </summary>
        /// <exception cref="IOException">Thrown if <see cref="NativeConsole.IsRedirected" /> is true</exception>
        int CursorLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the zero-based row index the cursor is located at.
        /// </summary>
        /// <exception cref="IOException">Thrown if <see cref="NativeConsole.IsRedirected" /> is true</exception>
        int CursorTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the console's title.
        /// </summary>
        /// <exception cref="IOException">Thrown if <see cref="NativeConsole.IsRedirected" /> is true</exception>
        string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the width of the console.
        /// Returns 80 if the console is being redirected.
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether the footer is visible.
        /// </summary>
        bool FooterVisible
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// The event raised when console cancelation occurs.
        /// </para>
        /// <para>
        /// If the console cancelation signal is already set when
        /// an event handler is added, the event handler will be
        /// automatically invoked.
        /// </para>
        /// </summary>
        event EventHandler Cancel;

        /// <summary>
        /// Resets the console colors.
        /// </summary>
        void ResetColor();

        /// <summary>
        /// Sets a pair of delegates that together display a footer at the bottom
        /// of the console.  The footer can be hidden so that new text can be written
        /// from that point.  Removes the previous footer and displays the new
        /// one automatically if the footer is visible.
        /// </summary>
        /// <param name="showFooter">A delegate to display the footer</param>
        /// <param name="hideFooter">A delegate to hide the footer, leaving the custor at
        /// the beginning of the line where the footer used to begin</param>
        void SetFooter(Block showFooter, Block hideFooter);

        /// <summary>
        /// Writes a character.
        /// </summary>
        /// <param name="c">The character to write</param>
        void Write(char c);

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="str">The string to write</param>
        void Write(string str);

        /// <summary>
        /// Writes a new line.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a string followed by a new line.
        /// </summary>
        /// <param name="str">The string to write</param>
        void WriteLine(string str);
    }
}
