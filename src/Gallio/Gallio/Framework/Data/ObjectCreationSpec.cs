using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// Encapsulates a specification for creating objects given values for
    /// a type's generic parameters, constructor parameters, fields and properties.
    /// </para>
    /// </summary>
    public sealed class ObjectCreationSpec : DataBindingSpec
    {
        private readonly ITypeInfo type;

        private Type resolvedType;
        private Type[] resolvedGenericArguments;
        private ConstructorInfo resolvedConstructor;
        private object[] resolvedConstructorArguments;
        private Dictionary<FieldInfo, object> resolvedFieldValues;
        private Dictionary<PropertyInfo, object> resolvedPropertyValues;

        /// <summary>
        /// Creates a new object specification.
        /// </summary>
        /// <param name="type">The type or generic type definition to be instantiated</param>
        /// <param name="slotValues">The slot values</param>
        /// <param name="converter">The converter to use for converting slot values
        /// to the required types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="slotValues"/> or <paramref name="converter"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/>
        /// has an element type or if <paramref name="slotValues" /> contains
        /// slots that are declared by different types or have incompatible values</exception>
        public ObjectCreationSpec(ITypeInfo type,
            IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues, IConverter converter)
            : base(slotValues, converter)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type.ElementType != null || type.IsGenericParameter)
                throw new ArgumentException("The type must not be an array, pointer, reference or generic parameter.", "type");
            ValidateSlots(type, slotValues);

            this.type = type;

            ResolveType();
            ResolveConstructor();
            ResolveFields();
            ResolveProperties();
        }

        /// <summary>
        /// Gets the type or generic type definition to be instantiated.
        /// </summary>
        public ITypeInfo Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the resolved type given any generic type arguments that may have
        /// been provided as slot values.
        /// </summary>
        public Type ResolvedType
        {
            get { return resolvedType; }
        }

        /// <summary>
        /// Creates an instance of the resolved type and initializes it using
        /// constructor parameter, field and property slot values.
        /// </summary>
        /// <returns>The new instance, never null</returns>
        public object CreateInstance()
        {
            object instance;
            if (resolvedConstructor != null)
                instance = resolvedConstructor.Invoke(resolvedConstructorArguments);
            else
                instance = Activator.CreateInstance(resolvedType);

            foreach (KeyValuePair<FieldInfo, object> fieldValue in resolvedFieldValues)
                fieldValue.Key.SetValue(instance, fieldValue.Value);

            foreach (KeyValuePair<PropertyInfo, object> propertyValue in resolvedPropertyValues)
                propertyValue.Key.SetValue(instance, propertyValue.Value, null);

            return instance;
        }

        /// <inheritdoc />
        protected override string FormatInternal(IFormatter formatter)
        {
            StringBuilder str = new StringBuilder();
            AppendFormattedGenericArguments(str, resolvedGenericArguments, formatter);
            AppendFormattedMethodArguments(str, resolvedConstructorArguments, formatter);
            AppendFormattedNamedValues(str, GetNamedValues(), formatter);
            return str.ToString();
        }

        private IEnumerable<KeyValuePair<string, object>> GetNamedValues()
        {
            foreach (KeyValuePair<FieldInfo, object> fieldValue in resolvedFieldValues)
                yield return new KeyValuePair<string, object>(fieldValue.Key.Name, fieldValue.Value);
            foreach (KeyValuePair<PropertyInfo, object> propertyValue in resolvedPropertyValues)
                yield return new KeyValuePair<string, object>(propertyValue.Key.Name, propertyValue.Value);
        }

        private void ResolveType()
        {
            int genericParameterCount = type.IsGenericTypeDefinition ? type.GenericArguments.Count : 0;
            resolvedGenericArguments = new Type[genericParameterCount];

            int seen = 0;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IGenericParameterInfo genericParameter = slotValue.Key as IGenericParameterInfo;
                if (genericParameter != null)
                {
                    resolvedGenericArguments[genericParameter.Position] = (Type) Converter.Convert(slotValue.Value, typeof(Type));
                    seen += 1;
                }
            }

            if (genericParameterCount != seen)
                throw new ArgumentException(String.Format("The type has {0} generic parameters but the bindings only provide values for {1} of them.",
                    genericParameterCount, seen));

            resolvedType = type.Resolve(true);
            if (genericParameterCount != 0)
                resolvedType = resolvedType.MakeGenericType(resolvedGenericArguments);
        }

        private void ResolveConstructor()
        {
            IConstructorInfo constructor = null;
            ParameterInfo[] resolvedConstructorParameters = null;

            int seen = 0;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IParameterInfo parameter = slotValue.Key as IParameterInfo;
                if (parameter != null)
                {
                    IConstructorInfo possibleConstructor = parameter.Member as IConstructorInfo;
                    if (possibleConstructor == null)
                        throw new ArgumentException(String.Format("The parameter slot '{0}' is not a constructor parameter.", parameter));

                    if (constructor == null)
                    {
                        constructor = possibleConstructor;

                        resolvedConstructor = ResolveMember(resolvedType, constructor.Resolve(true));
                        resolvedConstructorParameters = resolvedConstructor.GetParameters();
                        resolvedConstructorArguments = new object[resolvedConstructorParameters.Length];
                    }
                    else
                    {
                        if (!constructor.Equals(possibleConstructor))
                            throw new ArgumentException(String.Format("The parameter slot '{0}' belongs to a different constructor than the previous parameter slot.", parameter));
                    }

                    int position = parameter.Position;
                    resolvedConstructorArguments[position] = Converter.Convert(slotValue.Value, resolvedConstructorParameters[position].ParameterType);
                    seen += 1;
                }
            }

            if (resolvedConstructorParameters != null && resolvedConstructorParameters.Length != seen)
                throw new ArgumentException(String.Format("The constructor has {0} parameters but the bindings only provide values for {1} of them.",
                    resolvedConstructorParameters.Length, seen));

            if (constructor == null)
            {
                // Note: Value types don't have default constructors so we leave the constructor field null
                //       to remember to instantiate the structure a different way.
                if (!resolvedType.IsValueType)
                {
                    resolvedConstructor = resolvedType.GetConstructor(EmptyArray<Type>.Instance);
                    if (resolvedConstructor == null)
                        throw new ArgumentException("The bindings do not contain any constructor parameters but the class does not have a default constructor.");
                }

                resolvedConstructorArguments = EmptyArray<object>.Instance;
            }
        }

        private void ResolveFields()
        {
            resolvedFieldValues = new Dictionary<FieldInfo, object>();

            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IFieldInfo field = slotValue.Key as IFieldInfo;
                if (field != null)
                {
                    FieldInfo resolvedField = ResolveMember(resolvedType, field.Resolve(true));
                    resolvedFieldValues.Add(resolvedField, slotValue.Value);
                }
            }
        }

        private void ResolveProperties()
        {
            resolvedPropertyValues = new Dictionary<PropertyInfo, object>();

            foreach (KeyValuePair<ISlotInfo, object> slotValue in SlotValues)
            {
                IPropertyInfo property = slotValue.Key as IPropertyInfo;
                if (property != null)
                {
                    PropertyInfo resolvedProperty = ResolveMember(resolvedType, property.Resolve(true));
                    resolvedPropertyValues.Add(resolvedProperty, slotValue.Value);
                }
            }
        }

        private static void ValidateSlots(ITypeInfo type, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                    case CodeElementKind.Field:
                    case CodeElementKind.Property:
                        IMemberInfo member = (IMemberInfo) slot;
                        if (type.Equals(member.DeclaringType))
                            continue;
                        break;

                    case CodeElementKind.Parameter:
                        IParameterInfo parameter = (IParameterInfo) slot;
                        if (type.Equals(parameter.Member.DeclaringType))
                            continue;
                        break;
                }

                throw new ArgumentException(String.Format("Slot '{0}' is not valid for creating objects of type '{1}'.", slot, type), "slotValues");
            }
        }
    }
}
