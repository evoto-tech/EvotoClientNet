using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blockchain.Models;
using Models;
using MultiChainLib.Client;
using Newtonsoft.Json;

namespace EvotoClient.ViewModel
{
    public class ResultsViewModel : EvotoViewModelBase
    {
        public ResultsViewModel()
        {
            Data = new ObservableRangeCollection<KeyValuePair<string, int>>();
        }

        #region Methods

        public void SelectVote(BlockchainDetails blockchain)
        {
            Ui(() => { Loading = true; });

            Task.Run(async () =>
            {
                await ConnectToBlockchain(blockchain);
                await GetResults(blockchain);
            });
        }

        private async Task ConnectToBlockchain(BlockchainDetails blockchain)
        {
            if (MultiChainVm.Connected)
                if (MultiChainVm.Model.Name != blockchain.ChainString)
                    await MultiChainVm.Disconnect();
                else
                    return;

            // TODO: Varied host
            await MultiChainVm.Connect("localhost", blockchain.Port, blockchain.ChainString);
        }

        private async Task GetResults(BlockchainDetails blockchain)
        {
            // Get the answers (aka transactions sent to the registrar's wallet ID)
            var votes = await MultiChainVm.Model.GetAddressTransactions(blockchain.WalletId);

            // Read the questions from the blockchain
            var questions = await MultiChainVm.Model.GetQuestions();

            // Read the answers from hex
            var answers = votes
                .Select(v => MultiChainClient.ParseHexString(v.Data.First()))
                .Select(Encoding.UTF8.GetString)
                .Select(JsonConvert.DeserializeObject<BlockchainVoteModel>).ToList();

            // For each question, get its total for each answer
            var results = questions.Select(question =>
            {
                // Setup response dictionary, answer -> num votes
                var options = question.Answers.ToDictionary(a => a.Answer, a => 0);
                foreach (var answer in answers)
                {
                    // Each vote has answer for multiple questions. Only look at the one corresponding to our current question
                    foreach (var questionAnswer in answer.Answers.Where(a => a.Question == question.Number))
                    {
                        // In case we have anything unusual going on
                        if (!options.ContainsKey(questionAnswer.Answer))
                        {
                            Debug.WriteLine($"Unexpected answer for question {questionAnswer.Question}: {questionAnswer.Answer}");
                            continue;
                        }
                        options[questionAnswer.Answer]++;
                    }
                }

                return new
                {
                    question.Number,
                    question.Question,
                    Results = options
                };
            });

            Ui(() =>
            {
                Loading = false;
                Data.Clear();
                Data.AddRange(results.First().Results);
            });
        }

        #endregion

        #region Properties

        private MultiChainViewModel _multiChainVm;
        public MultiChainViewModel MultiChainVm => _multiChainVm ?? (_multiChainVm = GetVm<MultiChainViewModel>());

        private string _question;

        public string Question
        {
            get { return _question; }
            set { Set(ref _question, value); }
        }

        public ObservableRangeCollection<KeyValuePair<string, int>> Data { get; }

        public bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        #endregion
    }
}