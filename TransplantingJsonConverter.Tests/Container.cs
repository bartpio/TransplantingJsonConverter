using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TransplantingJsonConverter.Internal;

namespace TransplantingJsonConverter.Tests
{
    [JsonTransplantContainer]
    [JsonConverter(typeof(JsonTransplantConverter))]
    public class Container
    {
        [JsonTransplantRoot]
        public Rewt Rewt { get; set; }

        public Other1 Other1 { get; set; }

        public Other2 Other2 { get; set; }
    }
}
