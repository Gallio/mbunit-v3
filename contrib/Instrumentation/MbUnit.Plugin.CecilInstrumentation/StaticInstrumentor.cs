// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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