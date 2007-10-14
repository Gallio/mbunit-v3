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
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using MbUnit.Hosting;

namespace MbUnit.Hosting
{
    /// <summary>
    /// The default XML documentation resolver reads XML documentation files on
    /// demand when available and caches them in memory for subsequent accesses.
    /// It takes care of mapping member names to XML documentation conventions
    /// when asked to resolve the documentation for a member.
    /// </summary>
    /// <remarks>
    /// All operations are thread-safe.
    /// </remarks>
    public class DefaultXmlDocumentationResolver : IXmlDocumentationResolver
    {
        private readonly Dictionary<Assembly, CachedDocument> cachedDocuments;

        /// <summary>
        /// Creates an XML documentation loader.
        /// </summary>
        public DefaultXmlDocumentationResolver()
        {
            cachedDocuments = new Dictionary<Assembly, CachedDocument>();
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(@"type");

            CachedDocument document = GetDocument(type.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(type)) : null;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(@"field");

            CachedDocument document = GetDocument(field.DeclaringType.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(field)) : null;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(@"property");

            CachedDocument document = GetDocument(property.DeclaringType.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(property)) : null;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(EventInfo @event)
        {
            if (@event == null)
                throw new ArgumentNullException(@"event");

            CachedDocument document = GetDocument(@event.DeclaringType.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(@event)) : null;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");

            CachedDocument document = GetDocument(method.DeclaringType.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(method)) : null;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(@"member");

            Type type = member as Type ?? member.DeclaringType;
            CachedDocument document = GetDocument(type.Assembly);
            return document != null ? document.GetXmlDocumentation(FormatId(member)) : null;
        }

        private CachedDocument GetDocument(Assembly assembly)
        {
            lock (cachedDocuments)
            {
                CachedDocument document;
                if (cachedDocuments.TryGetValue(assembly, out document))
                    return document;

                string assemblyPath = Loader.GetAssemblyLocalPath(assembly);
                if (assemblyPath != null)
                {
                    string documentPath = Path.ChangeExtension(assemblyPath, @".xml");

                    if (File.Exists(documentPath))
                    {
                        document = CachedDocument.Load(documentPath);
                        cachedDocuments.Add(assembly, document);
                        return document;
                    }
                }

                cachedDocuments.Add(assembly, null);
                return null;
            }
        }

        private static string FormatId(Type type)
        {
            StringBuilder str = new StringBuilder(@"T:");
            AppendType(str, type, true);
            return str.ToString();
        }

        private static string FormatId(FieldInfo field)
        {
            StringBuilder str = new StringBuilder(@"F:");
            AppendType(str, field.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, field);
            return str.ToString();
        }

        private static string FormatId(PropertyInfo property)
        {
            // Note: To handle indexers with parameters bound to generic type arguments
            //       of the declaring type, we need to throw away all specialization of the property.
            //       We can do that by locating the same property on the generic type definition
            //       by its metadata token.  The framework doesn't help us out much here since
            //       we can't just resolve the metadata token for a property like we can for methods.
            ParameterInfo[] parameters = property.GetIndexParameters();
            if (parameters.Length != 0 && property.DeclaringType.IsGenericType
                && ! property.DeclaringType.IsGenericTypeDefinition)
            {
                Type genericTypeDefn = property.DeclaringType.GetGenericTypeDefinition();
                PropertyInfo unboundProperty = genericTypeDefn.GetProperty(property.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (unboundProperty == null || unboundProperty.MetadataToken != property.MetadataToken)
                    throw new NotSupportedException(String.Format("Could not resolve property '{0}' that was declared on a generic type.",
                        property));

                property = unboundProperty;
                parameters = property.GetIndexParameters();
            }

            StringBuilder str = new StringBuilder(@"P:");
            AppendType(str, property.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, property);
            AppendParameterList(str, parameters);
            return str.ToString();
        }

        private static string FormatId(EventInfo @event)
        {
            StringBuilder str = new StringBuilder(@"E:");
            AppendType(str, @event.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, @event);
            return str.ToString();
        }

        private static string FormatId(MethodBase method)
        {
            // Note: To handle methods with parameters (or return type in the case of conversion
            //       operators) bound to generic type arguments of the declaring type, we need to
            //       throw away all specialization of the method.  We can do that by resolving
            //       the raw method token.
            if (method.DeclaringType.IsGenericType)
                method = method.Module.ResolveMethod(method.MetadataToken);

            StringBuilder str = new StringBuilder(@"M:");
            AppendType(str, method.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, method);

            if (method.IsGenericMethod)
            {
                str.Append(@"``");
                str.Append(method.GetGenericArguments().Length);
            }

            AppendParameterList(str, method.GetParameters());

            if (method.IsSpecialName
                && (method.Name == @"op_Implicit" || method.Name == @"op_Explicit"))
            {
                str.Append('~');
                AppendType(str, ((MethodInfo)method).ReturnType, false);
            }

            return str.ToString();
        }

        private static string FormatId(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return FormatId((MethodBase) member);

                case MemberTypes.Event:
                    return FormatId((EventInfo) member);

                case MemberTypes.Field:
                    return FormatId((FieldInfo) member);

                case MemberTypes.Property:
                    return FormatId((PropertyInfo) member);

                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    return FormatId((Type) member);

                case MemberTypes.Custom:
                default:
                    // Note: XML doc spec doesn't say anything about custom members.
                    //       Using the "!" prefix flags the reference as an error which seems reasonable.
                    StringBuilder str = new StringBuilder(@"!:");
                    AppendType(str, member.DeclaringType, true);
                    str.Append('.');
                    AppendMemberName(str, member);
                    return str.ToString();
            }
        }

        private static void AppendType(StringBuilder str, Type type, bool useGenericDefinition)
        {
            // TODO: Handle custom type modifiers, pinned types, generic arrays,
            //       ranked arrays, and function pointer types
            if (type.HasElementType)
            {
                Type elementType = type.GetElementType();
                AppendType(str, elementType, useGenericDefinition);

                if (type.IsArray)
                {
                    str.Append(@"[]");
                }
                else if (type.IsByRef)
                {
                    str.Append('@');
                }
                else if (type.IsPointer)
                {
                    str.Append('*');
                }
                else
                {
                    // Don't know what to do with this.
                    str.Append('?');
                }

                return;
            }

            if (type.IsGenericParameter)
            {
                // Generic method parameters use 2 back-ticks instead of 1.
                if (type.DeclaringMethod != null)
                    str.Append('`');
                str.Append('`');
                str.Append(type.GenericParameterPosition);
                return;
            }

            if (type.IsNested)
            {
                AppendType(str, type.DeclaringType, true);
                str.Append('.');
            }
            else
            {
                string @namespace = type.Namespace;
                if (@namespace.Length != 0)
                {
                    str.Append(type.Namespace);
                    str.Append('.');
                }
            }

            AppendMemberName(str, type);

            if (type.IsGenericType)
            {
                // Note: The Type's Name already includes `x where x is the number of generic arguments.
                //       So if we just need the id of the generic type definition, we're done.
                //       Otherwise we need to strip off this token and append the concrete types
                //       of the arguments.  That case occurs when closed generic types appear in
                //       method signatures.
                if (! useGenericDefinition && ! type.IsGenericTypeDefinition)
                {
                    while (str[str.Length - 1] != '`')
                        str.Length -= 1;

                    str[str.Length - 1] = '{';

                    foreach (Type argument in type.GetGenericArguments())
                    {
                        AppendType(str, argument, false);
                        str.Append(',');
                    }

                    str[str.Length - 1] = '}';
                }
            }
        }

        private static void AppendMemberName(StringBuilder str, MemberInfo member)
        {
            int oldLength = str.Length;
            string memberName = member.Name;

            str.Append(memberName);
            str.Replace('.', '#', oldLength, memberName.Length);
        }

        private static void AppendParameterList(StringBuilder str, ParameterInfo[] parameters)
        {
            if (parameters.Length != 0)
            {
                str.Append('(');

                foreach (ParameterInfo parameter in parameters)
                {
                    AppendType(str, parameter.ParameterType, false);
                    str.Append(',');
                }

                str[str.Length - 1] = ')';
            }
        }

        private sealed class CachedDocument
        {
            private readonly Dictionary<string, string> members;

            private CachedDocument()
            {
                members = new Dictionary<string, string>();
            }

            public static CachedDocument Load(string documentPath)
            {
                // This is optimized somewhat to avoid reading in the XML document
                // as a big lump of objects.  We just need the contents; the structure
                // itself is irrelevant.

                CachedDocument document = new CachedDocument();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false; // needed for normalization to work
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreComments = true;
                settings.ValidationType = ValidationType.None;
                settings.ProhibitDtd = true;
                settings.CheckCharacters = false;

                using (XmlReader reader = XmlReader.Create(documentPath, settings))
                {
                    for (; ; )
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == @"member")
                            {
                                string name = reader.GetAttribute(@"name");
                                string content = NormalizeWhitespace(reader.ReadInnerXml());

                                document.members.Add(name, content);
                                continue;
                            }
                            else if (reader.Name != @"doc" && reader.Name != @"members")
                            {
                                reader.Skip();
                                continue;
                            }
                        }

                        if (!reader.Read())
                            break;
                    }
                }

