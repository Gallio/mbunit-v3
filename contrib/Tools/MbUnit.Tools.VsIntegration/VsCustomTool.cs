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
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MbUnit.Tools.VsIntegration
{
    /// <summary>
    /// Provides support for integrating custom tools in Visual Studio for
    /// code generation.
    /// </summary>
    /// <remarks>
    /// This class is derived from Microsoft's BaseCodeGeneratorWithSite sample
    /// on http://www.gotdotnet.com/ and the work of others in the community
    /// including Ian Griffiths, Tony Juricic and Chris Sells.  It has been
    /// repackaged here for ease of use.
    /// </remarks>
    [ComVisible(true)]
    public abstract class VsCustomTool : IVsSingleFileGenerator, IObjectWithSite 
	{
		private object site;

        /// <summary>
        /// Gets the site of the custom tool.
        /// </summary>
        protected object Site
        {
            get { return site; }
        }

        /// <summary>
        /// Generates code from a source file.
        /// </summary>
        /// <param name="fileName">The name of the source file</param>
        /// <param name="fileContents">The contents of the source file as a string</param>
        /// <param name="defaultNamespace">The default namespace</param>
        /// <returns>The generated contents as a byte array</returns>
		public abstract byte[] GenerateCode(string fileName, string fileContents, string defaultNamespace);

        /// <summary>
        /// Gets the default extension of the generated file.
        /// </summary>
		public abstract string DefaultExtension { get; }

        #region Registration
        /// <summary>
        /// Registers a custom tool with Visual Studio.
        /// The custom tool should have at least one <see cref="VsCustomToolRegistrationAttribute" />.
        /// </summary>
        /// <param name="customToolType">The custom tool type</param>
        [ComRegisterFunction]
        public static void Register(Type customToolType)
        {
            Guid customToolGuid = Marshal.GenerateGuidForType(customToolType);

            foreach (VsCustomToolRegistrationAttribute regAttribute in customToolType.GetCustomAttributes(typeof(VsCustomToolRegistrationAttribute), true))
            {
                foreach (string vsVersion in regAttribute.VsVersions)
                {
                    foreach (string vsCategoryGuid in regAttribute.VsCategoryGuids)
                    {
                        RegisterCustomTool(customToolGuid, regAttribute.Name, regAttribute.Description,
                            vsCategoryGuid, vsVersion);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters a custom tool with Visual Studio.
        /// The custom tool should have at least one <see cref="VsCustomToolRegistrationAttribute" />.
        /// </summary>
        /// <param name="customToolType">The custom tool type</param>
        [ComUnregisterFunction]
        public static void Unregister(Type customToolType)
        {
            Guid customToolGuid = Marshal.GenerateGuidForType(customToolType);

            foreach (VsCustomToolRegistrationAttribute regAttribute in customToolType.GetCustomAttributes(typeof(VsCustomToolRegistrationAttribute), true))
            {
                foreach (string vsVersion in regAttribute.VsVersions)
                {
                    foreach (string vsCategoryGuid in regAttribute.VsCategoryGuids)
                    {
                        UnregisterCustomTool(regAttribute.Name, vsCategoryGuid, vsVersion);
                    }
                }
            }
        }

        private static string GetKeyName(string customToolName, string vsCategoryGuid, string vsVersion)
        {
            return @"SOFTWARE\Microsoft\VisualStudio\" + vsVersion + @"\Generators\{" + vsCategoryGuid + @"}\" + customToolName;
        }

        private static void RegisterCustomTool(Guid customToolGuid, string customToolName, string customToolDescription,
            string vsCategoryGuid, string vsVersion)
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(GetKeyName(customToolName, vsCategoryGuid, vsVersion)))
            {
                key.SetValue(@"", customToolDescription);
                key.SetValue(@"CLSID", @"{" + customToolGuid + @"}");
                key.SetValue(@"GeneratesDesignTimeSource", 1);
            }
        }

        private static void UnregisterCustomTool(string customToolName, string vsCategoryGuid, string vsVersion)
        {
            Registry.LocalMachine.DeleteSubKey(GetKeyName(customToolName, vsCategoryGuid, vsVersion), false);
        }
        #endregion

        #region COM Interface Implementation
        string IVsSingleFileGenerator.GetDefaultExtension() 
		{
			return DefaultExtension;
		}

        void IVsSingleFileGenerator.Generate(string wszInputFilePath,
            string bstrInputFileContents, string wszDefaultNamespace,
            out IntPtr pbstrOutputFileContents, out int pbstrOutputFileContentsSize,
            IVsGeneratorProgress pGenerateProgress) 
		{
            if (wszInputFilePath == null)
                throw new ArgumentNullException(@"wszInputFilePath");
            if (bstrInputFileContents == null)
                throw new ArgumentNullException(@"bstrInputFileContents");
            if (wszDefaultNamespace == null)
                throw new ArgumentNullException(@"wszDefaultNamespace");

            try
            {
                byte[] codeBytes = GenerateCode(wszInputFilePath, bstrInputFileContents, wszDefaultNamespace);
                int len = codeBytes.Length;

                pbstrOutputFileContents = Marshal.AllocCoTaskMem(len);
                pbstrOutputFileContentsSize = len;
                Marshal.Copy(codeBytes, 0, pbstrOutputFileContents, len);
            }
            catch (Exception)
            {
                pbstrOutputFileContents = new IntPtr();
                pbstrOutputFileContentsSize = 0;
                throw;
            }
		}

		void IObjectWithSite.SetSite(object pUnkSite) 
		{
			site = pUnkSite;
		}

		void IObjectWithSite.GetSite(ref Guid riid, IntPtr ppvSite) 
		{
			const int E_FAIL = unchecked((int)0x80004005);
			const int E_POINTER = unchecked((int)0x80000005);
			const int E_NOINTERFACE = unchecked((int)0x80000004);

			// No where to put the resulting interface
			if (ppvSite == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(E_POINTER);
			Marshal.WriteIntPtr(ppvSite, IntPtr.Zero);

			// No site
			if (site == null)
                Marshal.ThrowExceptionForHR(E_FAIL);

			// QI
			IntPtr pUnk = Marshal.GetIUnknownForObject(site);
			IntPtr pvSite;
			int hr = Marshal.QueryInterface(pUnk, ref riid, out pvSite);
			Marshal.Release(pUnk);

			// No matching interface on site
			if ((hr < 0) || (pvSite == IntPtr.Zero))
                Marshal.ThrowExceptionForHR(E_NOINTERFACE);

			// Return requested interface
			Marshal.WriteIntPtr(ppvSite, pvSite);
        }
        #endregion
    }
}
