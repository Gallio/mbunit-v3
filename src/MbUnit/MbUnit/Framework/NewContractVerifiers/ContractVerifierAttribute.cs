using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using Gallio.Model;

namespace MbUnit.Framework.NewContractVerifiers
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ContractVerifierAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool IsPrimary
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluator"></param>
        /// <param name="codeElement"></param>
        /// <returns></returns>
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containingScope"></param>
        /// <param name="codeElement"></param>
        /// <param name="skipChildren"></param>
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            var type = ((IFieldInfo)codeElement).Resolve(true).FieldType;
            var instance = (IContractVerifier)Activator.CreateInstance(type);
            var contractTest = new PatternTest(codeElement.Name, codeElement, containingScope.TestDataContext.CreateChild());
            contractTest.IsTestCase = false;
            contractTest.Metadata.SetValue(MetadataKeys.TestKind, TestKinds.Group);
            instance.DeclareChildTests(containingScope.AddChildTest(contractTest));
        }
    }
}
