using System.Threading.Tasks;
using Api.Properties;
using Api.Requests;
using Api.Responses;
using Models;

namespace Api.Clients
{
    public class VoteClient : ApiClient
    {
        public VoteClient() : base(Resources.VoteController)
        {
        }

        public async Task<string> GetBlindSignature(string blockchain, string blindedToken)
        {
            var res = await
                PostAsync<BlindSignatureResponse>(Resources.GetBlindSignature,
                    new BlindedTokenRequestModel(blockchain, blindedToken));
            return res.Signature;
        }

        public async Task<RegiMeta> GetVote(string blockchain, string walletId, string token, string blindSignature)
        {
            var res = await
                PostAnonymousAsync<GetVoteResponse>(Resources.GetVoteAction,
                    new GetVoteRequestModel(blockchain, walletId, token, blindSignature));
            return new RegiMeta
            {
                TxId = res.TxId,
                RegistrarAddress = res.RegistrarAddress
            };
        }

        public async Task<GetPublicKeyResponse> GetPublicKey()
        {
            return await GetAsync<GetPublicKeyResponse>(Resources.GetPublicKeyAction);
        }
    }
}