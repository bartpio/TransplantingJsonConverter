using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransplantingJsonConverter.Tests
{
    [JsonTransplantContainer]
    [JsonConverter(typeof(JsonTransplantConverter))]
    public class ContainerWithConflictingProperty
    {
        [JsonTransplantRoot]
        public RewtWithConflictingProperty Rewt { get; set; }

        public Other1 Other1 { get; set; }

        public Other2 Other2 { get; set; }
    }
}
