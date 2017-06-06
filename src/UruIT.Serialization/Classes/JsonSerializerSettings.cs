using Newtonsoft.Json;
using System.Collections.Generic;

namespace UruIT.Serialization
{
    /// <summary>
    /// JSON serialization settings
    /// </summary>
    public class JsonSerializerSettings
    {
        //JSON.NET settings
        private readonly Newtonsoft.Json.JsonSerializerSettings settings;

        //Contract resolver
        private IContractResolver contractResolver;

        public IContractResolver ContractResolver
        {
            get { return contractResolver; }
            set
            {
                contractResolver = value;
                if (contractResolver is ContractResolver)
                    settings.ContractResolver = (ContractResolver)contractResolver;
            }
        }

        public IList<JsonConverter> Converters { get { return settings.Converters; } set { settings.Converters = value; } }

        public Formatting Formatting { get { return settings.Formatting; } set { settings.Formatting = value; } }

        public MissingMemberHandling MissingMemberHandling { get { return settings.MissingMemberHandling; } set { settings.MissingMemberHandling = value; } }

        public NullValueHandling NullValueHandling { get { return settings.NullValueHandling; } set { settings.NullValueHandling = value; } }

        public ReferenceLoopHandling ReferenceLoopHandling { get { return settings.ReferenceLoopHandling; } set { settings.ReferenceLoopHandling = value; } }

        public JsonSerializerSettings()
        {
            this.settings = new Newtonsoft.Json.JsonSerializerSettings();

            //Sets the contract resolver on the JSON.NET settings too
            var monContractResolver = new ContractResolver();
            contractResolver = monContractResolver;
            this.settings.ContractResolver = monContractResolver;
        }

        public JsonSerializerSettings(JsonSerializerSettings monSettings)
        {
            this.settings = new Newtonsoft.Json.JsonSerializerSettings();

            //It doesn't have deep copy
            this.ContractResolver = monSettings.ContractResolver.Clone();
            this.Converters = new List<JsonConverter>(monSettings.Converters);
            this.Formatting = monSettings.Formatting;
            this.MissingMemberHandling = monSettings.MissingMemberHandling;
            this.NullValueHandling = monSettings.NullValueHandling;
            this.ReferenceLoopHandling = monSettings.ReferenceLoopHandling;
        }

        public Newtonsoft.Json.JsonSerializerSettings GetJsonNetSettings()
        {
            return settings;
        }
    }
}