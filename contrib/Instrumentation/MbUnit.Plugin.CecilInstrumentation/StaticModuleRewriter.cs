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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    internal class StaticModuleRewriter
    {
        private readonly ModuleDefinition moduleDefinition;

        private TypeReference objectReference;
        private TypeReference objectArrayReference;
        private TypeReference staticInterceptorStubReference;
        private MethodReference executeReference;
        private FieldReference noArgumentsReference;
        private MethodReference staticInvocationConstructorReference;

        public StaticModuleRewriter(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        private void ImportReferences()
        {
            if (objectReference != null)
                return;

            objectReference = moduleDefinition.Import(typeof(object));
            objectArrayReference = moduleDefinition.Import(typeof(object[]));
            staticInterceptorStubReference = moduleDefinition.Import(typeof(StaticInterceptorStub));
            executeReference = moduleDefinition.Import(typeof(StaticInvocation).GetMethod(@"Execute"));
            noArgumentsReference = moduleDefinition.Import(typeof(StaticInvocation).GetField(@"NoArguments"));
            staticInvocationConstructorReference = moduleDefinition.Import(typeof(StaticInvocation).GetConstructor(
                new Type[] { typeof(StaticInterceptorStub), typeof(object), typeof(object[]) }));
        }

        public void Rewrite()
        {
            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
            {
                if (typeDefinition.Name != @"<Module>")
                    Rewrite(typeDefinition);
            }
        }

        private void Rewrite(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsInterface)
                return;

            if (typeDefinition.Methods.Count != 0)
            {
                List<MethodDefinition> addedMethods = new List<MethodDefinition>();

                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                    Rewrite(typeDefinition, methodDefinition, addedMethods);

                foreach (MethodDefinition addedMethod in addedMethods)
                    typeDefinition.Methods.Add(addedMethod);
            }

            foreach (TypeDefinition nestedTypeDefinition in typeDefinition.NestedTypes)
                Rewrite(nestedTypeDefinition);
        }

        private void Rewrite(TypeDefinition typeDefinition, MethodDefinition methodDefinition, List<MethodDefinition> addedMethods)
        {
            if (!methodDefinition.HasBody)
                return;

            ImportReferences();

            // Create an interceptor stub for the method.
            string stubFieldName = StaticInterceptorStub.GetStubFieldName(methodDefinition.Name);
            FieldDefinition stubFieldDefinition = new FieldDefinition(stubFieldName, staticInterceptorStubReference,
                FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.NotSerialized);
            typeDefinition.Fields.Add(stubFieldDefinition);

            // Clone the original method and give it a new name.
            MethodDefinition targetMethodDefinition = methodDefinition.Clone();
            targetMethodDefinition.Overrides.Clear();
            targetMethodDefinition.Attributes = (methodDefinition.Attributes & MethodAttributes.Static)
                | MethodAttributes.Private | MethodAttributes.HideBySig;
            targetMethodDefinition.CallingConvention = MethodCallingConvention.Default;
            targetMethodDefinition.Name = StaticInterceptorStub.GetTargetMethodName(methodDefinition.Name);

            addedMethods.Add(targetMethodDefinition);

            // Replace the original method with a stub that calls the callback.
            MethodBody body = new MethodBody(methodDefinition);
            body.InitLocals = true;
            methodDefinition.Body = body;
            CilWorker worker = body.CilWorker;

            /*** Obtain the invocation, if needed ***/
            // Load the stub if it has been initialized.
            VariableDefinition stubVariableDefinition = new VariableDefinition(staticInterceptorStubReference);
            body.Variables.Add(stubVariableDefinition);

            worker.Emit(OpCodes.Ldsfld, stubFieldDefinition);
            worker.Emit(OpCodes.Dup);
            worker.Emit(OpCodes.Stloc, stubVariableDefinition);
            Instruction fastPathEntryBranch = worker.Emit(OpCodes.Brfalse, body.Instructions.Outside);

            /*** Slow path ***/
            // Copy arguments to an array.
            VariableDefinition argumentsVariableDefinition = null;

            if (methodDefinition.Parameters.Count != 0)
            {
                argumentsVariableDefinition = new VariableDefinition(objectArrayReference);
                body.Variables.Add(argumentsVariableDefinition);

                worker.Emit(OpCodes.Ldc_I4, methodDefinition.Parameters.Count);
                worker.Emit(OpCodes.Newarr, objectReference);
                worker.Emit(OpCodes.Stloc, argumentsVariableDefinition);

                foreach (ParameterDefinition param in methodDefinition.Parameters)
                {
                    if ((param.Attributes & ParameterAttributes.In) != 0)
                    {
                        worker.Emit(OpCodes.Ldloc, argumentsVariableDefinition);
                        worker.Emit(OpCodes.Ldc_I4, param.Sequence);
                        worker.Emit(OpCodes.Ldarg, param);
                        worker.Emit(OpCodes.Box, objectReference);
                        worker.Emit(OpCodes.Stelem_Ref);
                    }
                }
            }

            // Create the invocation.
            worker.Emit(OpCodes.Ldloc, stubVariableDefinition);
            if (methodDefinition.HasThis)
                worker.Emit(OpCodes.Ldarg_0);
            else
                worker.Emit(OpCodes.Ldnull);
            if (argumentsVariableDefinition == null)
                worker.Emit(OpCodes.Ldsfld, noArgumentsReference);
            else
                worker.Emit(OpCodes.Ldloc, argumentsVariableDefinition);
            worker.Emit(OpCodes.Newobj, staticInvocationConstructorReference);

            // Execute it (leaves the result on the stack).
            worker.Emit(OpCodes.Call, executeReference);

            // Copy any ref and out arguments back out of the invocation's array.
            if (argumentsVariableDefinition != null)
            {
                foreach (ParameterDefinition param in methodDefinition.Parameters)
                {
                    if ((param.Attributes & ParameterAttributes.Out) != 0)
                    {
                        worker.Emit(OpCodes.Ldloc, argumentsVariableDefinition);
                        worker.Emit(OpCodes.Ldc_I4, param.Sequence);
                        worker.Emit(OpCodes.Ldelem_Ref);
                        worker.Emit(OpCodes.Unbox_Any, param.ParameterType);
                        worker.Emit(OpCodes.Starg, param);
                    }
                }
            }

            // Unbox the result if needed and return.
            if (CecilUtils.IsVoid(methodDefinition.ReturnType.ReturnType))
                worker.Emit(OpCodes.Pop);
            else
                worker.Emit(OpCodes.Unbox_Any, methodDefinition.ReturnType.ReturnType);

            Instruction slowPathReturnBranch = worker.Emit(OpCodes.Br, body.Instructions.Outside);

            /*** Fast path ***/
            // Load up all arguments.
            Instruction fastPathEntryInstr = null;

            if (methodDefinition.HasThis)
                fastPathEntryInstr = worker.Emit(OpCodes.Ldarg, methodDefinition.This);
            foreach (ParameterDefinition param in methodDefinition.Parameters)
            {
                Instruction instr = worker.Emit(OpCodes.Ldarg, param);
                if (fastPathEntryInstr == null)
                    fastPathEntryInstr = instr;
            }

            // Emit a tail call back to the original method for the fast-path without an invocation.
            worker.Emit(OpCodes.Tail);
            worker.Emit(OpCodes.Call, targetMethodDefinition);

            /*** Common return ***/
            Instruction returnInstr = worker.Emit(OpCodes.Ret);

            // Patch branches.
            fastPathEntryBranch.Operand = fastPathEntryInstr;
            slowPathReturnBranch.Operand = returnInstr;
        }
    }
}
