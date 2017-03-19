using System;
using System.Runtime.Serialization;
using Models.Forms;

namespace Api.Requests
{
    [Serializable]
    public class VerifyEmailRequestModel
    {
        public VerifyEmailRequestModel(VerifyEmailModel model)
        {
            Email = model.Email;
            Code = model.Token;
        }

        [DataMember(Name = "email")]
        public string Email { get; }

        [DataMember(Name = "code")]
        public string Code { get; private set; }
    }
}