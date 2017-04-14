namespace EvotoClient.ViewModel
{
    public class VoteProgressItemViewModel : EvotoViewModelBase
    {
        public VoteProgressItemViewModel(string name, int step)
        {
            Name = name;
            Step = $"{step}.";
        }

        #region Methods

        public void SetInProgress()
        {
            InProgress = true;
        }

        public void SetComplete(bool success)
        {
            InProgress = false;
            IsComplete = success;
            IsError = !success;
        }

        #endregion

        #region Properties

        public string Step { get; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private bool _inProgress;

        public bool InProgress
        {
            get { return _inProgress; }
            set { Set(ref _inProgress, value); }
        }

        private bool _isComplete;

        public bool IsComplete
        {
            get { return _isComplete; }
            set { Set(ref _isComplete, value); }
        }

        private bool _isError;

        public bool IsError
        {
            get { return _isError; }
            set { Set(ref _isError, value); }
        }

        #endregion
    }
}