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

namespace Gallio.Common
{
    /// <summary>
    /// Represents a delegate that decorates an action.
    /// </summary>
    /// <typeparam name="T">The type of object the action is performed upon.</typeparam>
    /// <param name="obj">The object to act upon.</param>
    /// <param name="action">The action to decorate which should be called in
    /// the middle of applying the decoration.</param>
    public delegate void ActionDecorator<T>(T obj, Action<T> action);

    /// <summary>
    /// Represents a delegate that decorates an action.
    /// </summary>
    /// <typeparam name="T1">The first argument type.</typeparam>
    /// <typeparam name="T2">The second argument type.</typeparam>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="action">The action to decorate which should be called in
    /// the middle of applying the decoration.</param>
    public delegate void ActionDecorator<T1, T2>(T1 arg1, T2 arg2, Action<T1, T2> action);
}