using System.Reflection.Emit;
using System.Resources;
using Mono.Cecil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Builds resources in a dynamic assembly from Cecil definitions.
    /// </summary>
    internal sealed class DynamicResourceBuilder : BaseStructureVisitor
    {
        private readonly DynamicModule module;

        public DynamicResourceBuilder(DynamicModule module)
        {
            this.module = module;
        }

        public override void VisitAssemblyLinkedResource(AssemblyLinkedResource res)
        {
            // TODO.
        }

        public override void VisitEmbeddedResource(EmbeddedResource res)
        {
            // TODO: Description?
            IResourceWriter resourceWriter = module.Builder.DefineResource(res.Name, "",
                (System.Reflection.ResourceAttributes)res.Flags);

            resourceWriter.AddResource(res.Name, res.Data);
            resourceWriter.Generate();
        }

        public override void VisitLinkedResource(LinkedResource res)
        {
            // TODO.
        }
    }
}