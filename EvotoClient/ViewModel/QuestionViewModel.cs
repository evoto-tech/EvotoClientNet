using System.Collections.Generic;

namespace EvotoClient.ViewModel
{
    public class QuestionViewModel : EvotoViewModelBase
    {
        private VoteViewModel _vvm;

        public QuestionViewModel(VoteViewModel vvm)
        {
            _vvm = vvm;
        }

        #region Properties

        private int _questionNumber;

        public int QuestionNumber
        {
            get { return _questionNumber; }
            set { Set(ref _questionNumber, value); }
        }

        private string _question;

        public string Question
        {
            get { return _question; }
            set { Set(ref _question, value); }
        }

        public List<AnswerViewModel> Answers { get; set; }

        private AnswerViewModel _selectedAnswer;

        public AnswerViewModel SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                Set(ref _selectedAnswer, value);
                RaisePropertyChanged(nameof(HasAnswer));
                _vvm.VoteChanged();
            }
        }

        public bool HasAnswer => SelectedAnswer != null;

        #endregion
    }
}