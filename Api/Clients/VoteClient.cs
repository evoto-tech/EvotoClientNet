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

        public async Task<bool> HasVoted(string blockchain)
        {
            var res = await
                GetAsync<HasVotedResponse>(Resources.GetHasVotedAction, blockchain);
            return res.Voted;
        }

        public async Task<RegiMeta> GetVote(string blockchain, string walletId, string token, string blindSignature)
        {
            var res = await
                PostAnonymousAsync<GetVoteResponse>(Resources.GetVoteAction,
                    new GetVoteRequestModel(blockchain, walletId, token, blindSignature));
            return new RegiMeta
            {
                TxId = res.TxId,
                RegistrarAddress = res.RegistrarAddress,
                Words = res.Words
            };
        }

        public async Task<string> GetPublicKey(string blockchain)
        {
            var res = await GetAsync<GetPublicKeyResponse>(Resources.GetPublicKeyAction, blockchain);
            return res.PublicKey;
        }

        public async Task<string> GetDecryptKey(string blockchain)
        {
            var res = await GetAnonymousAsync<GetDecryptKeyResponse>(Resources.GetDecryptKeyAction, blockchain);
            return res.PrivateKey;
        }
    }
}