using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTools.src
{
	public class SceneChecker
	{
		private readonly List<ICheck> checks_ = new List<ICheck>();

		public void RegisterCheck(ICheck check) 
		{
			checks_.Add(check);
		}

		public void RunChecksInCurrentScenes(string @namespace)
		{
			RunChecksInScenes(@namespace, GetActiveScenes().Select(scene => scene.path).ToList());
		}
		
		private IEnumerable<Scene> GetActiveScenes()
		{
			var scenes = new List<Scene>();
			for (var i = 0; i < EditorSceneManager.loadedSceneCount; i++)
			{
				scenes.Add(SceneManager.GetSceneAt(i));
			}
			return scenes;
		}

		private void RunChecksInScenes(string @namespace, IEnumerable<string> scenePaths)
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

		private void CheckScene(string @namespace, Scene loadedScene)
		{
			var allMonoBehaviours = loadedScene.GetRootGameObjects().SelectMany(obj => obj.GetComponentsInChildren<MonoBehaviour>()).ToArray();
			
			var allPassed = allMonoBehaviours.Aggregate(true, 
				(current, monoBehaviour) => current & PerformChecks(@namespace, monoBehaviour, monoBehaviour.gameObject));
			
			if (allPassed)
				Debug.Log("Scene passed the check!");
			else
				Debug.LogError("Unity Scene Check did not pass!");
		}

		private bool PerformChecks(string @namespace, object behaviour, object gameObject = null)
		{
			var fields = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			var allPassed = true;
			
			foreach (var check in checks_)
				foreach (var field in fields)
					allPassed &= check.Check(@namespace, behaviour, field, gameObject);

			return allPassed;
		}
	}
}
