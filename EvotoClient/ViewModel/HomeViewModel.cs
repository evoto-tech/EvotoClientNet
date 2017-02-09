using System.Diagnostics;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public class HomeViewModel : EvotoViewModelBase
    {
        private readonly HomeClient _homeApiClient;
        private bool _loading;
        private BlockchainDetails _selectedVote;

        public RelayCommand ProceedCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public HomeViewModel()
        {
            _homeApiClient = new HomeClient();

            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            mainVm.OnLogin += async (e, u) => await GetVotes();

            ProceedCommand = new RelayCommand(DoProceed);
            RefreshCommand = new RelayCommand(async() => await GetVotes());

            Votes = new ObservableRangeCollection<BlockchainDetails>();
        }

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        public ObservableRangeCollection<BlockchainDetails> Votes { get; }

        public BlockchainDetails SelectedVote
        {
            get { return _selectedVote; }
            set { Set(ref _selectedVote, value); }
        }

        private async Task GetVotes()
        {
            try
            {
                var votes = await _homeApiClient.GetCurrentVotes();
                Debug.WriteLine("Votes:");
                Debug.WriteLine(votes);
                Ui(() =>
                {
                    Votes.Clear();
                    Votes.AddRange(votes);
                });
            }
            catch (ApiException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void DoProceed()
        {
            Debug.WriteLine(SelectedVote);
        }
    }
}