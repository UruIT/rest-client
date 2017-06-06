using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UruIT.Serialization.Tests
{
    public class EitherConverterExample1
    {
        public int Value1 { get; set; }

        public string Value2 { get; set; }
    }

    public class EitherConverterExample2
    {
        public int Value1 { get; set; }

        public EitherStrict<EitherConverterExample2, int> Value2 { get; set; }
    }

    [TestClass]
    public class EitherConverterTests
    {
        private Newtonsoft.Json.JsonSerializerSettings GetSettings()
        {
            return new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.None,
                Converters = new List<JsonConverter>()
				{
					new JsonConverters.EitherConverter()
				}
            };
        }

        #region Serialization

        [TestMethod]
        public void WhenItsLeftWithDifferentPrimitiveTypesThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<int, string> data = 10;

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":10}", json);
        }

        [TestMethod]
        public void WhenItsRightWithDifferentPrimitiveTypesThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<int, string> data = "Hola";

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Right\":\"Hola\"}", json);
        }

        [TestMethod]
        public void WhenLeftWithSamePrimitiveTypesThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<int, int> data = EitherStrict.Left<int, int>(10);

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":10}", json);
        }

        [TestMethod]
        public void WhenRightWithSamePrimitiveTypesThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<int, int> data = EitherStrict.Right<int, int>(22);

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Right\":22}", json);
        }

        [TestMethod]
        public void WhenOtherTypeThenDoesntSerialize()
        {
            // Arrange
            var settings = GetSettings();
            int data = 10;

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("10", json);
        }

        [TestMethod]
        public void WhenLeftAndClassTypeThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherConverterExample1, int> data = new EitherConverterExample1()
            {
                Value1 = 10,
                Value2 = "Value"
            };

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":{\"Value1\":10,\"Value2\":\"Value\"}}", json);
        }

        [TestMethod]
        public void WhenRightAndClassTypeThenSerializesOk()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherConverterExample1, int> data = 10;

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Right\":10}", json);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenSerializesOkCase1()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherConverterExample2, int> data = new EitherConverterExample2()
            {
                Value1 = 10,
                Value2 = new EitherConverterExample2()
                {
                    Value1 = 20,
                    Value2 = 30
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":{\"Value1\":10,\"Value2\":{\"Left\":{\"Value1\":20,\"Value2\":{\"Right\":30}}}}}", json);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenSerializesOkCase2()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherConverterExample2, int> data = new EitherConverterExample2()
            {
                Value1 = 10,
                Value2 = 20
            };

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":{\"Value1\":10,\"Value2\":{\"Right\":20}}}", json);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenSerializesOkCase3()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherConverterExample2, int> data = 10;

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Right\":10}", json);
        }

        [TestMethod]
        public void WhenNestedEitherThenSerializesOkCase1()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherStrict<int, int>, int> data = EitherStrict.Left<EitherStrict<int, int>, int>(EitherStrict.Left<int, int>(10));

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":{\"Left\":10}}", json);
        }

        [TestMethod]
        public void WhenNestedEitherThenSerializesOkCase2()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherStrict<int, int>, int> data = EitherStrict.Left<EitherStrict<int, int>, int>(EitherStrict.Right<int, int>(20));

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Left\":{\"Right\":20}}", json);
        }

        [TestMethod]
        public void WhenNestedEitherThenSerializesOkCase3()
        {
            // Arrange
            var settings = GetSettings();
            EitherStrict<EitherStrict<int, int>, int> data = EitherStrict.Right<EitherStrict<int, int>, int>(30);

            // Act
            string json = JsonConvert.SerializeObject(data, settings);

            // Assert
            Assert.AreEqual("{\"Right\":30}", json);
        }

        #endregion Serialization

        #region Deserialization

        [TestMethod]
        public void WhenLeftAndDifferentPrimitiveTypesThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":10}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, string>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.AreEqual(10, data.Left);
        }

        [TestMethod]
        public void WhenRightAndDifferentPrimitiveTypesThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":\"Hello\"}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, string>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsRight);
            Assert.AreEqual("Hello", data.Right);
        }

        [TestMethod]
        public void WhenLeftAndSamePrimitiveTypesThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":10}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.AreEqual(10, data.Left);
        }

        [TestMethod]
        public void WhenRightAndSamePrimitiveTypesThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":22}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsRight);
            Assert.AreEqual(22, data.Right);
        }

        [TestMethod]
        public void WhenAnotherTypeThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = @"10";

            // Act
            int data = JsonConvert.DeserializeObject<int>(json, settings);

            // Assert
            Assert.AreEqual(10, data);
        }

        [TestMethod]
        public void WhenLeftAndClassTypeThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":{\"Value1\":10,\"Value2\":\"Value\"}}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherConverterExample1, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.AreEqual(10, data.Left.Value1);
            Assert.AreEqual("Value", data.Left.Value2);
        }

        [TestMethod]
        public void WhenRightAndClassTypeThenDeserializesOk()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":10}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherConverterExample1, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsRight);
            Assert.AreEqual(10, data.Right);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenDeserializesOkCase1()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":{\"Value1\":10,\"Value2\":{\"Left\":{\"Value1\":20,\"Value2\":{\"Right\":30}}}}}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherConverterExample2, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.AreEqual(10, data.Left.Value1);
            Assert.IsTrue(data.Left.Value2.IsLeft);
            Assert.AreEqual(20, data.Left.Value2.Left.Value1);
            Assert.AreEqual(30, data.Left.Value2.Left.Value2);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenDeserializesOkCase2()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":{\"Value1\":10,\"Value2\":{\"Right\":20}}}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherConverterExample2, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.AreEqual(10, data.Left.Value1);
            Assert.IsTrue(data.Left.Value2.IsRight);
            Assert.AreEqual(20, data.Left.Value2.Right);
        }

        [TestMethod]
        public void WhenClassTypeWithNestedEitherThenDeserializesOkCase3()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":10}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherConverterExample2, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsRight);
            Assert.AreEqual(10, data.Right);
        }

        [TestMethod]
        public void WhenNestedEitherThenDeserializesOkCase1()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":{\"Left\":10}}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherStrict<int, int>, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.IsTrue(data.Left.IsLeft);
            Assert.AreEqual(10, data.Left.Left);
        }

        [TestMethod]
        public void WhenNestedEitherThenDeserializesOkCase2()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":{\"Right\":20}}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherStrict<int, int>, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsLeft);
            Assert.IsTrue(data.Left.IsRight);
            Assert.AreEqual(20, data.Left.Right);
        }

        [TestMethod]
        public void WhenNestedEitherThenDeserializesOkCase3()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":30}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<EitherStrict<int, int>, int>>(json, settings);

            // Assert
            Assert.IsTrue(data.IsRight);
            Assert.AreEqual(30, data.Right);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidJsonThenThrowsErrorCase1()
        {
            // Arrange
            var settings = GetSettings();
            string json = "[1,2,3]";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidJsonThenThrowsErrorCase2()
        {
            // Arrange
            var settings = GetSettings();
            string json = "2";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidJsonThenThrowsErrorCase3()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"AnotherValue\":10}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidJsonThenThrowsErrorCase4()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Left\":10,\"AnotherValue\":20}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidJsonThenThrowsErrorCase5()
        {
            // Arrange
            var settings = GetSettings();
            string json = "{\"Right\":10,\"AnotherValue\":20}";

            // Act
            var data = JsonConvert.DeserializeObject<EitherStrict<int, int>>(json, settings);
        }

        #endregion Deserialization
    }
}