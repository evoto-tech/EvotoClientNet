using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class GetDecryptKeyResponse
    {
        [DataMember]
        public string PrivateKey { get; private set; }
    }
}