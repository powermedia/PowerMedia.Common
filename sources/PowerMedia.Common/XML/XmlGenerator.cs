using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PowerMedia.Common.XML
{
    /// <summary>
    /// Base class for XML generators
    /// it`s not static to allow parametrization of derived classes
    /// main functions to implement are GenerateXmlGeneratorsList and GenerateAttributesList
    /// default implementation of GenerateElementsList use GenerateXmlGeneratorsList and generate
    /// XElement for each of generated XMLGenerators
    /// GenerateElementsList can be overrided if needed
    /// </summary>
    
    public abstract class XmlGenerator
    {
        /// <summary>
        /// name for the most outer of element of generated xml
        /// read-only
        /// </summary>
        public abstract string XmlElementName{ get; }
        
        /// <summary>
        /// Generate list of XmlGenerators which are later used for generating descendants elements
        /// </summary>
        /// <returns></returns>
        protected abstract List<XmlGenerator> GenerateXmlGeneratorsList();

        /// <summary>
        /// Generate descendant elements list, default implementation use GenerateXmlGeneratorsList
        /// </summary>
        /// <returns></returns>
        protected virtual List<XElement> GenerateElementsList()
        {
            return (GenerateXmlGeneratorsList()).ConvertAll<XElement>(

                delegate(XmlGenerator xmlGenerator) {return xmlGenerator.GetXML(); });
        }

        /// <summary>
        /// Generate attributes for the main element
        /// </summary>
        /// <returns></returns>
        protected abstract List<XAttribute> GenerateAttributesList();

        /// <summary>
        /// give generated xml, can yield different result
        /// each time (depends on implementation in derived class)
        /// </summary>
        /// <returns></returns>
        public XElement GetXML()
        {
            XElement result = new XElement(XmlElementName, GenerateElementsList());
            result.Add(GenerateAttributesList());
            return result;
        }
    }
}
