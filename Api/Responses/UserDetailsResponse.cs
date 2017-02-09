using System;
using System.Runtime.Serialization;
using Models;

namespace Api.Responses
{
    [Serializable]
    internal class UserDetailsResponse
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public string Email { get; private set; }

        [DataMember]
        public string CompanyId { get; private set; }

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