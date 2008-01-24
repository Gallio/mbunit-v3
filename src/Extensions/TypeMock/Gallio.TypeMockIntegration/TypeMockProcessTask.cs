using System;
using System.Diagnostics;
using Gallio.Concurrency;
using TypeMock.Integration;

namespace Gallio.TypeMockIntegration
{
    /// <summary>
    /// A <see cref="ProcessTask" /> that uses the TypeMock integration
    /// <see cref="TypeMockProcess" /> feature to launch hosting process
    /// with TypeMock attached.
    /// </summary>
    public class TypeMockProcessTask : ProcessTask
    {
        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public TypeMockProcessTask(string executablePath, string arguments)
            : base(executablePath, arguments)
        {
        }
        
        /// <inheritdoc />
        protected override Process StartProcess(ProcessStartInfo startInfo)
        {
            return TypeMockProcess.Start(startInfo);
        }
    }
}
