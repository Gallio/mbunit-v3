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
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Gallio.Icarus.Tests.Utilities
{
    internal class EventAssert
    {
        private static readonly Dictionary<object, List<EventArgs>> events = new Dictionary<object, List<EventArgs>>();
        private static readonly Dictionary<object, List<KeyValuePair<string, Delegate>>> delegates = new Dictionary<object, List<KeyValuePair<string, Delegate>>>();

        internal static void Attach(object publisher)
        {
            List<KeyValuePair<string, Delegate>> delegateList;
            if (!delegates.TryGetValue(publisher, out delegateList))
            {
                delegateList = new List<KeyValuePair<string, Delegate>>();
                delegates.Add(publisher, delegateList);
            }
            else
            {
                throw new Exception("Already attached!");
            }

            foreach (var eventInfo in publisher.GetType().GetEvents())
            {
                var returnType = GetDelegateReturnType(eventInfo.EventHandlerType);

                if (returnType != typeof(void))
                    throw new ApplicationException("Delegate has a return type.");

                var handler = new DynamicMethod("", null, GetDelegateParameterTypes(eventInfo.EventHandlerType), 
                    typeof(EventAssert));

                // Generate a method body. This method loads a string, calls 
                // the Show method overload that takes a string, pops the 
                // return value off the stack (because the handler has no
                // return type), and returns.
                //
                ILGenerator ilgen = handler.GetILGenerator();

                // Complete the dynamic method by calling its CreateDelegate
                // method. Use the "add" accessor to add the delegate to
                // the invocation list for the event.
                //
                var dEmitted = handler.CreateDelegate(eventInfo.EventHandlerType);
                eventInfo.GetAddMethod().Invoke(publisher, new[] { dEmitted });
                delegateList.Add(new KeyValuePair<string, Delegate>(eventInfo.Name, dEmitted));
            }
        }

        internal static void Detach(object publisher)
        {
            List<KeyValuePair<string, Delegate>> delegateList;
            if (!delegates.TryGetValue(publisher, out delegateList))
                throw new Exception("Not attached!");

            foreach (var pair in delegateList)
            {
                var eventInfo = publisher.GetType().GetEvent(pair.Key);
                eventInfo.GetRemoveMethod().Invoke(publisher, new[] { pair.Value });
            }
        }

        private static Type[] GetDelegateParameterTypes(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
                throw new ApplicationException("Not a delegate.");

            MethodInfo invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            ParameterInfo[] parameters = invoke.GetParameters();
            Type[] typeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i] = parameters[i].ParameterType;
            }
            return typeParameters;
        }

        private static Type GetDelegateReturnType(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
                throw new ApplicationException("Not a delegate.");

            MethodInfo invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            return invoke.ReturnType;
        }
    }
}
