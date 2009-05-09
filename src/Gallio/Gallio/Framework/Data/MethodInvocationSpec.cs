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
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// Encapsulates a specification for invoking a method given values for
    /// its generic parameters and formal parameters.
    /// </para>
    /// </summary>
    public sealed class MethodInvocationSpec : DataBindingSpec
    {
        private readonly IMethodInfo method;

        private readonly Type resolvedType;
        private MethodInfo resolvedMethod;
        private Type[] resolvedGenericArguments;
        private object[] resolvedArguments;

        /// <summary>
        /// Creates a new method specification.
        /// </summary>
        /// <param name="resolvedType">The non-generic type or generic type instantiation
        /// that declares the method to be invoked or is a subtype of the declaring type.
        /// This parameter is used to resolve the method to its declaring type.</param>
        /// <param name="method">The method or generic method definition to be instantiated</param>
        /// <param name="slotValues">The slot values</param>
        /// <param name="converter">The converter to use for converting slot values
        /// to the required types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="resolvedType"/>,
        /// <paramref name="method"/>, <paramref name="slotValues"/> or <paramref name="converter"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="slotValues" /> contains
        /// slots that are declared by different methods or have incompatible values</exception>
        public MethodInvocationSpec(Type resolvedType, IMethodInfo method,
            IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues, IConverter converter)
            : base(slotValues, converter)
        {
            if (resolvedType == null)
                throw new ArgumentNullException("resolvedType");
            if (method == null)
                throw new ArgumentNullException("method");
            ValidateSlots(method, slotValues);

            this.resolvedType = resolvedType;
            this.method = method;

            ResolveMethod();
            ResolveArguments();
        }

        /// <summary>
        /// Gets the method or generic method definition to be invoked.
        /// </summary>
        public IMethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Gets the resolved type that declares the method.
        /// </summary>
        public Type ResolvedType
        {
            get { return resolvedType; }
        }

        /// <summary>
        /// Gets the resolved method given any generic method arguments that may have
        /// been provided as slot values.
        /// </summary>
        public MethodInfo ResolvedMethod
        {
            get { return resolvedMethod; }
        }

        /// <summary>
        /// <para>
        /// Gets the resolved method arguments.
        /// </para>
        /// <para>
        /// The values have already been converted to appropriate types for invoking the method.
        /// </para> 
        /// </summary>
        public object[] ResolvedArguments
        {
            get { return resolvedArguments; }
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="obj">The object on which to invoke the method.  This value is ignored
        /// if the method is static.</param>
        /// <returns>The method result value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is
        /// null but the method is non-static</exception>
        /// <exception cref="Exception">Any exception thrown by the invoked method</exception>
        [DebuggerStepThrough, DebuggerHidden]
        public object Invoke(object obj)
        {
            if (obj == null && !resolvedMethod.IsStatic)
                throw new ArgumentNullException("obj", "The object must not be null if the method is non-static.");

            return ExceptionUtils.InvokeMethodWithoutTargetInvocationException(resolvedMethod, obj, resolvedArguments);
        }

        /// <inheritdoc />
        protected override string FormatImpl(string entity, IFormatter formatter)
        {
            StringBuilder str = new StringBuilder(entity);
            AppendFormattedGenericArguments(str, resolvedGenericArguments, formatter);
            AppendFormattedMethodArguments(str, resolvedArguments, formatter);
            return str.ToString();
        }

        private void ResolveMethod()
        {
            int genericParameterCount = method.IsGenericMethodDefinition ? method.GenericArguments.Count : 0;
            resolvedGenericArguments = new Type[genericParameterCount];

            int seen = 0;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IGenericParameterInfo genericParameter = slotValue.Key as IGenericParameterInfo;
                if (genericParameter != null)
                {
                    resolvedGenericArguments[genericParameter.Position] = (Type)Converter.Convert(slotValue.Value, typeof(Type));
                    seen += 1;
                }
            }

            if (genericParameterCount != seen)
                throw new ArgumentException(String.Format("The method has {0} generic parameters but the bindings only provide values for {1} of them.",
                    genericParameterCount, seen));

            resolvedMethod = ResolveMember(resolvedType, method.Resolve(true));
            if (genericParameterCount != 0)
                resolvedMethod = resolvedMethod.MakeGenericMethod(resolvedGenericArguments);
        }

        private void ResolveArguments()
        {
            ParameterInfo[] resolvedParameters = resolvedMethod.GetParameters();
            resolvedArguments = new object[resolvedParameters.Length];

            int seen = 0;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IParameterInfo parameter = slotValue.Key as IParameterInfo;
                if (parameter != null)
                {
                    int position = parameter.Position;
                    resolvedArguments[position] = Converter.Convert(slotValue.Value, resolvedParameters[position].ParameterType);
                    seen += 1;
                }
            }

            if (resolvedParameters.Length != seen)
                throw new ArgumentException(String.Format("The method has {0} parameters but the bindings only provide values for {1} of them.",
                    resolvedParameters.Length, seen));
        }

        private static void ValidateSlots(IMethodInfo method, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                        IGenericParameterInfo genericParameter = (IGenericParameterInfo)slot;
                        if (method.Equals(genericParameter.DeclaringMethod))
                            continue;
                        break;

                    case CodeElementKind.Parameter:
                        IParameterInfo parameter = (IParameterInfo)slot;
                        if (method.Equals(parameter.Member))
                            continue;
                        break;
                }

                throw new ArgumentException(String.Format("Slot '{0}' is not valid for invoking method '{1}'.", slot, method), "slotValues");
            }
        }
    }
}
