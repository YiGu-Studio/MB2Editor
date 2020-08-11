using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEditor;
using MB2Editor.Model;
using MB2Editor.Manager;

namespace MB2Editor.EditorView
{
    public class NotSupportView : MB2CustomEditorView
    {
        string data;
        string renderName;
        public string GetData()
        {
            return data;
        }

        public void OnEnable(string data)
        {
            this.data = data;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(renderName, "Not Supported to edit");
            EditorGUILayout.EndHorizontal();
        }

        public void Init(ViewConfig config)
        {
            renderName = config.RenderName;
        }
    }

    // Generic view
    public class ElementView : MB2CustomEditorView
    {
        static XmlWriterSettings xmlSettings = new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment };

        Dictionary<string, MB2CustomEditorView> attributeViews = new Dictionary<string, MB2CustomEditorView>();
        Dictionary<string, MB2CustomEditorView> nestedElementViews = new Dictionary<string, MB2CustomEditorView>();
        Dictionary<string, bool> nestedElementExpanded = new Dictionary<string, bool>();

        ElementConfig elementConfig;
        List<string> activeAttributes;
        List<string> activeElements;
        string name;

        public string GetData()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(xml, xmlSettings);

            xmlWriter.WriteStartElement(elementConfig.Name);

            foreach (var attribute in activeAttributes)
            {
                xmlWriter.WriteAttributeString(attribute, attributeViews[attribute].GetData());
            }


            foreach (var element in activeElements)
            {
                xmlWriter.WriteRaw(nestedElementViews[element].GetData());
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            return xml.ToString();
        }

        public void Init(ViewConfig elementConfig)
        {
            if (elementConfig is ElementConfig)
            {
                this.elementConfig = elementConfig as ElementConfig;
                name = elementConfig.Name;

                foreach (var attribute in BeforeInitAttribute(this.elementConfig.Attributes))
                {
                    MB2CustomEditorView view;
                    if (!ElementViewManager.GetView(attribute.Type, out view))
                    {
                        view = new NotSupportView();
                    }
                    view.Init(attribute);

                    attributeViews.Add(attribute.Name, view);
                }
                AfterInitAttribute(attributeViews);

                foreach (var element in BeforeInitNestedElement(this.elementConfig.NestedElements))
                {
                    MB2CustomEditorView view;
                    if (!ElementViewManager.GetView(element.Name, out view))
                    {
                        view = new ElementView();
                    }
                    view.Init(element);
                    nestedElementExpanded.Add(element.Name, true);
                    nestedElementViews.Add(element.Name, view);
                }
                AfterInitNestedElement(nestedElementViews);
            }
        }

        public void OnEnable(string data)
        {
            activeAttributes = new List<string>();
            activeElements = new List<string>();
            if (data != null && data.Length > 0)
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(data);

                if (xml.DocumentElement.Name != name)
                {
                    throw new Exception("Read Data Error when process element \'" + name + "\', the file may be damaged.");
                }

                Dictionary<string, string> nestedData = new Dictionary<string, string>();
                foreach (XmlNode attr in xml.DocumentElement.Attributes)
                {
                    nestedData.Add(attr.Name, attr.Value);
                }
                AfterProcessAttributeData(nestedData);
                EnableAttributes(nestedData);


                nestedData = new Dictionary<string, string>();
                foreach (XmlNode elem in xml.DocumentElement.ChildNodes)
                {
                    nestedData.Add(elem.Name, elem.OuterXml);
                }
                AfterProcessNestedElementData(nestedData);
                EnableElements(nestedData);
            }
            else
            {
                EnableAttributes();
                EnableElements();
            }
        }

        void EnableAttributes(Dictionary<string, string> nestedData = null)
        {
            foreach (var attr in elementConfig.Attributes)
            {
                bool hasAttribute = nestedData != null ? nestedData.ContainsKey(attr.Name) : false;
                if (hasAttribute || !attr.Optional)
                {
                    activeAttributes.Add(attr.Name);
                    attributeViews[attr.Name].OnEnable(hasAttribute ? nestedData[attr.Name] : null);
                }
            }
        }

        void EnableElements(Dictionary<string, string> nestedData = null)
        {
            foreach (var element in elementConfig.NestedElements)
            {
                bool hasElement = nestedData != null ? nestedData.ContainsKey(element.Name) : false;
                if (hasElement || !element.Optional)
                {
                    activeElements.Add(element.Name);
                    nestedElementViews[element.Name].OnEnable(hasElement ? nestedData[element.Name]: null);
                }
            }
        }

        public void OnGUI()
        {
            // attributes
            AttributeGUI();

            // more Elements
            NestedElementGUI();
        }

        void AttributeGUI()
        {
            foreach (var attribute in activeAttributes)
            {
                AttributeOnGUI(attributeViews[attribute]);
            }
        }

        void NestedElementGUI()
        {
            foreach (var element in activeElements)
            {
                nestedElementExpanded[element] = NestedElementOnGUI(nestedElementViews[element], nestedElementExpanded[element], element);
            }
        }


        // hook Api

        /// <summary>
        /// Called Before initializing attributes of current element
        /// </summary>
        /// <param name="attributes">all attributes for current element</param>
        /// <returns>attributes used for initialization</returns>
        protected virtual AttributeConfig[] BeforeInitAttribute(AttributeConfig[] attributes)
        {
            return attributes;
        }

        /// <summary>
        /// Called After initializing attributes of current element
        /// </summary>
        /// <param name="views">generated views for each attribute</param>
        protected virtual void AfterInitAttribute(Dictionary<string, MB2CustomEditorView> views)
        {
            return;
        }

        /// <summary>
        /// Called Before initializing nested elements of current element
        /// </summary>
        /// <param name="elements">all attributes for current element</param>
        /// <returns>attributes used for initialization</returns>
        protected virtual ElementConfig[] BeforeInitNestedElement(ElementConfig[] elements)
        {
            return elements;
        }

        /// <summary>
        /// Called After initializing nested elements of current element
        /// </summary>
        /// <param name="views">generated views for each element</param>
        protected virtual void AfterInitNestedElement(Dictionary<string, MB2CustomEditorView> views)
        {
            return;
        }

        /// <summary>
        /// Called After processing data for attributes of current element
        /// </summary>
        /// <param name="data">generated data for each attribute</param>
        protected virtual void AfterProcessAttributeData(Dictionary<string, string> data)
        {
            return;
        }

        /// <summary>
        /// Called After processing data for nested elements of current element
        /// </summary>
        /// <param name="data">generated data for each attribute</param>
        protected virtual void AfterProcessNestedElementData(Dictionary<string, string> data)
        {
            return;
        }

        /// <summary>
        /// Called when the attribute view will be render, Call OnGUI on it to render
        /// </summary>
        /// <param name="currentView">the attribute view</param>
        protected virtual void AttributeOnGUI(MB2CustomEditorView currentView)
        {
            currentView.OnGUI();
        }
        
        /// <summary>
        /// Called when the nested element view will be render, Call OnGUI on it to render
        /// </summary>
        /// <param name="currentView">the nested element view</param>
        /// <param name="isOpen">whether the view is expanded by user</param>
        /// <param name="elementName">the name of the element</param>
        /// <returns>whether the view is expanded by user</returns>
        protected virtual bool NestedElementOnGUI(MB2CustomEditorView currentView, bool isOpen, string elementName)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, elementName);
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (isOpen)
            {
                EditorGUI.indentLevel++;
                currentView.OnGUI();
                EditorGUI.indentLevel--;
            }
            return isOpen;
        }

    }

}
