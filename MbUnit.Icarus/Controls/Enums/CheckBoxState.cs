using System;

namespace MbUnit.Icarus.Controls.Enums
{
    [FlagsAttribute]
    public enum CheckBoxStates
    {
        Unchecked = 1,
        Checked = 2,
        Indeterminate = CheckBoxStates.Unchecked | CheckBoxStates.Checked
    }
}
