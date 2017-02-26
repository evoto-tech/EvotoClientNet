using System.Diagnostics;
using System.Threading.Tasks;
using Blockchain;
using Blockchain.Models;
using GalaSoft.MvvmLight.Ioc;
using Models;

namespace EvotoClient.ViewModel
{
    public class MultiChainViewModel : EvotoViewModelBase
    {
        private readonly MultiChainHandler _multiChainHandler;

        private MultichainModel _multichain;

        private string _status = "Not Connected";

        public MultiChainViewModel()
        {
            _multiChainHandler = SimpleIoc.Default.GetInstance<MultiChainHandler>();

            _multiChainHandler.OnConnect += (sender, args) =>
            {
                UpdateStatus();
            };
            UpdateStatus();
        }

        public void Connect(string hostname, int port, string blockchainName)
        {
            // Connect in background thread
            Task.Factory.StartNew(async () =>
            {
                _multichain = await _multiChainHandler.Connect(hostname, blockchainName, port);
            });
        }

        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        public bool Connected => _multichain != null && _multichain.Connected;

        private void UpdateStatus()
        {
            Ui(() => {
                Status = Connected ? "Connected" : "Not Connected";
            });
        }

        public override void Cleanup()
        {
            Debug.WriteLine("Cleaning Up");
            if (_multichain != null)
                _multiChainHandler.DisconnectAndClose(_multichain).Wait();

            base.Cleanup();
        }
    }
}