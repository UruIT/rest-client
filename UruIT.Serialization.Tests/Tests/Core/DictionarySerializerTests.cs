using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public class DictionarySerializerTests
    {
        protected readonly DictionarySerializer serializer;

        public DictionarySerializerTests()
        {
            this.serializer = new DictionarySerializer();
        }

        protected string ConvertToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.GetType() == typeof(bool))
            {
                return ((bool)value).ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower();
            }
            else
            {
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private class EmptyType
        {
        }

        private class SimpleType
        {
            public EmptyType EmptyType { get; set; }
        }

        private class ComplexType
        {
            public string Name { get; set; }

            public int Int { get; set; }

            public float Float { get; set; }

            public bool Bool { get; set; }

            public SimpleType Missing { get; set; }

            public class NestedType
            {
                public string NestedName { get; set; }

                public System.Net.AuthenticationSchemes Authentication { get; set; }
            }

            public NestedType Nested { get; set; }

            public IEnumerable<string> List { get; set; }
        }

        [TestClass]
        public class SerializeObject : DictionarySerializerTests
        {
            private void TestPrimitiveType<T>(T value, Func<T, string> toString = null)
            {
                // act
                var result = serializer.SerializeObject(value);

                // assert
                Assert.AreEqual(1, result.Count, "Serialization of {0}", value);
                Assert.IsTrue(result.ContainsKey(string.Empty));
                if (toString == null)
                {
                    Assert.AreEqual(ConvertToString(value), result[string.Empty]);
                }
                else
                {
                    Assert.AreEqual(toString(value), result[string.Empty]);
                }
            }

            [TestMethod]
            public void WhenPrimitiveTypeThenDictionaryWithSelfValue()
            {
                TestPrimitiveType((short)100);
                TestPrimitiveType((int)12345);
                TestPrimitiveType((long)239524983);
                TestPrimitiveType(true);
                TestPrimitiveType(1325.45f);
                TestPrimitiveType(10000.302d);
                TestPrimitiveType(1325.239m);
                TestPrimitiveType(System.Net.AuthenticationSchemes.Basic);
                TestPrimitiveType(DateTime.Parse("2016/05/06 18:54"));
                TestPrimitiveType("prueba");
                TestPrimitiveType(typeof(ComplexType), x => x.AssemblyQualifiedName);
            }

            private void TestEnumerableType<T>(IEnumerable<T> enumerable)
            {
                // act
                var result = serializer.SerializeObject(enumerable);

                // assert
                Assert.AreEqual(enumerable.Count(), result.Count);
                for (int i = 0; i < enumerable.Count(); i++)
                {
                    var clave = string.Format("[{0}]", i);
                    Assert.IsTrue(result.ContainsKey(clave));
                    Assert.AreEqual(ConvertToString(enumerable.ElementAt(i)), result[clave]);
                }
            }

            [TestMethod]
            public void WhenEnumerableTypeThenDictionaryWithIndexedValues()
            {
                TestEnumerableType(new List<int> { 1, 2, 3, 4, 5, 6, 7 });
                TestEnumerableType(new int[] { 1, 2, 3, 4, 5, 6, 7 });
                TestEnumerableType(new HashSet<int> { 1, 2, 3, 4, 5, 6, 7 });
            }

            [TestMethod]
            public void WhenDictionaryTypeThenDictionaryWithIndexedValues()
            {
                // arrange
                var dictionary = new Dictionary<int, int>
				{
					{ 1, 1 },
					{ 2, 2 },
				};

                // act
                var result = serializer.SerializeObject(dictionary);

                // assert
                Assert.AreEqual(4, result.Count);
                Assert.AreEqual("1", result["[0].Key"]);
                Assert.AreEqual("1", result["[0].Value"]);
                Assert.AreEqual("2", result["[1].Key"]);
                Assert.AreEqual("2", result["[1].Value"]);
            }

            [TestMethod]
            [Ignore]
            public void WhenEmptyTypeThen()
            {
                // arrange
                //TODO: This fails. Investigate
                var values = new Dictionary<OptionStrict<EmptyType>, Dictionary<string, string>>
				{
					{ OptionStrict<EmptyType>.Nothing, new Dictionary<string, string> { { string.Empty, null } } },
					{ new EmptyType(), new Dictionary<string, string> { { string.Empty, "" } } },
				};

                foreach (var value in values)
                {
                    // act
                    var result = serializer.SerializeObject(value.Key.HasValue ? value.Key : (EmptyType)null);

                    // assert
                    Assert.AreEqual(value.Value.Count, result.Count);
                    foreach (var item in value.Value)
                    {
                        Assert.IsTrue(result.ContainsKey(item.Key));
                        Assert.AreEqual(item.Value, result[item.Key]);
                    }
                }
            }

            [TestMethod]
            [Ignore]
            public void WhenSimpleTypeThen()
            {
                //TODO: This fails. Investigate
                OptionStrict<SimpleType> test = OptionStrict<SimpleType>.Nothing;
                var values01 = new Dictionary<OptionStrict<SimpleType>, Dictionary<string, string>>
				{
					{ test, new Dictionary<string, string> { { string.Empty, null } } },
				};

                var values1 = new Dictionary<OptionStrict<SimpleType>, Dictionary<string, string>>
				{
					{ OptionStrict<SimpleType>.Nothing, new Dictionary<string, string> { { string.Empty, null } } },
				};

                // arrange
                var values = new Dictionary<OptionStrict<SimpleType>, Dictionary<string, string>>
				{
					{ OptionStrict<SimpleType>.Nothing, new Dictionary<string, string> { { string.Empty, null } } },
					{ new SimpleType(), new Dictionary<string, string> { { "EmptyType", null } } },
					{ new SimpleType { EmptyType = new EmptyType() }, new Dictionary<string, string> { { "EmptyType", "" } } },
				};

                foreach (var value in values)
                {
                    // act
                    var result = serializer.SerializeObject(value.Key.HasValue ? value.Key : (SimpleType)null);

                    // assert
                    Assert.AreEqual(value.Value.Count, result.Count);
                    foreach (var item in value.Value)
                    {
                        Assert.IsTrue(result.ContainsKey(item.Key));
                        Assert.AreEqual(item.Value, result[item.Key]);
                    }
                }
            }

            [TestMethod]
            public void WhenComplexTypeThenDictionaryWithPropertyNamesValues()
            {
                // arrange
                var complexType = new ComplexType
                {
                    Name = "name string",
                    Int = 1203,
                    Float = 1245.689f,
                    Bool = true,
                    Nested = new ComplexType.NestedType
                    {
                        NestedName = "nested name",
                        Authentication = System.Net.AuthenticationSchemes.Basic,
                    },
                    List = new string[]
					{
						null,
						"#2",
					},
                };

                // act
                var result = serializer.SerializeObject(complexType);

                // assert
                Assert.AreEqual(9, result.Count);
                Assert.AreEqual("name string", result["Name"]);
                Assert.AreEqual("1203", result["Int"]);
                Assert.AreEqual("1245.689", result["Float"]);
                Assert.AreEqual("true", result["Bool"]);
                Assert.AreEqual(null, result["Missing"]);
                Assert.AreEqual("nested name", result["Nested.NestedName"]);
                Assert.AreEqual("Basic", result["Nested.Authentication"]);
                Assert.AreEqual(null, result["List[0]"]);
                Assert.AreEqual("#2", result["List[1]"]);
            }

            [TestMethod]
            public void WhenOptionNothingThenDictionaryWithNothing()
            {
                serializer.AddConverter(new DictionaryConverters.OptionConverter());

                // act
                OptionStrict<object> optValue = OptionStrict<object>.Nothing;
                var result = serializer.SerializeObject(optValue);

                // assert
                Assert.AreEqual(1, result.Count, "Serialización de {0}", optValue);
                Assert.IsTrue(result.ContainsKey("Nothing"));
                Assert.AreEqual(string.Empty, result["Nothing"]);
            }

            [TestMethod]
            public void WhenOptionJustThenDictionaryWithValue()
            {
                serializer.AddConverter(new DictionaryConverters.OptionConverter());

                // act
                TestPrimitiveType(new JustStrict<short>(((short)100)));
                TestPrimitiveType(new JustStrict<int>((int)12345));
                TestPrimitiveType(new JustStrict<long>((long)239524983));
                TestPrimitiveType(new JustStrict<float>(1325.45f), f => f.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                TestPrimitiveType(new JustStrict<double>(10000.302d), d => d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                TestPrimitiveType(new JustStrict<decimal>(1325.239m), d => d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestClass]
        public class DeserializeObject : DictionarySerializerTests
        {
            private void TestPrimitiveType<T>(Dictionary<string, T> dictionary, Func<T, string> toString = null)
            {
                // arrange
                var dictString = dictionary.ToDictionary(x => x.Key, x => toString == null ? ConvertToString(x.Value) : toString(x.Value));

                // act
                var result = serializer.DeserializeObject<T>(dictString);

                // assert
                Assert.AreEqual(dictionary.First().Value, result);
            }

            [TestMethod]
            public void WhenTargetPrimitiveTypeThenValues()
            {
                TestPrimitiveType(new Dictionary<string, short> { { string.Empty, (short)100 } });
                TestPrimitiveType(new Dictionary<string, int> { { string.Empty, (int)12345 } });
                TestPrimitiveType(new Dictionary<string, long> { { string.Empty, (long)239524983 } });
                TestPrimitiveType(new Dictionary<string, bool> { { string.Empty, true } });
                TestPrimitiveType(new Dictionary<string, float> { { string.Empty, 1325.45f } });
                TestPrimitiveType(new Dictionary<string, double> { { string.Empty, 10000.302d } });
                TestPrimitiveType(new Dictionary<string, decimal> { { string.Empty, 1325.239m } });
                TestPrimitiveType(new Dictionary<string, DateTime> { { string.Empty, DateTime.Parse("2016/05/06 18:54") } });
                TestPrimitiveType(new Dictionary<string, System.Net.AuthenticationSchemes> { { string.Empty, System.Net.AuthenticationSchemes.Basic } });
                TestPrimitiveType(new Dictionary<string, string> { { string.Empty, "prueba" } });
                TestPrimitiveType(new Dictionary<string, Type> { { string.Empty, typeof(ComplexType) } }, x => x.AssemblyQualifiedName);
            }

            private void TestEnumerableType<E>(Dictionary<string, string> dictionary)
                where E : IEnumerable<int>
            {
                // act
                var result = serializer.DeserializeObject<E>(dictionary);

                // assert
                Assert.AreEqual(dictionary.Count, result.Count());
                for (int i = 0; i < dictionary.Count; i++)
                {
                    Assert.AreEqual(i + 1, result.ElementAt(i));
                }
            }

            [TestMethod]
            public void WhenTargetEnumerableTypesThenListedValues()
            {
                // arrange
                var dictionary = new Dictionary<string, string>
				{
					{ "[0]", "1" },
					{ "[1]", "2" },
					{ "[2]", "3" },
					{ "[3]", "4" },
					{ "[4]", "5" },
					{ "[5]", "6" },
					{ "[6]", "7" },
				};

                TestEnumerableType<List<int>>(dictionary);
                TestEnumerableType<int[]>(dictionary);
                TestEnumerableType<HashSet<int>>(dictionary);
            }

            [TestMethod]
            public void WhenTargetDictionaryTypeThenDictionaryValues()
            {
                // arrange
                var dictionary = new Dictionary<string, string>
				{
					{ "[0].Key", "1" },
					{ "[0].Value", "1" },
					{ "[1].Key", "2" },
					{ "[1].Value", "2" },
				};

                // act
                var result = serializer.DeserializeObject<Dictionary<int, int>>(dictionary);

                // assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(1, result[1]);
                Assert.AreEqual(2, result[2]);
            }

            [TestMethod]
            public void WhenTargetEmptyTypeThenValues()
            {
                // arrange
                var values = new Dictionary<Dictionary<string, string>, EmptyType>
				{
					{ new Dictionary<string, string> { { string.Empty, null } }, null },
					{ new Dictionary<string, string> { { string.Empty, "" } }, new EmptyType() },
				};

                foreach (var value in values)
                {
                    // act
                    var result = serializer.DeserializeObject<EmptyType>(value.Key);

                    // assert
                    Assert.AreEqual(value.Value != null, result != null);
                }
            }

            [TestMethod]
            public void WhenTargetSimpleTypeThenValues()
            {
                // arrange
                var values = new Dictionary<Dictionary<string, string>, SimpleType>
				{
					{ new Dictionary<string, string> { { string.Empty, null } }, null },
					{ new Dictionary<string, string> { { "EmptyType", null } }, new SimpleType() },
					{ new Dictionary<string, string> { { "EmptyType", "" } }, new SimpleType { EmptyType = new EmptyType() } },
				};

                foreach (var value in values)
                {
                    // act
                    var result = serializer.DeserializeObject<SimpleType>(value.Key);

                    // assert
                    Assert.AreEqual(value.Value != null, result != null);
                    if (value.Value != null)
                    {
                        Assert.AreEqual(value.Value.EmptyType != null, result.EmptyType != null);
                    }
                }
            }

            [TestMethod]
            public void WhenTargetComplexTypeThenComplexValues()
            {
                // arrange
                var dictionary = new Dictionary<string, string>
				{
					{ "Name", "name" },
					{ "Int", "1203" },
					{ "Float", "1245.689" },
					{ "Bool", "true" },
					{ "Missing", null },
					{ "Nested.NestedName", "nested name" },
					{ "Nested.Authentication", "Basic" },
					{ "List[0]", null },
					{ "List[1]", "#2" },
				};

                // act
                var result = serializer.DeserializeObject<ComplexType>(dictionary);

                // assert
                Assert.AreEqual("name", result.Name);
                Assert.AreEqual(1203, result.Int);
                Assert.AreEqual(1245.689f, result.Float);
                Assert.AreEqual(true, result.Bool);
                Assert.AreEqual(null, result.Missing);
                Assert.AreEqual("nested name", result.Nested.NestedName);
                Assert.AreEqual(System.Net.AuthenticationSchemes.Basic, result.Nested.Authentication);
                Assert.AreEqual(2, result.List.Count());
                Assert.AreEqual(null, result.List.ElementAt(0));
                Assert.AreEqual("#2", result.List.ElementAt(1));
            }

            [TestMethod]
            public void WhenTargetOptionNothingThenNothing()
            {
                // arrange
                serializer.AddConverter(new DictionaryConverters.OptionConverter());

                var dictString = new Dictionary<string, string> { { "Nothing", string.Empty } };

                // act
                var result = serializer.DeserializeObject<OptionStrict<int>>(dictString);

                // assert
                Assert.IsFalse(result.HasValue);
            }

            [TestMethod]
            public void WhenTargetOptionJustThenJust()
            {
                // arrange
                serializer.AddConverter(new DictionaryConverters.OptionConverter());

                // act
                TestPrimitiveType(new Dictionary<string, OptionStrict<short>> { { string.Empty, new JustStrict<short>((short)100) } });
                TestPrimitiveType(new Dictionary<string, OptionStrict<int>> { { string.Empty, new JustStrict<int>((int)12345) } });
                TestPrimitiveType(new Dictionary<string, OptionStrict<long>> { { string.Empty, new JustStrict<long>((long)239524983) } });
                TestPrimitiveType(new Dictionary<string, OptionStrict<float>> { { string.Empty, new JustStrict<float>(1325.45f) } }, x => x.Value.ToString(CultureInfo.InvariantCulture));
                TestPrimitiveType(new Dictionary<string, OptionStrict<double>> { { string.Empty, new JustStrict<double>(10000.302d) } }, x => x.Value.ToString(CultureInfo.InvariantCulture));
                TestPrimitiveType(new Dictionary<string, OptionStrict<decimal>> { { string.Empty, new JustStrict<decimal>(1325.239m) } }, x => x.Value.ToString(CultureInfo.InvariantCulture));
                TestPrimitiveType(new Dictionary<string, OptionStrict<string>> { { string.Empty, new JustStrict<string>("prueba") } });
            }
        }
    }
}