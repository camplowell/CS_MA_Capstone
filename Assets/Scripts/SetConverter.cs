
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SetConverter : JsonConverter<HashSet<string>>
{
    public override HashSet<string> ReadJson(JsonReader reader, Type objectType, HashSet<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        Debug.Assert(token.Type == JTokenType.Array);
        return new HashSet<string>(token.ToObject<List<string>>());
    }

    public override void WriteJson(JsonWriter writer, HashSet<string> value, JsonSerializer serializer)
    {
        List<string> list = new List<string>(value);
        serializer.Serialize(writer, list.ToArray());
    }
}