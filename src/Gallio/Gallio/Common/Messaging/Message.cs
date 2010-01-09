// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Xml.Serialization;
using Gallio.Common.Normalization;
using Gallio.Common.Validation;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// Base class for all message objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses of this type should be declared to be both XML-serializable
    /// (using <see cref="XmlRootAttribute" /> and <see cref="XmlTypeAttribute" />)
    /// and binary serializable (using <see cref="SerializableAttribute" />).
    /// They should also override <see cref="Validate" /> to validate all message
    /// properties.  Validation is performed before sending and after receiving messages
    /// to enforce basic message integrity constraints that are not checked by the serialization
    /// protocol itself.
    /// </para>
    /// <para>
    /// Message types do not need to be declared in advanced.  Consequently plugins
    /// may defined their own custom message types as long as they have a distinct name.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This is an example of a message type.
    /// </para>
    /// <code><![CDATA[
    /// [Serializable]
    /// [XmlRoot("submitOrderMessage", Namespace = "http://www.fabrikam.org/messages")]
    /// [XmlType(Namespace = "http://www.fabrikam.org/messages")]
    /// public sealed class SubmitOrderMessage : Message
    /// {
    ///     [XmlAttribute("item")]
    ///     public string Item { get; set; }
    ///     
    ///     [XmlAttribute("quantity")]
    ///     public int Quantity { get; set; }
    ///     
    ///     public override void Validate()
    ///     {
    ///         ValidationUtils.ValidateNotNull(Item);
    ///         
    ///         if (Quantity < 1 || Quantity > 10000)
    ///             throw new ValidationException("Quantity should be between 1 and 10000.");
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="IValidatable"/>
    /// <seealso cref="ValidationUtils"/>
    [Serializable]
    public class Message : IValidatable, INormalizable<Message>
    {
        /// <inheritdoc />
        public virtual void Validate()
        {
        }

        /// <inheritdoc />
        public virtual Message Normalize()
        {
            return this;
        }
    }
}
