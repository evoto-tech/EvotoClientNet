using System;
using System.Runtime.Serialization;

namespace Api.Requests
{
    [Serializable]
    public class ResendVerificationRequestModel
    {
        public ResendVerificationRequestModel(string email)
        {
            Email = email;
        }

        [DataMember(Name = "email")]
        public string Email { get; }
    }
}