using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace UruIT.Serialization
{
    /// <summary>
    /// Contract resolver used for serialization
    /// </summary>
    public class ContractResolver : DefaultContractResolver, IContractResolver
    {
        public IPropertyContract PropertyContract { get; set; }

        public IObjectContract ObjectContract { get; set; }

        public ContractResolver()
        {
            PropertyContract = new DefaultPropertyContract();
            ObjectContract = new DefaultObjectContract();
        }

        private ContractResolver(IPropertyContract propertyContract, IObjectContract objectContract)
        {
            this.PropertyContract = propertyContract;
            this.ObjectContract = objectContract;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var parentObject = base.CreateObjectContract(objectType);
            return ObjectContract.CreateObjectContract(parentObject, objectType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var parentProperty = base.CreateProperty(member, memberSerialization);
            return PropertyContract.CreateProperty(parentProperty, member, memberSerialization);
        }

        public IContractResolver Clone()
        {
            return new ContractResolver(PropertyContract.Clone(), ObjectContract.Clone());
        }
    }
}