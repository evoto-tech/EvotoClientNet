using System;
using System.Runtime.Serialization;
using Models;

namespace Api.Responses
{
    [Serializable]
    internal class UserDetailsResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        public UserDetails MapToModel()
        {
            return new UserDetails
            {
                Id = Id,
                Email = Email
            };
        }
    }
}