using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Api.Clients;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public class HomeViewModel : EvotoViewModelBase
    {
        private readonly HomeClient _homeApiClient;

        private bool _loading;

        public HomeViewModel()
        {
            _homeApiClient = new HomeClient();

            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            mainVm.OnLogin += async (e, u) => await GetVotes(e, u);

            Votes = new ObservableRangeCollection<BlockchainDetails>();
        }

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        public ObservableRangeCollection<BlockchainDetails> Votes;

        private async Task GetVotes(object sender, UserDetails details)
        {
            var votes = await _homeApiClient.GetCurrentVotes();
            Ui(() =>
            {
                Votes.Clear();
                Votes.AddRange(votes);
            });
        }
    }
}