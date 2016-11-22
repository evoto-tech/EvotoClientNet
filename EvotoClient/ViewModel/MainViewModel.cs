using System.Diagnostics;
using System.Threading.Tasks;
using EvotoClient.Blockchain;
using GalaSoft.MvvmLight;

namespace EvotoClient.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IMultiChainHandler _multiChainHandler;
        private bool _connected;

        private string _status = "Not Connected";

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IMultiChainHandler multiChainHandler)
        {
            _multiChainHandler = multiChainHandler;

            _multiChainHandler.OnConnect += (sender, args) =>
            {
                _connected = true;
                UpdateStatus();
            };
            UpdateStatus();

            // Connect in background thread to not slow the gui
            Task.Factory.StartNew(() => { _multiChainHandler.Connect().Wait(); });
        }

        /// <summary>
        ///     Gets the WelcomeTitle property.
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
            if (_connected)
            {
                _multiChainHandler.DisconnectAndClose();
            }
            else
            {
                _multiChainHandler.Close();
            }

            base.Cleanup();
        }
    }
}