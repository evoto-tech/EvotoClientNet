using System;
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

        [DataMember]
        public int Port { get; private set; }

        [DataMember]
        public string WalletId { get; private set; }

        public BlockchainDetails ToModel()
        {
            return new BlockchainDetails
            {
                Name = Name,
                ExpiryDate = ExpiryDate,
                ChainString = ChainString,
                Port = Port,
                WalletId = WalletId
            };
        }
    }
}