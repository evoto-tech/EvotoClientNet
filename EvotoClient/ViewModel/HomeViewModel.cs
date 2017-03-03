using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public class HomeViewModel : EvotoViewModelBase
    {
        private readonly HomeClient _homeApiClient;

        public HomeViewModel()
        {
            _homeApiClient = new HomeClient();

            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            mainVm.OnLogin += async (e, u) => await GetVotes();

            ProceedCommand = new RelayCommand(DoProceed);
            RefreshCommand = new RelayCommand(async () => await GetVotes());

            Votes = new ObservableRangeCollection<BlockchainDetails>();
        }

        #region Commands

        public RelayCommand ProceedCommand { get; }
        public RelayCommand RefreshCommand { get; }

        #endregion

        #region Properties

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        private bool _noVotes;

        public bool NoVotes
        {
            get { return _noVotes; }
            set { Set(ref _noVotes, value); }
        }

        public bool NoVotesMessageVisible => !Loading && NoVotes;

        public bool VotesVisible => !Loading && !NoVotes;

        public ObservableRangeCollection<BlockchainDetails> Votes { get; }

        private BlockchainDetails _selectedVote;

        public BlockchainDetails SelectedVote
        {
            get { return _selectedVote; }
            set { Set(ref _selectedVote, value); }
        }

        #endregion

        #region Methods

        private void DoProceed()
        {
            var voteView = GetVm<VoteViewModel>();
            voteView.SelectVote(SelectedVote);
            MainVm.ChangeView(EvotoView.Vote);
        }

        private async Task GetVotes()
        {
            Ui(() => { Loading = true; });

            var votes = (await _homeApiClient.GetCurrentVotes()).ToList();
            Debug.WriteLine("Votes:");
            Ui(() =>
            {
                Loading = false;
                if (votes.Any())
                {
                    Votes.Clear();
                    Votes.AddRange(votes);
                    NoVotes = false;
                }
                else
                {
                    NoVotes = true;
                }
            });
        }

        #endregion
    }
}