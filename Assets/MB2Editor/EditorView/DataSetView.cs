using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MB2Editor.Model;


namespace MB2Editor.EditorView
{
    class DataSetView : MB2CustomEditorView
    {
        static XmlWriterSettings xmlSettings = new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Document };

        ElementConfig elementConfig;
        string[] baseModels;
        string filter;


        public string GetData()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(xml, xmlSettings);

            xmlWriter.WriteStartElement(elementConfig.Name);

            if (baseModels != null)
            {
                foreach (var element in baseModels)
                {
                    BaseModel model = AssetDatabase.LoadAssetAtPath<BaseModel>(AssetDatabase.GUIDToAssetPath(element));
                    xmlWriter.WriteRaw(model.serilizedData);
                }
            }
            Resources.UnloadUnusedAssets();
            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            return xml.ToString();
        }

        public void Init(ViewConfig config)
        {
            if(config is ElementConfig && (config as ElementConfig).IsContainer)
            {
                elementConfig = config as ElementConfig;
                string nameSpace = elementConfig.DataSetConfig.NameSpace;
                var filters = elementConfig.NestedElements.Select((element) => "l: " + element.Name + "@" + nameSpace);

                if (filters.Any())
                {
                    filter = string.Join(" ", filters);
                }
                else
                {
                    filter = null;
                }
            }

            ForceUpdate();
        }

        public void OnEnable(string data)
        {

        }

        public void OnGUI()
        {
            //TODO: may have GUI later
        }

        public void ForceUpdate()
        {
            baseModels = null;
            if(filter != null) {
                baseModels = AssetDatabase.FindAssets(filter);
            }
        }
    }
}