                return document;
            }

            public string GetXmlDocumentation(string id)
            {
                string content;
                members.TryGetValue(id, out content);
                return content;
            }

            /// <summary>
            /// The XML documentation gets pretty-printed with indentation by the compiler.
            /// We try to recover the original formatting by stripping out the minimal leading
            /// whitespace from each line.
            /// </summary>
            private static string NormalizeWhitespace(string content)
            {
                int count = content.Length;
                if (count == 0)
                    return string.Empty;

                // Compute the amount of leading whitespace that appears on all non-empty lines.
                int leading = int.MaxValue;
                for (int i = 0, currentLeading = 0; i < count; )
                {
                    char c = content[i++];

                    if (c == ' ')
                    {
                        currentLeading += 1;
                    }
                    else if (c == '\n')
                    {
                        currentLeading = 0;
                    }
                    else
                    {
                        if (currentLeading < leading)
                            leading = currentLeading;

                        currentLeading = 0;

                        while (i < count && content[i++] != '\n') ;
                    }
                }

                // Normalize the content.
                // Strip leading and trailing empty lines and newlines.
                // Remove leading whitespace up to the previously computed limit.
                StringBuilder output = new StringBuilder(count);
                int lastContentPos = 0;
                for (int i = 0, skippedLeading = 0; i < count; )
                {
                    char c = content[i++];

                    if (c == ' ')
                    {
                        skippedLeading += 1;

                        if (skippedLeading > leading)
                            output.Append(' ');
                    }
                    else if (c == '\n')
                    {
                        skippedLeading = 0;

                        if (lastContentPos == 0)
                        {
                            output.Length = 0; // strip leading empty lines
                        }
                        else
                        {
                            output.Append('\n');
                        }
                    }
                    else
                    {
                        skippedLeading = 0;

                        output.Append(c);
                        lastContentPos = output.Length;

                        while (i < count && (c = content[i++]) != '\n')
                        {
                            output.Append(c);
                            if (c != ' ')
                                lastContentPos = output.Length;
                        }

                        output.Length = lastContentPos; // strip trailing whitespace
                        output.Append('\n');
                    }
                }

                output.Length = lastContentPos; // strip trailing empty lines and newlines

                return output.ToString();
            }
        }
    }
}