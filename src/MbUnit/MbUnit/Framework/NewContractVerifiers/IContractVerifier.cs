using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework.NewContractVerifiers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContractVerifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        void DeclareChildTests(PatternEvaluationScope scope);
    }
}
