using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public sealed class JsonSerializerTests
    {
        private JsonSerializerTests()
        {
        }

        [TestClass]
        public class TryDeserialize
        {
            [TestMethod]
            public void WhenJsonOkThenReturnsJust()
            {
                string json = "12";
                var serializer = new JsonSerializer();
                Assert.AreEqual(12, serializer.TryDeserialize<int>(json));
            }

            [TestMethod]
            public void WhenInvalidJsonThenReturnsNothing()
            {
                string json = "12Hello";
                var serializer = new JsonSerializer();
                Assert.AreEqual(OptionStrict<int>.Nothing, serializer.TryDeserialize<int>(json));
            }

            [TestMethod]
            public void WhenInvalidJsonThenIfObjectIsClassThenReturnNothing()
            {
                string json = "34534";
                var serializer = new JsonSerializer();
                Assert.AreEqual(OptionStrict<TryDeserializeTest>.Nothing, serializer.TryDeserialize<TryDeserializeTest>(json));
            }

            private class TryDeserializeTest
            {
                public int Value1 { get; set; }

                public string Value2 { get; set; }
            }
        }

        [TestClass]
        public class MissingValuesDeSerialization
        {
            private class MissingValuesTestClass
            {
                public int Value1 { get; set; }

                public string Value2 { get; set; }
            }

            private readonly IJsonSerializer jsonSerializer;

            public MissingValuesDeSerialization()
            {
                jsonSerializer = new JsonSerializer();
            }

            [TestMethod]
            public void WhenFieldComesButIDontExpectItThenWithCheckThrowsError()
            {
                jsonSerializer.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;

                string json = "{ 'Value1': 10, 'Value2': 'Test', 'Value3': 2312234 }";
                Assert.IsFalse(jsonSerializer.TryDeserialize<MissingValuesTestClass>(json).HasValue);
            }

            [TestMethod]
            public void WhenFieldComesButIDontExpectItTheNWithoutCheckItIgnoresIt()
            {
                jsonSerializer.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;

                string json = "{ 'Value1': 10, 'Value2': 'Test', 'Value3': 2312234 }";
                var res = jsonSerializer.TryDeserialize<MissingValuesTestClass>(json);
                Assert.AreEqual(10, res.Value.Value1);
                Assert.AreEqual("Test", res.Value.Value2);
            }

            [TestMethod]
            public void WhenFieldComesAndIDontExpectItThenByDefaultThrowsError()
            {
                string json = "{ 'Value1': 10, 'Value2': 'Test', 'Value3': 2312234 }";
                Assert.IsFalse(jsonSerializer.TryDeserialize<MissingValuesTestClass>(json).HasValue);
            }

            [TestMethod]
            public void WhenFieldDoesntComeAndIExpectItThenWithCheckThrowsError()
            {
                jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.Always);

                string json = "{ 'Value1': 10 }";
                Assert.IsFalse(jsonSerializer.TryDeserialize<MissingValuesTestClass>(json).HasValue);
            }

            [TestMethod]
            public void WhenFieldDoesntComeAndIExpectItThenWithoutCheckItIgnoresIt()
            {
                jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.Default);

                string json = "{ 'Value1': 10 }";
                var res = jsonSerializer.TryDeserialize<MissingValuesTestClass>(json);
                Assert.AreEqual(10, res.Value.Value1);
                Assert.IsNull(res.Value.Value2);
            }

            [TestMethod]
            public void WhenFieldComesWithNullAndIExpectItThenWithCheckNullItIgnoresIt()
            {
                jsonSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);

                string json = "{ 'Value1': 10, 'Value2': null }";
                var res = jsonSerializer.TryDeserialize<MissingValuesTestClass>(json);
                Assert.AreEqual(10, res.Value.Value1);
                Assert.IsNull(res.Value.Value2);
            }

            [TestMethod]
            public void WhenFieldDoesntComeAndIExpectItThenByDefaultThrowsError()
            {
                string json = "{ 'Value1': 10 }";
                Assert.IsFalse(jsonSerializer.TryDeserialize<MissingValuesTestClass>(json).HasValue);
            }
        }

        [TestClass]
        public class Immutability
        {
            [TestMethod]
            public void WhenAddConverterInCopyThenOriginalIsTheSame()
            {
                // Arrange
                var oriConverter = new JsonSerializer();

                // Act
                var newConverter = ((JsonSerializer)oriConverter.Clone()).AddConverter(new JsonConverters.DateTimeAsUnixTimeConverter());

                // Assert
                Assert.AreEqual(1, newConverter.Settings.Converters.Count);
                Assert.AreEqual(0, oriConverter.Settings.Converters.Count);
            }

            [TestMethod]
            public void WhenAddConverterReferencesOriginalThenItsAdded()
            {
                // Arrange
                var oriConverter = new JsonSerializer();

                // Act
                var newConverter = oriConverter.AddConverter(new JsonConverters.DateTimeAsUnixTimeConverter());

                // Assert
                Assert.AreEqual(1, newConverter.Settings.Converters.Count);
                Assert.AreEqual(1, oriConverter.Settings.Converters.Count);
            }

            [TestMethod]
            public void WhenCopysPropertiesAreModifiedThenOriginalStaysTheSame()
            {
                // Arrange
                var oriConverter = new JsonSerializer();
                oriConverter.Settings.ContractResolver.PropertyContract = new DefaultPropertyContract();
                oriConverter.Settings.ContractResolver.ObjectContract = new DefaultObjectContract();
                oriConverter.Settings.Formatting = Newtonsoft.Json.Formatting.None;
                oriConverter.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                oriConverter.Settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                // Act
                var newConverter = (JsonSerializer)oriConverter.Clone();
                newConverter.Settings.ContractResolver.PropertyContract = new ChangeFieldNamePropertyContract(typeof(int), "From", "To");
                newConverter.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);
                newConverter.Settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                newConverter.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
                newConverter.Settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;

                // Assert
                Assert.AreEqual(Newtonsoft.Json.MissingMemberHandling.Ignore, oriConverter.Settings.MissingMemberHandling);
                Assert.AreEqual(Newtonsoft.Json.ReferenceLoopHandling.Ignore, oriConverter.Settings.ReferenceLoopHandling);
                Assert.AreEqual(Newtonsoft.Json.Formatting.None, oriConverter.Settings.Formatting);
                Assert.IsInstanceOfType(oriConverter.Settings.ContractResolver.PropertyContract, typeof(DefaultPropertyContract));
                Assert.IsInstanceOfType(oriConverter.Settings.ContractResolver.ObjectContract, typeof(DefaultObjectContract));
            }
        }
    }
}