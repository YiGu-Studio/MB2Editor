using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using UnityEditor;
using UnityEngine;
using MB2Editor.EditorView;
using MB2Editor.Manager;
using MB2Editor.Model;


namespace MB2Editor
{
    public class Build : Editor
    {
        // Only for early access
        // reactor this parts
        [MenuItem("MB2 Project/Build")]
        static void SimpleBuild()
        {
            string outputPath;
            if((outputPath = EditorUtility.SaveFolderPanel("Save output to", "", "")) != "")
            {
                float ratio = 0;
                float step = 0;
                if (Directory.EnumerateFileSystemEntries(outputPath).Any())
                {
                    if(!EditorUtility.DisplayDialog("Target not Empty", "The target folder isn't empty, Overwrite?", "Yes", "Cancel"))
                    {
                        return;
                    }
                }

                EditorUtility.DisplayProgressBar("Building Project", "", ratio);

                // Step 1: Create Folder
                EditorUtility.DisplayProgressBar("Building Project", "Create project structure", (ratio += 0.1f));
                string moduleDataFolder = Path.Combine(outputPath, "ModuleData");
                Directory.CreateDirectory(moduleDataFolder);

                // Step 2: Create DataSet
                ElementConfig[] configs = ConfigManager.Datasets;
                step = 0.7f / configs.Length;
                foreach(var config in configs)
                {
                    EditorUtility.DisplayProgressBar("Building Project", "Create Dataset " + config.Name, (ratio += step));

                    MB2CustomEditorView view;
                    if (!ElementViewManager.GetView(config.Name, out view))
                    {
                        view = new DataSetView();
                    }
                    view.Init(config);

                    File.WriteAllText(moduleDataFolder + "/" + config.Name + ".xml", view.GetData());
                }

                // Step 3: Create SubModule.xml
                EditorUtility.DisplayProgressBar("Building Project", "Create SubModule.xml", (ratio = 0.8f));
                CreateSubModule(outputPath + "SubModule.xml");

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Built finished", "Built successfully!", "Ok");
                EditorUtility.RevealInFinder(moduleDataFolder);
            }
            
        }
        
        static void CreateSubModule(string path)
        {
            StringBuilder xml = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(xml);

            //Things go here

            xmlWriter.Close();
        }
    }
}