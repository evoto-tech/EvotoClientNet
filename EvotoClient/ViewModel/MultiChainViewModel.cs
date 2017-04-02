using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using Blockchain;
using Blockchain.Models;
using GalaSoft.MvvmLight.Ioc;
using MultiChainLib.Model;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace EvotoClient.ViewModel
{
    public class MultiChainViewModel : EvotoViewModelBase
    {
        private readonly MultiChainHandler _multiChainHandler;

        private MultichainModel _multichain;

        public MultiChainViewModel()
        {
            _multiChainHandler = SimpleIoc.Default.GetInstance<MultiChainHandler>();
        }

        #region Properties

        public MultichainModel Model
        {
            get
            {
                if (!Connected)
                    throw new Exception("Not connected to blockchain");
                return _multichain;
            }
        }

        public bool Connected => (_multichain != null) && _multichain.Connected;

        #endregion

        #region Methods

        public async Task Connect(string hostname, int port, string blockchainName)
        {
            // Only connect to one multichain at a time
            if (Connected)
                throw new Exception("Must disconnect from blockchain before connecting to a new one");

            var localPort = MultiChainTools.GetNewPort(EPortType.ClientMultichainD);
            var rpcPort = MultiChainTools.GetNewPort(EPortType.ClientRpc);
            _multichain = await _multiChainHandler.Connect(hostname, blockchainName, port, localPort, rpcPort, false);
        }

        public async Task Disconnect()
        {
            if (!Connected)
                return;

            await _multiChainHandler.DisconnectAndClose(_multichain);
        }

        public override void Cleanup()
        {
            Debug.WriteLine("Cleaning Up");
            if (_multichain != null)
                Task.Run(async () => { await _multiChainHandler.DisconnectAndClose(_multichain); }).Wait();

            base.Cleanup();
        }

        public async Task Vote(List<QuestionViewModel> questions)
        {
            var voteClient = new VoteClient();

            // Create our token
            var token = GenerateRandomToken();

            try
            {
                var keyInfo = await voteClient.GetPublicKey(Model.Name);
                var publicKey = new RsaKeyParameters(false, new BigInteger(keyInfo.Modulus),
                    new BigInteger(keyInfo.Exponent));

                var blindedToken = RsaTools.BlindMessage(token, publicKey);

                var blindSignature = await voteClient.GetBlindSignature(Model.Name, blindedToken.Blinded.ToString());

                var unblindedSignature = RsaTools.UnblindMessage(new BigInteger(blindSignature), blindedToken.Random,
                    publicKey);

                // TODO: Sleep
                var walletId = await Model.GetNewWalletAddress();

                var regiMeta = await voteClient.GetVote(Model.Name, walletId, token, unblindedSignature.ToString());

                var txIds = new List<CreateRawTransactionTxIn>
                {
                    new CreateRawTransactionTxIn {TxId = regiMeta.TxId, Vout = 0}
                };

                var toInfo = new List<CreateRawTransactionAmount>
                {
                    new CreateRawTransactionAsset
                    {
                        Address = regiMeta.RegistrarAddress,
                        Qty = 1,
                        Name = MultiChainTools.VOTE_ASSET_NAME
                    }
                };

                var answerModel = new BlockchainVoteModel
                {
                    Answers = questions.Select(q => new BlockchainVoteAnswerModel
                    {
                        Answer = q.SelectedAnswer.Answer,
                        Question = q.QuestionNumber
                    }).ToList()
                };

                await Model.WriteTransaction(txIds, toInfo, answerModel);
            }
            catch (ApiException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static string GenerateRandomToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion
    }
}