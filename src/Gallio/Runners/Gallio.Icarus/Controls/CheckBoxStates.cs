using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Icarus.Controls.Enums
{
    [FlagsAttribute]
    public enum CheckBoxStates
    {
        Unchecked = 1,
        Checked = 2,
        Indeterminate = Unchecked | Checked
    }
}
