using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestComponent" />.
    /// </summary>
    public abstract class BaseTestComponent : ITestComponent
    {
        private string id;
        private string name;
        private CodeReference codeReference;
        private MetadataMap metadata;

        /// <summary>
        /// Initializes a test component.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition of the component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/>
        /// is null</exception>
        public BaseTestComponent(/*string id, */string name, CodeReference codeReference)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (codeReference == null)
                throw new ArgumentNullException("reflectedDefinition");

            this.id = ""; // interim non-null value until initialized
            this.name = name;
            this.codeReference = codeReference;
            this.metadata = new MetadataMap();
        }

        /// <inheritdoc />
        public string Id
        {
            get { return id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                id = value;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                name = value;
            }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return codeReference; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                codeReference = value;
            }
        }

        /// <inheritdoc />
        public virtual TestComponentInfo ToInfo()
        {
            TestComponentInfo info = new TestComponentInfo();
            PopulateInfo(info);
            return info;
        }

        /// <summary>
        /// Populates the component info structure with information about this component.
        /// </summary>
        /// <param name="info">The component info</param>
        protected void PopulateInfo(TestComponentInfo info)
        {
            info.Name = name;
            info.CodeReference = codeReference.ToInfo();
            info.Metadata = metadata.ToInfo();
        }
    }
}
