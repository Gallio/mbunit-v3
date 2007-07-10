namespace MbUnit.Framework
{
    using System;
    
    /// <summary>
    /// Base class for all attributes that are part of the MbUnit framework.
    /// </summary>
    /// <remarks>
    /// Base class for all attributes of MbUnit.
    /// </remarks>
    public class PatternAttribute : System.Attribute
    {
        private string description = null;

        public PatternAttribute()
        { }

        public PatternAttribute(string description)
        {
            this.description = description;
        }

        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
    }
}
