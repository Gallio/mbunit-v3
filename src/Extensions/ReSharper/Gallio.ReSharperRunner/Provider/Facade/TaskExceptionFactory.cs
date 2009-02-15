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
using System.Reflection;
using System.Security.Permissions;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
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