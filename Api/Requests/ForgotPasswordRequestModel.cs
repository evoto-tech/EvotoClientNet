using System;
using System.Runtime.Serialization;
using Models.Forms;

namespace Api.Requests
{
    [Serializable]
    public class ForgotPasswordRequestModel
    {
        public ForgotPasswordRequestModel()
        {
        }

        public ForgotPasswordRequestModel(ForgotPasswordModel model)
        {
            Email = model.Email;
        }

        [DataMember(Name = "email")]
        public string Email { get; }
    }
}