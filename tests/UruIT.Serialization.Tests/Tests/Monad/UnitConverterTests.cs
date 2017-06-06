using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad.Utility;
using Newtonsoft.Json;
using System;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public class UnitConverterTests
    {
        protected Newtonsoft.Json.JsonSerializerSettings settings;

        protected UnitConverterTests()
        {
            settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.None,
            };
            settings.Converters.Add(new JsonConverters.UnitConverter());
        }

        [TestClass]
        public class Serializacion : UnitConverterTests
        {
            [TestMethod]
            public void WhenUnitThenSerializaOk()
            {
                // Arrange
                // Act
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(Unit.Default, settings);

                // Assert
                Assert.AreEqual("{}", json);
            }

            [TestMethod]
            public void WhenOtroTipoThenNoSerializa()
            {
                // Arrange
                int dato = 10;

                // Act
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(dato, settings);

                // Assert
                Assert.AreEqual("10", json);
            }
        }

        [TestClass]
        public class Deserializacion : UnitConverterTests
        {
            [TestMethod]
            public void WhenUnitThenDeserializaOk()
            {
                // Arrange
                string json = "{}";

                // Act
                var dato = Newtonsoft.Json.JsonConvert.DeserializeObject<Unit>(json, settings);

                // Assert
                Assert.AreEqual(Unit.Default, dato);
            }

            [TestMethod]
            public void WhenOtroTipoThenNoDeserializa()
            {
                // Arrange
                string json = @"10";

                // Act
                var dato = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(json, settings);

                // Assert
                Assert.AreEqual(10, dato);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenJsonInvalidoThenTiraErrorCaso1()
            {
                // Arrange
                string json = "23234";

                // Act
                var dato = Newtonsoft.Json.JsonConvert.DeserializeObject<Unit>(json, settings);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenJsonInvalidoThenTiraErrorCaso2()
            {
                // Arrange
                string json = "{ 'Campo': 'Valor'}";

                // Act
                var dato = Newtonsoft.Json.JsonConvert.DeserializeObject<Unit>(json, settings);
            }
        }
    }
}