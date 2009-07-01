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
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Gallio.Common.Security
{
    /// <summary>
    /// Impersonate a user according to the specified credentials.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Ref. <a href="http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158#4">Microsoft Help and Support</a>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// using (Impersonation("User", "Domain", "Pa$$w0rd"))
    /// {
    ///     // Some code to run as the specified user...
    /// }
    /// ]]></code>
    /// </example>
    public class Impersonation : IDisposable
	{
        private WindowsImpersonationContext impersonationContext;
        
        /// <summary>
        /// Starts the impersonation process. 
		/// </summary>
		/// <param name="userName">The name of the user to impersonate.</param>
        /// <param name="domain">The domain where the user is defined.</param>
		/// <param name="password">The password.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="userName"/>, <paramref name="domain"/>, 
        /// or <paramref name="password"/> is null.</exception>
        /// <exception cref="Win32Exception">Thrown when the impersonation process has failed.</exception>
		public Impersonation(string userName, string domain, string password)
		{
            if (userName == null)
                throw new ArgumentNullException("userName");
            if (domain == null)
                throw new ArgumentNullException("domain");
            if (password == null)
                throw new ArgumentNullException("password");

            Start(userName, domain, password);
		}

        private void Start(string userName, string domain, string password)
        {
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

            try
            {
                if (NativeMethods.RevertToSelf() &&
                    NativeMethods.LogonUser(userName, domain, password, 2, 0, ref token) != 0 &&
                    NativeMethods.DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Win32Exception exception)
            {
                throw new ImpersonationException(String.Format("Cannot impersonate the specified user ({0})", exception.Message), exception);
            }
			finally
			{
                if (token != IntPtr.Zero)
                    NativeMethods.CloseHandle(token);

                if (tokenDuplicate != IntPtr.Zero)
                    NativeMethods.CloseHandle(tokenDuplicate);
            }
		}

        /// <summary>
        ///  Stops the impersonation process.
        /// </summary>
        public void Dispose()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
                impersonationContext = null;
            }
        }
	}
}
