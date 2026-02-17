using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public static class PreserveManager
{
    public async static Task<bool> PreserveData(object source, object target)
    {
        bool hasPreservedFields = false;
        FieldInfo[] sourceFields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] targetFields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < sourceFields.Length && i < targetFields.Length; i++)
        {
            if (targetFields[i].IsDefined(typeof(PreservedField), false) && sourceFields[i] != null)
            {
                if (targetFields[i].GetValue(target) is null || sourceFields[i].GetValue(source) is null) continue;
                Type targetFieldType = targetFields[i].FieldType;
                if (targetFieldType.IsPrimitive || targetFieldType == typeof(string))
                {
                    targetFields[i].SetValue(target, sourceFields[i].GetValue(source));
                    hasPreservedFields = true;
                }
                else if (targetFieldType.IsGenericType)
                {
                    if (targetFieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList sourceList = (IList)sourceFields[i].GetValue(source);
                        IList targetList = (IList)targetFields[i].GetValue(target);

                        for (int j = 0; j < Math.Min(sourceList.Count, targetList.Count); j++)
                        {
                            if (targetList[j] is null || sourceList[j] is null) continue;
                            targetList[j] = sourceList[j];
                            hasPreservedFields = true;
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        IDictionary sourceDictionary = (IDictionary)sourceFields[i].GetValue(source);
                        IDictionary targetDictionary = (IDictionary)targetFields[i].GetValue(target);

                        foreach (object key in targetDictionary.Keys.Cast<object>().ToList())
                        {
                            if (sourceDictionary[key] is null || targetDictionary[key] is null) continue;
                            if (sourceDictionary.Contains(key))
                            {
                                targetDictionary[key] = sourceDictionary[key];
                                hasPreservedFields = true;
                            }
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Queue<>))
                    {
                        Queue sourceQueue = (Queue)sourceFields[i].GetValue(source);
                        Queue targetQueue = (Queue)targetFields[i].GetValue(target);
                        object[] targetArray = targetQueue.ToArray();
                        object[] sourceArray = sourceQueue.ToArray();

                        for (int j = 0; j < Math.Min(targetQueue.Count, sourceQueue.Count); j++)
                        {
                            if (targetArray[j] is null || sourceArray[j] is null) continue;
                            targetArray[j] = sourceArray[j];
                        }
                        targetQueue.Clear();
                        foreach (object item in targetArray)
                        {
                            targetQueue.Enqueue(item);
                            hasPreservedFields = true;
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Stack<>))
                    {
                        Stack sourceStack = (Stack)sourceFields[i].GetValue(source);
                        Stack targetStack = (Stack)targetFields[i].GetValue(target);
                        object[] targetArray = targetStack.ToArray();
                        object[] sourceArray = sourceStack.ToArray();
                        Array.Reverse(targetArray);
                        Array.Reverse(sourceArray);
                        for (int j = 0; j < Math.Min(targetStack.Count, sourceStack.Count); j++)
                        {
                            if (targetArray[j] is null || sourceArray[j] is null) continue;
                            targetArray[j] = sourceArray[j];
                        }
                        targetStack.Clear();
                        foreach (object item in targetArray)
                        {
                            targetStack.Push(item);
                            hasPreservedFields = true;
                        }
                    }
                }
                else if (targetFieldType.IsArray)
                {
                    Array sourceArray = (Array)sourceFields[i].GetValue(source);
                    Array targetArray = (Array)targetFields[i].GetValue(target);
                    for (int j = 0; j < Math.Min(sourceArray.Length, targetArray.Length); j++)
                    {
                        if (sourceArray.GetValue(j) is null || targetArray.GetValue(j) is null) continue;
                        targetArray.SetValue(sourceArray.GetValue(j), j);
                        hasPreservedFields = true;
                    }
                }
                else
                {
                    targetFields[i].SetValue(target, sourceFields[i].GetValue(source));
                    hasPreservedFields = true;
                }
            }
            else if (!targetFields[i].IsDefined(typeof(PreservedField), false))
            {
                Type targetFieldType = targetFields[i].FieldType;
                if (targetFields[i].GetValue(target) is null || sourceFields[i].GetValue(source) is null) continue;
                if (targetFieldType.IsPrimitive || targetFieldType == typeof(string)) continue;

                if (targetFieldType.IsGenericType)
                {
                    if (targetFieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (typeof(IList).IsAssignableFrom(targetFieldType))
                        {
                            IList sourceList = (IList)sourceFields[i].GetValue(source);
                            IList targetList = (IList)targetFields[i].GetValue(target);
                            for (int j = 0; j < sourceList.Count && j < targetList.Count; j++)
                            {
                                object sourceElement = sourceList[j];
                                object targetElement = targetList[j];
                                if (sourceElement is null || targetElement is null) continue;
                                if (targetElement.GetType().IsPrimitive || targetElement.GetType() == typeof(string)) continue;
                                bool listElementHasPreservedField = await PreserveData(sourceElement, targetElement);
                                if (listElementHasPreservedField)
                                    hasPreservedFields = true;
                            }
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        if (typeof(IDictionary).IsAssignableFrom(targetFieldType))
                        {
                            IDictionary sourceDictionary = (IDictionary)sourceFields[i].GetValue(source);
                            IDictionary targetDictionary = (IDictionary)targetFields[i].GetValue(target);
                            foreach (var key in targetDictionary.Keys)
                            {
                                var sourceValue = sourceDictionary[key];
                                var targetValue = targetDictionary[key];
                                if (sourceValue is null || targetValue is null) continue;
                                if (targetValue.GetType().IsPrimitive || targetValue.GetType() == typeof(string)) continue;
                                bool dictionaryHasPreservedField = await PreserveData(sourceValue, targetValue);
                                if (dictionaryHasPreservedField)
                                    hasPreservedFields = true;
                            }
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Queue<>))
                    {
                        var sourceQueue = (Queue)sourceFields[i].GetValue(source);
                        var targetQueue = (Queue)targetFields[i].GetValue(target);
                        if (sourceQueue.Count <= 0 || targetQueue.Count <= 0) continue;
                        var sourceIEnumarable = (IEnumerable)sourceFields[i].GetValue(source);
                        var targetIEnumarable = (IEnumerable)targetFields[i].GetValue(target);
                        var sourceEnumarator = sourceIEnumarable.GetEnumerator();
                        var targetEnumarator = targetIEnumarable.GetEnumerator();
                        while (sourceEnumarator.MoveNext() && targetEnumarator.MoveNext())
                        {
                            var sourceElement = sourceEnumarator.Current;
                            var targetElement = targetEnumarator.Current;
                            if (sourceElement is null || targetElement is null) continue;
                            if (targetElement.GetType().IsPrimitive || targetElement.GetType() == typeof(string)) continue;
                            bool queueElementHasPreservedField = await PreserveData(sourceElement, targetElement);
                            if (queueElementHasPreservedField)
                                hasPreservedFields = true;
                        }
                    }
                    else if (targetFieldType.GetGenericTypeDefinition() == typeof(Stack<>))
                    {
                        var sourceStack = (Stack)sourceFields[i].GetValue(source);
                        var targetStack = (Stack)targetFields[i].GetValue(source);
                        if (sourceStack.Count <= 0 || targetStack.Count <= 0) continue;
                        object[] sourceArray = sourceStack.ToArray();
                        object[] targetArray = targetStack.ToArray();
                        Array.Reverse(sourceArray);
                        Array.Reverse(targetArray);
                        for (int j = 0; j < sourceArray.Length && j < targetArray.Length; j++)
                        {
                            var sourceElement = sourceArray.GetValue(j);
                            var targetElement = targetArray.GetValue(j);
                            if (sourceElement.Equals(null) || targetElement.Equals(null)) continue;
                            if (targetElement.GetType().IsPrimitive || targetElement.GetType() == typeof(string)) continue;
                            bool arrayElementHasPreservedField = await PreserveData(sourceElement, targetElement);
                            if (arrayElementHasPreservedField)
                                hasPreservedFields = true;
                        }
                        // var sourceIEnumarable = (IEnumerable)sourceFields[i].GetValue(source);
                        // var targetIEnumarable = (IEnumerable)targetFields[i].GetValue(target);
                        // var sourceEnumarator = sourceIEnumarable.GetEnumerator();
                        // var targetEnumarator = targetIEnumarable.GetEnumerator();
                        // while (sourceEnumarator.MoveNext() && targetEnumarator.MoveNext())
                        // {
                        //     var sourceElement = sourceEnumarator.Current;
                        //     var targetElement = targetEnumarator.Current;
                        //     if (sourceElement is null || targetElement is null) continue;
                        //     if (targetElement.GetType().IsPrimitive || targetElement.GetType() == typeof(string)) continue;
                        //     bool queueElementHasPreservedField = await PreserveData(sourceElement, targetElement);
                        //     if (queueElementHasPreservedField)
                        //         hasPreservedFields = true;
                        // }
                    }
                }
                else if (targetFieldType.IsArray)
                {
                    Array targetArray = (Array)targetFields[i].GetValue(target);
                    Array sourceArray = (Array)sourceFields[i].GetValue(source);
                    for (int j = 0; j < sourceArray.Length && j < targetArray.Length; j++)
                    {
                        var sourceElement = sourceArray.GetValue(j);
                        var targetElement = targetArray.GetValue(j);
                        if (sourceElement.Equals(null) || targetElement.Equals(null)) continue;
                        if (targetElement.GetType().IsPrimitive || targetElement.GetType() == typeof(string)) continue;
                        bool arrayElementHasPreservedField = await PreserveData(sourceElement, targetElement);
                        if (arrayElementHasPreservedField)
                            hasPreservedFields = true;
                    }
                }
                else
                {
                    object subSourceObject = sourceFields[i].GetValue(source);
                    object subTargetObject = targetFields[i].GetValue(target);
                    bool subObjectHasPreservedField = await PreserveData(subSourceObject, subTargetObject);
                    if (subObjectHasPreservedField)
                        hasPreservedFields = true;
                }
            }
        }
        return hasPreservedFields;
    }
}