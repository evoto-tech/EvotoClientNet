using System.Diagnostics;
using EvotoClient.Blockchain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace EvotoClient.ViewModel
{
    public class MultiChainViewModel : ViewModelBase
    {
        private readonly IMultiChainHandler _multiChainHandler;
        private bool _connected;

        private string _status = "Not Connected";

        public MultiChainViewModel()
        {
            _multiChainHandler = SimpleIoc.Default.GetInstance<IMultiChainHandler>();

            _multiChainHandler.OnConnect += (sender, args) =>
            {
                _connected = true;
                UpdateStatus();
            };
            UpdateStatus();

            // Connect in background thread
            // TODO: This is annoying
            //Task.Factory.StartNew(() => { _multiChainHandler.Connect().Wait(); });
        }

        /// <summary>
        ///     Gets the Status property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private void UpdateStatus()
        {
            Status = _connected ? "Connected" : "Not Connected";
        }

        public override void Cleanup()
        {
            Debug.WriteLine("Cleaning Up");
            if (_connected) _multiChainHandler.DisconnectAndClose();
            else _multiChainHandler.Close();

            base.Cleanup();
        }
    }
}