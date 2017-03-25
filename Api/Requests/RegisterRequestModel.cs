using System;
using System.Runtime.Serialization;
using Models.Forms;

namespace Api.Requests
{
    [Serializable]
    public class RegisterRequestModel
    {
        public RegisterRequestModel(RegisterModel model)
        {
            Email = model.Email;
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
        }

        [DataMember(Name = "email")]
        public string Email { get; }

        [DataMember(Name = "password")]
        public string Password { get; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; }
    }
}