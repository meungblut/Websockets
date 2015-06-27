using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEungblut.Websockets
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class JsonSerialisationSettings : JsonSerializerSettings
    {
        public JsonSerialisationSettings()
        {
            Converters = new JsonConverter[]
            {
                new StringEnumConverter(),
            };

            ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}

