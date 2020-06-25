using System.Xml;
using System.Collections.Generic;
using MB2Editor.Model;


namespace MB2Editor.Manager
{
    static class ConfigManager
    {
        static Dictionary<string, DataSetConfig> registeredConfig = new Dictionary<string, DataSetConfig>();
        static List<ElementConfig> datasets = new List<ElementConfig>();

        public static ElementConfig[] Datasets { get { return datasets.ToArray(); } }

        public static void Register(string nameSpace, DataSetConfig config)
        {
            registeredConfig.Add(nameSpace, config);
            datasets.AddRange(config.Datasets);
        }

        public static void RegisterFromXsd(XmlDocument doc)
        {
            XSDParser parser = new XSDParser();
            DataSetConfig config = parser.LoadFromXsd(doc);
            Register(config.NameSpace, config);
        }

        public static bool GetConfig(string nameSpace, out DataSetConfig config)
        {
            if (nameSpace != null)
            {
                return registeredConfig.TryGetValue(nameSpace, out config);
            }
            else
            {
                config = null;
                return false;
            }
        }
    }
}
