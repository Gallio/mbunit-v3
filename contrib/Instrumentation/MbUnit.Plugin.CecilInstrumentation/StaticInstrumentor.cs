using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Castle.Core.Interceptor;
using MbUnit.Instrumentation;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// This instrumentor uses Mono.Cecil to rewrite the IL within an assembly to
    /// inject static calls to an instrumentation routine before loading it.
    /// At runtime, all method calls are redirected to the instrumentor which performs
    /// a lookup for interceptors to run.
    /// </summary>
    public class StaticInstrumentor : IInstrumentor
    {
        /// <inheritdoc />
        public Assembly InstrumentAndLoad(string assemblyPath, string instrumentedAssemblySavePath)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException(@"assemblyPath");

            // TODO: Cache the instrumented assembly on disk so we can reload it quickly later
            //       if nothing has changed.
            AssemblyDefinition assemblyDefinition = AssemblyFactory.GetAssembly(assemblyPath);
            Rewrite(assemblyDefinition);

            MemoryStream stream = new MemoryStream();
            AssemblyFactory.SaveAssembly(assemblyDefinition, stream);

            if (instrumentedAssemblySavePath != null)
                AssemblyFactory.SaveAssembly(assemblyDefinition, instrumentedAssemblySavePath);

            return Assembly.Load(stream.ToArray());
        }

        /// <inheritdoc />
        public void AddInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");
            if (interceptor == null)
                throw new ArgumentNullException(@"interceptor");

            StaticInterceptorStub.AddInterceptor(method, interceptor);
        }

        /// <inheritdoc />
        public bool RemoveInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");
            if (interceptor == null)
                throw new ArgumentNullException(@"interceptor");

            return StaticInterceptorStub.RemoveInterceptor(method, interceptor);
        }

        private void Rewrite(AssemblyDefinition assemblyDefinition)
        {
            foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
            {
                Rewrite(moduleDefinition);
            }
        }

        private void Rewrite(ModuleDefinition moduleDefinition)
        {
            StaticModuleRewriter rewriter = new StaticModuleRewriter(moduleDefinition);
            rewriter.Rewrite();
        }
    }
}