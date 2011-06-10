using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// DynamicObject derived class for containing an XML Element and all of its child nodes
    /// </summary>
    public class XmlDataObject : Gallio.Framework.Data.DataObjects.DynamicObject
    {
        // Name of the original XML element
        private string elementName = "";

        /// <summary>
        /// Constructor requires passing the original Element Name for printing purposes
        /// </summary>
        public XmlDataObject(string ElementName)
        {
            this.elementName = ElementName;
        }

        #region Accessors
        /// <summary>
        /// Original name of the XML Element
        /// </summary>
        public string ElementName
        {
            get
            {
                return elementName;
            }
        }
        #endregion

        #region One or Many methods
        /// <summary>
        /// Intended usage is for determined XmlDataObjects retrieved from the 
        /// XmlDataObject object graph are repeat Elements
        /// </summary>
        public static bool IsMany(object Subject)
        {
            if (Subject == null)
                throw new ArgumentNullException();

            if (Subject.GetType() == typeof(List<XmlDataObject>))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Logical inverse of the IsMany method
        /// </summary>
        public static bool IsOne(object Subject)
        {
            return !IsMany(Subject);
        }

        /// <summary>
        /// Complements the AsList(this List&lt;XmlDataObject&gt;) extension method - enables polymorphic
        /// treatment of either a single element, or a sequence of elements.
        /// </summary>
        public static List<XmlDataObject> AsList(List<XmlDataObject> Subject)
        {
            return Subject;
        }

        /// <summary>
        /// Complements the AsList(this List&lt;XmlDataObject&gt;) extension method - forces single
        /// elements into a List so we can make the assumption that we're always dealing with lists.
        /// </summary>
        public static List<XmlDataObject> AsList(XmlDataObject Subject)
        {
            return new List<XmlDataObject>() { Subject };
        }
        #endregion

        #region ToString() functionality for XmlDataObject
        private readonly string Indentation = "    ";
        private readonly string NewLine = Environment.NewLine;

        /// <summary>
        /// Override prints Element Name
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(elementName + "  ");
            foreach (KeyValuePair<string, object> Member in this.AttributesAndValue())
                sb.Append(Member.Key + ":" + Member.Value + "  ");

            return sb.ToString();
        }

        /// <summary>
        /// Easy-to-read format dumps one KeyValuePair per line
        /// </summary>
        public virtual string ToStringWithNewLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(elementName + Environment.NewLine);
            foreach (KeyValuePair<string, object> Member in this.storage)
                sb.Append("    " + Member.Key + ": " + Member.Value + "  " + Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// Traverses and prints object graph child nodes to maximum Depth.
        /// Passing a negative value (Depth &lt; 0) will print the entire object graph. 
        /// </summary>
        public string ToString(int Depth)
        {
            return this.ToString(Depth, "");
        }

        /// <summary>
        /// Prints the root element value (innertext) and all of its attributes
        /// </summary>
        protected string ToString(string CurrentIndentation)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CurrentIndentation + elementName + NewLine);
            foreach (KeyValuePair<string, object> Member in this.AttributesAndValue())
                sb.Append(CurrentIndentation + Indentation + Member.Key + ":" + Member.Value + NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// Worker method travesers to Depth in the object graph and prints each of the Dynamic Objects
        /// </summary>
        /// <param name="CurrentIndentation">Indentation of</param>
        /// <param name="Depth">Pass the number of levels-deep we'd like to traverse. Pass -1 to print
        /// the entire object graph.</param>
        protected string ToString(int Depth, string CurrentIndentation)
        {
            if (Depth == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            
            // First print the current node
            sb.Append(this.ToString(CurrentIndentation));

            // Next, we'll print off it's child nodes
            foreach (KeyValuePair<string, object> Member in DistinctChildNodes())
            {
                sb.Append(((XmlDataObject)(Member.Value)).ToString(Depth - 1, CurrentIndentation + Indentation));
            }

            // Finally: cycle through all of the Lists of XmlDataObjects
            foreach (KeyValuePair<string, object> Member in ListsOfRepeatChildNodes())
            {
                List<XmlDataObject> ListOfRepeatNodes = (List<XmlDataObject>)Member.Value;

                foreach (XmlDataObject InnerMember in ListOfRepeatNodes)
                {
                    sb.Append(InnerMember.ToString(Depth - 1, CurrentIndentation + Indentation));
                }
            }

            return sb.ToString();
        }
        #endregion

        #region Convenience accessors for internal use
        /// <returns>Enumerable of KeyValuePairs of names and child XmlDataObject</returns>
        protected IEnumerable<KeyValuePair<string, object>> DistinctChildNodes()
        {
            return storage.Where(x => x.Value.GetType() == typeof(XmlDataObject));
        }

        /// <returns>Enumerable of KeyValuePairs of names and values</returns>
        protected IEnumerable<KeyValuePair<string, object>> AttributesAndValue()
        {
            return this.storage.Where(x => x.Value.GetType() == typeof(string));
        }

        /// <returns>Enumerable of KeyValuePairs of names and all the lists of same-named Elements</returns>
        protected IEnumerable<KeyValuePair<string, object>> ListsOfRepeatChildNodes()
        {
            return storage.Where(x => x.Value.GetType() == typeof(List<XmlDataObject>));
        }
        #endregion
    }
}

