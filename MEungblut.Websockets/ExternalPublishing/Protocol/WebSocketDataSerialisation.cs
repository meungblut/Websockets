namespace MEungblut.Websockets.ExternalPublishing.Protocol
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class WebSocketDataSerialisation
    {
        private const string ContentTypePrefix = "content-type:application/vnd.";

        private static Dictionary<string, Type> valuesToDeserialise = new Dictionary<string, Type>();

        public string GetString(object objectToSerialise)
        {
            var typeNamespace = objectToSerialise.GetType().Namespace;
            var splitNamespaces = typeNamespace.Split('.');
            var firstTwopartsOfNamespace = (splitNamespaces[0] + "." + splitNamespaces[1] + "." + objectToSerialise.GetType().Name).ToLower() + "+json";

            var messageBody = JsonConvert.SerializeObject(objectToSerialise, Formatting.None, new JsonSerialisationSettings());

            return ContentTypePrefix + firstTwopartsOfNamespace + "\r\n\r\n" + messageBody;
        } 
         
        public object GetObject(string messageToDeserialise)
        {
            var messageParts = messageToDeserialise.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!valuesToDeserialise.ContainsKey(messageParts[0])) throw new UnknownMediaTypeForDeserialisationException(messageParts[0]);

            Type typeToDeserialiseTo = valuesToDeserialise[messageParts[0]];

            var deserialisedObject = JsonConvert.DeserializeObject(messageParts[1], typeToDeserialiseTo);

            return deserialisedObject;
        } 

        //TODO: come back and sort this.
        public static void AddTypeToDeserialise(string contentType, Type typeToDeserialiseTo)
        {
            valuesToDeserialise.Add(contentType, typeToDeserialiseTo);
        }
    }
}