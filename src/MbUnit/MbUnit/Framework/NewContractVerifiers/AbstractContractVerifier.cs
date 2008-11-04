using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework.NewContractVerifiers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractContractVerifier : IContractVerifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public abstract void DeclareChildTests(PatternEvaluationScope scope);
    }
}
