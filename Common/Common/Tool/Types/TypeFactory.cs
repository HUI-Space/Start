using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Start
{
    public static class TypeFactory
    {
        private static readonly Dictionary<string, Type> _simple = new Dictionary<string, Type>()
        {
            ["bool"] = typeof(TBool),
            ["int"] = typeof(TInt),
            ["uint"] = typeof(TUInt),
            ["float"] = typeof(TFloat),
            ["double"] = typeof(TDouble),
            ["long"] = typeof(TLong),
            ["ulong"] = typeof(TULong),
            ["short"] = typeof(TShort),
            ["ushort"] = typeof(TUShort),
            ["byte"] = typeof(TByte),
            ["sbyte"] = typeof(TSByte),
            ["char"] = typeof(TChar),
            ["string"] = typeof(TString),
            ["DateTime"] = typeof(TDateTime),
            ["FP"] = typeof(TFP),
        };

        private static readonly Dictionary<Type, Type> _complex = new Dictionary<Type, Type>()
        {
            [typeof(bool)] = typeof(TBool),
            [typeof(int)] = typeof(TInt),
            [typeof(uint)] = typeof(TUInt),
            [typeof(float)] = typeof(TFloat),
            [typeof(double)] = typeof(TDouble),
            [typeof(long)] = typeof(TLong),
            [typeof(ulong)] = typeof(TULong),
            [typeof(short)] = typeof(TShort),
            [typeof(ushort)] = typeof(TUShort),
            [typeof(byte)] = typeof(TByte),
            [typeof(sbyte)] = typeof(TSByte),
            [typeof(char)] = typeof(TChar),
            [typeof(string)] = typeof(TString),
            [typeof(DateTime)] = typeof(TDateTime),
            [typeof(FP)] = typeof(TFP),
            [typeof(Array)] = typeof(TArray),
            [typeof(Array[,])] = typeof(TDoubleArray),
            [typeof(List<>)] = typeof(TList),
            [typeof(LinkedList<>)] = typeof(TLinkedList),
            [typeof(HashSet<>)] = typeof(THashSet),
            [typeof(SortedSet<>)] = typeof(TSortedSet),
            [typeof(Queue<>)] = typeof(TQueue),
            [typeof(Stack<>)] = typeof(TStack),
            [typeof(ObservableCollection<>)] = typeof(TObservableCollection),
            [typeof(Dictionary<,>)] = typeof(TDictionary),
            [typeof(SortedDictionary<,>)] = typeof(TSortedDictionary),
            [typeof(ConcurrentDictionary<,>)] = typeof(TConcurrentDictionary),
        };
        
        public static bool TryGetType(Type type,out TType tType)
        {
            tType = null;
            if (_complex.TryGetValue(type, out Type newType))
            {
                tType = (TType)Activator.CreateInstance(newType);
                return true;
            }
            return false;
        }
        
        public static bool TryGetType(Type type,out Type tType)
        { 
            return _complex.TryGetValue(type, out tType);
        }
        

        public static bool TryGetType(string typeString,out TType type)
        {
            type = null;
            if (_simple.TryGetValue(typeString, out Type tType))
            {
                type = (TType)Activator.CreateInstance(tType);
                return true;
            }
            if (typeString.EndsWith("[]"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 2,2);
                if (TryGetType(typeName1,out TType type1))
                {
                    TArray genericType = new TArray();
                    genericType.SetTType(type1);
                    type = genericType;
                    return true;
                }
            }

            if (typeString.EndsWith("[,]"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 3,3);
                if (TryGetType(typeName1,out TType type1))
                {
                    TDoubleArray doubleArray = new TDoubleArray();
                    doubleArray.SetTType(type1);
                    type = doubleArray;
                    return true;
                }
            }
            if (typeString.StartsWith("List<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 5);
                if (TryGetType(typeName1,out TType type1))
                {
                    TList list = new TList();
                    list.SetTType(type1);
                    type = list;
                    return true;
                }
            }
            if (typeString.StartsWith("LinkedList<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 11);
                if (TryGetType(typeName1,out TType type1))
                {
                    TLinkedList linkedList = new TLinkedList();
                    linkedList.SetTType(type1);
                    type = linkedList;
                    return true;
                }
            }
            
            if (typeString.StartsWith("HashSet<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 8);
                if (TryGetType(typeName1,out TType type1))
                {
                    THashSet hashSet = new THashSet();
                    hashSet.SetTType(type1);
                    type = hashSet;
                    return true;
                }
            }
            if (typeString.StartsWith("SortedSet<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 10);
                if (TryGetType(typeName1,out TType type1))
                {
                    TSortedSet sortedSet = new TSortedSet();
                    sortedSet.SetTType(type1);
                    type = sortedSet;
                    return true;
                }
            }
            if (typeString.StartsWith("Queue<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 6);
                if (TryGetType(typeName1,out TType type1))
                {
                    TQueue queue = new TQueue();
                    queue.SetTType(type1);
                    type = queue;
                    return true;
                }
            }
            if (typeString.StartsWith("Stack<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 6);
                if (TryGetType(typeName1,out TType type1))
                {
                    TStack stack = new TStack();
                    stack.SetTType(type1);
                    type = stack;
                    return true;
                }
            }
            
            if (typeString.StartsWith("ObservableCollection<") && typeString.EndsWith(">"))
            {
                string typeName1 = typeString.Remove(typeString.Length - 1, 1).Remove(0, 21);
                if (TryGetType(typeName1,out TType type1))
                {
                    TObservableCollection observableCollection = new TObservableCollection();
                    observableCollection.SetTType(type1);
                    type = observableCollection;
                    return true;
                }
            }
            
            
            if (typeString.StartsWith("Dictionary<") && typeString.EndsWith(">"))
            {
                if (TryParseOuterDictionaryTypes(typeString,"Dictionary",out string outerKeyTypeString, out string outerValueTypeString))
                {
                    if (TryGetType(outerKeyTypeString, out TType type1) && TryGetType(outerValueTypeString, out TType type2))
                    {
                        TDictionary dictionary = new TDictionary();
                        dictionary.SetTType(type1, type2);
                        type = dictionary;
                        return true;
                    }
                }
            }
            
            if (typeString.StartsWith("SortedDictionary<") && typeString.EndsWith(">"))
            { 
                if (TryParseOuterDictionaryTypes(typeString,"SortedDictionary",out string outerKeyTypeString, out string outerValueTypeString))
                {
                    if (TryGetType(outerKeyTypeString, out TType type1) && TryGetType(outerValueTypeString, out TType type2))
                    {
                        TSortedDictionary sortedDictionary = new TSortedDictionary();
                        sortedDictionary.SetTType(type1, type2);
                        type = sortedDictionary;
                        return true;
                    }
                }
            }
            
            if (typeString.StartsWith("ConcurrentDictionary<") && typeString.EndsWith(">"))
            {
                if (TryParseOuterDictionaryTypes(typeString,"ConcurrentDictionary",out string outerKeyTypeString, out string outerValueTypeString))
                {
                    if (TryGetType(outerKeyTypeString, out TType type1) && TryGetType(outerValueTypeString, out TType type2))
                    {
                        TConcurrentDictionary concurrentDictionary = new TConcurrentDictionary();
                        concurrentDictionary.SetTType(type1, type2);
                    }
                }
            }
            return false;
        }
        
        
        private static bool TryParseOuterDictionaryTypes(
            string typeString,
            string typeName,
            out string outerKeyTypeString, 
            out string outerValueTypeString)
        {
            outerKeyTypeString = null;
            outerValueTypeString = null;

            // 基本验证
            if (string.IsNullOrWhiteSpace(typeString))
                return false;

            // 检查是否是Dictionary泛型类型
            string dictPrefix = $"{typeName}<";
            if (!typeString.StartsWith(dictPrefix) || !typeString.EndsWith(">"))
                return false;

            // 提取泛型参数部分（去掉"Dictionary<"和最后的">"）
            string genericContent = typeString.Substring(dictPrefix.Length, typeString.Length - dictPrefix.Length - 1);
            if (string.IsNullOrWhiteSpace(genericContent))
                return false;

            // 分割外层泛型参数（处理嵌套泛型的情况）
            var parameters = SplitGenericParameters(genericContent).ToList();
            if (parameters.Count != 2)
                return false;

            outerKeyTypeString = parameters[0].Trim();
            outerValueTypeString = parameters[1].Trim();
            return true;
        }
        
        /// <summary>
        /// 分割泛型参数（处理嵌套的情况）
        /// </summary>
        private static IEnumerable<string> SplitGenericParameters(string input)
        {
            int startIndex = 0;
            int bracketDepth = 0;  // 用于跟踪尖括号<...>的深度
            int arrayBracketCount = 0;  // 用于跟踪数组括号[]的数量

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
        
                if (c == '<')
                {
                    bracketDepth++;
                }
                else if (c == '>')
                {
                    if (bracketDepth > 0)
                        bracketDepth--;
                }
                else if (c == '[')
                {
                    arrayBracketCount++;
                }
                else if (c == ']')
                {
                    if (arrayBracketCount > 0)
                        arrayBracketCount--;
                }
                // 只有当括号深度为0且没有未闭合的数组括号时，才是顶级参数的分隔符
                else if (c == ',' && bracketDepth == 0 && arrayBracketCount == 0)
                {
                    yield return input.Substring(startIndex, i - startIndex).Trim();
                    startIndex = i + 1;
                }
            }

            // 返回最后一个参数
            if (startIndex <= input.Length - 1)
            {
                string lastParam = input.Substring(startIndex).Trim();
                if (!string.IsNullOrEmpty(lastParam))
                    yield return lastParam;
            }
        }
    }
}