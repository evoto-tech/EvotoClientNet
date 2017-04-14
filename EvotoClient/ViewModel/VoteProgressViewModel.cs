using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace EvotoClient.ViewModel
{
    public class VoteProgressViewModel : EvotoViewModelBase
    {
        private static readonly List<string> steps = new List<string>
        {
            "Obtaining Registrar Public Key",
            "Blinding Identity Using Blind Signature",
            "Waiting Randomly to Obscure Identity",
            "Requesting Vote using Blind Signature",
            "Waiting for Vote to be Confirmed on Blockchain",
            "Sending Vote"
        };

        public VoteProgressViewModel()
        {
            Progress = new Progress<int>(UpdateProgress);
            var step = 1;
            ProgressItems =
                new ObservableCollection<VoteProgressItemViewModel>(
                    steps.Select(s => new VoteProgressItemViewModel(s, step++)));
        }

        public ObservableCollection<VoteProgressItemViewModel> ProgressItems { get; }

        public IProgress<int> Progress { get; }

        private void UpdateProgress(int stepComplete)
        {
            if (stepComplete > ProgressItems.Count)
            {
                Debug.WriteLine($"Error, invalid step complete: {stepComplete}");
                return;
            }

            Ui(() =>
            {
                if (stepComplete > 1)
                {
                    var previousStep = ProgressItems[stepComplete - 2];
                    previousStep.SetComplete(true);
                }

                var currentStep = ProgressItems[stepComplete - 1];
                currentStep.SetInProgress();
            });
        }
    }
}