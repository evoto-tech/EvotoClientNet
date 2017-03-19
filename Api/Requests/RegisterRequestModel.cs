using System;
using System.Runtime.Serialization;
using Models;
using Models.Forms;

namespace Api.Requests
{
    [Serializable]
    public class RegisterRequestModel
    {
        public RegisterRequestModel(RegisterModel model)
        {
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
            CompanyId = model.CompanyId;
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
        }

        [DataMember(Name = "email")]
        public string Email { get; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; }

        [DataMember(Name = "lastName")]
        public string LastName { get; }

        [DataMember(Name = "companyId")]
        public string CompanyId { get; }

        [DataMember(Name = "password")]
        public string Password { get; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; }
    }
}