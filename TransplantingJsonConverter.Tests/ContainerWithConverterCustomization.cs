using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TransplantingJsonConverter.Internal;

namespace TransplantingJsonConverter.Tests
{
    [JsonTransplantContainer]
    [JsonConverter(typeof(JsonTransplantConverter), typeof(TransplantDefinitionFactory), new object[] { nameof(StringComparer.Ordinal) })]
    public sealed class ContainerWithConverterCustomization : Container
    {
    }
}
