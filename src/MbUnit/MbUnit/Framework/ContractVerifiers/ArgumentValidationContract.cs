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
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Core;
using Gallio.Common;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the argument validation of a method/constructor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>x</strong> : y.
    /// </item>
    /// <item>
    /// <strong>x</strong> : y.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Example
    /// <code><![CDATA[
    /// // TODO...
    /// ]]></code>
    /// </example>
    /// <typeparam name="TTarget"></typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class ArgumentValidationContract<TTarget>
    {
        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1,TTarget> For<T1>(Func<T1, TTarget> constructor)
        {
            var options = new ArgumentValidationContractOptions<T1, TTarget>();
            return options.For(constructor);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, TTarget> For<T1>(Action<TTarget, T1> method)
        {
            var options = new ArgumentValidationContractOptions<T1, TTarget>();
            return options.For(method);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, TTarget> For<T1, T2>(Func<T1, T2, TTarget> constructor)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, TTarget>();
            return options.For(constructor);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, TTarget> For<T1, T2>(Action<TTarget, T1, T2> method)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, TTarget>();
            return options.For(method);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, T3, TTarget> For<T1, T2, T3>(Func<T1, T2, T3, TTarget> constructor)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, T3, TTarget>();
            return options.For(constructor);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, T3, TTarget> For<T1, T2, T3>(Action<TTarget, T1, T2, T3> method)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, T3, TTarget>();
            return options.For(method);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> For<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TTarget> constructor)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, T3, T4, TTarget>();
            return options.For(constructor);
        }

        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> For<T1, T2, T3, T4>(Action<TTarget, T1, T2, T3, T4> method)
        {
            var options = new ArgumentValidationContractOptions<T1, T2, T3, T4, TTarget>();
            return options.For(method);
        }

        private ArgumentValidationContract()
        {
        }
    }
}



