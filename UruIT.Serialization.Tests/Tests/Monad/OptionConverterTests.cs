using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Newtonsoft.Json;
using System;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public class OptionConverterTests
    {
        protected IJsonSerializer jsonSerializer;

        protected OptionConverterTests()
        {
            jsonSerializer = new JsonSerializer();
            jsonSerializer.Settings.Formatting = Formatting.None;
        }

        [TestClass]
        public class BasicOptionConverter : OptionConverterTests
        {
            protected BasicOptionConverter()
                : base()
            {
                jsonSerializer.Settings.Converters.Add(new JsonConverters.OptionConverter());
            }

            [TestClass]
            public class Serialization : BasicOptionConverter
            {
                [TestMethod]
                public void WhenJustWithPrimitiveTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<int> data = 10;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":10}", json);
                }

                [TestMethod]
                public void WhenNothingWithPrimitiveTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<int> data = OptionStrict<int>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }

                [TestMethod]
                public void WhenAnotherTypeThenSerializesOk()
                {
                    // Arrange
                    int data = 10;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("10", json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample1> data = new OptionConverterExample1()
                    {
                        Value1 = 10,
                        Value2 = "Value"
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":{\"Value1\":10,\"Value2\":\"Value\"}}", json);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample1> data = OptionStrict<OptionConverterExample1>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }

                [TestMethod]
                public void WhenClassTypeWithNestedOptionThenSerializesOk()
                {
                    // Arrange
                    OptionConverterExample2 data = new OptionConverterExample2()
                    {
                        Value1 = 10,
                        Value2 = new OptionConverterExample2()
                        {
                            Value1 = 20,
                            Value2 = OptionStrict<OptionConverterExample2>.Nothing
                        }
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Value1\":10,\"Value2\":{\"Just\":{\"Value1\":20,\"Value2\":null}}}", json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeWithNestedOptionThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample2> data = new OptionConverterExample2()
                    {
                        Value1 = 10,
                        Value2 = new OptionConverterExample2()
                        {
                            Value1 = 20,
                            Value2 = OptionStrict<OptionConverterExample2>.Nothing
                        }
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":{\"Value1\":10,\"Value2\":{\"Just\":{\"Value1\":20,\"Value2\":null}}}}", json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeWithNestedOptionWithNothingThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample2> data = new OptionConverterExample2()
                    {
                        Value1 = 10,
                        Value2 = OptionStrict<OptionConverterExample2>.Nothing
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":{\"Value1\":10,\"Value2\":null}}", json);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeWithNestedOptionThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample2> data = OptionStrict<OptionConverterExample2>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }

                [TestMethod]
                public void WhenNestedOptionThenSerializesOkCase1()
                {
                    // Arrange
                    OptionStrict<OptionStrict<int>> data = new JustStrict<OptionStrict<int>>(new JustStrict<int>(10));

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":{\"Just\":10}}", json);
                }

                [TestMethod]
                public void WhenNestedOptionThenSerializesOkCase2()
                {
                    // Arrange
                    OptionStrict<OptionStrict<int>> data = new JustStrict<OptionStrict<int>>(OptionStrict<int>.Nothing);

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Just\":null}", json);
                }

                [TestMethod]
                public void WhenNestedOptionThenSerializesOkCase3()
                {
                    // Arrange
                    OptionStrict<OptionStrict<int>> data = OptionStrict<OptionStrict<int>>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }
            }

            [TestClass]
            public class Deserialization : BasicOptionConverter
            {
                [TestMethod]
                public void WhenJustAndPrimitiveTypeThenDeserializeOk()
                {
                    // Arrange
                    string json = "{\"Just\":10}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.AreEqual(10, data.Value);
                }

                [TestMethod]
                public void WhenNothingAndPrimitiveTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenAnotherTypeThenDoesntDeserialize()
                {
                    // Arrange
                    string json = @"10";

                    // Act
                    var data = jsonSerializer.DeserializeObject<int>(json);

                    // Assert
                    Assert.AreEqual(10, data);
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentException))]
                public void WhenInvalidJsonThenDeserializesWithErrorCase1()
                {
                    // Arrange
                    string json = "[1,2,3]";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentException))]
                public void WhenInvalidJsonThenDeserializesWithErrorCase2()
                {
                    // Arrange
                    string json = "3";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentException))]
                public void WhenInvalidJsonThenDeserializesWithErrorCase3()
                {
                    // Arrange
                    string json = "\"AnotherValue\"";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentException))]
                public void WhenInvalidJsonThenDeserializesWithErrorCase4()
                {
                    // Arrange
                    string json = "{\"AnotherValue\":10}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentException))]
                public void WhenInvalidJsonThenDeserializesWithErrorCase5()
                {
                    // Arrange
                    string json = "{\"Just\":10,\"AnotherValue\":22}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Just\":{\"Value1\":10,\"Value2\":\"Value\"}}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample1>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsNotNull(data.Value);
                    Assert.AreEqual(10, data.Value.Value1);
                    Assert.AreEqual("Value", data.Value.Value2);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample1>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenJustAndClassTypeWithNestedOptionWithJustThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Just\":{\"Value1\":10,\"Value2\":{\"Just\":{\"Value1\":20,\"Value2\":null}}}}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample2>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsNotNull(data.Value);
                    Assert.AreEqual(10, data.Value.Value1);
                    Assert.IsTrue(data.Value.Value2.HasValue);
                    Assert.AreEqual(20, data.Value.Value2.Value.Value1);
                    Assert.IsFalse(data.Value.Value2.Value.Value2.HasValue);
                }

                [TestMethod]
                public void WhenJustAndClassTypeWithNestedOptionWithNothingThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Just\":{\"Value1\":10,\"Value2\":null}}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample2>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsNotNull(data.Value);
                    Assert.AreEqual(10, data.Value.Value1);
                    Assert.IsFalse(data.Value.Value2.HasValue);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeWithNestedOptionThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample2>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenNestedOptionThenDeserializesOkCase1()
                {
                    // Arrange
                    string json = "{\"Just\":{\"Just\":10}}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionStrict<int>>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsTrue(data.Value.HasValue);
                    Assert.AreEqual(10, data.Value.Value);
                }

                [TestMethod]
                public void WhenNestedOptionThenDeserializesOkCase2()
                {
                    // Arrange
                    string json = "{\"Just\":null}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionStrict<int>>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsFalse(data.Value.HasValue);
                }

                [TestMethod]
                public void WhenNestedOptionThenDeserializesOkCase3()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionStrict<int>>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }
            }
        }

        [TestClass]
        public class OptionAsNullConverterTests : OptionConverterTests
        {
            protected OptionAsNullConverterTests()
                : base()
            {
                jsonSerializer.Settings.Converters.Add(new JsonConverters.OptionAsNullConverter());
            }

            [TestClass]
            public class Serialization : OptionAsNullConverterTests
            {
                [TestMethod]
                public void WhenJustAndPrimitiveTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<int> data = 10;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("10", json);
                }

                [TestMethod]
                public void WhenNothingAndPrimitiveTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<int> data = OptionStrict<int>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }

                [TestMethod]
                public void WhenAnotherTypeThenDoesntSerialize()
                {
                    // Arrange
                    int data = 10;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("10", json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample1> data = new OptionConverterExample1()
                    {
                        Value1 = 10,
                        Value2 = "Value"
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Value1\":10,\"Value2\":\"Value\"}", json);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeThenSerializesOk()
                {
                    // Arrange
                    OptionStrict<OptionConverterExample1> data = OptionStrict<OptionConverterExample1>.Nothing;

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("null", json);
                }

                [TestMethod]
                public void WhenClassWithNullAndClassTypeThenSerializesOk()
                {
                    // Arrange
                    OptionConverterExample2 data = new OptionConverterExample2()
                    {
                        Value1 = 10,
                        Value2 = OptionStrict<OptionConverterExample2>.Nothing
                    };

                    // Act
                    string json = jsonSerializer.SerializeObject(data);

                    // Assert
                    Assert.AreEqual("{\"Value1\":10,\"Value2\":null}", json);
                }
            }

            [TestClass]
            public class Deserialization : OptionAsNullConverterTests
            {
                [TestMethod]
                public void WhenJustAndPrimitiveTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "10";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.AreEqual(10, data.Value);
                }

                [TestMethod]
                public void WhenNothingAndPrimitiveTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenNothingAndPrimitiveTypeAndSettingsWithNullAllowedThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";
                    jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenNothingAndPrimitiveTypeWithSettingByDefaultThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";
                    jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.Default);

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenAnotherTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "10";

                    // Act
                    var data = jsonSerializer.DeserializeObject<int>(json);

                    // Assert
                    Assert.AreEqual(10, data);
                }

                [TestMethod]
                [ExpectedException(typeof(JsonReaderException))]
                public void WhenInvalidJsonThenDeserializationThrowsErrorCase1()
                {
                    // Arrange
                    string json = "[1,2,3]";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(JsonReaderException))]
                public void WhenInvalidJsonThenDeserializationThrowsErrorCase2()
                {
                    // Arrange
                    string json = "\"AnotherValue\"";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                [ExpectedException(typeof(JsonReaderException))]
                public void WhenInvalidJsonThenDeserializationThrowsErrorCase3()
                {
                    // Arrange
                    string json = "{\"AnotherValue\":10}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<int>>(json);
                }

                [TestMethod]
                public void WhenJustAndClassTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Value1\":10,\"Value2\":\"Value\"}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample1>>(json);

                    // Assert
                    Assert.IsTrue(data.HasValue);
                    Assert.IsNotNull(data.Value);
                    Assert.AreEqual(10, data.Value.Value1);
                    Assert.AreEqual("Value", data.Value.Value2);
                }

                [TestMethod]
                public void WhenNothingAndClassTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "null";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionStrict<OptionConverterExample1>>(json);

                    // Assert
                    Assert.IsFalse(data.HasValue);
                }

                [TestMethod]
                public void WhenClassWithNullAndClassTypeThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Value1\":10,\"Value2\":null}";

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionConverterExample2>(json);

                    // Assert
                    Assert.IsFalse(data.Value2.HasValue);
                }

                [TestMethod]
                public void WhenClassWithNullAndClassTypeAndSettingsThatAllowNullThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Value1\":10,\"Value2\":null}";
                    jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionConverterExample2>(json);

                    // Assert
                    Assert.IsFalse(data.Value2.HasValue);
                }

                [TestMethod]
                public void WhenClassWithNullAndClassTypeAndSettingsByDefaultThenDeserializesOk()
                {
                    // Arrange
                    string json = "{\"Value1\":10,\"Value2\":null}";
                    jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.Default);

                    // Act
                    var data = jsonSerializer.DeserializeObject<OptionConverterExample2>(json);

                    // Assert
                    Assert.IsFalse(data.Value2.HasValue);
                }
            }
        }
    }

    public class OptionConverterExample1
    {
        public int Value1 { get; set; }

        public string Value2 { get; set; }
    }

    public class OptionConverterExample2
    {
        public int Value1 { get; set; }

        public OptionStrict<OptionConverterExample2> Value2 { get; set; }
    }
}