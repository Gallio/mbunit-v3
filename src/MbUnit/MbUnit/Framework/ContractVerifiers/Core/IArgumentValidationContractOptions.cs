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
using System.Text;
using Gallio.Common;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// TODO...
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IArgumentValidationContractOptions<T1, TTarget> : IContract
    {
        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="arg1"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, TTarget> ShouldThrow<TException>(T1 arg1)
            where TException : Exception;

        /// <summary>
        /// TODO...
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, TTarget> With(Func<TTarget> factory);
    }

    /// <summary>
    /// TODO...
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IArgumentValidationContractOptions<T1, T2, TTarget> : IContract
    {
        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2)
            where TException : Exception;

        /// <summary>
        /// TODO...
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, TTarget> With(Func<TTarget> factory);
    }

    /// <summary>
    /// TODO...
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IArgumentValidationContractOptions<T1, T2, T3, TTarget> : IContract
    {
        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, T3, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2, T3 arg3)
            where TException : Exception;

        /// <summary>
        /// TODO...
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, T3, TTarget> With(Func<TTarget> factory);
    }

    /// <summary>
    /// TODO...
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> : IContract
    {
        /// <summary>
        /// TODO...
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> ShouldThrow<TException>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where TException : Exception;

        /// <summary>
        /// TODO...
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IArgumentValidationContractOptions<T1, T2, T3, T4, TTarget> With(Func<TTarget> factory);
    }
}
