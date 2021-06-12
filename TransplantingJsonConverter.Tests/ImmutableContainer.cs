using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransplantingJsonConverter.Tests
{
    [JsonTransplantContainer]
    [JsonConverter(typeof(JsonTransplantConverter))]
    public class ImmutableContainer
    {
        [JsonConstructor]
        public ImmutableContainer(Rewt rewt, Other1 other1, Other2 other2)
        {
            UsedJsonConstructor = true;
            Rewt = rewt ?? throw new ArgumentNullException(nameof(rewt));
            Other1 = other1 ?? throw new ArgumentNullException(nameof(other1));
            Other2 = other2 ?? throw new ArgumentNullException(nameof(other2));
        }

        public ImmutableContainer(Container container) : this(container.Rewt, container.Other1, container.Other2)
        {
            UsedOtherConstructor = true;
            UsedJsonConstructor = false;
        }

        [JsonIgnore]
        public bool UsedJsonConstructor { get; set; }

        [JsonIgnore]
        public bool UsedOtherConstructor { get; set; }

        [JsonTransplantRoot]
        public Rewt Rewt { get; }

        public Other1 Other1 { get; }

        public Other2 Other2 { get; }
    }
}
