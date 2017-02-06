using System;
using System.Runtime.Serialization;
using Models;

namespace Api.Responses
{
    [Serializable]
    internal class UserDetailsResponse
    {
        [DataMember]
        public int UserId { get; }

        [DataMember]
        public string Email { get; }

        [DataMember]
        public string CompanyId { get; }

        public UserDetails MapToModel()
        {
            return new UserDetails
            {
                UserId = UserId,
                CompanyId = CompanyId,
                Email = Email
            };
        }
    }
}