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
using System.Text;
using MbUnit.Framework.Kernel.Collections;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Describes a type in a dynamic assembly that has not yet been fully
    /// created.  This is useful because many reflection methods do not become
    /// available until after <see cref="TypeBuilder.CreateType" /> is called.
    /// </summary>
    internal class DynamicType
    {
        private const string ConstructorName = ".ctor";
        private readonly TypeBuilder builder;
        private readonly DynamicModule module;
        private readonly MultiMap<string, object> members;

        public DynamicType(TypeBuilder builder, DynamicModule module)
        {
            this.builder = builder;
            this.module = module;

            members = new MultiMap<string, object>();

            if (builder != null)
                Assembly.RegisterDynamicType(builder, this);
        }

        public TypeBuilder Builder
        {
            get { return builder; }
        }

        public DynamicModule Module
        {
            get { return module; }
        }

        public DynamicAssembly Assembly
        {
            get { return module.Assembly; }
        }

        public DynamicType DefineNestedType(string name, TypeAttributes attr, Type parentType, int typeSize)
        {
            TypeBuilder typeBuilder = builder.DefineNestedType(name, attr, parentType, typeSize);
            members.Add(name, typeBuilder);

            DynamicType type = new DynamicType(typeBuilder, module);
            return type;
        }

        public FieldBuilder DefineField(string name, Type fieldType, FieldAttributes attr)
        {
            FieldBuilder fieldBuilder = builder.DefineField(name, fieldType, attr);
            members.Add(name, fieldBuilder);

            return fieldBuilder;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attr, Type returnType, Type[] paramTypes)
        {
            PropertyBuilder propertyBuilder = builder.DefineProperty(name, attr, returnType, paramTypes);
            members.Add(name, propertyBuilder);

            return propertyBuilder;
        }

        public EventBuilder DefineEvent(string name, EventAttributes attr, Type eventType)
        {
            EventBuilder eventBuilder = builder.DefineEvent(name, attr, eventType);
            members.Add(name, eventBuilder);

            return eventBuilder;
        }

        public DynamicConstructor DefineConstructor(MethodAttributes attr, CallingConventions conv, Type[] paramTypes)
        {
            ConstructorBuilder constructorBuilder = builder.DefineConstructor(attr, conv, paramTypes);
            DynamicConstructor constructor = new DynamicConstructor(constructorBuilder, paramTypes);
            members.Add(ConstructorName, constructor);

            return constructor;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attr, CallingConventions conv)
        {
            MethodBuilder methodBuilder = builder.DefineMethod(name, attr, conv);
            return methodBuilder;
        }

        public Type GetNestedType(string name)
        {
            return FindMember<Type>(name, delegate
            {
                return true;
            });
        }

        public FieldInfo GetField(string name)
        {
            return FindMember<FieldInfo>(name, delegate
            {
                return true;
            });
        }

        public PropertyInfo GetProperty(string name)
        {
            return FindMember<PropertyInfo>(name, delegate
            {
                return true;
            });
        }

        internal PropertyInfo GetProperty(string name, Type returnType, Type[] paramTypes)
        {
            return FindMember<PropertyInfo>(name, delegate(PropertyInfo member)
            {
                return member.PropertyType == returnType
                    && CompareParameterTypes(member.GetIndexParameters(), paramTypes);
            });
        }

        public MethodInfo GetMethod(string name, bool instance, Type[] types)
        {
            return FindMember<MethodInfo>(name, delegate(MethodInfo member)
            {
                return member.IsStatic != instance
                    && CompareParameterTypes(member.GetParameters(), types);
            });
        }

        public DynamicConstructor GetConstructor(bool instance, Type[] types)
        {
            return FindMember<DynamicConstructor>(ConstructorName, delegate(DynamicConstructor member)
            {
                return member.Builder.IsStatic != instance
                    && CompareParameterTypes(member.ParameterTypes, types);
            });
        }

        private T FindMember<T>(string name, Predicate<T> predicate)
            where T : class
        {
            foreach (object member in members[name])
            {
                T memberOfType = member as T;
                if (memberOfType != null && predicate(memberOfType))
                    return memberOfType;
            }

            return null;
        }

        private static bool CompareParameterTypes(Type[] parameterTypes, Type[] types)
        {
            if (parameterTypes.Length != types.Length)
                return false;

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (parameterTypes[i] != types[i])
                    return false;
            }

            return true;
        }

        private static bool CompareParameterTypes(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types[i])
                    return false;
            }

            return true;
        }
    }
}
