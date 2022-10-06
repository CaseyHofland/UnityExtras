#nullable enable
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityExtras.Editor
{
    public static partial class Menus
    {
        [MenuItem("Window/Find Missing Scripts")]
        private static void FindMissingScripts()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var gameObjects = scene.GetRootGameObjects();

                foreach (var gameObject in gameObjects)
                {
                    FindMissingScripts(gameObject);
                }
            }
            Debug.Log("Find Missing Scripts completed");

            static void FindMissingScripts(GameObject gameObject)
            {
                var components = gameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        var name = gameObject.name;
                        var transform = gameObject.transform;
                        while (transform.parent != null)
                        {
                            name = transform.parent.name + "/" + name;
                            transform = transform.parent;
                        }
                        Debug.Log(name + " has an empty script attached in position: " + i, gameObject);
                    }
                }

                // Now recurse through each child GO (if there are any):
                foreach (Transform childTransform in gameObject.transform)
                {
                    FindMissingScripts(childTransform.gameObject);
                }
            }
        }
    }
}
