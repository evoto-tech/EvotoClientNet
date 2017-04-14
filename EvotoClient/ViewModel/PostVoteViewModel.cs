using System;
using System.Windows;
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
            HomeCommand = new RelayCommand(DoHome);
            CopyCommand = new RelayCommand(DoCopy);
        }

        public void Voted(BlockchainDetails blockchain, string words)
        {
            CopyStatus = "";
            MagicWords = words;
            _blockchain = blockchain;

            Encrypted = blockchain.ShouldEncryptResults;
            ProceedCommand.RaiseCanExecuteChanged();
        }

        #region Commands

        public RelayCommand ProceedCommand { get; }
        public RelayCommand HomeCommand { get; }
        public RelayCommand CopyCommand { get; }

        #endregion

        #region Properties

        public bool _encrypted;

        public bool Encrypted
        {
            get { return _encrypted; }
            set { Set(ref _encrypted, value); }
        }

        public string EncryptedText
        {
            get
            {
                var remaining = GetTimeRemaining();
                var time = _blockchain.ExpiryDate.ToShortTimeString();
                var date = _blockchain.ExpiryDate.ToShortDateString();

                return
                    "This blockchain is encrypted. \n" +
                    "This means that the results are not accessible until after the vote has finished.\n" +
                    $"Please check back in {remaining} ({time} {date}).";
            }
        }


        private string _magicWords;

        public string MagicWords
        {
            get { return _magicWords; }
            set { Set(ref _magicWords, value); }
        }

        private string _copyStatus;

        public string CopyStatus
        {
            get { return _copyStatus; }
            set { Set(ref _copyStatus, value); }
        }

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
            return !Encrypted;
        }

        private void DoHome()
        {
            MainVm.ChangeView(EvotoView.Home);
        }

        private void DoCopy()
        {
            Clipboard.SetText(MagicWords);
            CopyStatus = "Copied!";
        }

        private string GetTimeRemaining()
        {
            var span = _blockchain.ExpiryDate - DateTime.Now;
            return $"{span.Days} days, {span.Hours} hours, {span.Minutes} minutes";
        }

        #endregion
    }
}