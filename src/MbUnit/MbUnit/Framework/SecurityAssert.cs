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

// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux


using System.Security.Principal;

namespace MbUnit.Framework
{
    /// <summary>
	/// Security Assertion class
	/// </summary>
	public sealed class SecurityAssert
	{
		private SecurityAssert(){}
		
		#region Authentication and Identity related
		/// <summary>
		/// Asserts that <paramref name="identity"/> is authenticated.
		/// </summary>
		public static void IsAuthenticated(IIdentity identity)
		{
			Assert.IsNotNull(identity);
			Assert.IsTrue(identity.IsAuthenticated, 
			              "Identity {0} not authentitcated",
			              identity.Name);			
		}

		/// <summary>
		/// Asserts that <paramref name="identity"/> is not authenticated.
		/// </summary>		
		public static void IsNotAuthenticated(IIdentity identity)
		{
			Assert.IsNotNull(identity);
			Assert.IsFalse(identity.IsAuthenticated, 
			              "Identity {0} authentitcated",
			              identity.Name);						
		}
		
		/// <summary>
		/// Asserts that the current windows identity is authenticated.
		/// </summary>		
		public static void WindowIsAuthenticated()
		{
			IsAuthenticated(WindowsIdentity.GetCurrent());
		}

		/// <summary>
		/// Asserts that the current windows identity is not authenticated.
		/// </summary>				
		public static void WindowIsNotAuthenticated()
		{
			IsNotAuthenticated(WindowsIdentity.GetCurrent());			
		}	
		
		/// <summary>
		/// Asserts that the current windows identity is in <param name="role"/>.
		/// </summary>				
		public static void WindowsIsInRole(WindowsBuiltInRole role)
		{
			WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			Assert.IsTrue(
				principal.IsInRole(WindowsBuiltInRole.Administrator),
				"User {0} is not in role {0}",
				principal.Identity.Name,
				role
				);
		}

		/// <summary>
		/// Asserts that the current windows identity is in 
		/// <see cref="WindowsBuiltInRole.Administrator"/> role.
		/// </summary>						
		public static void WindowsIsInAdministrator()
		{
			WindowsIsInRole(WindowsBuiltInRole.Administrator);
		}
		
		/// <summary>
		/// Asserts that the current windows identity is in 
		/// <see cref="WindowsBuiltInRole.Guest"/> role.
		/// </summary>								
		public static void WindowsIsInGuest()
		{
			WindowsIsInRole(WindowsBuiltInRole.Guest);
		}
		
		/// <summary>
		/// Asserts that the current windows identity is in 
		/// <see cref="WindowsBuiltInRole.PowerUser"/> role.
		/// </summary>								
		public static void WindowsIsInPowerUser()
		{
			WindowsIsInRole(WindowsBuiltInRole.PowerUser);
		}		
		
		/// <summary>
		/// Asserts that the current windows identity is in 
		/// <see cref="WindowsBuiltInRole.User"/> role.
		/// </summary>								
		public static void WindowsIsInUser()
		{
			WindowsIsInRole(WindowsBuiltInRole.User);
		}
		#endregion 
		
		#region SQL Injection
		// http://sqljunkies.com/WebLog/rhurlbut/archive/2003/09/28/243.aspx
		
		#endregion
		
		#region Buffer overrun
		
		#endregion
	}
}
