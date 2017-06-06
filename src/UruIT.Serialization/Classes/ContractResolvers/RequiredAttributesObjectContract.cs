using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace UruIT.Serialization
{
    /// <summary>
    /// Contract resolver that requires properties to be present with a specific required level
    /// </summary>
    public class RequiredAttributesObjectContract : IObjectContract
    {
        private readonly RequiredLevel requiredLevel;

        /// <summary>
        /// Creates the property contract.
        /// </summary>
        /// <param name="requiredLevel">Level of requirement for the property. Can be obligatory, optional, etc</param>
        public RequiredAttributesObjectContract(RequiredLevel requiredLevel)
        {
            this.requiredLevel = requiredLevel;
        }

        public JsonObjectContract CreateObjectContract(JsonObjectContract parentObjectContract, Type objectType)
        {
            parentObjectContract.ItemRequired = (Required)requiredLevel;
            return parentObjectContract;
        }

        public IObjectContract Clone()
        {
            return new RequiredAttributesObjectContract(requiredLevel);
        }
    }

    /// <summary>
    /// Requirement level for JSON properties
    /// </summary>
    public enum RequiredLevel
    {
        /// <summary>
        /// Property can be defined in the JSON or not.
        /// </summary>
        Default = 0,

        /// <summary>
        /// The property must be defined, but it can be null.
        /// </summary>
        AllowNull = 1,

        /// <summary>
        /// The property must be defined and must not be null.
        /// </summary>
        Always = 2,
    }
}