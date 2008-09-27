using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Concurrency
{
    /// <summary>
    /// An action that reads the contents of an object.  It should not modify the object in any way.
    /// </summary>
    /// <param name="obj">The object</param>
    /// <typeparam name="T">The type of object</typeparam>
    public delegate void ReadAction<T>(T obj);
}
