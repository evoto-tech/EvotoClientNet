using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Properties;
using Api.Responses;
using Models;

namespace Api.Clients
{
    public class HomeClient : ApiClient
    {
        public HomeClient() : base(Resources.HomeController)
        {
        }

        public async Task<IEnumerable<BlockchainDetails>> GetCurrentVotes()
        {
            var res = await GetAsync<IEnumerable<BlockchainResponse>>(Resources.CurrentVotesAction);
            return res.Select(r => r.ToModel());
        }
    }
}