using System.IO;
using System.Text;
using System.Xml.Serialization;
using MB2Editor.Model;
using UnityEditor;
using UnityEngine;

namespace MB2Editor
{
    public class Build : Editor
    {
        // This part of code is just for internal test 
        // Demo Only
        // How to Serialize Class to xml
        [MenuItem("MB2 Project/Build")]
        static void BuildProject()
        {
            string[] charactersGUIDs = AssetDatabase.FindAssets("t:NPCCharactersModel");
            foreach (string guid in charactersGUIDs)
            {
                NPCCharactersModel nPCCharactersModel = AssetDatabase.LoadAssetAtPath<NPCCharactersModel>(AssetDatabase.GUIDToAssetPath(guid));
                StreamWriter streamWriter = new StreamWriter(new MemoryStream(), Encoding.UTF8);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(NPCCharactersModel));
                xmlSerializer.Serialize(streamWriter, nPCCharactersModel);
                streamWriter.Flush();
                StreamReader reader = new StreamReader(streamWriter.BaseStream);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Debug.Log(reader.ReadToEnd());

            }
        }
    }
}