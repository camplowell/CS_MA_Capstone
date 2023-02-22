
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable, JsonConverter(typeof(PrototypeDictConverter))]
public class PrototypeDict : IEnumerable<Prototype> {
    private Dictionary<string, Prototype> dict = new Dictionary<string, Prototype>();

    public Dictionary<string, Prototype>.ValueCollection Values => dict.Values;
    public Dictionary<string, Prototype>.KeyCollection Keys => dict.Keys;

    public PrototypeDict() {}
    public PrototypeDict(IEnumerable<Prototype> prototypes) {
        AddAll(prototypes);
    }

    public void Add(Prototype prototype) {
        dict.Add(prototype.key, prototype);
    }

    public void AddAll(IEnumerable<Prototype> prototypes) {
        foreach(Prototype proto in prototypes) {
            Add(proto);
        }
    }

    public IEnumerator<Prototype> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    public Prototype this[string key] {
        get => this.dict[key];
        set {this.dict[key] = value; }
    }
}

public class PrototypeDictConverter : JsonConverter<PrototypeDict> {

    public override PrototypeDict ReadJson(JsonReader reader, Type objectType, PrototypeDict existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        Debug.Assert(token.Type == JTokenType.Array);
        return new PrototypeDict(token.ToObject<List<Prototype>>());
    }

    public override void WriteJson(JsonWriter writer, PrototypeDict value, JsonSerializer serializer)
    {
        List<Prototype> prototypes = new List<Prototype>(value.Values);
        serializer.Serialize(writer, prototypes);
    }
}