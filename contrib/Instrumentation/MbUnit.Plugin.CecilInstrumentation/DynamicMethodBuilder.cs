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
using System.Reflection;
using System.Reflection.Emit;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Utilities;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Builds methods in a dynamic assembly from Cecil definitions.
    /// </summary>
    internal sealed class DynamicMethodBuilder
    {
        private readonly DynamicAssemblyBuilder outer;
        private readonly Dictionary<VariableDefinition, LocalBuilder> locals;
        private readonly Dictionary<Instruction, Block> actions;
        private readonly Dictionary<Instruction, Label> labels;

        private MethodBase method;
        private ILGenerator generator;

        public DynamicMethodBuilder(DynamicAssemblyBuilder outer)
        {
            this.outer = outer;

            locals = new Dictionary<VariableDefinition, LocalBuilder>();
            actions = new Dictionary<Instruction, Block>();
            labels = new Dictionary<Instruction, Label>();
        }

        public void BuildIL(MethodBase method, ILGenerator generator, Mono.Cecil.Cil.MethodBody body)
        {
            try
            {
                this.method = method;
                this.generator = generator;

                DeclareLocals(body.Variables);
                MarkExceptionHandlers(body.ExceptionHandlers);
                CreateLabels(body);

                EmitInstructions(body);
            }
            finally
            {
                this.method = null;
                this.generator = null;
                locals.Clear();
                actions.Clear();
                labels.Clear();
            }
        }

        private void DeclareLocals(VariableDefinitionCollection variableDefinitions)
        {
            foreach (VariableDefinition variableDefinition in variableDefinitions)
            {
                Type type = outer.ResolveType(variableDefinition.VariableType);
                LocalBuilder localBuilder = generator.DeclareLocal(type);

                locals.Add(variableDefinition, localBuilder);
            }
        }

        private void MarkExceptionHandlers(ExceptionHandlerCollection exceptionHandlers)
        {
            foreach (ExceptionHandler exceptionHandler in exceptionHandlers)
            {
                AddActionAfter(exceptionHandler.TryStart, delegate
                {
                    generator.BeginExceptionBlock();
                });

                if (exceptionHandler.Type == ExceptionHandlerType.Filter)
                {
                    AddActionAfter(exceptionHandler.FilterStart, delegate
                    {
                        generator.BeginExceptFilterBlock();
                    });
                }

                AddActionAfter(exceptionHandler.HandlerStart, delegate
                {
                    switch (exceptionHandler.Type)
                    {
                        case ExceptionHandlerType.Catch:
                            generator.BeginCatchBlock(outer.ResolveType(exceptionHandler.CatchType));
                            break;
                        case ExceptionHandlerType.Fault:
                            generator.BeginFaultBlock();
                            break;
                        case ExceptionHandlerType.Finally:
                            generator.BeginFinallyBlock();
                            break;
                    }
                });

                AddActionBefore(exceptionHandler.HandlerEnd, delegate
                {
                    generator.EndExceptionBlock();
                });
            }
        }

        private void CreateLabels(Mono.Cecil.Cil.MethodBody body)
        {
            foreach (Instruction instruction in body.Instructions)
            {
                labels.Add(instruction, generator.DefineLabel());
            }
        }

        private void EmitInstructions(Mono.Cecil.Cil.MethodBody body)
        {
            // This loop is loosely based on some code in Mono.Cecil
            // that clones a method body.
            foreach (Instruction instruction in body.Instructions)
            {
                RunActions(instruction);
                generator.MarkLabel(labels[instruction]);

                switch (instruction.OpCode.OperandType)
                {
                    case Mono.Cecil.Cil.OperandType.InlineParam:
                    case Mono.Cecil.Cil.OperandType.ShortInlineParam:
                        generator.Emit(MapOpCode(instruction.OpCode), MapParameter((ParameterDefinition)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineVar:
                    case Mono.Cecil.Cil.OperandType.ShortInlineVar:
                        generator.Emit(MapOpCode(instruction.OpCode), MapLocal((VariableDefinition)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineField:
                        generator.Emit(MapOpCode(instruction.OpCode), MapField((FieldReference)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineMethod:
                        EmitWithMethodOperand(MapOpCode(instruction.OpCode), MapMethod((MethodReference)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineType:
                        generator.Emit(MapOpCode(instruction.OpCode), MapType((TypeReference)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineTok:
                        if (instruction.Operand is TypeReference)
                            generator.Emit(MapOpCode(instruction.OpCode), MapType((TypeReference)instruction.Operand));
                        else if (instruction.Operand is FieldReference)
                            generator.Emit(MapOpCode(instruction.OpCode), MapField((FieldReference)instruction.Operand));
                        else if (instruction.Operand is MethodReference)
                            EmitWithMethodOperand(MapOpCode(instruction.OpCode), MapMethod((MethodReference)instruction.Operand));
                        else
                            throw new NotSupportedException(string.Format("Unexpected token operand type: {0}.",
                                instruction.Operand.GetType()));
                        break;

                    case Mono.Cecil.Cil.OperandType.ShortInlineI:
                        generator.Emit(MapOpCode(instruction.OpCode), (sbyte) instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineI:
                        generator.Emit(MapOpCode(instruction.OpCode), (int)instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineI8:
                        generator.Emit(MapOpCode(instruction.OpCode), (long) instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.ShortInlineR:
                        generator.Emit(MapOpCode(instruction.OpCode), (float) instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineR:
                        generator.Emit(MapOpCode(instruction.OpCode), (double) instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineString:
                        generator.Emit(MapOpCode(instruction.OpCode), (string) instruction.Operand);
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineSwitch:
                        generator.Emit(MapOpCode(instruction.OpCode), MapLabels((Instruction[])instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineSig:
                        generator.Emit(MapOpCode(instruction.OpCode), MapSignature((CallSite)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlineNone:
                        generator.Emit(MapOpCode(instruction.OpCode));
                        break;

                    case Mono.Cecil.Cil.OperandType.ShortInlineBrTarget:
                    case Mono.Cecil.Cil.OperandType.InlineBrTarget:
                        generator.Emit(MapOpCode(instruction.OpCode), MapLabel((Instruction)instruction.Operand));
                        break;

                    case Mono.Cecil.Cil.OperandType.InlinePhi:
                    default:
                        throw new NotSupportedException(string.Format("Unexpected operand type: {0}.", instruction.OpCode.OperandType));
                }
            }
        }

        private void EmitWithMethodOperand(System.Reflection.Emit.OpCode opcode, MethodBase methodBase)
        {
            MethodInfo methodInfo = methodBase as MethodInfo;
            if (methodInfo != null)
                generator.Emit(opcode, methodInfo);
            else
                generator.Emit(opcode, (ConstructorInfo)methodBase);
        }

        private void AddActionBefore(Instruction instruction, Block block)
        {
            Block oldBlock;
            if (actions.TryGetValue(instruction, out oldBlock))
                actions[instruction] = (Block) Delegate.Combine(block, oldBlock);
            else
                actions.Add(instruction, block);
        }

        private void AddActionAfter(Instruction instruction, Block block)
        {
            Block oldBlock;
            if (actions.TryGetValue(instruction, out oldBlock))
                actions[instruction] = (Block)Delegate.Combine(oldBlock, block);
            else
                actions.Add(instruction, block);
        }

        private void RunActions(Instruction instruction)
        {
            Block block;
            if (actions.TryGetValue(instruction, out block))
                block();
        }

        private int MapParameter(ParameterDefinition parameterDefinition)
        {
            return parameterDefinition.Sequence;
        }

        private LocalBuilder MapLocal(VariableDefinition variableDefinition)
        {
            return locals[variableDefinition];
        }

        private MethodBase MapMethod(MethodReference methodReference)
        {
            return outer.ResolveMethod(methodReference);
        }

        private FieldInfo MapField(FieldReference fieldReference)
        {
            return outer.ResolveField(fieldReference);
        }

        private Type MapType(TypeReference typeReference)
        {
            return outer.ResolveType(typeReference);
        }

        private Type MapReturnType(MethodReturnType returnType)
        {
            return outer.ResolveReturnType(returnType);
        }

        private SignatureHelper MapSignature(CallSite signature)
        {
            Type returnType = MapReturnType(signature.ReturnType);
            // TODO...

            SignatureHelper helper;
            switch (signature.MetadataToken.TokenType)
            {
                    /*
                    case TokenType.Method:
                        helper = SignatureHelper.GetMethodSigHelper(module,
                            (System.Reflection.CallingConventions) signature.CallingConvention,
                            returnType);
                        break;

                    case TokenType.Field:
                        helper = SignatureHelper.GetFieldSigHelper(module);
                        break;

                    case TokenType.Param:
                        helper = SignatureHelper.GetLocalVarSigHelper(module);
                        break;

                    case TokenType.Property:
                        helper = SignatureHelper.GetPropertySigHelper(module, returnType, parameterTypes);
                        break;
                    */
                default:
                    throw new NotSupportedException(string.Format("Unexpected metadata token type: {0}.", signature.MetadataToken.TokenType));
            }

            return helper;
        }

        private Label MapLabel(Instruction instruction)
        {
            return labels[instruction];
        }

        private Label[] MapLabels(Instruction[] instructions)
        {
            return GenericUtils.ConvertAllToArray<Instruction, Label>(instructions, MapLabel);
        }

        private static System.Reflection.Emit.OpCode MapOpCode(Mono.Cecil.Cil.OpCode cecilOpcode)
        {
            switch (cecilOpcode.Code)
            {
                case Code.Nop: return System.Reflection.Emit.OpCodes.Nop;
                case Code.Break: return System.Reflection.Emit.OpCodes.Break;
                case Code.Ldarg_0: return System.Reflection.Emit.OpCodes.Ldarg_0;
                case Code.Ldarg_1: return System.Reflection.Emit.OpCodes.Ldarg_1;
                case Code.Ldarg_2: return System.Reflection.Emit.OpCodes.Ldarg_2;
                case Code.Ldarg_3: return System.Reflection.Emit.OpCodes.Ldarg_3;
                case Code.Ldloc_0: return System.Reflection.Emit.OpCodes.Ldloc_0;
                case Code.Ldloc_1: return System.Reflection.Emit.OpCodes.Ldloc_1;
                case Code.Ldloc_2: return System.Reflection.Emit.OpCodes.Ldloc_2;
                case Code.Ldloc_3: return System.Reflection.Emit.OpCodes.Ldloc_3;
                case Code.Stloc_0: return System.Reflection.Emit.OpCodes.Stloc_0;
                case Code.Stloc_1: return System.Reflection.Emit.OpCodes.Stloc_1;
                case Code.Stloc_2: return System.Reflection.Emit.OpCodes.Stloc_2;
                case Code.Stloc_3: return System.Reflection.Emit.OpCodes.Stloc_3;
                case Code.Ldarg_S: return System.Reflection.Emit.OpCodes.Ldarg_S;
                case Code.Ldarga_S: return System.Reflection.Emit.OpCodes.Ldarga_S;
                case Code.Starg_S: return System.Reflection.Emit.OpCodes.Starg_S;
                case Code.Ldloc_S: return System.Reflection.Emit.OpCodes.Ldloc_S;
                case Code.Ldloca_S: return System.Reflection.Emit.OpCodes.Ldloca_S;
                case Code.Stloc_S: return System.Reflection.Emit.OpCodes.Stloc_S;
                case Code.Ldnull: return System.Reflection.Emit.OpCodes.Ldnull;
                case Code.Ldc_I4_M1: return System.Reflection.Emit.OpCodes.Ldc_I4_M1;
                case Code.Ldc_I4_0: return System.Reflection.Emit.OpCodes.Ldc_I4_0;
                case Code.Ldc_I4_1: return System.Reflection.Emit.OpCodes.Ldc_I4_1;
                case Code.Ldc_I4_2: return System.Reflection.Emit.OpCodes.Ldc_I4_2;
                case Code.Ldc_I4_3: return System.Reflection.Emit.OpCodes.Ldc_I4_3;
                case Code.Ldc_I4_4: return System.Reflection.Emit.OpCodes.Ldc_I4_4;
                case Code.Ldc_I4_5: return System.Reflection.Emit.OpCodes.Ldc_I4_5;
                case Code.Ldc_I4_6: return System.Reflection.Emit.OpCodes.Ldc_I4_6;
                case Code.Ldc_I4_7: return System.Reflection.Emit.OpCodes.Ldc_I4_7;
                case Code.Ldc_I4_8: return System.Reflection.Emit.OpCodes.Ldc_I4_8;
                case Code.Ldc_I4_S: return System.Reflection.Emit.OpCodes.Ldc_I4_S;
                case Code.Ldc_I4: return System.Reflection.Emit.OpCodes.Ldc_I4;
                case Code.Ldc_I8: return System.Reflection.Emit.OpCodes.Ldc_I8;
                case Code.Ldc_R4: return System.Reflection.Emit.OpCodes.Ldc_R4;
                case Code.Ldc_R8: return System.Reflection.Emit.OpCodes.Ldc_R8;
                case Code.Dup: return System.Reflection.Emit.OpCodes.Dup;
                case Code.Pop: return System.Reflection.Emit.OpCodes.Pop;
                case Code.Jmp: return System.Reflection.Emit.OpCodes.Jmp;
                case Code.Call: return System.Reflection.Emit.OpCodes.Call;
                case Code.Calli: return System.Reflection.Emit.OpCodes.Calli;
                case Code.Ret: return System.Reflection.Emit.OpCodes.Ret;
                case Code.Br_S: return System.Reflection.Emit.OpCodes.Br_S;
                case Code.Brfalse_S: return System.Reflection.Emit.OpCodes.Brfalse_S;
                case Code.Brtrue_S: return System.Reflection.Emit.OpCodes.Brtrue_S;
                case Code.Beq_S: return System.Reflection.Emit.OpCodes.Beq_S;
                case Code.Bge_S: return System.Reflection.Emit.OpCodes.Bge_S;
                case Code.Bgt_S: return System.Reflection.Emit.OpCodes.Bgt_S;
                case Code.Ble_S: return System.Reflection.Emit.OpCodes.Ble_S;
                case Code.Blt_S: return System.Reflection.Emit.OpCodes.Blt_S;
                case Code.Bne_Un_S: return System.Reflection.Emit.OpCodes.Bne_Un_S;
                case Code.Bge_Un_S: return System.Reflection.Emit.OpCodes.Bge_Un_S;
                case Code.Bgt_Un_S: return System.Reflection.Emit.OpCodes.Bgt_Un_S;
                case Code.Ble_Un_S: return System.Reflection.Emit.OpCodes.Ble_Un_S;
                case Code.Blt_Un_S: return System.Reflection.Emit.OpCodes.Blt_Un_S;
                case Code.Br: return System.Reflection.Emit.OpCodes.Br;
                case Code.Brfalse: return System.Reflection.Emit.OpCodes.Brfalse;
                case Code.Brtrue: return System.Reflection.Emit.OpCodes.Brtrue;
                case Code.Beq: return System.Reflection.Emit.OpCodes.Beq;
                case Code.Bge: return System.Reflection.Emit.OpCodes.Bge;
                case Code.Bgt: return System.Reflection.Emit.OpCodes.Bgt;
                case Code.Ble: return System.Reflection.Emit.OpCodes.Ble;
                case Code.Blt: return System.Reflection.Emit.OpCodes.Blt;
                case Code.Bne_Un: return System.Reflection.Emit.OpCodes.Bne_Un;
                case Code.Bge_Un: return System.Reflection.Emit.OpCodes.Bge_Un;
                case Code.Bgt_Un: return System.Reflection.Emit.OpCodes.Bgt_Un;
                case Code.Ble_Un: return System.Reflection.Emit.OpCodes.Ble_Un;
                case Code.Blt_Un: return System.Reflection.Emit.OpCodes.Blt_Un;
                case Code.Switch: return System.Reflection.Emit.OpCodes.Switch;
                case Code.Ldind_I1: return System.Reflection.Emit.OpCodes.Ldind_I1;
                case Code.Ldind_U1: return System.Reflection.Emit.OpCodes.Ldind_U1;
                case Code.Ldind_I2: return System.Reflection.Emit.OpCodes.Ldind_I2;
                case Code.Ldind_U2: return System.Reflection.Emit.OpCodes.Ldind_U2;
                case Code.Ldind_I4: return System.Reflection.Emit.OpCodes.Ldind_I4;
                case Code.Ldind_U4: return System.Reflection.Emit.OpCodes.Ldind_U4;
                case Code.Ldind_I8: return System.Reflection.Emit.OpCodes.Ldind_I8;
                case Code.Ldind_I: return System.Reflection.Emit.OpCodes.Ldind_I;
                case Code.Ldind_R4: return System.Reflection.Emit.OpCodes.Ldind_R4;
                case Code.Ldind_R8: return System.Reflection.Emit.OpCodes.Ldind_R8;
                case Code.Ldind_Ref: return System.Reflection.Emit.OpCodes.Ldind_Ref;
                case Code.Stind_Ref: return System.Reflection.Emit.OpCodes.Stind_Ref;
                case Code.Stind_I1: return System.Reflection.Emit.OpCodes.Stind_I1;
                case Code.Stind_I2: return System.Reflection.Emit.OpCodes.Stind_I2;
                case Code.Stind_I4: return System.Reflection.Emit.OpCodes.Stind_I4;
                case Code.Stind_I8: return System.Reflection.Emit.OpCodes.Stind_I8;
                case Code.Stind_R4: return System.Reflection.Emit.OpCodes.Stind_R4;
                case Code.Stind_R8: return System.Reflection.Emit.OpCodes.Stind_R8;
                case Code.Add: return System.Reflection.Emit.OpCodes.Add;
                case Code.Sub: return System.Reflection.Emit.OpCodes.Sub;
                case Code.Mul: return System.Reflection.Emit.OpCodes.Mul;
                case Code.Div: return System.Reflection.Emit.OpCodes.Div;
                case Code.Div_Un: return System.Reflection.Emit.OpCodes.Div_Un;
                case Code.Rem: return System.Reflection.Emit.OpCodes.Rem;
                case Code.Rem_Un: return System.Reflection.Emit.OpCodes.Rem_Un;
                case Code.And: return System.Reflection.Emit.OpCodes.And;
                case Code.Or: return System.Reflection.Emit.OpCodes.Or;
                case Code.Xor: return System.Reflection.Emit.OpCodes.Xor;
                case Code.Shl: return System.Reflection.Emit.OpCodes.Shl;
                case Code.Shr: return System.Reflection.Emit.OpCodes.Shr;
                case Code.Shr_Un: return System.Reflection.Emit.OpCodes.Shr_Un;
                case Code.Neg: return System.Reflection.Emit.OpCodes.Neg;
                case Code.Not: return System.Reflection.Emit.OpCodes.Not;
                case Code.Conv_I1: return System.Reflection.Emit.OpCodes.Conv_I1;
                case Code.Conv_I2: return System.Reflection.Emit.OpCodes.Conv_I2;
                case Code.Conv_I4: return System.Reflection.Emit.OpCodes.Conv_I4;
                case Code.Conv_I8: return System.Reflection.Emit.OpCodes.Conv_I8;
                case Code.Conv_R4: return System.Reflection.Emit.OpCodes.Conv_R4;
                case Code.Conv_R8: return System.Reflection.Emit.OpCodes.Conv_R8;
                case Code.Conv_U4: return System.Reflection.Emit.OpCodes.Conv_U4;
                case Code.Conv_U8: return System.Reflection.Emit.OpCodes.Conv_U8;
                case Code.Callvirt: return System.Reflection.Emit.OpCodes.Callvirt;
                case Code.Cpobj: return System.Reflection.Emit.OpCodes.Cpobj;
                case Code.Ldobj: return System.Reflection.Emit.OpCodes.Ldobj;
                case Code.Ldstr: return System.Reflection.Emit.OpCodes.Ldstr;
                case Code.Newobj: return System.Reflection.Emit.OpCodes.Newobj;
                case Code.Castclass: return System.Reflection.Emit.OpCodes.Castclass;
                case Code.Isinst: return System.Reflection.Emit.OpCodes.Isinst;
                case Code.Conv_R_Un: return System.Reflection.Emit.OpCodes.Conv_R_Un;
                case Code.Unbox: return System.Reflection.Emit.OpCodes.Unbox;
                case Code.Throw: return System.Reflection.Emit.OpCodes.Throw;
                case Code.Ldfld: return System.Reflection.Emit.OpCodes.Ldfld;
                case Code.Ldflda: return System.Reflection.Emit.OpCodes.Ldflda;
                case Code.Stfld: return System.Reflection.Emit.OpCodes.Stfld;
                case Code.Ldsfld: return System.Reflection.Emit.OpCodes.Ldsfld;
                case Code.Ldsflda: return System.Reflection.Emit.OpCodes.Ldsflda;
                case Code.Stsfld: return System.Reflection.Emit.OpCodes.Stsfld;
                case Code.Stobj: return System.Reflection.Emit.OpCodes.Stobj;
                case Code.Conv_Ovf_I1_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_I1_Un;
                case Code.Conv_Ovf_I2_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_I2_Un;
                case Code.Conv_Ovf_I4_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_I4_Un;
                case Code.Conv_Ovf_I8_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_I8_Un;
                case Code.Conv_Ovf_U1_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_U1_Un;
                case Code.Conv_Ovf_U2_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_U2_Un;
                case Code.Conv_Ovf_U4_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_U4_Un;
                case Code.Conv_Ovf_U8_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_U8_Un;
                case Code.Conv_Ovf_I_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_I_Un;
                case Code.Conv_Ovf_U_Un: return System.Reflection.Emit.OpCodes.Conv_Ovf_U_Un;
                case Code.Box: return System.Reflection.Emit.OpCodes.Box;
                case Code.Newarr: return System.Reflection.Emit.OpCodes.Newarr;
                case Code.Ldlen: return System.Reflection.Emit.OpCodes.Ldlen;
                case Code.Ldelema: return System.Reflection.Emit.OpCodes.Ldelema;
                case Code.Ldelem_I1: return System.Reflection.Emit.OpCodes.Ldelem_I1;
                case Code.Ldelem_U1: return System.Reflection.Emit.OpCodes.Ldelem_U1;
                case Code.Ldelem_I2: return System.Reflection.Emit.OpCodes.Ldelem_I2;
                case Code.Ldelem_U2: return System.Reflection.Emit.OpCodes.Ldelem_U2;
                case Code.Ldelem_I4: return System.Reflection.Emit.OpCodes.Ldelem_I4;
                case Code.Ldelem_U4: return System.Reflection.Emit.OpCodes.Ldelem_U4;
                case Code.Ldelem_I8: return System.Reflection.Emit.OpCodes.Ldelem_I8;
                case Code.Ldelem_I: return System.Reflection.Emit.OpCodes.Ldelem_I;
                case Code.Ldelem_R4: return System.Reflection.Emit.OpCodes.Ldelem_R4;
                case Code.Ldelem_R8: return System.Reflection.Emit.OpCodes.Ldelem_R8;
                case Code.Ldelem_Ref: return System.Reflection.Emit.OpCodes.Ldelem_Ref;
                case Code.Stelem_I: return System.Reflection.Emit.OpCodes.Stelem_I;
                case Code.Stelem_I1: return System.Reflection.Emit.OpCodes.Stelem_I1;
                case Code.Stelem_I2: return System.Reflection.Emit.OpCodes.Stelem_I2;
                case Code.Stelem_I4: return System.Reflection.Emit.OpCodes.Stelem_I4;
                case Code.Stelem_I8: return System.Reflection.Emit.OpCodes.Stelem_I8;
                case Code.Stelem_R4: return System.Reflection.Emit.OpCodes.Stelem_R4;
                case Code.Stelem_R8: return System.Reflection.Emit.OpCodes.Stelem_R8;
                case Code.Stelem_Ref: return System.Reflection.Emit.OpCodes.Stelem_Ref;
                case Code.Ldelem_Any: return System.Reflection.Emit.OpCodes.Ldelem; // *
                case Code.Stelem_Any: return System.Reflection.Emit.OpCodes.Stelem; // *
                case Code.Unbox_Any: return System.Reflection.Emit.OpCodes.Unbox_Any;
                case Code.Conv_Ovf_I1: return System.Reflection.Emit.OpCodes.Conv_Ovf_I1;
                case Code.Conv_Ovf_U1: return System.Reflection.Emit.OpCodes.Conv_Ovf_U1;
                case Code.Conv_Ovf_I2: return System.Reflection.Emit.OpCodes.Conv_Ovf_I2;
                case Code.Conv_Ovf_U2: return System.Reflection.Emit.OpCodes.Conv_Ovf_U2;
                case Code.Conv_Ovf_I4: return System.Reflection.Emit.OpCodes.Conv_Ovf_I4;
                case Code.Conv_Ovf_U4: return System.Reflection.Emit.OpCodes.Conv_Ovf_U4;
                case Code.Conv_Ovf_I8: return System.Reflection.Emit.OpCodes.Conv_Ovf_I8;
                case Code.Conv_Ovf_U8: return System.Reflection.Emit.OpCodes.Conv_Ovf_U8;
                case Code.Refanyval: return System.Reflection.Emit.OpCodes.Refanyval;
                case Code.Ckfinite: return System.Reflection.Emit.OpCodes.Ckfinite;
                case Code.Mkrefany: return System.Reflection.Emit.OpCodes.Mkrefany;
                case Code.Ldtoken: return System.Reflection.Emit.OpCodes.Ldtoken;
                case Code.Conv_U2: return System.Reflection.Emit.OpCodes.Conv_U2;
                case Code.Conv_U1: return System.Reflection.Emit.OpCodes.Conv_U1;
                case Code.Conv_I: return System.Reflection.Emit.OpCodes.Conv_I;
                case Code.Conv_Ovf_I: return System.Reflection.Emit.OpCodes.Conv_Ovf_I;
                case Code.Conv_Ovf_U: return System.Reflection.Emit.OpCodes.Conv_Ovf_U;
                case Code.Add_Ovf: return System.Reflection.Emit.OpCodes.Add_Ovf;
                case Code.Add_Ovf_Un: return System.Reflection.Emit.OpCodes.Add_Ovf_Un;
                case Code.Mul_Ovf: return System.Reflection.Emit.OpCodes.Mul_Ovf;
                case Code.Mul_Ovf_Un: return System.Reflection.Emit.OpCodes.Mul_Ovf_Un;
                case Code.Sub_Ovf: return System.Reflection.Emit.OpCodes.Sub_Ovf;
                case Code.Sub_Ovf_Un: return System.Reflection.Emit.OpCodes.Sub_Ovf_Un;
                case Code.Endfinally: return System.Reflection.Emit.OpCodes.Endfinally;
                case Code.Leave: return System.Reflection.Emit.OpCodes.Leave;
                case Code.Leave_S: return System.Reflection.Emit.OpCodes.Leave_S;
                case Code.Stind_I: return System.Reflection.Emit.OpCodes.Stind_I;
                case Code.Conv_U: return System.Reflection.Emit.OpCodes.Conv_U;
                case Code.Arglist: return System.Reflection.Emit.OpCodes.Arglist;
                case Code.Ceq: return System.Reflection.Emit.OpCodes.Ceq;
                case Code.Cgt: return System.Reflection.Emit.OpCodes.Cgt;
                case Code.Cgt_Un: return System.Reflection.Emit.OpCodes.Cgt_Un;
                case Code.Clt: return System.Reflection.Emit.OpCodes.Clt;
                case Code.Clt_Un: return System.Reflection.Emit.OpCodes.Clt_Un;
                case Code.Ldftn: return System.Reflection.Emit.OpCodes.Ldftn;
                case Code.Ldvirtftn: return System.Reflection.Emit.OpCodes.Ldvirtftn;
                case Code.Ldarg: return System.Reflection.Emit.OpCodes.Ldarg;
                case Code.Ldarga: return System.Reflection.Emit.OpCodes.Ldarga;
                case Code.Starg: return System.Reflection.Emit.OpCodes.Starg;
                case Code.Ldloc: return System.Reflection.Emit.OpCodes.Ldloc;
                case Code.Ldloca: return System.Reflection.Emit.OpCodes.Ldloca;
                case Code.Stloc: return System.Reflection.Emit.OpCodes.Stloc;
                case Code.Localloc: return System.Reflection.Emit.OpCodes.Localloc;
                case Code.Endfilter: return System.Reflection.Emit.OpCodes.Endfilter;
                case Code.Unaligned: return System.Reflection.Emit.OpCodes.Unaligned;
                case Code.Volatile: return System.Reflection.Emit.OpCodes.Volatile;
                case Code.Tail: return System.Reflection.Emit.OpCodes.Tailcall; // *
                case Code.Initobj: return System.Reflection.Emit.OpCodes.Initobj;
                case Code.Constrained: return System.Reflection.Emit.OpCodes.Constrained;
                case Code.Cpblk: return System.Reflection.Emit.OpCodes.Cpblk;
                case Code.Initblk: return System.Reflection.Emit.OpCodes.Initblk;
                    //case Code.No: return System.Reflection.Emit.OpCodes.No; // no such opcode
                case Code.Rethrow: return System.Reflection.Emit.OpCodes.Rethrow;
                case Code.Sizeof: return System.Reflection.Emit.OpCodes.Sizeof;
                case Code.Refanytype: return System.Reflection.Emit.OpCodes.Refanytype;
                case Code.Readonly: return System.Reflection.Emit.OpCodes.Readonly;
                default:
                    throw new NotSupportedException(string.Format("Unsupported opcode: {0}.", cecilOpcode));
            }
        }
    }
}