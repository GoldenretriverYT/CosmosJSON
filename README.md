# CosmosJSON
CosmosJSON is a fast, cosmos-compatible JSON parser

# Usage
CosmosJSON directly parses into .NET objects; you can not provide a class to parse it into as we do not have access to reflection.

### Parsing
```cs
var parser = new Parser(myJson);
var obj = parser.Parse(); // can throw exceptions; return type is Dictionary<string, object>
```

### Serializing
```cs
var str = Serializer.Serialize(myObj); // can throw exceptions
```

### Types
Whilst CosmosJSON uses plain .NET objects, you can be assured that its one of these:
  - int
  - float
  - string
  - bool
  - null
  - object[] (JsonArray)
  - Dictionary<string, object> (Nested JsonObject)

The same types should be the ones passed to Serializer.Serialize.
