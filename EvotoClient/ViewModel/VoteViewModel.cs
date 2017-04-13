using System;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Exceptions;
using EvotoClient.Views;
using GalaSoft.MvvmLight.Command;
using Models;

namespace EvotoClient.ViewModel
{
    public class VoteViewModel : EvotoViewModelBase
    {
        private BlockchainDetails _currentDetails;

        public VoteViewModel()
        {
            VoteCommand = new RelayCommand(DoVote, CanVote);
            BackCommand = new RelayCommand(DoBack);

            NextCommand = new RelayCommand(DoNext, CanNext);
            PrevCommand = new RelayCommand(DoPrev, CanPrev);

            ReconnectCommand = new RelayCommand(Connect);

            Loaded += (sender, args) => { TransitionView = ((VoteView) sender).pageTransition; };
        }

        #region Commands

        public RelayCommand VoteCommand { get; }
        public RelayCommand BackCommand { get; }

        public RelayCommand NextCommand { get; }
        public RelayCommand PrevCommand { get; }

        public RelayCommand ReconnectCommand { get; }

        #endregion

        #region Properties

        private MultiChainViewModel _multiChainVm;
        public MultiChainViewModel MultiChainVm => _multiChainVm ?? (_multiChainVm = GetVm<MultiChainViewModel>());

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RaisePropertyChanged(nameof(VoteVisble));
            }
        }

        public bool VoteVisble => !Loading;

        public string QuestionText
        {
            get
            {
                if ((Questions == null) || !Questions.Any() || (CurrentQuestion == 0))
                    return "";
                return $"Question {CurrentQuestion} of {TotalQuestions}";
            }
        }

        public ObservableRangeCollection<QuestionViewModel> Questions =
            new ObservableRangeCollection<QuestionViewModel>();

        public PageTransition TransitionView { private get; set; }

        private int _currentQuestion;

        public int CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                Set(ref _currentQuestion, value);
                RaisePropertyChanged(nameof(QuestionText));
                NextCommand.RaiseCanExecuteChanged();
                PrevCommand.RaiseCanExecuteChanged();
            }
        }

        private int _totalQuestions;

        public int TotalQuestions
        {
            get { return _totalQuestions; }
            set
            {
                Set(ref _totalQuestions, value);
                VoteCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(QuestionText));
            }
        }

        private bool _cannotConnect;

        public bool CannotConnect
        {
            get { return _cannotConnect; }
            set { Set(ref _cannotConnect, value); }
        }

        #endregion

        #region Methods

        public void SelectVote(BlockchainDetails blockchain)
        {
            _currentDetails = blockchain;

            Connect();
        }

        private void Connect()
        {
            if (Loading)
                return;

            Ui(() =>
            {
                Loading = true;
                CannotConnect = false;
                TotalQuestions = 0;
                CurrentQuestion = 0;
            });

            Task.Run(async () =>
            {
                try
                {
                    await ConnectToBlockchain(_currentDetails);
                    await GetVoteDetails();
                }
                catch (CouldNotConnectToBlockchainException)
                {
                    Ui(() =>
                    {
                        Loading = false;
                        CannotConnect = true;
                    });
                }
            });
        }

        public void VoteChanged()
        {
            VoteCommand.RaiseCanExecuteChanged();
        }

        private async Task ConnectToBlockchain(BlockchainDetails blockchain)
        {
            if (MultiChainVm.Connected)
                if (MultiChainVm.Model.Name != blockchain.ChainString)
                    await MultiChainVm.Disconnect();
                else
                    return;

            await MultiChainVm.Connect(blockchain.Host, blockchain.Port, blockchain.ChainString);
        }

        private async Task GetVoteDetails()
        {
            var questions = await MultiChainVm.Model.GetQuestions();
            var questionVMs = questions.Select(q => new QuestionViewModel(this)
            {
                QuestionNumber = q.Number,
                Question = q.Question,
                Answers = q.Answers.Select(a => new AnswerViewModel
                {
                    Answer = a.Answer,
                    Info = a.Info
                }).ToList()
            }).ToList();

            Ui(() =>
            {
                Loading = false;

                Questions.Clear();
                Questions.AddRange(questionVMs);

                TotalQuestions = questionVMs.Count;
                CurrentQuestion = 1;
                TransitionView.ShowPage(Questions.First());
            });
        }

        private void DoVote()
        {
            if (!MultiChainVm.Connected)
                throw new Exception("Not connected");
            Ui(() => { Loading = true; });

            Task.Run(async () =>
            {
                var words = await MultiChainVm.Vote(Questions.ToList(), _currentDetails);
                Ui(() =>
                {
                    Loading = false;

                    var postVoteVm = GetVm<PostVoteViewModel>();
                    postVoteVm.Voted(_currentDetails, words);
                    MainVm.ChangeView(EvotoView.PostVote);
                });
            });
        }

        private bool CanVote()
        {
            if (!Questions.Any())
                return false;
            return Questions.All(q => q.HasAnswer);
        }

        private void DoBack()
        {
            MainVm.ChangeView(EvotoView.Home);
        }

        private void DoNext()
        {
            CurrentQuestion++;
            var page = Questions[CurrentQuestion - 1];
            TransitionView.ShowPage(page);
        }

        private bool CanNext()
        {
            return CurrentQuestion < TotalQuestions;
        }

        private void DoPrev()
        {
            CurrentQuestion--;
            var page = Questions[CurrentQuestion - 1];
            TransitionView.ShowPage(page, false);
        }

        private bool CanPrev()
        {
            return CurrentQuestion > 1;
        }

        #endregion
    }
}