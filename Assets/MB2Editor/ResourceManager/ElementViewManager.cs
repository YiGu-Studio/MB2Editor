using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MB2Editor.EditorView;

namespace MB2Editor.Manager
{   
    public static class ElementViewManager
    {

        static Dictionary<string, Dictionary<string, Type>> registeredView = new Dictionary<string, Dictionary<string, Type>>();
        static Dictionary<string, Type> registeredViewGlobal = new Dictionary<string, Type>();
        
        static void checkView(Type view)
        {
            if (!typeof(MB2CustomEditorView).IsAssignableFrom(view))
            {
                throw new Exception(view.Name + "doesn't implement interface MB2CustomEditorView");
            }
        }
        public static void RegisterAssemby(Assembly assembly)
        {
            var allViews = from t in assembly.GetTypes()
                           let attribute = t.GetCustomAttribute<ElementViewRegistry>(false)
                           where attribute != null
                           select new { Type = t, Attribute = attribute as ElementViewRegistry };

            foreach (var view in allViews)
            {
                if (view.Attribute.isGlobal)
                {
                    RegisterGlobal(view.Attribute.element, view.Type);
                }
                else
                {
                    Register(view.Attribute.nameSpace, view.Attribute.element, view.Type);
                }
            }
        }

        public static void RegisterGlobal(string elementName, Type view)
        {
            checkView(view);
            registeredViewGlobal.Add(elementName, view);
        }

        public static void Register(string nameSpace, string elementName, Type view)
        {
            checkView(view);
            Dictionary<string, Type> dict;
            if(!registeredView.TryGetValue(nameSpace, out dict))
            {
                dict = new Dictionary<string, Type>();
                registeredView.Add(nameSpace, dict);
            } 
            dict.Add(elementName, view);
        }

        static MB2CustomEditorView CreateView(Type viewType)
        {
            return Activator.CreateInstance(viewType) as MB2CustomEditorView;
        }

        public static bool GetView(string elementName, out MB2CustomEditorView view, string nameSpace = null)
        {
            if (elementName != null)
            {
                Type viewType;
                if (nameSpace != null)
                { 
                    Dictionary<string, Type> dict;
                    if (registeredView.TryGetValue(nameSpace, out dict))
                    {
                        if (dict.TryGetValue(elementName, out viewType))
                        {
                            view = CreateView(viewType);
                            return true;
                        }
                    }
                }
                if(registeredViewGlobal.TryGetValue(elementName, out viewType))
                {
                    view = CreateView(viewType);
                    return true;
                }
            }
            view = null;
            return false;
         }
    }
}
