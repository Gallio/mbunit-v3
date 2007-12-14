using Gallio.Model;
using System.Management.Automation;

namespace Gallio.PowerShellCmdlet
{
    /// <summary>
    /// A PowerShell Cmdlet for running Gallio.
    /// </summary>
    /// <todo>Implement</todo>
    [Cmdlet("Run", "Gallio")]
    public class GallioCmdlet : PSCmdlet
    {
        #region Private Members

        private string filter;

        #endregion

        #region Public Properties

        /// <summary>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/summary/*' />
        /// </summary>
        /// <remarks>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/remarks/*' />
        /// </remarks>
        /// <example>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/example/*' />
        /// </example>
        [Alias("F")]
        [Parameter]
        public string Filter
        {
            set { filter = value; }
        }

        #endregion
    }
}
