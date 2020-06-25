using System.Xml;
using UnityEngine;
using UnityEditor;
using MB2Editor.EditorView;
using MB2Editor.Manager;
using MB2Editor.ResourceManager;

namespace MB2Editor
{
    [InitializeOnLoad]
    class StartUp
    {
        // Call this function when editor start up
        static StartUp()
        {
            InitLayout();

            ElementViewManager.RegisterAssemby(typeof(StringInput).Assembly);
            //TODO: TEST Only, for NPCCharacter
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.dataPath + @"/MB2Editor/Resources/XSD/NPCCharacters.xsd");
            ConfigManager.RegisterFromXsd(doc);

            UpdateSupportFileType();
        }
        
        static void InitLayout()
        {
            if(EditorUtility.DisplayDialog("Switch Layout", "Switch Layout to MB2 Editor Model?", "Yes", "Keep Current"))
            {
                // need to update
                LayoutUtility.LoadLayoutFromAsset(@"MB2Editor\Resources\layout\developer.layout");
            }
        }

        static void UpdateSupportFileType()
        {
            FileTypeManager.Register("NPCCharacters", "NPCCharacter");
        }
	}
}
