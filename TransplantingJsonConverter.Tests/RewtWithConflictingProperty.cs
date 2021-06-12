using System;
using System.Collections.Generic;
using System.Text;

namespace TransplantingJsonConverter.Tests
{
    public class RewtWithConflictingProperty
    {
        public int RootA { get; set; }

        public int RootB { get; set; }

        public int A { get; set; }

        public int B { get; set; }

        public string Other1 { get; set; }
    }
}
