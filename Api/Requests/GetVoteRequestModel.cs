using System;
using System.Runtime.Serialization;

namespace Api.Requests
{
    [Serializable]
    public class GetVoteRequestModel
    {
        public GetVoteRequestModel(string blockchain, string walletId, string token, string blindSignature)
        {
            Blockchain = blockchain;
            WalletId = walletId;
            Token = token;
            BlindSignature = blindSignature;
        }

        [DataMember(Name = "blockchain")]
        public string Blockchain { get; }

        [DataMember(Name = "walletId")]
        public string WalletId { get; }

        [DataMember(Name = "token")]
        public string Token { get; }

        [DataMember(Name = "blindSignature")]
        public string BlindSignature { get; }
    }
}