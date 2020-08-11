using System.Linq;
using UnityEditor;
using UnityEngine;
using MB2Editor.Model;
using MB2Editor.Manager;
using MB2Editor.ResourceManager;

namespace MB2Editor.EditorView
{
    public enum EditorNotSupport
    {
        Support,
        NoView,
        NoUpdate,
        Unassigned,
    };

    [CustomEditor(typeof(BaseModel), true)]
    public class MB2CustomEditor : Editor
    {
        EditorNotSupport notSupport;
        MB2CustomEditorView view;
        BaseModel model;

        void OnEnable()
        {
            notSupport = EditorNotSupport.Support;
            model = this.target as BaseModel;

            // need to be assigned
            if (model.NameSpace.Equals("MB2Editor") && model.element.Equals("Unassigned"))
            {
                notSupport = EditorNotSupport.Unassigned;
            }
            else
            {
                //TODO: need cache view here for perfermance
                DataSetConfig config;
                ElementConfig elementConfig;
                if (ConfigManager.GetConfig(model.NameSpace, out config) &&
                    ((elementConfig = config.elements.First((ele) => ele.Name.Equals(model.element))) != null))
                {
                    // check version
                    if (model.version != config.version)
                    {
                        if (config.DataUpdate != null)
                        {
                            string updatedData;
                            if (config.DataUpdate.Update(model.element, model.serilizedData, model.version, out updatedData))
                            {
                                model.version = config.version;
                                model.serilizedData = updatedData;
                            }
                            else
                                notSupport = EditorNotSupport.NoUpdate;
                        }
                        else
                            notSupport = EditorNotSupport.NoUpdate;
                    }
                    if (notSupport == EditorNotSupport.Support)
                    {
                        view = new ElementView();
                        view.Init(elementConfig);
                        view.OnEnable(model.serilizedData);
                    }
                }
                else
                {
                    notSupport = EditorNotSupport.NoView;
                }
            }
        }
        void OnAssignTypeGUI()
        {
            EditorGUILayout.LabelField("NameSpace:");
            string[] nameSpaces = FileTypeManager.NameSpaces;
            int index = EditorGUILayout.Popup(0, nameSpaces);
            EditorGUILayout.LabelField("Element:");
            string[] elements = FileTypeManager.GetElement(nameSpaces[index]);
            int index2 = EditorGUILayout.Popup(0, elements);
            if (GUILayout.Button("Create"))
            {
                DataSetConfig config;
                if(ConfigManager.GetConfig(nameSpaces[index], out config))
                {
                    model.NameSpace = nameSpaces[index];
                    model.element = elements[index2];
                    model.version = config.version;
                    AssetDatabase.SetLabels(model, new string[] { model.element + "@" + model.NameSpace });
                    EditorUtility.SetDirty(model);
                    OnEnable();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "The config of namespace " + nameSpaces[index] + "not found, please make sure relative xsd exists.", "Ok");
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (notSupport != EditorNotSupport.Support)
            {
                switch (notSupport)
                {
                    case EditorNotSupport.NoUpdate:
                        EditorGUILayout.HelpBox("The version of data doesn't match current editor", MessageType.Warning);
                        break;
                    case EditorNotSupport.Unassigned:
                        EditorGUILayout.HelpBox("You need to assign the type of data before editing", MessageType.Warning);
                        OnAssignTypeGUI();
                        break;
                    case EditorNotSupport.NoView:
                    default:
                        EditorGUILayout.HelpBox("This Element not support to edit", MessageType.Warning);
                        break;
                }
                
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                view.OnGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    //TODO: Implement REDO/UNDO
                    //Undo.RecordObject(target, "update serialized data");
                    model.serilizedData = view.GetData();
                    EditorUtility.SetDirty(this.target);
                }
            }

        }
    }

}