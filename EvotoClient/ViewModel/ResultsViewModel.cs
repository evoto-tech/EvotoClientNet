using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blockchain.Models;
using EvotoClient.Views;
using GalaSoft.MvvmLight.Command;
using Models;
using MultiChainLib.Client;
using Newtonsoft.Json;

namespace EvotoClient.ViewModel
{
    public class ResultsViewModel : EvotoViewModelBase
    {
        public ResultsViewModel()
        {
            BackCommand = new RelayCommand(DoBack);

            NextCommand = new RelayCommand(DoNext, CanNext);
            PrevCommand = new RelayCommand(DoPrev, CanPrev);

            Loaded += (sender, args) => { TransitionView = ((ResultsView) sender).pageTransition; };
        }

        #region Commands

        public RelayCommand BackCommand { get; }

        public RelayCommand NextCommand { get; }
        public RelayCommand PrevCommand { get; }

        #endregion

        #region Properties

        private MultiChainViewModel _multiChainVm;
        public MultiChainViewModel MultiChainVm => _multiChainVm ?? (_multiChainVm = GetVm<MultiChainViewModel>());

        public ObservableRangeCollection<ChartViewModel> Results =
            new ObservableRangeCollection<ChartViewModel>();

        public PageTransition TransitionView { private get; set; }

        public bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RaisePropertyChanged(nameof(ResultsVisible));
            }
        }

        public bool ResultsVisible => !Loading;

        private int _currentResults;

        public int CurrentResults
        {
            get { return _currentResults; }
            set
            {
                Set(ref _currentResults, value);
                NextCommand.RaiseCanExecuteChanged();
                PrevCommand.RaiseCanExecuteChanged();
            }
        }

        private int _totalResults;

        public int TotalResults
        {
            get { return _totalResults; }
            set { Set(ref _totalResults, value); }
        }

        #endregion

        #region Methods

        public void SelectVote(BlockchainDetails blockchain)
        {
            Ui(() => { Loading = true; });

            Task.Run(async () =>
            {
                await ConnectToBlockchain(blockchain);
                await GetResults(blockchain);
            });
        }

        private async Task ConnectToBlockchain(BlockchainDetails blockchain)
        {
            if (MultiChainVm.Connected)
                if (MultiChainVm.Model.Name != blockchain.ChainString)
                    await MultiChainVm.Disconnect();
                else
                    return;

            // TODO: Varied host
            await MultiChainVm.Connect("localhost", blockchain.Port, blockchain.ChainString);
        }

        private async Task GetResults(BlockchainDetails blockchain)
        {
            // Insure the blockchain is up to date
            await MultiChainVm.Model.WaitUntilBlockchainSynced(blockchain.Blocks);

            try
            {
                // Get the answers (aka transactions sent to the registrar's wallet ID)
                var txs = await MultiChainVm.Model.GetAddressTransactions(blockchain.WalletId);

                // Read the questions from the blockchain
                var questions = await MultiChainVm.Model.GetQuestions();

                // Read the answers from hex
                var answers = Enumerable.ToList(txs
                    .Where(t => t.Data != null && t.Data.Any())
                    .Select(v => MultiChainClient.ParseHexString(v.Data.First()))
                    .Select(Encoding.UTF8.GetString)
                    .Select(JsonConvert.DeserializeObject<BlockchainVoteModel>));

                // For each question, get its total for each answer
                var results = questions.Select(question =>
                {
                    // Setup response dictionary, answer -> num votes
                    var options = question.Answers.ToDictionary(a => a.Answer, a => 0);
                    foreach (var answer in answers)
                        foreach (var questionAnswer in answer.Answers.Where(a => a.Question == question.Number))
                        {
                            // In case we have anything unusual going on
                            if (!options.ContainsKey(questionAnswer.Answer))
                            {
                                Debug.WriteLine(
                                    $"Unexpected answer for question {questionAnswer.Question}: {questionAnswer.Answer}");
                                continue;
                            }
                            options[questionAnswer.Answer]++;
                        }

                    return new
                    {
                        question.Number,
                        question.Question,
                        Results = options
                    };
                }).ToList();

                var chartVms = results.Select(q => new ChartViewModel(q.Results)
                {
                    Question = q.Question
                }).ToList();

                Ui(() =>
                {
                    Loading = false;
                    Results.Clear();
                    Results.AddRange(chartVms);

                    TotalResults = chartVms.Count;
                    CurrentResults = 1;
                    TransitionView.ShowPage(Results.First());
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void DoBack()
        {
            MainVm.ChangeView(EvotoView.Home);
        }

        private void DoNext()
        {
            CurrentResults++;
            var page = Results[CurrentResults - 1];
            TransitionView.ShowPage(page);
        }

        private bool CanNext()
        {
            return CurrentResults < TotalResults;
        }

        private void DoPrev()
        {
            CurrentResults--;
            var page = Results[CurrentResults - 1];
            TransitionView.ShowPage(page, false);
        }

        private bool CanPrev()
        {
            return CurrentResults > 1;
        }

        #endregion
    }
}