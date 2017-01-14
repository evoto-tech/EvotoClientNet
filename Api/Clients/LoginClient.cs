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

        public async Task<UserDetails> Login(LoginModel loginModel)
        {
            var res = await Exec<UserDetailsResponse>("", Method.POST, loginModel);
            return res.MapToModel();
        }

        public async Task<UserDetails> Register(RegisterModel registerModel)
        {
            var res = await Exec<UserDetailsResponse>("", Method.PUT, registerModel);
            return res.MapToModel();
        }
    }
}