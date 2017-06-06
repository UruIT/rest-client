using Moq;
using System.Collections.Generic;

namespace UruIT.Serialization.Mocks
{
    public class IJsonSerializerMock : Mock<IJsonSerializer>
    {
        /// <summary>
        /// Mocks "Setting" using the settings passed by parameter.
        /// </summary>
        public IJsonSerializerMock SettingsMock(JsonSerializerSettings settings)
        {
            SetupGet(x => x.Settings).Returns(settings);

            return this;
        }

        /// <summary>
        /// Mocks "SerializeObject" with a dictionary of objects to serialize.
        /// </summary>
        public IJsonSerializerMock SerializeObjectMock(Dictionary<object, string> serializations)
        {
            Setup(x => x.SerializeObject(It.IsAny<object>()))
                .Returns((object value) => serializations[value]);

            return this;
        }

        /// <summary>
        /// Mocks "DeserializeObject" with a dictionary of objects to deserialize.
        /// </summary>
        public IJsonSerializerMock DeserializeObjectMock<T>(Dictionary<string, T> deserializations)
        {
            Setup(x => x.DeserializeObject<T>(It.IsAny<string>()))
                .Returns((string value) => deserializations[value]);

            return this;
        }

        /// <summary>
        /// Mocks "Clone" by returning the same object.
        /// </summary>
        public IJsonSerializerMock CloneMock()
        {
            Setup(m => m.Clone()).Returns(this.Object);
            return this;
        }
    }
}