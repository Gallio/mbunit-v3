using System;

namespace MbUnit.Icarus.Controls.Enums
{
    [FlagsAttribute]
    public enum CheckBoxState
    {
        Unchecked = 1,
        Checked = 2,
        Indeterminate = CheckBoxState.Unchecked | CheckBoxState.Checked
    }
}
