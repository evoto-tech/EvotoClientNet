using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class GetPublicKeyResponse
    {
        // These types are actually BigIntegers, but we don't want to reference BouncyCastle here (oops)

        [DataMember]
        public string Modulus { get; private set; }

        [DataMember]
        public string Exponent { get; private set; }
    }
}