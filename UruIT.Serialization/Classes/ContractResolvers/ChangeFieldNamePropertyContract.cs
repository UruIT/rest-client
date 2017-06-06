using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace UruIT.Serialization
{
    /// <summary>
    /// JSON.NET's ContractResolver that takes a field with a determined name and changes it for another name
    /// </summary>
    public class ChangeFieldNamePropertyContract : IPropertyContract
    {
        /// <summary>
        /// Original field
        /// </summary>
        private readonly string fieldFrom;

        /// <summary>
        /// New field
        /// </summary>
        private readonly string fieldTo;

        /// <summary>
        /// Type of the object to use this conversion on
        /// </summary>
        private readonly Type declaringType;

        public ChangeFieldNamePropertyContract(Type declaringType, string fieldFrom, string fieldTo)
        {
            this.declaringType = declaringType;
            this.fieldFrom = fieldFrom;
            this.fieldTo = fieldTo;
        }

        public Newtonsoft.Json.Serialization.JsonProperty CreateProperty(JsonProperty parentProperty, System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
        {
            if (parentProperty.DeclaringType == declaringType)
            {
                //Si el nombre de la propiedad es el de origen, lo modifica
                if (parentProperty.PropertyName.Equals(fieldFrom, System.StringComparison.OrdinalIgnoreCase))
                {
                    parentProperty.PropertyName = fieldTo;
                }
            }
            return parentProperty;
        }

        public IPropertyContract Clone()
        {
            return new ChangeFieldNamePropertyContract(declaringType, fieldFrom, fieldTo);
        }
    }
}