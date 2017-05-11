using Newtonsoft.Json.Serialization;
using System;

namespace UruIT.Serialization
{
    /// <summary>
    /// Indicates how to admnistrate objects
    /// </summary>
    public interface IObjectContract
    {
        /// <summary>
        /// Takes the contract of an already created object, the type of the object and returns a new contract
        /// </summary>
        JsonObjectContract CreateObjectContract(JsonObjectContract parentObjectContract, Type objectType);

        /// <summary>
        /// Creates an exact copy
        /// </summary>
        IObjectContract Clone();
    }
}