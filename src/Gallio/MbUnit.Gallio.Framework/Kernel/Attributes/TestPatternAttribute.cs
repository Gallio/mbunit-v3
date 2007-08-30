using System;
using System.Reflection;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// Generates a method template from the annotated method and sets its
    /// <see cref="ITemplate.IsGenerator" /> property to true.  Subclasses
    /// can contribute actions to the template to govern how test generation
    /// takes place.  By default, the generated tests will do nothing.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given method.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public abstract class TestPatternAttribute : MethodPatternAttribute
    {
        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
            base.Apply(builder, methodTemplate);

            methodTemplate.IsGenerator = true;
        }
    }
}