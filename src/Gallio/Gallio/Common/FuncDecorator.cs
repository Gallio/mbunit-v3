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

using System;

namespace Gallio.Common
{
    /// <summary>
    /// Represents a delegate that decorates a function.
    /// </summary>
    /// <typeparam name="T">The type of object the function is performed upon.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="obj">The object to act upon.</param>
    /// <param name="func">The function to decorate which should be called in
    /// the middle of applying the decoration.</param>
    public delegate TResult FuncDecorator<T, TResult>(T obj, Func<T, TResult> func);
}