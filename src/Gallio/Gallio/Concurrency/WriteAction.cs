using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Concurrency
{
    /// <summary>
    /// An action that reads or writes the contents of an object.
    /// </summary>
    /// <param name="obj">The object</param>
    /// <typeparam name="T">The type of object</typeparam>
    public delegate void WriteAction<T>(T obj);
}
