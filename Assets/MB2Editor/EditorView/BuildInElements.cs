using System;
using UnityEditor;
using MB2Editor.Manager;
using MB2Editor.Model;


namespace MB2Editor.EditorView
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ElementViewRegistry : Attribute
    {
        public string nameSpace { get; }
        public string element { get; }
        public bool isGlobal { get; }
        /// <summary>
        /// The information used to register view
        /// </summary>
        /// <param name="nameSpace">the namespace for the element. nullable when isGlobal is true</param>
        /// <param name="element">the element name</param>
        /// <param name="isGlobal">whether register the view as a global view. apply it for all elements with same name if no specific view in its' namespace</param>
        public ElementViewRegistry(string nameSpace, string element, bool isGlobal) => (this.nameSpace, this.element, this.isGlobal) = (nameSpace, element, isGlobal);
    }

    public interface MB2CustomEditorView
    {
        void Init(ViewConfig config);
        void OnEnable(string data);
        void OnGUI();
        string GetData();
    }

    [ElementViewRegistry(null, "xs:string", true)]
    public class StringInput : MB2CustomEditorView
    {
        string renderName;
        string data;
        public string GetData()
        {
            return data;
        }

        public void Init(ViewConfig config)
        {
            renderName = config.RenderName;
        }

        public void OnEnable(string data)
        {
            this.data = data;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(renderName);
            data = EditorGUILayout.TextField(data);
            EditorGUILayout.EndHorizontal();
        }
    }
}
