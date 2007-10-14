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
using System.Reflection;

namespace MbUnit.Hosting
{
    /// <summary>
    /// Resolves members to XML documentation contents.
    /// </summary>
    public interface IXmlDocumentationResolver
    {
        /// <summary>
        /// Gets the XML documentation for a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The XML documentation for the type, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        string GetXmlDocumentation(Type type);

        /// <summary>
        /// Gets the XML documentation for a field.
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>The XML documentation for the field, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/> is null</exception>
        string GetXmlDocumentation(FieldInfo field);

        /// <summary>
        /// Gets the XML documentation for a property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The XML documentation for the property, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/> is null</exception>
        string GetXmlDocumentation(PropertyInfo property);

        /// <summary>
        /// Gets the XML documentation for an event.
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>The XML documentation for the event, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is null</exception>
        string GetXmlDocumentation(EventInfo @event);

        /// <summary>
        /// Gets the XML documentation for a method or constructor.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The XML documentation for the method, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null</exception>
        string GetXmlDocumentation(MethodBase method);

        /// <summary>
        /// Gets the XML documentation for a member.
        /// </summary>
        /// <param name="member">The member</param>
        /// <returns>The XML documentation for the member, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null</exception>
        string GetXmlDocumentation(MemberInfo member);
    }
}