using System;
using System.Runtime.Serialization;

namespace Api.Requests
{
    [Serializable]
    public class BlindedTokenRequestModel
    {
        public BlindedTokenRequestModel(string blockchain, string token)
        {
            Blockchain = blockchain;
            Token = token;
        }

        [DataMember(Name = "blockchain")]
        public string Blockchain { get; }

        [DataMember(Name = "token")]
        public string Token { get; }
    }
}