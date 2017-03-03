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
            VoteCommand = new RelayCommand(DoVote);
            BackCommand = new RelayCommand(DoBack);
        }

        public RelayCommand VoteCommand { get; }
        public RelayCommand BackCommand { get; }

        public void SelectVote(BlockchainDetails blockchain)
        {
            Task.Run(async () =>
            {
                await ConnectToBlockchain(blockchain);
                await GetVoteDetails();
            });
        }

        private async Task ConnectToBlockchain(BlockchainDetails blockchain)
        {
            if (MultiChainVm.Connected)
            {
                if (MultiChainVm.Model.Name != blockchain.Name)
                    await MultiChainVm.Disconnect();
                else
                    return;
            }

            // TODO: Varied host
            await MultiChainVm.Connect("localhost", blockchain.Port, blockchain.ChainString);
        }

        private async Task GetVoteDetails()
        {
            var questions = await MultiChainVm.GetQuestions();
            Ui(() => { Question = questions.First(); });
        }

        private void DoVote()
        {
            Debug.WriteLine("Voted");
        }

        private void DoBack()
        {
            MainVm.ChangeView(EvotoView.Home);
        }

        #region Properties

        private MultiChainViewModel _multiChainVm;
        public MultiChainViewModel MultiChainVm => _multiChainVm ?? (_multiChainVm = GetVm<MultiChainViewModel>());

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
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
            set { Set(ref _selectedAnswer, value); }
        }

        #endregion
    }
}