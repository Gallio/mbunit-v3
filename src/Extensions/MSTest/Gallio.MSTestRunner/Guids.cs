using System;
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.MSTestRunner
{
    internal static class Guids
    {
        public const string MSTestRunnerPkgGuidString = "9e600ffc-344d-4e6f-89c0-ded6afb42459";
        public const string MSTestRunnerCmdSetGuidString = "8433bd03-19c1-4919-b7ba-9d13bb423b41";

        public const string GallioTestTypeGuidString = "F3589083-259C-4054-87F7-75CDAD4B08E5";
        public static readonly TestType GallioTestType = new TestType(new Guid(GallioTestTypeGuidString));
    };
}