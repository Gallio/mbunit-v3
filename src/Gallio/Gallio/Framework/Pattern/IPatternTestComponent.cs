using System;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// An interface shared by <see cref="PatternTest" /> and <see cref="PatternTestParameter" />.
    /// </summary>
    public interface IPatternTestComponent : ITestComponent
    {
        /// <summary>
        /// Gets the data context of the component.
        /// </summary>
        PatternTestDataContext DataContext { get; }

        /// <summary>
        /// Sets the name of the component.
        /// </summary>
        void SetName(string value);
    }
}
