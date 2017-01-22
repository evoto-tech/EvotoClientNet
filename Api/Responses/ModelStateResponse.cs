using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class ModelStateResponse
    {
        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public IDictionary<string, IEnumerable<string>> ModelState { get; private set; }
    }
}