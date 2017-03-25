using System;
using System.Collections.Generic;
using System.Linq;
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
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
            CustomFields = model.CustomFields.Select(cf => new RegisterCustomFieldRequestModel(cf));
        }

        [DataMember(Name = "email")]
        public string Email { get; }

        [DataMember(Name = "password")]
        public string Password { get; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; }

        [DataMember(Name = "customFields")]
        public IEnumerable<RegisterCustomFieldRequestModel> CustomFields { get; }
    }

    [Serializable]
    public class RegisterCustomFieldRequestModel
    {
        public RegisterCustomFieldRequestModel(CustomUserField model)
        {
            Name = model.Name;
            Value = model.Value;
        }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "value")]
        public string Value { get; private set; }
    }
}