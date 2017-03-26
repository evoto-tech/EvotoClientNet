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
        private readonly VoteClient _voteClient;

        public HomeViewModel()
        {
            _homeApiClient = new HomeClient();
            _voteClient = new VoteClient();

            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();

            if (mainVm.LoggedIn)
                Task.Run(async () => { await GetVotes(); });
            else
                mainVm.OnLogin += async (e, u) => await GetVotes();

            ProceedCommand = new RelayCommand(DoProceed, CanProceed);
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
            set
            {
                Set(ref _selectedVote, value);
                ProceedCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Methods

        private void DoProceed()
        {
            Loading = true;

            // Contact the Registrar to see if we have voted on this vote yet
            Task.Run(async () =>
            {
                var voted = await _voteClient.HasVoted(SelectedVote.ChainString);

                if (!voted)
                {
                    MainVm.ChangeView(EvotoView.Vote);
                    var voteView = GetVm<VoteViewModel>();
                    voteView.SelectVote(SelectedVote);
                }
                else
                {
                    MainVm.ChangeView(EvotoView.Results);
                    var resultsVm = GetVm<ResultsViewModel>();
                    resultsVm.SelectVote(SelectedVote);
                }
            });

            
        }

        private bool CanProceed()
        {
            return SelectedVote != null;
        }

        private async Task GetVotes()
        {
            Debug.WriteLine("Getting Votes");
            Ui(() => { Loading = true; });

            var votes = (await _homeApiClient.GetCurrentVotes()).ToList();

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