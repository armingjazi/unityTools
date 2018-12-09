using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Tools
{
	public class SceneChecker : EditorWindow 
	{
		private string @namespace = "test";

		[MenuItem("Tools/Scene Check")]
		private static void Init()
		{
			var window = (SceneChecker)GetWindow(typeof(SceneChecker), false, "Scene Checker");
			window.Show();
		}
		
		public void OnGUI()
		{
			@namespace = EditorGUILayout.TextField("namespace", @namespace);
			if (GUILayout.Button("Check Scene for Not Set Serialized Fields..."))
			{
				Checks.Clear();
				RegisterCheck(new SerializedFieldSetCheck());
				RunChecksInCurrentScenes(@namespace);
			}
		}
		
		private static readonly List<ICheck> Checks = new List<ICheck>();

		private static void RegisterCheck(ICheck check) 
		{
			Checks.Add(check);
		}

		private static void RunChecksInCurrentScenes(string @namespace)
		{
			RunChecksInScenes(@namespace, GetActiveScenes().Select(scene => scene.path).ToList());
		}
		
		private static IEnumerable<Scene> GetActiveScenes()
		{
			var scenes = new List<Scene>();
			for (var i = 0; i < EditorSceneManager.loadedSceneCount; i++)
			{
				scenes.Add(SceneManager.GetSceneAt(i));
			}
			return scenes;
		}

		private static void RunChecksInScenes(string @namespace, IEnumerable<string> scenePaths)
		{
			foreach (var scenePath in scenePaths)
			{
				var path = scenePath;
				var loadedScene = GetActiveScenes().FirstOrDefault(scene => scene.path == path);
				if (loadedScene == default(Scene))
				{
					loadedScene = EditorSceneManager.OpenScene(scenePath);
				}

				CheckScene(@namespace, loadedScene);
			}
		}

		private static void CheckScene(string @namespace, Scene loadedScene)
		{
			var allMonoBehaviours = loadedScene.GetRootGameObjects().SelectMany(obj => obj.GetComponentsInChildren<MonoBehaviour>()).ToArray();
			
			var allPassed = allMonoBehaviours.Aggregate(true, 
				(current, monoBehaviour) => current & PerformChecks(@namespace, monoBehaviour, monoBehaviour.gameObject));
			
			if (allPassed)
				Debug.Log("Scene passed the check!");
			else
				Debug.LogError("Unity Scene Check did not pass!");
		}

		private static bool PerformChecks(string @namespace, object behaviour, object gameObject = null)
		{
			var fields = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			var allPassed = true;
			
			foreach (var check in Checks)
				foreach (var field in fields)
					allPassed &= check.Check(@namespace, behaviour, field, gameObject);

			return allPassed;
		}
	}
}