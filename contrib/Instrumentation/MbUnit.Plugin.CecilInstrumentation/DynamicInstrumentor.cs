using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Castle.Core.Interceptor;
using MbUnit.Instrumentation;
using Mono.Cecil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// This instrumentor uses Mono.Cecil to parse an assembly and load it into
    /// the dynamic assembly context.  At runtime, individual methods are instrumented
    /// by selectively modifying the IL code with a <see cref="MethodRental" />.
    /// </summary>
    public class DynamicInstrumentor : IInstrumentor
    {
        /// <inheritdoc />
        public Assembly InstrumentAndLoad(string assemblyPath, string instrumentedAssemblySavePath)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException(@"assemblyPath");

            AssemblyDefinition assemblyDefinition = AssemblyFactory.GetAssembly(assemblyPath);

            DynamicAssemblyBuilder builder = new DynamicAssemblyBuilder(assemblyDefinition);

            DynamicAssembly dynamicAssembly = builder.Build(instrumentedAssemblySavePath);

            if (instrumentedAssemblySavePath != null)
                dynamicAssembly.Save();

            return dynamicAssembly.Builder;
        }

        /// <inheritdoc />
        public void AddInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");
            if (interceptor == null)
                throw new ArgumentNullException(@"interceptor");

            // FIXME: Just a quick and dirty check to verify that we can replace method bodies.
            // This doesn't actually install an interceptor but it causes the method to not run.
            // The trick for actually doing that will be to inject code at the beginning
            // of the method that looks for a thread-local "proceed" flag.  If it's not
            // set, then it should call the interceptors.  Otherwise it should clear it
            // immediately and run the method to completion.  -- Jeff.
            Byte[] methodBytes = {
                0x03,
                0x30,
                0x0A,
                0x00,
                0x01,                // code size
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x2a                // ret
            };

            // Get the pointer to the method body.
            GCHandle handle = new GCHandle();
            try
            {
                handle = GCHandle.Alloc(methodBytes, GCHandleType.Pinned);
                IntPtr addr = handle.AddrOfPinnedObject();
                MethodRental.SwapMethodBody(method.DeclaringType, method.MetadataToken, addr, methodBytes.Length, MethodRental.JitOnDemand);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        /// <inheritdoc />
        public bool RemoveInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");
            if (interceptor == null)
                throw new ArgumentNullException(@"interceptor");

            return false;
        }
    }
}
