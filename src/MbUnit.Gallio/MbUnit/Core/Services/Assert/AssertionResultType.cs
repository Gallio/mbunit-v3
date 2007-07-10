using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Assert
{
    /// <summary>
    /// Describes the result of an assertion evaluation.
    /// Each value is associated with a given outcome of an assertion evaluation.
    /// </summary>
    /// <remarks>
    /// The types are ordered according to certainty of failure.  For instance,
    /// if an assertion was skipped, less can be said about it than if it were
    /// ignored or inconclusive because it was not evaluated.  An error is certainly
    /// not a successful assertion but it is less conclusive than outright failure since
    /// it may have been caused by a programming error.
    /// 
    /// If the order of the enum constants if changed, take care to ensure that
    /// codes less than or equal to success correspond to all "ok" conditions.
    /// </remarks>
    [XmlType]
    public enum AssertionResultType
    {
        /// <summary>
        /// The assertion was not evaluated.
        /// The assertion result message should explain why the assertion was skipped.
        /// </summary>
        /// <remarks>
        /// This result type is intended to be used when blocks of assertions or entire tests are skipped.
        /// </remarks>
        [XmlEnum("skip")]
        Skip = 0,

        /// <summary>
        /// The assertion was evaluated but its result was ignored during verification.
        /// The assertion result message should explain why the assertion was ignored.
        /// </summary>
        /// <remarks>
        /// This result type is intended to be used when blocks of assertions are
        /// collected then ignored.  The inner result array the ignore assertion result should
        /// contain the actual assertion results.
        /// </remarks>
        [XmlEnum("ignore")]
        Ignore = 1,

        /// <summary>
        /// The assertion could not be evaluated with certainty.
        /// The assertion result message should provide information about why the
        /// assertion was inconclusive, such as a warning about a violated
        /// environmental invariant.
        /// </summary>
        [XmlEnum("inconclusive")]
        Inconclusive = 2,

        /// <summary>
        /// The assertion succeeded.
        /// The assertion result message is generally blank but may provide
        /// additional information about why the assertion was considered a success.
        /// </summary>
        [XmlEnum("success")]
        Success = 3,

        /// <summary>
        /// The assertion could not be evaluated due to an error or exception.
        /// The assertion result message should explain what error occurred.  If the
        /// error was due to an exception, it should also be attached to the result.
        /// </summary>
        [XmlEnum("error")]
        Error = 4,

        /// <summary>
        /// The assertion was evaluated and shown to have failed.
        /// The assertion result message should explain why the assertion failed.
        /// </summary>
        [XmlEnum("failure")]
        Failure = 5
    }
}
