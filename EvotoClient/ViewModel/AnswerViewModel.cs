namespace EvotoClient.ViewModel
{
    public class AnswerViewModel : EvotoViewModelBase
    {
        #region Properties

        private string _answer;

        public string Answer
        {
            get { return _answer; }
            set { Set(ref _answer, value); }
        }

        private string _info;

        public string Info
        {
            get { return _info; }
            set { Set(ref _info, value); }
        }

        #endregion
    }
}