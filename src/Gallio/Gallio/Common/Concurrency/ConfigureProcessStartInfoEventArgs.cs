using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// An event dispatched by the <see cref="ProcessTask"/> when its
    /// <see cref="ProcessStartInfo" /> is being configured to enable customization.
    /// </summary>
    public class ConfigureProcessStartInfoEventArgs : EventArgs
    {
        private readonly ProcessStartInfo processStartInfo;

        /// <summary>
        /// Creates event arguments.
        /// </summary>
        /// <param name="processStartInfo">The process start info being configured</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="processStartInfo"/> is null</exception>
        public ConfigureProcessStartInfoEventArgs(ProcessStartInfo processStartInfo)
        {
            if (processStartInfo == null)
                throw new ArgumentNullException("processStartInfo");

            this.processStartInfo = processStartInfo;
        }

        /// <summary>
        /// Gets the process start info being configured.
        /// </summary>
        public ProcessStartInfo ProcessStartInfo
        {
            get { return processStartInfo; }
        }
    }
}
