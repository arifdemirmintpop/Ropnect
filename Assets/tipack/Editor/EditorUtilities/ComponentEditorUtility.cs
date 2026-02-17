using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace tiplay.EditorUtilities
{
    public static class ComponentEditorUtility
    {
        public static Component[] FindAllFromActiveScene()
        {
            return GameObject.FindObjectsOfType<Component>(true);
        }

        public static T[] FindAllFromActiveScene<T>(bool includeReferences) where T : UnityEngine.Object
        {
            Component[] components = FindAllFromActiveScene();
            HashSet<T> foundeds = new HashSet<T>();

            void AddToList(T reference)
            {
                //var source = PrefabEditorUtility.FindSourcePrefab(reference);
                //foundeds.Add(source ?? reference);
                foundeds.Add(reference);
            }

            foreach (var component in components)
            {
                if (component is T)
                {
                    AddToList(component as T);
                    continue;
                }

                if (component.TryGetReference(out T reference))
                {
                    AddToList(reference);
                    continue;
                }
            }

            return foundeds.ToArray();
        }

        private static bool TryGetReference<T>(this Component component, out T reference) where T : UnityEngine.Object
        {
            reference = null;
            SerializedObject serialized = new SerializedObject(component);
            SerializedProperty property = serialized.GetIterator();

            while (property.NextVisible(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                if (property.objectReferenceValue == null)
                    continue;

                if (property.objectReferenceValue is T)
                {
                    reference = property.objectReferenceValue as T;
                    return true;
                }

                if (property.objectReferenceValue is GameObject)
                    if ((property.objectReferenceValue as GameObject).TryGetComponent(out reference))
                        return true;

                if (property.objectReferenceValue is Component)
                    if ((property.objectReferenceValue as Component).TryGetComponent(out reference))
                        return true;
            }

            return false;
        }

        public static Component[] FindReferenced(Component[] components, GameObject source)
        {
            return components.Where(component => component.HasReference(source)).ToArray();
        }

        private static bool HasReference(this Component component, GameObject source)
        {
            SerializedObject serialized = new SerializedObject(component);
            SerializedProperty property = serialized.GetIterator();

            while (property.NextVisible(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                if (property.objectReferenceValue == null)
                    continue;

                if (property.objectReferenceValue is GameObject)
                    if (property.objectReferenceValue == source)
                        return true;

                if (property.objectReferenceValue is Component)
                    if ((property.objectReferenceValue as Component).gameObject == source)
                        return true;
            }

            return false;
        }
    }
}