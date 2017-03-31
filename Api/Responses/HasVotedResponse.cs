using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class HasVotedResponse
    {
        [DataMember]
        public bool Voted { get; private set; }
    }
}