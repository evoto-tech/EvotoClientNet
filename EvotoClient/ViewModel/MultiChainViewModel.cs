using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blockchain;
using Blockchain.Models;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;

namespace EvotoClient.ViewModel
{
    public class MultiChainViewModel : EvotoViewModelBase
    {
        private readonly MultiChainHandler _multiChainHandler;

        private MultichainModel _multichain;

        public MultiChainViewModel()
        {
            _multiChainHandler = SimpleIoc.Default.GetInstance<MultiChainHandler>();

            _multiChainHandler.OnConnect += (sender, args) => { UpdateStatus(); };
            UpdateStatus();
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

        private string _status = "Not Connected";

        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
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
            _multichain = await _multiChainHandler.Connect(hostname, blockchainName, port, localPort);
        }

        private void UpdateStatus()
        {
            Ui(() => { Status = Connected ? "Connected" : "Not Connected"; });
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
                _multiChainHandler.DisconnectAndClose(_multichain).Wait();

            base.Cleanup();
        }

        public async Task<List<BlockchainQuestionModel>> GetQuestions()
        {
            // TODO: Handle multiple questions. For now assume exactly 1
            var result = await Model.GetStreamKeyItems(MultiChainTools.ROOT_STREAM_NAME, MultiChainTools.QUESTIONS_KEY);
            var hex = result.First().Data;

            var bytes = Enumerable.Range(0, hex.Length)
                .Where(x => x%2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
            var text = Encoding.UTF8.GetString(bytes);
            return new List<BlockchainQuestionModel>
            {
                JsonConvert.DeserializeObject<BlockchainQuestionModel>(text)
            };
        }

        #endregion
    }
}