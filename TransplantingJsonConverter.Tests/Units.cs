using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace TransplantingJsonConverter.Tests
{
    public class Units
    {
        public static string Canonicalize(string str)
        {
            if (!(str is null))
            {
                return Regex.Replace(str, @"\s+", "");
            }
            else
            {
                return str;
            }
        }

        [Test]
        public void TestSerializerOutput()
        {
            var cont = new Container();
            cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
            cont.Other1 = new Other1() { A = 1000 };
            cont.Other2 = new Other2() { B = 2000 };
            var jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
            Assert.That(jsonned, Is.Not.Null);
            TestContext.WriteLine(jsonned);

            var expected = @"    {
      ""RootA"": 100,
      ""RootB"": 200,
      ""A"": 101,
      ""B"": 201,
      ""Other1"": {
        ""A"": 1000
      },
      ""Other2"": {
        ""B"": 2000,
        ""NestedContainer"": null
      }
    }
";

            Assert.That(Canonicalize(jsonned), Is.EqualTo(Canonicalize(expected)));
        }

        [Test]
        public void TestDeserialize([Values(false, true)] bool useImmutable)
        {
            // note in this scenario, the props with value 999 are extra nonsense to be ignored
            var jsonned = @"    {
      ""RootA"": 100,
      ""RootB"": 200,
      ""A"": 101,
      ""B"": 201,
      ""RootExtraness"": 999,
      ""Other1"": {
        ""A"": 1000,
        ""B"": 999,
        ""RootA"": 999,
        ""RootB"": 999,
        ""Other1Extraness"": 999
      },
      ""Other2"": {
        ""A"": 999,
        ""B"": 2000,
        ""RootA"": 999,
        ""RootB"": 999,
        ""NestedContainer"": null,
        ""Other2Extraness"": 999
      }
    }
";
            // put deserialized container in dcont
            dynamic dcont;
            if (!useImmutable)
            {
                dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            }
            else
            {
                dcont = JsonConvert.DeserializeObject<ImmutableContainer>(jsonned);
                Assert.That(dcont.UsedJsonConstructor, Is.True);
                Assert.That(dcont.UsedOtherConstructor, Is.False);
            }
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Not.Null);
            Assert.That(dcont.Other2, Is.Not.Null);
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(100));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(200));
            Assert.That(dcont.Rewt.A, Is.EqualTo(101));
            Assert.That(dcont.Rewt.B, Is.EqualTo(201));
            Assert.That(dcont.Other1.A, Is.EqualTo(1000));
            Assert.That(dcont.Other2.B, Is.EqualTo(2000));
        }

        [Test]
        public void TestDeserializeNullOthers()
        {
            var jsonned = @"    {
      ""RootA"": 100,
      ""RootB"": 200,
      ""A"": 101,
      ""B"": 201,
      ""RootExtraness"": 999,
      ""Other1"": null,
      ""Other2"": null
    }
