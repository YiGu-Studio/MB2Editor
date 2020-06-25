using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MB2Editor.Manager;
using MB2Editor.Model;


namespace MB2Editor.EditorView
{
    class DataSetPanel: EditorWindow
    {
        EditorNotSupport notSupport;
        BaseModel model;
        MB2CustomEditorView[] datasetViews;

        public DataSetPanel()
        {
            notSupport = EditorNotSupport.Unassigned;
            titleContent = new GUIContent("DataSet Viewer");
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/DataSetManager")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            DataSetPanel window = (DataSetPanel)GetWindow(typeof(DataSetPanel));
            window.Show();
        }

        void OnGUI()
        {
            if (notSupport != EditorNotSupport.Support)
            {
                switch (notSupport)
                {
                    case EditorNotSupport.NoView:
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No dataset contains current selected module data", MessageType.Warning);
                        break;
                    case EditorNotSupport.Unassigned:
                    default:
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No avaliable Content:\nPlease select a module data file to view", MessageType.Warning);
                        break;
                }
            }
            else
            {

            }
        }


        void OnSelectionChange()
        {
            Object obj = Selection.activeObject;
            if(obj == null || obj is BaseModel)
            {
                OnModelSelected(obj as BaseModel);
            }
        }

        void OnModelSelected(BaseModel target)
        {
            model = target;
            if (model == null || model.NameSpace.Equals("MB2Editor") || model.element.Equals("Unassigned"))
            {
                notSupport = EditorNotSupport.Unassigned;
            }
            else
            {
                DataSetConfig config;
                if(ConfigManager.GetConfig(model.NameSpace, out config))
                {
                    //TODO: need cache view here for perfermance
                    var supportDataSet = config.Datasets.Where((element) => element.NestedElements.Any((nestedElement) => nestedElement.Equals(model.name)));
                    datasetViews = supportDataSet.Select((dataset) =>
                    {
                        MB2CustomEditorView view;
                        if (!ElementViewManager.GetView(dataset.Name, out view))
                        {
                            view = new NotSupportView();
                        }
                        view.Init(dataset);
                        return view;
                    }).ToArray();
                    

                    if (datasetViews.Length > 0)
                    {
                        notSupport = EditorNotSupport.Support;
                    }
                    else
                    {
                        notSupport = EditorNotSupport.NoView;
                    }
                }
                else
                {
                    notSupport = EditorNotSupport.NoView;
                }
            }
        }
    }
}
