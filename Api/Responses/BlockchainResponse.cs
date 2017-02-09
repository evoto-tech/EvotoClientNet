using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Models;

namespace Api.Responses
{
    [Serializable]
    public class BlockchainResponse
    {
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public DateTime ExpiryDate { get; private set; }

        [DataMember]
        public string ChainString { get; private set; }

        public BlockchainDetails ToModel()
        {
            return new BlockchainDetails
            {
                Name = Name,
                ExpiryDate = ExpiryDate,
                ChainString = ChainString
            };
        }
    }
}