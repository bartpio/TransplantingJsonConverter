# Transplanting JSON Converter
A JSON converter (for use with [Newtonsoft Json.NET](https://www.newtonsoft.com/json)), that transplants properties from an inner root object, to an outer container object. The serialized JSON looks like that of the inner root object, but augmented with other properties found on the outer container object. Deserialization is supported as well.

# Example
## Outermost container type
A Container type facilitates serialization and deserialization that centers around a particular inner root type, augmented with additional properties.

Container class must be decorated with JsonTransplantContainer attribute, and it is suggested that it also be decorated with a JsonConverter attribute dictating JsonTransplantConverter as shown.

The container class must have exactly one public property decorated with JsonTransplantRoot attribute. The JSON output object will center around "Rewt" and will not actually have a property called "Rewt". It will, however, have properties called "Other1" and "Other2".

```csharp
[JsonTransplantContainer]
[JsonConverter(typeof(JsonTransplantConverter))]
public class Container
{
    [JsonTransplantRoot]
    public Rewt Rewt { get; set; }

    public Other1 Other1 { get; set; }

    public Other2 Other2 { get; set; }
}
```

## Inner root type
The JSON output will center around this type. There's no attribute clutter here.
```csharp
public class Rewt
{
    public int RootA { get; set; }

    public int RootB { get; set; }

    public int A { get; set; }

    public int B { get; set; }
}
```

## Augmentation types
Optionally, standard Newtonsoft attributes may be used to control serialization. 
```csharp
public class Other1
{
    public int A { get; set; }
}

public class Other2
{
    public int B { get; set; }

    [JsonIgnore]
    public string DontSerialize { get; set; } = "a value";
}
```

## JSON output
The output is centered around the "Rewt" type, which is considered the "inner root" in the C# type (as designated using JsonTransplantRoot attribute), but serves as (part of) the outer schema in the JSON output. The "Other1" and "Other2" properties have been serialized as if we weren't using any special converter. Note that properties belonging to the "Other" types (ex. Other1.A) are not grafted on to the root type.

The "A" and "B" properties on the "Other" types are completely unrelated to their namesakes on the "Rewt" type.

```json
{
  "RootA": 100,
  "RootB": 200,
  "A": 101,
  "B": 201,
  "Other1": {
    "A": 1000
  },
  "Other2": {
    "B": 2000
  }
}
```

# TODOS
Use better terminology here. ❝Inner root❞? Really⁇