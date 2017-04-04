using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class GetVoteResponse
    {
        [DataMember]
        public string TxId { get; private set; }

        [DataMember]
        public string RegistrarAddress { get; private set; }

        [DataMember]
        public string Words { get; private set; }
    }
}