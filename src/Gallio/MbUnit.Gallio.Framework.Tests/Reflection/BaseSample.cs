using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Tests.Reflection
{
    public class BaseSample
    {
        private string _baseString = "Base var";

        private int _baseInt = 12;

        private int BaseInteger
        {
            get { return _baseInt; }
            set { _baseInt = value; }
        }

        private string Wow()
        {
            return "Wow!";
        }

        private string OhYhea(string text)
        {
            return text + " Oh, Yhea!";
        }
    }
}
