using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core
{
    /// <summary>
    /// A delegate used to execute a block of side-effecting code with no
    /// parameters or results.
    /// </summary>
    /// <exception cref="Exception">Any exception might be thrown.</exception>
    public delegate void Block();
}