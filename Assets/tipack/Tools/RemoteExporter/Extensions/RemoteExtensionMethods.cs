using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace tiplay.RemoteExporterTool
{
    public static class RemoteExtensionMethods
    {
#if UNITY_EDITOR
        public async static Task<bool> ExportRemoteVariables(this ScriptableObject _scriptableObject, RemoteData _remoteData, object _object)
        {
            bool hasRemoteVariables = false;
            FieldInfo[] fields = _object.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.IsDefined(typeof(Remote), false))
                {
                    if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Stack<>))
                    {
                        var actualTypes = (System.Collections.IEnumerable)field.GetValue(_object);
                        Type listType = typeof(List<>).MakeGenericType(field.FieldType.GetGenericArguments()[0]);
                        IList deepCopiedList = (IList)Activator.CreateInstance(listType);
                        foreach (var element in actualTypes)
                            deepCopiedList.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(element), element.GetType()));
                        ArrayList.Adapter(deepCopiedList).Reverse();
                        _remoteData.variables[field.Name] = deepCopiedList;
                        hasRemoteVariables = true;
                    }
                    else
                    {
                        object value = field.GetValue(_object);
                        _remoteData.variables[field.Name] = value;
                        hasRemoteVariables = true;
                        // object value = field.GetValue(_object);
                        // if (value is float floatValue)
                        // {
                        //     if (floatValue % 1 == 0)
                        //     {
                        //         _remoteData.variables[field.Name] = (int)floatValue;
                        //         hasRemoteVariables = true;
                        //     }
                        //     else
                        //     {
                        //         _remoteData.variables[field.Name] = floatValue;
                        //         hasRemoteVariables = true;
                        //     }
                        // }
                        // else
                        // {
                        //     _remoteData.variables[field.Name] = value;
                        //     hasRemoteVariables = true;
                        // }
                    }
                }
                else if (!field.IsDefined(typeof(Remote), false))
                {
                    if (field.GetValue(_object) == null) continue;
                    if (field.FieldType.IsPrimitive) continue;
                    if (field.FieldType == typeof(string)) continue;

                    if (field.FieldType.IsGenericType)
                    {
                        if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            bool listHasRemoteData = false;
                            List<object> extractedRemotes = new List<object>();

                            if (typeof(IList).IsAssignableFrom(field.FieldType))
                            {
                                IList list = (IList)field.GetValue(_object);
                                foreach (object element in list)
                                {
                                    if (element == null) continue;

                                    Type elementType = element.GetType();
                                    if (elementType.IsPrimitive) continue;
                                    if (elementType == typeof(string)) continue;

                                    RemoteData listElementRemoteData = new RemoteData();
                                    bool listElementHasRemoteData = await _scriptableObject.ExportRemoteVariables(listElementRemoteData, element);//await ManageRemoteVariables(listElementRemoteData, element);
                                    if (listElementHasRemoteData)
                                    {
                                        listHasRemoteData = true;
                                        extractedRemotes.Add(listElementRemoteData.variables);
                                    }
                                    else if (!listElementHasRemoteData)
                                    {
                                        var data = new Dictionary<string, object> { { "thisElementDoesNotHaveRemoteData", null } };
                                        extractedRemotes.Add(data);
                                    }
                                }
                                if (listHasRemoteData)
                                {
                                    object value = extractedRemotes;
                                    _remoteData.variables[field.Name] = value;
                                    hasRemoteVariables = true;
                                }
                            }
                        }
                        else if (field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if (typeof(IDictionary).IsAssignableFrom(field.FieldType))
                            {
                                IDictionary dictionary = (IDictionary)field.GetValue(_object);

                                bool dictionaryHasRemoteData = false;
                                Dictionary<string, object> extractedRemotes = new Dictionary<string, object>();

                                foreach (var key in dictionary.Keys)
                                {
                                    var value = dictionary[key];
                                    if (value == null) continue;

                                    Type valueType = value.GetType();
                                    if (valueType.IsPrimitive) continue;
                                    if (valueType == typeof(string)) continue;

                                    RemoteData dictionaryElementRemoteData = new RemoteData();
                                    bool dictionaryElementHasRemoteData = await _scriptableObject.ExportRemoteVariables(dictionaryElementRemoteData, value);
                                    if (dictionaryElementHasRemoteData)
                                    {
                                        dictionaryHasRemoteData = true;
                                        extractedRemotes.Add(key.ToString(), dictionaryElementRemoteData.variables);
                                    }
                                }
                                if (dictionaryHasRemoteData)
                                {
                                    object value = extractedRemotes;
                                    _remoteData.variables[field.Name] = value;
                                    hasRemoteVariables = true;
                                }
                            }
                        }
                        else if (field.FieldType.GetGenericTypeDefinition() == typeof(Queue<>))
                        {
                            bool queueHasRemoteData = false;
                            List<object> extractedRemotes = new List<object>();

                            object queueObject = field.GetValue(_object);
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            var queue = (System.Collections.IEnumerable)queueObject;
                            foreach (var element in queue)
                            {
                                if (element == null) continue;
                                Type elementType = element.GetType();
                                if (elementType.IsPrimitive) continue;
                                if (elementType == typeof(string)) continue;

                                RemoteData queueElementRemoteData = new RemoteData();
                                bool queueElementHasRemoteData = await _scriptableObject.ExportRemoteVariables(queueElementRemoteData, element);
                                if (queueElementHasRemoteData)
                                {
                                    queueHasRemoteData = true;
                                    extractedRemotes.Add(queueElementRemoteData.variables);
                                }
                                else if (!queueElementHasRemoteData)
                                {
                                    var data = new Dictionary<string, object> { { "thisElementDoesNotHaveRemoteData", null } };
                                    extractedRemotes.Add(data);
                                }
                            }
                            if (queueHasRemoteData)
                            {
                                object value = extractedRemotes;
                                _remoteData.variables[field.Name] = value;
                                hasRemoteVariables = true;
                            }
                        }
                        else if (field.FieldType.GetGenericTypeDefinition() == typeof(Stack<>))
                        {
                            bool stackHasRemoteData = false;
                            List<object> extractedRemotes = new List<object>();

                            object stackObject = field.GetValue(_object);
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            var stack = (System.Collections.IEnumerable)stackObject;
                            foreach (var element in stack)
                            {
                                if (element == null) continue;
                                Type elementType = element.GetType();
                                if (elementType.IsPrimitive) continue;
                                if (elementType == typeof(string)) continue;

                                RemoteData stackElementRemoteData = new RemoteData();
                                bool stackElementHasRemoteData = await _scriptableObject.ExportRemoteVariables(stackElementRemoteData, element);
                                if (stackElementHasRemoteData)
                                {
                                    stackHasRemoteData = true;
                                    extractedRemotes.Add(stackElementRemoteData.variables);
                                }
                                else if (!stackElementHasRemoteData)
                                {
                                    var data = new Dictionary<string, object> { { "thisElementDoesNotHaveRemoteData", null } };
                                    extractedRemotes.Add(data);
                                }
                            }
                            if (stackHasRemoteData)
                            {
                                extractedRemotes.Reverse();
                                object value = extractedRemotes;
                                _remoteData.variables[field.Name] = value;
                                hasRemoteVariables = true;
                            }
                        }
                    }
                    else if (field.FieldType.IsArray)
                    {
                        bool arrayHasRemoteData = false;
                        List<object> extractedRemotes = new List<object>();
                        Array array = (Array)field.GetValue(_object);
                        for (int i = 0; i < array.Length; i++)
                        {
                            var element = array.GetValue(i);
                            if (element.Equals(null)) continue;

                            Type elementType = element.GetType();

                            if (elementType.IsPrimitive) continue;
                            if (elementType == typeof(string)) continue;

                            RemoteData arrayElementRemoteData = new RemoteData();
                            bool arrayElementHasRemoteData = await _scriptableObject.ExportRemoteVariables(arrayElementRemoteData, element);
                            if (arrayElementHasRemoteData)
                            {
                                arrayHasRemoteData = true;
                                extractedRemotes.Add(arrayElementRemoteData.variables);
                            }
                            else if (!arrayElementHasRemoteData)
                            {
                                var data = new Dictionary<string, object> { { "thisElementDoesNotHaveRemoteData", null } };
                                extractedRemotes.Add(data);
                            }

                        }
                        if (arrayHasRemoteData)
                        {
                            object value = extractedRemotes;
                            _remoteData.variables[field.Name] = value;
                            hasRemoteVariables = true;
                        }
                    }
                    else
                    {
                        object subObject = field.GetValue(_object);
                        RemoteData subRemoteData = new RemoteData();
                        bool hasSubRemoteVariables = await _scriptableObject.ExportRemoteVariables(subRemoteData, subObject);
                        if (hasSubRemoteVariables)
                        {
                            object value = subRemoteData.variables;
                            _remoteData.variables[field.Name] = value;
                            hasRemoteVariables = true;
                        }
                    }
                }
            }
            return hasRemoteVariables;
        }
#endif
    }
    
}