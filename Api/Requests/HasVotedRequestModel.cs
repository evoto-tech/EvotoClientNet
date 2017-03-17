using System;
using System.Runtime.Serialization;

namespace Api.Requests
{
    [Serializable]
    public class HasVotedRequestModel
    {
        public HasVotedRequestModel(string blockchain)
        {
            Blockchain = blockchain;
        }

        [DataMember(Name = "blockchain")]
        public string Blockchain { get; }
    }
}