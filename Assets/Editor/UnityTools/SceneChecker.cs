using UnityEditor;
using UnityEngine;
using UnityTools.src;

namespace Editor.UnityTools
{
	public class SceneCheckerWindow : EditorWindow 
	{
		private string @namespace = "test";

		[MenuItem("Tools/Scene Check")]
		private static void Init()
		{
			var window = (SceneCheckerWindow)GetWindow(typeof(SceneCheckerWindow), false, "Scene Checker");
			window.Show();
		}
		
		public void OnGUI()
		{
			@namespace = EditorGUILayout.TextField("namespace", @namespace);
			if (GUILayout.Button("Check Scene for Not Set Serialized Fields..."))
			{
				var sceneChecker = new SceneChecker();
				sceneChecker.RegisterCheck(new SerializedFieldSetCheck());
				sceneChecker.RunChecksInCurrentScenes(@namespace);
			}
		}
		
	}
}