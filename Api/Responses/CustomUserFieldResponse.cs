using System;
using System.Runtime.Serialization;
using Models;

namespace Api.Responses
{
    [Serializable]
    public class CustomUserFieldResponse
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "required")]
        public bool Required { get; private set; }

        [DataMember(Name = "type")]
        public string Type { get; private set; }

        public CustomUserField MapToModel()
        {
            return new CustomUserField
            {
               Name = Name,
               Required = Required,
               Type = Type
            };
        }
    }
}