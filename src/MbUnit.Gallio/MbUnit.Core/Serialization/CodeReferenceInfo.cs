using System;
using System.Xml.Serialization;
using MbUnit.Framework.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a code reference in a portable manner for serialization.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class CodeReferenceInfo
    {
        private string assemblyName;
        private string namespaceName;
        private string typeName;
        private string memberName;
        private string parameterName;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public CodeReferenceInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public CodeReferenceInfo(CodeReference obj)
        {
            assemblyName = obj.Assembly != null ? obj.Assembly.FullName : null;
            namespaceName = obj.Namespace;
            typeName = obj.Type != null ? obj.Type.FullName : null;
            memberName = obj.Member != null ? obj.Member.Name : null;
            parameterName = obj.Parameter != null ? obj.Parameter.Name : null;
        }

        /// <summary>
        /// Gets or sets the assembly name, or null if none.
        /// </summary>
        /// <seealso cref="CodeReference.Assembly"/>
        [XmlAttribute("assembly")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        /// <summary>
        /// Gets or sets the namespace name, or null if none.
        /// </summary>
        /// <seealso cref="CodeReference.Namespace"/>
        [XmlAttribute("namespace")]
        public string NamespaceName
        {
            get { return namespaceName; }
            set { namespaceName = value; }
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name, or null if none.
        /// </summary>
        /// <seealso cref="CodeReference.Type"/>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the member name, or null if none.
        /// </summary>
        /// <seealso cref="CodeReference.Member"/>
        [XmlAttribute("member")]
        public string MemberName
        {
            get { return memberName; }
            set { memberName = value; }
        }

        /// <summary>
        /// Gets or sets the parameter name, or null if none.
        /// </summary>
        /// <seealso cref="CodeReference.Parameter"/>
        [XmlAttribute("parameter")]
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }
    }
}
