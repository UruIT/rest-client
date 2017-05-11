using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UruIT.Serialization.JsonConverters;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public class DateTimeAsUnixTimeConverterTests
    {
        protected Newtonsoft.Json.JsonSerializerSettings GetSettings()
        {
            return new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.None,
                Converters = new List<JsonConverter>()
				{
					new DateTimeAsUnixTimeConverter()
				}
            };
        }

        [TestClass]
        public class Serialization : DateTimeAsUnixTimeConverterTests
        {
            [TestMethod]
            public void WhenFechaOkThenDateTimeAsUnixTimeConverterSerializesOk()
            {
                // Arrange
                var settings = GetSettings();
                var date = new DateTime(2014, 10, 9, 2, 9, 3);

                // Act
                string json = JsonConvert.SerializeObject(date, settings);

                // Assert
                Assert.AreEqual("1412820543000", json);
            }
        }

        [TestClass]
        public class Deserialization : DateTimeAsUnixTimeConverterTests
        {
            [TestMethod]
            public void WhenFechaOkThenDateTimeAsUnixTimeConverterDeserializesOk()
            {
                // Arrange
                var settings = GetSettings();
                string json = "1412820543000";

                // Act
                var date = JsonConvert.DeserializeObject<DateTime>(json, settings);

                // Assert
                Assert.AreEqual(new DateTime(2014, 10, 9, 2, 9, 3), date);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenInvalidJsonThenDateTimeAsUnixTimeConverterThrowsError1()
            {
                // Arrange
                var settings = GetSettings();
                string json = "{\"OtroValor\":10}";

                // Act
                var date = JsonConvert.DeserializeObject<DateTime>(json, settings);
            }
        }
    }
}