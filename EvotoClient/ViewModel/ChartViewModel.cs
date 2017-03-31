using System.Collections.Generic;
using Models;

namespace EvotoClient.ViewModel
{
    public class ChartViewModel : EvotoViewModelBase
    {
        public ChartViewModel(IDictionary<string, int> results)
        {
            Data = new ObservableRangeCollection<KeyValuePair<string, int>>(results);
        }

        private string _question;

        public ObservableRangeCollection<KeyValuePair<string, int>> Data { get; set; }

        public string Question
        {
            get { return _question; }
            set { Set(ref _question, value); }
        }

        public void UpdateData(IDictionary<string, int> results)
        {
            Data.Clear();
            Data.AddRange(results);
        }
    }
}