namespace Gallio.Icarus.Controls
{
    /// <summary>
    /// Provides constant names for test progress bar styles.
    /// </summary>
    public static class TestStatusBarStyles
    {
        /// <summary>
        /// Any failures cause the whole bar to turn the fail test colour (like NUnit).
        /// </summary>
        public const string UnitTest = "UnitTest";

        /// <summary>
        /// Each section (pass/fail/skip) is kept separate (like MbUnit).
        /// </summary>
        public const string Integration = "Integration";
    }
}
