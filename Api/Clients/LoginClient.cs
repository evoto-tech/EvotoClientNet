using System.Threading.Tasks;
using Api.Properties;
using Api.Responses;
using Models;
using RestSharp;

namespace Api.Clients
{
    public class LoginClient : ApiClient
    {
        public LoginClient() : base(Resources.LoginController)
        {
        }

        public async Task<UserDetails> Login(string email, string password)
        {
            var res = await Exec<UserDetailsResponse>("", Method.POST, new {email, password});
            return res.MapToModel();
        }

        public async Task<UserDetails> Register(object details)
        {
            var res = await Exec<UserDetailsResponse>("", Method.PUT, details);
            return res.MapToModel();
        }
    }
}