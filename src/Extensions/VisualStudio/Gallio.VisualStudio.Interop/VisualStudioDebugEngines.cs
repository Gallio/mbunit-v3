using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gallio.VisualStudio.Interop
{
    internal class VisualStudioDebugEngines
    {
        /// <summary>
        /// "Silverlight"
        /// </summary>
        public static readonly Guid Silverlight = new Guid("032F4B8C-7045-4B24-ACCF-D08C9DA108FE");

        /// <summary>
        /// "T-SQL" (for SQL Server 2000)
        /// </summary>
        public static readonly Guid TSql2000 = new Guid("5AF6F83C-B555-11D1-8418-00C04FA302A1");

        /// <summary>
        /// "T-SQL" (for SQL Server 2005)
        /// </summary>
        public static readonly Guid TSql2005 = new Guid("1202F5B4-3522-4149-BAD8-58B2079D704F");

        /// <summary>
        /// "Native"
        /// </summary>
        public static readonly Guid Native = new Guid("3B476D35-A401-11D2-AAD4-00C04F990171");

        /// <summary>
        /// "Managed"
        /// </summary>
        public static readonly Guid Managed = new Guid("449EC4CC-30D2-4032-9256-EE18EB41B62B");
        
        /// <summary>
        /// "Managed (v2.0, v1.1, v1.0)"
        /// </summary>
        public static readonly Guid Managed20 = new Guid("5FFF7536-0C87-462D-8FD2-7971D948E6DC");

        /// <summary>
        /// "Managed (v4.0)"
        /// </summary>
        public static readonly Guid Managed40 = new Guid("FB0D4648-F776-4980-95F8-BB7F36EBC1EE");

        /// <summary>
        /// "Workflow"
        /// </summary>
        public static readonly Guid Workflow = new Guid("6589AE11-3B51-494A-AC77-91DA1B53F35A");

        /// <summary>
        /// "Managed/Native"
        /// </summary>
        public static readonly Guid ManagedAndNative = new Guid("92EF0900-2251-11D2-B72E-0000F87572EF");

        /// <summary>
        /// "Script"
        /// </summary>
        public static readonly Guid Script = new Guid("F200A7E7-DEA5-11D0-B854-00A0244A1DE2");
    }
}
