using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
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
        }

        #region Commands

        public RelayCommand VoteCommand { get; }
        public RelayCommand BackCommand { get; }

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

        private BlockchainQuestionModel _question;

        public BlockchainQuestionModel Question
        {
            get { return _question; }
            set { Set(ref _question, value); }
        }

        private string _selectedAnswer;

        public string SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                Set(ref _selectedAnswer, value);
                VoteCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Methods

        public void SelectVote(BlockchainDetails blockchain)
        {
            Loading = true;
            Task.Run(async () =>
            {
                await ConnectToBlockchain(blockchain);
                await GetVoteDetails();
            });
        }

        private async Task ConnectToBlockchain(BlockchainDetails blockchain)
        {
            if (MultiChainVm.Connected)
                if (MultiChainVm.Model.Name != blockchain.Name)
                    await MultiChainVm.Disconnect();
                else
                    return;

            // TODO: Varied host
            await MultiChainVm.Connect("localhost", blockchain.Port, blockchain.ChainString);
        }

        private async Task GetVoteDetails()
        {
            var questions = await MultiChainVm.GetQuestions();
            Ui(() =>
            {
                Loading = false;
                Question = questions.First();
            });
        }

        private bool CanVote()
        {
            return !string.IsNullOrEmpty(SelectedAnswer);
        }

        private void DoVote()
        {
            if (!MultiChainVm.Connected)
            {
                throw new Exception("Not connected");
            }
                Task.Run(() =>
                {
                    MultiChainVm.Vote(SelectedAnswer);
                });
        }

        private void DoBack()
        {
            MainVm.ChangeView(EvotoView.Home);
        }

        #endregion
    }
}