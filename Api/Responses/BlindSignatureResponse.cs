using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class BlindSignatureResponse
    {
        [DataMember]
        public string Signature { get; private set; }
    }
}