";
            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Null);
            Assert.That(dcont.Other2, Is.Null);
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(100));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(200));
            Assert.That(dcont.Rewt.A, Is.EqualTo(101));
            Assert.That(dcont.Rewt.B, Is.EqualTo(201));
        }

        [Test]
        public void TestDeserializeCompletelyBlank()
        {
            var jsonned = @"{ }";
            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Null);
            Assert.That(dcont.Other2, Is.Null);
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(0));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(0));
            Assert.That(dcont.Rewt.A, Is.EqualTo(0));
            Assert.That(dcont.Rewt.B, Is.EqualTo(0));
        }

        [Test]
        public void TestDeserializeCompletelyNull()
        {
            var jsonned = @"null";
            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Null);
        }

        [Test]
        public void TestSerializeDeserialize([Values(false, true)] bool useConverterCustomization)
        {
            string jsonned;
            {
                var cont = useConverterCustomization ? new ContainerWithConverterCustomization() : new Container();
                cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other1 = new Other1() { A = 1000 };
                cont.Other2 = new Other2() { B = 2000 };
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);
                TestContext.WriteLine(jsonned);
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Not.Null);
            Assert.That(dcont.Other2, Is.Not.Null);
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(100));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(200));
            Assert.That(dcont.Rewt.A, Is.EqualTo(101));
            Assert.That(dcont.Rewt.B, Is.EqualTo(201));
            Assert.That(dcont.Other1.A, Is.EqualTo(1000));
            Assert.That(dcont.Other2.B, Is.EqualTo(2000));
        }

        [Test]
        public void TestSerializeDeserializeMissingOtherprop()
        {
            string jsonned;
            {
                var cont = new Container();
                cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other1 = new Other1() { A = 1000 };  // setting Other1, while Other2 left null
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);
                TestContext.WriteLine(jsonned);
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Not.Null);
            Assert.That(dcont.Other2, Is.Null); // <-- the key check for this particular testcase!
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(100));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(200));
            Assert.That(dcont.Rewt.A, Is.EqualTo(101));
            Assert.That(dcont.Rewt.B, Is.EqualTo(201));
            Assert.That(dcont.Other1.A, Is.EqualTo(1000));
        }

        [Test]
        public void TestSerializeDeserializeMissingRootprop()
        {
            string jsonned;
            {
                var cont = new Container();
                cont.Other1 = new Other1() { A = 1000 };  // note this will go nowhere at all
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);
                TestContext.WriteLine(jsonned);
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Null);
        }

        [Test]
        public void TestSerializeHonorsJsonIgnore()
        {
            string jsonned;
            {
                var cont = new Container();
                cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other2 = new Other2() { B = 9999 };
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);
                TestContext.WriteLine(jsonned);
                Assert.That(jsonned, Does.Not.Contain(nameof(Other2.DontSerialize)));
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
        }

        [Test]
        public void TestSerializeDeserializeWithNestedContainer()
        {
            string jsonned;
            {
                var cont = new Container();
                cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other1 = new Other1() { A = 888 };
                cont.Other2 = new Other2() { B = 9999 };
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);

                var anothercontainer = JsonConvert.DeserializeObject<Container>(jsonned);
                Assert.That(anothercontainer.Other2.NestedContainer, Is.Null);
                anothercontainer.Other1.A = 777;
                cont.Other2.NestedContainer = anothercontainer;
                jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                Assert.That(jsonned, Is.Not.Null);
                TestContext.WriteLine(jsonned);
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<Container>(jsonned);
            Assert.That(dcont, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Not.Null);
            Assert.That(dcont.Other1.A, Is.EqualTo(888));
            Assert.That(dcont.Other2, Is.Not.Null);
            Assert.That(dcont.Other2.NestedContainer, Is.Not.Null);
            Assert.That(dcont.Other2.NestedContainer.Other1, Is.Not.Null);
            Assert.That(dcont.Other2.NestedContainer.Other1.A, Is.EqualTo(777));
        }

        [Test]
        public void TestSerializeConflictingPropertyThrows()
        {
            string jsonned;
            {
                var cont = new ContainerWithConflictingProperty();
                cont.Rewt = new RewtWithConflictingProperty() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other1 = new Other1() { A = 1000 };
                cont.Other2 = new Other2() { B = 2000 };
                var exc = Assert.Throws<JsonTransplantException>(() =>
                {
                    jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                });

                Assert.That(exc.InnerException, Is.InstanceOf<ArgumentException>());
                Assert.That(exc.Message, Does.Contain(nameof(RewtWithConflictingProperty.Other1)));
            }
        }

        [Test]
        public void TestDeserializeImmutable([Values(false, true)] bool serializeImmutableToo)
        {
            string jsonned;
            {
                var cont = new Container();
                cont.Rewt = new Rewt() { RootA = 100, RootB = 200, A = 101, B = 201 };
                cont.Other1 = new Other1() { A = 1000 };
                cont.Other2 = new Other2() { B = 2000 };
                if (serializeImmutableToo)
                {
                    var immu = new ImmutableContainer(cont);
                    Assert.That(immu.UsedJsonConstructor, Is.False);
                    Assert.That(immu.UsedOtherConstructor, Is.True);
                    jsonned = JsonConvert.SerializeObject(immu, Formatting.Indented);
                }
                else
                {
                    jsonned = JsonConvert.SerializeObject(cont, Formatting.Indented);
                }
                Assert.That(jsonned, Is.Not.Null);
            }

            // put deserialized container in dcont
            var dcont = JsonConvert.DeserializeObject<ImmutableContainer>(jsonned);
            Assert.That(dcont, Is.Not.Null);

            Assert.That(dcont.UsedJsonConstructor, Is.True);
            Assert.That(dcont.UsedOtherConstructor, Is.False);

            Assert.That(dcont.Rewt, Is.Not.Null);
            Assert.That(dcont.Other1, Is.Not.Null);
            Assert.That(dcont.Other2, Is.Not.Null);
            Assert.That(dcont.Rewt.RootA, Is.EqualTo(100));
            Assert.That(dcont.Rewt.RootB, Is.EqualTo(200));
            Assert.That(dcont.Rewt.A, Is.EqualTo(101));
            Assert.That(dcont.Rewt.B, Is.EqualTo(201));
            Assert.That(dcont.Other1.A, Is.EqualTo(1000));
            Assert.That(dcont.Other2.B, Is.EqualTo(2000));
        }
    }
}