using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NAntRunner.XML
{
    /// <summary>
    /// Implementation of XmlNode.
    /// </summary>
    /// 
    public class XmlNode
    {
        // Private attributes
        private IDictionary<string, string> attributes;


        /// <summary>
        /// Initialization of a node.
        /// </summary>
        /// <param name="name">Name of a XML node.</param>
        /// <param name="text">Text of a XML node.</param>
        /// <param name="lineNumber">Start line number of a XML node.</param>
        internal XmlNode(string name, string text, int lineNumber)
        {
            Name = name;
            Text = text;
            LineNumber = lineNumber;
            attributes = new Dictionary<string, string>();
            Children = new List<XmlNode>();
        }

        /// <summary>
        /// Get the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Get the line.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Get the list of children.
        /// </summary>
        public IList<XmlNode> Children { get; }

        /// <summary>
        /// Get the list of attribute name.
        /// </summary>
        public ICollection<string> Attributes => attributes.Keys;

        /// <summary>
        /// Return a type descriptor associated to current node
        /// </summary>
        public ICustomTypeDescriptor Descriptor => new XmlDescriptor(this);
        
        /// <summary>
        /// Get the value of an attribute.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        public string this[string attributeName] => attributes.ContainsKey(attributeName) ? attributes[attributeName] : null;
        
        /// <summary>
        /// Add an attribute.
        /// </summary>
        /// <param name="key">The name of the attribute to add.</param>
        /// <param name="value">The value of the attribute to add.</param>
        public void Add(string key, string value)
        {
            attributes.Add(key, value);
        }
        
        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void Add(XmlNode child)
        {
            Children.Add(child);            
        }
        
        /// <summary>
        /// Add a list of children node.
        /// </summary>
        /// <param name="nodeList">The list of chidren to add.</param>
        public void Add(IList<XmlNode> nodeList)
        {
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                    Add(node);
            }
        }

        /// <summary>
        /// Determines whether the node has the specified attribute.
        /// </summary>
        /// <param name="attributeName">The attribute name to check.</param>
        /// <returns>Returns true if the attribute exists and false if not.</returns>
        public bool HasAttribute(string attributeName)
        {
            return this[attributeName] != null;
        }
        
        /// <summary>
        /// Pretty print output of a node.
        /// </summary>
        /// <returns>The pretty print string.</returns>
        public override string ToString()
        {
            return ToStringRecursive(0);
        }
        
        /// <summary>
        /// Internal method to indent the output of ToString.
        /// </summary>
        /// <param name="level">The level of indentation.</param>
        /// <returns>The print string of a node.</returns>
        public string ToStringRecursive(int level)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Indent
            for (int i = 0; i < level; i++)
                stringBuilder.Append("   ");

            // Name of node
            stringBuilder.Append(Name);

            // List of attributes if any
            if (attributes != null)
            {
                ICollection<string> keys = attributes.Keys;
                foreach (string key in keys)
                {
                    // Attribute name
                    stringBuilder.Append(" ");
                    stringBuilder.Append(key);

                    // Attribute value
                    stringBuilder.Append("=");
                    stringBuilder.Append(attributes[key]);
                }
            }

            stringBuilder.Append("\n");

            // List of children if any
            if (Children != null)
            {
                // Recursively append each child
                foreach (XmlNode child in Children)
                    stringBuilder.Append(child.ToStringRecursive(level + 1));
            }

            return stringBuilder.ToString();
        }
    }
}
