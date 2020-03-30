using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtension
{
    public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return "{\n" + string.Join(",\n", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }
}
