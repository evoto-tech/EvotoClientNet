using System;
using System.Runtime.Serialization;
using Models.Forms;

namespace Api.Requests
{
    [Serializable]
    public class ResetPasswordRequestModel
    {
        public ResetPasswordRequestModel()
        {
        }

        public ResetPasswordRequestModel(ResetPasswordModel model)
        {
            Email = model.Email;
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
            Code = model.Token;
        }

        [DataMember(Name = "email")]
        public string Email { get; }

        [DataMember(Name = "password")]
        public string Password { get; private set; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; private set; }

        [DataMember(Name = "code")]
        public string Code { get; private set; }
    }
}