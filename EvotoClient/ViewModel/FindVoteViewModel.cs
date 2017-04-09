using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace EvotoClient.ViewModel
{
    public class FindVoteViewModel : EvotoViewModelBase
    {
        private List<BlockchainVoteModelPlainText> _answers;

        public FindVoteViewModel()
        {
            BackCommand = new RelayCommand(DoBack);
            FindCommand = new RelayCommand(DoFind);
        }

        public void SetResults(List<BlockchainVoteModelPlainText> results)
        {
            _answers = results;
        }

        #region Commands

        public RelayCommand BackCommand { get; }

        public RelayCommand FindCommand { get; }

        #endregion

        #region Properties

        private string _magicWords;

        public string MagicWords
        {
            get { return _magicWords; }
            set { Set(ref _magicWords, value); }
        }

        private bool _notFound;

        public bool NotFound
        {
            get { return _notFound; }
            set { Set(ref _notFound, value); }
        }

        private string _answerText;

        public string AnswerText
        {
            get { return _answerText; }
            set { Set(ref _answerText, value); }
        }

        private MultiChainViewModel _multiChainVm;
        public MultiChainViewModel MultiChainVm => _multiChainVm ?? (_multiChainVm = GetVm<MultiChainViewModel>());

        #endregion

        #region Methods

        public void DoBack()
        {
            MainVm.ChangeView(EvotoView.Results);
            var resultsVm = GetVm<ResultsViewModel>();
            resultsVm.ShowPage();
        }

        public void DoFind()
        {
            Task.Run(() =>
            {
                var answer = _answers.FirstOrDefault(a => a.MagicWords == MagicWords);
                var answerText = answer == null ? "" : JsonConvert.SerializeObject(answer.Answers);
                Ui(() =>
                {
                    NotFound = (answer == null);
                    AnswerText = answerText;
                });
            });
        }

        #endregion
    }
}