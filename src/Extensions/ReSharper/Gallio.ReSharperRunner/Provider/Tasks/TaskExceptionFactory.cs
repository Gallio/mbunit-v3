using System;
using System.Reflection;
using System.Security.Permissions;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// Works around the fact that <see cref="TaskException" />
    /// does not provide a constructor that accepts raw string parameters.
    /// </summary>
    internal static class TaskExceptionFactory
    {
        private static readonly FieldInfo typeField = typeof(TaskException).GetField("myType", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo messageField = typeof(TaskException).GetField("myMessage", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo stackTraceField = typeof(TaskException).GetField("myStackTrace", BindingFlags.NonPublic | BindingFlags.Instance);

        [ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        public static TaskException CreateTaskException(string type, string message, string stackTrace)
        {
            // Note: We explicitly use the same boxed TaskException struct for all 3 operations.
            object ex = new TaskException();
            typeField.SetValue(ex, type);
            messageField.SetValue(ex, message);
            stackTraceField.SetValue(ex, stackTrace);
            return (TaskException)ex;
        }
    }
}
