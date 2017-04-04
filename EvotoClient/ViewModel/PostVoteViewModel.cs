using System;
using GalaSoft.MvvmLight.Command;
using Models;

namespace EvotoClient.ViewModel
{
    public class PostVoteViewModel : EvotoViewModelBase
    {
        private BlockchainDetails _blockchain;

        public PostVoteViewModel()
        {
            ProceedCommand = new RelayCommand(DoProceed, CanProceed);
        }

        #region Commands

        public RelayCommand ProceedCommand { get; }

        #endregion

        public void Voted(BlockchainDetails blockchain, string words)
        {
            MagicWords = words;
            _blockchain = blockchain;
            RaisePropertyChanged(nameof(ResultsEnabled));
        }

        #region Properties

        private string _magicWords;

        public string MagicWords
        {
            get { return $"Magic Words: " + _magicWords; }
            set { Set(ref _magicWords, value); }
        }

        public bool ResultsEnabled
            => string.IsNullOrWhiteSpace(_blockchain.EncryptKey) || (_blockchain.ExpiryDate > DateTime.Now);

        #endregion

        #region Methods

        private void DoProceed()
        {
            var resultsVm = GetVm<ResultsViewModel>();
            resultsVm.SelectVote(_blockchain);
            MainVm.ChangeView(EvotoView.Results);
        }

        private bool CanProceed()
        {
            return ResultsEnabled;
        }

        #endregion
    }
}