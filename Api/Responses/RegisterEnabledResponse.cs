using System;
using System.Runtime.Serialization;

namespace Api.Responses
{
    [Serializable]
    public class RegisterEnabledResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; private set; }
    }
}