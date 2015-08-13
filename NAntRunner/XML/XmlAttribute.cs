using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NAntRunner.XML
{
    /// <summary>
    /// Simple implementation of the PropertyDescriptor abstract class.
    /// This class is responsible to describe a single property visualized
    /// by the PropertyGrid as simple row (property-name and property-value). 
    /// class PropertyDescriptor.
    /// </summary>
    internal class XmlAttribute : PropertyDescriptor
    {   
        /// <summary>
        /// Owner of the key.
        /// </summary>
        private IDictionary<string, object> dictionary;
        
        /// <summary>
        /// Key to describe.
        /// </summary>
        private string key;
        
        /// <summary>
        /// Initialize the key descriptor.
        /// </summary>
        /// <param name="dictionary">Dictionary of attributes.</param>
        /// <param name="key">The key.</param>
        internal XmlAttribute(IDictionary<string, object> dictionary, string key)
            : base(key, null)
        {
            this.dictionary = dictionary;
            this.key = key;
        }
        
        /// <summary>
        /// Get the type of the dictionary entry for key.
        /// </summary>
        public override Type PropertyType
        {
            get { return dictionary[key].GetType(); }
        }

        
        /// <summary>
        /// Set the value for the entry key.
        /// </summary>
        /// <param name="component">Unused.</param>
        /// <param name="value">The value of the key entry.</param>
        public override void SetValue(object component, object value)
        {
            dictionary[key] = value;
        }
        
        /// <summary>
        /// Return the value for the entry key.
        /// </summary>
        /// <param name="component">Unused.</param>
        /// <returns>The value of the key entry.</returns>
        public override object GetValue(object component)
        {
            return dictionary[key];
        }
        
        /// <summary>
        /// Determines whether the key is read only.
        /// </summary>
        public override bool IsReadOnly => false;

        /// <summary>
        /// Get the Component Type.
        /// </summary>
        public override Type ComponentType => null;

        /// <summary>
        /// Determines whether the component can be reseted.
        /// </summary>
        /// <param name="component">Unused.</param>
        /// <returns>False.</returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Resest the component.
        /// </summary>
        /// <param name="component">Unused.</param>
        public override void ResetValue(object component)
        {
        }
        
        /// <summary>
        /// Determines whether the component should be serialized.
        /// </summary>
        /// <param name="component">Unused.</param>
        /// <returns>False.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
