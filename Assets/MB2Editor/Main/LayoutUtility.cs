using System;
using System.IO;
using System.Xml;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace MB2Editor
{
	public static class LayoutUtility
	{

		private static MethodInfo _miLoadWindowLayout;
		private static MethodInfo _miSaveWindowLayout;
		private static MethodInfo _miReloadWindowLayoutMenu;
		private static bool _available;
		private static string _layoutsPath;

		static LayoutUtility()
		{
			Type tyWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
			Type tyEditorUtility = Type.GetType("UnityEditor.EditorUtility,UnityEditor");
			Type tyInternalEditorUtility = Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");
			if (tyWindowLayout != null && tyEditorUtility != null && tyInternalEditorUtility != null)
			{
				MethodInfo miGetLayoutsPath = tyWindowLayout.GetMethod("GetLayoutsPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				_miLoadWindowLayout = tyWindowLayout.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(bool) }, null);
				_miSaveWindowLayout = tyWindowLayout.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
				_miReloadWindowLayoutMenu = tyInternalEditorUtility.GetMethod("ReloadWindowLayoutMenu", BindingFlags.Public | BindingFlags.Static);

				if (miGetLayoutsPath == null || _miLoadWindowLayout == null || _miSaveWindowLayout == null || _miReloadWindowLayoutMenu == null)
					return;

				_layoutsPath = (string)miGetLayoutsPath.Invoke(null, null);
				if (string.IsNullOrEmpty(_layoutsPath))
					return;

				_available = true;
			}
		}

		// Gets a value indicating whether all required Unity API functionality is available for usage.
		public static bool IsAvailable
		{
			get { return _available; }
		}

		// Gets absolute path of layouts directory. Returns `null` when not available.
		public static string LayoutsPath
		{
			get { return _layoutsPath; }
		}

		// Save current window layout to asset file. `assetPath` must be relative to project directory.
		public static void SaveLayoutToAsset(string assetPath)
		{
			SaveLayout(Path.Combine(Application.dataPath, assetPath));
		}

		// Load window layout from asset file. `assetPath` must be relative to project directory.
		public static void LoadLayoutFromAsset(string assetPath)
		{
			if (_miLoadWindowLayout != null)
			{
				string path = Path.Combine(Application.dataPath, assetPath);
				_miLoadWindowLayout.Invoke(null, new object[] { path, true });
			}
		}

		// Save current window layout to file. `path` must be absolute.
		public static void SaveLayout(string path)
		{
			if (_miSaveWindowLayout != null)
				_miSaveWindowLayout.Invoke(null, new object[] { path });
		}
	}
}
