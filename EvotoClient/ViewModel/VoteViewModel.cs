using System;
using System.Linq;
using System.Threading.Tasks;
using EvotoClient.Views;
using GalaSoft.MvvmLight.Command;
using Models;

namespace EvotoClient.ViewModel
{
    public class VoteViewModel : EvotoViewModelBase
    {
        public VoteViewModel()
        {
            VoteCommand = new RelayCommand(DoVote, CanVote);
            BackCommand = new RelayCommand(DoBack);

            NextCommand = new RelayCommand(DoNext, CanNext);
            PrevCommand = new RelayCommand(DoPrev, CanPrev);

            Loaded += (sender, args) => { TransitionView = ((VoteView) sender).pageTransition; };
        }

        #region Commands

        public RelayCommand VoteCommand { get; }
        public RelayCommand BackCommand { get; }

        public RelayCommand NextCommand { get; }
        public RelayCommand PrevCommand { get; }

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

        public bool VoteVisble => !Loading && !Voted;

        public string QuestionText
        {
            get
            {
                if ((Questions == null) || !Questions.Any() || (CurrentQuestion == 0))
                    return "";
                return $"Question {CurrentQuestion} of {TotalQuestions}";
            }
        }

        private bool _voted;

        public bool Voted
        {
            get { return _voted; }
            set
            {
                Set(ref _voted, value);
                VoteCommand.RaiseCanExecuteChanged();
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

        #endregion

        #region Methods

        public void SelectVote(BlockchainDetails blockchain)
        {
            Ui(() => { Loading = true; });

            Task.Run(async () =>
            {
                await ConnectToBlockchain(blockchain);
                await GetVoteDetails();
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

            // TODO: Varied host
            await MultiChainVm.Connect("localhost", blockchain.Port, blockchain.ChainString);
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
                Voted = false;

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
                await MultiChainVm.Vote(Questions.ToList());
                Ui(() =>
                {
                    Voted = true;
                    Loading = false;
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