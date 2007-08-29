using System;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Describes a step in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="IStep"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class StepInfo : IStep
    {
        private string id;
        private string name;
        private string parentId;
        private string testId;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private StepInfo()
        {
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="id">The step id</param>
        /// <param name="name">The step name</param>
        /// <param name="testId">The test id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/>, <paramref name="name"/>,
        /// or <paramref name="testId"/> is null</exception>
        public StepInfo(string id, string name, string testId)
        {
            if (id == null)
                throw new ArgumentNullException(@"id");
            if (name == null)
                throw new ArgumentNullException(@"name");
            if (testId == null)
                throw new ArgumentNullException(@"testId");

            this.id = id;
            this.name = name;
            this.testId = testId;
        }

        /// <summary>
        /// Copies the contents of a test component.
        /// </summary>
        /// <param name="obj">The model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public StepInfo(IStep obj)
        {
            if (obj == null)
                throw new ArgumentNullException(@"obj");

            id = obj.Id;
            name = obj.Name;
            testId = obj.Test.Id;

            if (obj.Parent != null)
                parentId = obj.Parent.Id;
        }

        /// <summary>
        /// Gets or sets the id of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the parent step.
        /// </summary>
        [XmlAttribute("parentId")]
        public string ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        /// <summary>
        /// Gets or sets the id of the test to which the step belongs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("testId")]
        public string TestId
        {
            get { return testId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testId = value;
            }
        }

        IStep IStep.Parent
        {
            get { throw new NotSupportedException(); }
        }

        ITest IStep.Test
        {
            get { throw new NotSupportedException(); }
        }
    }
}