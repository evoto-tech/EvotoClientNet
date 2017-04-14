using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
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
            RefreshCommand = new RelayCommand(async () => await GetVotes(), CanRefresh);

            VoteCollection = new ObservableRangeCollection<BlockchainViewModel>();
            VoteSource = CollectionViewSource.GetDefaultView(VoteCollection);
            VoteSource.Filter = VoteFilter;
            VoteSource.SortDescriptions.Add(new SortDescription("ExpiryDate", ListSortDirection.Ascending));
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
            set
            {
                Set(ref _loading, value);
                RaisePropertyChanged(nameof(VotesVisible));
                RaisePropertyChanged(nameof(NoVotesMessageVisible));
                RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _noVotes;

        public bool NoVotes
        {
            get { return _noVotes; }
            set
            {
                Set(ref _noVotes, value);
                RaisePropertyChanged(nameof(VotesVisible));
                RaisePropertyChanged(nameof(NoVotesMessageVisible));
            }
        }

        private bool _showAllVotes;

        public bool ShowAllVotes
        {
            get { return _showAllVotes; }
            set
            {
                Set(ref _showAllVotes, value);
                VoteSource.Refresh();
            }
        }

        public bool NoVotesMessageVisible => !Loading && NoVotes;

        public bool VotesVisible => !Loading && !NoVotes;

        public ObservableRangeCollection<BlockchainViewModel> VoteCollection { get; }
        public ICollectionView VoteSource { get; }

        private BlockchainViewModel _selectedVote;

        public BlockchainViewModel SelectedVote
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
            if (SelectedVote == null)
                return;

            Loading = true;

            // Contact the Registrar to see if we have voted on this vote yet
            Task.Run(async () =>
            {
                var showResults = true;
                if (SelectedVote.IsCurrent)
                {
                    showResults = await _voteClient.HasVoted(SelectedVote.ChainString);

                    Ui(() => { Loading = false; });
                }

                if (!showResults)
                {
                    MainVm.ChangeView(EvotoView.Vote);
                    var voteView = GetVm<VoteViewModel>();
                    voteView.SelectVote(SelectedVote.GetModel());
                }
                else
                {
                    MainVm.ChangeView(EvotoView.Results);
                    var resultsVm = GetVm<ResultsViewModel>();
                    resultsVm.SelectVote(SelectedVote.GetModel());
                }
            });
        }

        private bool CanProceed()
        {
            return SelectedVote != null;
        }

        private bool CanRefresh()
        {
            return !Loading;
        }

        private async Task GetVotes()
        {
            Ui(() => { Loading = true; });

            var votes = (await _homeApiClient.GetCurrentVotes()).ToList();
            var voteVms = votes.Select(v => new BlockchainViewModel(v));

            Ui(() =>
            {
                Loading = false;
                if (votes.Any())
                {
                    VoteCollection.Clear();
                    VoteCollection.AddRange(voteVms);
                    VoteSource.Refresh();
                    NoVotes = false;
                }
                else
                {
                    NoVotes = true;
                }
            });
        }

        private bool VoteFilter(object item)
        {
            return ShowAllVotes || ((BlockchainViewModel) item).IsCurrent;
        }

        #endregion
    }
}