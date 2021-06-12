using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransplantingJsonConverter.Tests
{
    public class Other2
    {
        public int B { get; set; }

        [JsonIgnore]
        public string DontSerialize { get; set; } = "a value";

        public Container NestedContainer { get; set; }
    }
}
