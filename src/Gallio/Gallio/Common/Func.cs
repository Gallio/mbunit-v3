// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common
{
    /// <summary>
    /// A function with zero arguments.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <returns>The function result.</returns>
    public delegate TResult Func<TResult>();

    /// <summary>
    /// A function with one argument.
    /// </summary>
    /// <typeparam name="T1">The first argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="arg1">The first argument.</param>
    /// <returns>The function result.</returns>
    public delegate TResult Func<T1, TResult>(T1 arg1);

    /// <summary>
    /// A function with two arguments.
    /// </summary>
    /// <typeparam name="T1">The first argument type.</typeparam>
    /// <typeparam name="T2">The second argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>The function result.</returns>
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);

    /// <summary>
    /// A function with three arguments.
    /// </summary>
    /// <typeparam name="T1">The first argument type.</typeparam>
    /// <typeparam name="T2">The second argument type.</typeparam>
    /// <typeparam name="T3">The third argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <returns>The function result.</returns>
    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// A function with four arguments.
    /// </summary>
    /// <typeparam name="T1">The first argument type.</typeparam>
    /// <typeparam name="T2">The second argument type.</typeparam>
    /// <typeparam name="T3">The third argument type.</typeparam>
    /// <typeparam name="T4">The fourth argument type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <returns>The function result.</returns>
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}