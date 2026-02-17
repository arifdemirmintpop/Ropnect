using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;

namespace tiplay.EditorUtilities
{
    public static class PrefabEditorUtility
    {
        public static T FindSourcePrefab<T>(T instance) where T : UnityEngine.Object
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(instance);
        }

        public static T[] FindAllSourcePrefabsFromActiveScene<T>() where T : UnityEngine.Object
        {
            return GameObject.FindObjectsOfType<T>()
                .Select(PrefabUtility.GetCorrespondingObjectFromSource)
                .ToArray();
        }


        /// <summary>
        /// GameObject listesini prefab ile değiştirir
        /// </summary>
        /// <param name="gameObjects">Sahnede değiştirilecek GameObjectler</param>
        /// <param name="prefab">GameObjectlerin yerine geçecek prefab</param>
        public static void ReplaceAll(GameObject[] gameObjects, GameObject prefab)
        {
            List<GameObject> instances = new List<GameObject>();

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].scene == null || !gameObjects[i].scene.isLoaded)
                    continue;

                GameObject instance = Replace(gameObjects[i], prefab);
                instances.Add(instance);
            }

            Selection.objects = instances.ToArray();
            SceneView.RepaintAll();
        }

        /// <summary>
        /// GameObjecti prefab ile değiştirir
        /// </summary>
        /// <param name="gameObject">Sahnedeki GameObject</param>
        /// <param name="prefab">GameObjectin yerine geçecek prefab</param>
        /// <returns></returns>
        public static GameObject Replace(GameObject gameObject, GameObject prefab)
        {
            Transform instance = ((GameObject)PrefabUtility.InstantiatePrefab(prefab, gameObject.scene)).transform;
            instance.parent = gameObject.transform.parent;
            instance.localPosition = gameObject.transform.localPosition;
            instance.localRotation = gameObject.transform.localRotation;
            instance.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
            Undo.RegisterCreatedObjectUndo(instance.gameObject, "Replacer");
            Undo.DestroyObjectImmediate(gameObject);

            return instance.gameObject;
        }
    }
}