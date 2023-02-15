using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable, JsonConverter(typeof(StringSetConverter))]
public class StringSet: IEnumerable<string> {
    private HashSet<string> mySet = new HashSet<string>();

    public int Count => mySet.Count;

    public StringSet() {}

    public StringSet(IEnumerable<string> values) {
        UnionWith(values);
    }

    public void Add(string value) {
        mySet.Add(value);
    }

    public void UnionWith(IEnumerable<string> values) {
        if (values == null) {
            Debug.Log("null values");
        }
        mySet.UnionWith(values);
    }

    public bool Contains(string value) {
        return mySet.Contains(value);
    }

    public void Remove(string value) {
        mySet.Remove(value);
    }

    /** Returns true if any elements have been removed. **/
    public bool IntersectWith(IEnumerable<string> values) {
        int size = mySet.Count;
        mySet.IntersectWith(values);
        return size != mySet.Count;
    }

    public string ElementAt(int index) {
        return System.Linq.Enumerable.ElementAt(mySet, index);
    }

    public void Clear() {
        mySet.Clear();
    }

    public IEnumerator<string> GetEnumerator()
    {
        return mySet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return mySet.GetEnumerator();
    }
}

public class StringSetConverter : Newtonsoft.Json.JsonConverter<StringSet>
{
    public override StringSet ReadJson(JsonReader reader, Type objectType, StringSet existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        StringSet set = new StringSet();
        JToken token = JToken.Load(reader);
        Debug.Assert(token.Type == JTokenType.Array);
        set.UnionWith(token.ToObject<List<string>>());

        return set;
    }

    public override void WriteJson(JsonWriter writer, StringSet value, JsonSerializer serializer)
    {
        Debug.Log("Writing array");
        List<string> arr = new List<string>(value);
        serializer.Serialize(writer, arr);
    }
}