using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MB2Editor.Model
{
    class XSDParser
    {
        DataSetConfig dataSetConfig;
        List<ElementConfig> elements;
        List<ElementConfig> datasets;

        void ParseComplexType(XmlElement element, ElementConfig currentConfig)
        {
            // move deeper
            
            List<ElementConfig> nestedElement = new List<ElementConfig>();
            List<AttributeConfig> attributes = new List<AttributeConfig>();
            foreach (XmlNode ele in element.ChildNodes)
            {
                if (ele is XmlElement)
                {
                    switch (ele.Name)
                    {
                        case "xs:choice":
                            currentConfig.IsContainer = true;
                            foreach (XmlElement e in ele.ChildNodes)
                            {
                                if (e is XmlElement)
                                    nestedElement.Add(ParseElement(e));
                            }
                            break;
                        case "xs:sequence":
                            currentConfig.IsContainer = false;
                            foreach (XmlElement e in ele.ChildNodes)
                            {
                                if (e is XmlElement)
                                    nestedElement.Add(ParseElement(e));
                            }
                            break;
                        case "xs:attribute":
                            AttributeConfig config = new AttributeConfig()
                            {
                                Name = (ele as XmlElement).GetAttribute("name"),
                                Optional = (ele as XmlElement).GetAttribute("use").Equals("optional"),
                                Type = (ele as XmlElement).GetAttribute("type"),
                                Extra = ""
                            };

                            //TODO: read renderName somewhere
                            config.RenderName = config.Name;

                            if (string.IsNullOrEmpty(config.Type))
                            {
                                // type resove
                                if ((ele.FirstChild as XmlElement).Name.Equals("xs:simpleType") && (ele.FirstChild.FirstChild as XmlElement).Name.Equals("xs:restriction"))
                                {
                                    XmlElement restriction = ele.FirstChild.FirstChild as XmlElement;
                                    config.Type = restriction.GetAttribute("base");
                                    config.Extra = restriction.InnerXml;
                                }
                                else
                                {
                                    throw new Exception("Attribute can't be resolved in config file, element " + ele.Name);
                                }
                            }
                            attributes.Add(config);
                            break;
                        default:
                            throw new Exception("Not support element type " + element.Name);
                    }
                }
            }
            currentConfig.Attributes = attributes.ToArray();
            currentConfig.NestedElements = nestedElement.ToArray();
        }

        ElementConfig ParseElement(XmlElement currentElement)
        {
            if (!string.IsNullOrEmpty(currentElement.GetAttribute("ref")))
            {
                // reference
                string reference = currentElement.GetAttribute("ref");
                ElementConfig referElement = elements.First((ele) => ele.Name.Equals(reference));
                if (referElement == null)
                {
                    throw new Exception("No refer element found in the document for ref: " + reference);
                }
                return referElement;
            }
            else
            {
                // create new element
                ElementConfig elementConfig = new ElementConfig();
                if (string.IsNullOrEmpty(elementConfig.Name = currentElement.GetAttribute("name")))
                {
                    throw new Exception("No name found in element");
                }

                elementConfig.DataSetConfig = dataSetConfig;
                elements.Add(elementConfig);

                if (currentElement.GetAttribute("msdata:IsDataSet").Equals("true"))
                {
                    datasets.Add(elementConfig);
                }

                elementConfig.RenderName = elementConfig.Name;
                elementConfig.Optional = false;// currentElement.GetAttribute("minOccurs").Equals("0");

                // nested elements
                foreach (XmlNode ele in currentElement.ChildNodes)
                {
                    if (ele is XmlElement)
                    {
                        switch (ele.Name)
                        {
                            case "xs:complexType":
                                ParseComplexType(ele as XmlElement, elementConfig);
                                break;
                            case "xs:simpleType":
                            default:
                                throw new Exception("Not support element type " + ele.Name);
                        }
                    }
                }

                return elementConfig;
            }
        }

        public DataSetConfig LoadFromXsd(XmlDocument doc)
        {
            // read schema
            try
            {
                XmlElement currentElement = doc.DocumentElement;
                if (!currentElement.Name.Equals("xs:schema"))
                {
                    throw new Exception("No schema found in document");
                }

                dataSetConfig = new DataSetConfig();
                if (string.IsNullOrEmpty(dataSetConfig.NameSpace = currentElement.GetAttribute("id")))
                {
                    throw new Exception("No id found in schema");
                }

                elements = new List<ElementConfig>();
                datasets = new List<ElementConfig>();
                foreach (XmlNode element in currentElement.ChildNodes)
                {
                    if (element is XmlElement)
                    {
                        ParseElement(element as XmlElement);
                    }
                }

                dataSetConfig.elements = elements.ToArray();
                dataSetConfig.Datasets = datasets.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("Parse xsd file failed with error " + e.Message);
            }

            return dataSetConfig;
        }
    }
}
