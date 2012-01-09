using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PowerMedia.Common.XML
{
    public class XmlTextExtractor
    {
        private XmlDocument document = new XmlDocument();
        private const string Space = " ";
        public void LoadXmlDocument(string filename)
        {
            document.Load(filename);
        }

        public string ExtractText()
        {
            Queue<XmlNode> nodes = new Queue<XmlNode>();
            foreach (var item in document.ChildNodes)
            {
                nodes.Enqueue((XmlNode)item);
            }
            StringBuilder innerTxt = new StringBuilder();
            while (nodes.Count != 0)
            {
                XmlNode currentNode = nodes.Dequeue();
                if (currentNode.NodeType != XmlNodeType.Element && currentNode.NodeType != XmlNodeType.Text)
                {
                    continue;
                }

                if (currentNode.HasChildNodes == true)
                {
                    foreach (var item in currentNode.ChildNodes)
                    {
                        nodes.Enqueue((XmlNode)item);
                    }
                    continue;
                }

                if (currentNode.NodeType != XmlNodeType.Text)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(currentNode.InnerText))
                {
                    continue;
                }
                innerTxt.Append(currentNode.InnerText);
                innerTxt.Append(Space);

            }
            return innerTxt.ToString();
        }
    }
}
