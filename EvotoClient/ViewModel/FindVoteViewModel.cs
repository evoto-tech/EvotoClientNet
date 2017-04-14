using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using GalaSoft.MvvmLight.Command;

namespace EvotoClient.ViewModel
{
    public class FindVoteViewModel : EvotoViewModelBase
    {
        private List<BlockchainVoteModelPlainText> _answers;
        private List<BlockchainQuestionModel> _questions;

        public FindVoteViewModel()
        {
            BackCommand = new RelayCommand(DoBack);
            FindCommand = new RelayCommand(DoFind, CanFind);
        }

        public void SetResults(List<BlockchainQuestionModel> questions, List<BlockchainVoteModelPlainText> answers)
        {
            _questions = questions;
            _answers = answers;
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
            set
            {
                Set(ref _magicWords, value);
                FindCommand.RaiseCanExecuteChanged();
            }
        }

        private string _lastMagicWords;

        public string LastMagicWords
        {
            get { return _lastMagicWords; }
            set { Set(ref _lastMagicWords, value); }
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
            resultsVm.ReInit();
        }

        public void DoFind()
        {
            Task.Run(() =>
            {
                LastMagicWords = MagicWords;
                
                var answer = _answers.FirstOrDefault(a => a.MagicWords == MagicWords.Trim());
                Ui(() =>
                {
                    if (answer == null)
                        AnswerText = "Answer Not Found";
                    else
                        AnswerText = FormatAnswer(answer);

                    FindCommand.RaiseCanExecuteChanged();
                });
            });
        }

        private string FormatAnswer(BlockchainVoteModelPlainText answers)
        {
            var answerText = "";
            foreach (var q in _questions)
            {
                if (answerText != "")
                    answerText += "\n";
                answerText += $"Question: {q.Question}\n";
                var answer = answers.Answers.FirstOrDefault(a => a.Question == q.Number);
                if (answer == null)
                    answerText += "Answer Not Found!";
                else
                    answerText += "Answer: " + answer.Answer;
            }
            return answerText;
        }
        
        private bool CanFind()
        {
            if (string.IsNullOrWhiteSpace(MagicWords))
                return false;
            return MagicWords != LastMagicWords;
        }

        #endregion
    }
}