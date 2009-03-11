using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model.Serialization;

namespace Gallio.Model.Messages
{
    /// <summary>
    /// A test exploration listener observes the progress of test exploration as a series of events.
    /// </summary>
    public interface ITestExplorationListener
    {
        /// <summary>
        /// Notifies the listener that a subtree of tests has been merged into the test model.
        /// </summary>
        /// <param name="parentTestId">The id of the parent test, or null if adding the root</param>
        /// <param name="test">The test at the top of the subtree was merged</param>
        void NotifySubtreeMerged(string parentTestId, TestData test);

        /// <summary>
        /// Notifies the listener that an annotation has been added to the test model.
        /// </summary>
        /// <param name="annotation">The annotation that was added</param>
        void NotifyAnnotationAdded(AnnotationData annotation);
    }
}
