using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable, JsonConverter(typeof(PrototypeCollectionConverter))]
public class PrototypeCollection : IEnumerable<TilePrototype>
{
    private Dictionary<string, TilePrototype> dictionary = new Dictionary<string, TilePrototype>();

    public Dictionary<string, TilePrototype>.ValueCollection Values => dictionary.Values;
    public Dictionary<string, TilePrototype>.KeyCollection Keys => dictionary.Keys;

    public PrototypeCollection() {

    }

    public PrototypeCollection(IEnumerable<TilePrototype> items) {
        AddAll(items);
    }

    public void Add(TilePrototype prototype) {
        dictionary.Add(prototype.protoID, prototype);
    }

    public void AddAll(IEnumerable<TilePrototype> items) {
        foreach(TilePrototype item in items) {
            dictionary.Add(item.protoID, item);
        }
    }

    public bool Intersect(IEnumerable<TilePrototype> items) {
        bool hasTrimmed = false;
        foreach (TilePrototype proto in items) {
            if (Contains(proto.protoID)) {
                hasTrimmed = true;
                Remove(proto.protoID);
            }
        }
        return hasTrimmed;
    }

    public TilePrototype Get(string key) {
        return dictionary[key];
    }

    public TilePrototype this[string key] {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public void Remove(string key) {
        dictionary.Remove(key);
    }

    public bool Contains(string key) {
        return dictionary.ContainsKey(key);
    }

    public IEnumerator<TilePrototype> GetEnumerator()
    {
        return ((IEnumerable<TilePrototype>)Values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Values).GetEnumerator();
    }
}

public class PrototypeCollectionConverter : JsonConverter<PrototypeCollection>
{
    public override PrototypeCollection ReadJson(JsonReader reader, Type objectType, PrototypeCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        Debug.Assert(token.Type == JTokenType.Array);
        return new PrototypeCollection(token.ToObject<List<TilePrototype>>());
    }

    public override void WriteJson(JsonWriter writer, PrototypeCollection value, JsonSerializer serializer)
    {
        List<TilePrototype> prototypes = new List<TilePrototype>(value.Values);
        serializer.Serialize(writer, prototypes);
    }
}
