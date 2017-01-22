using System.Threading.Tasks;
using Api.Properties;
using Api.Responses;
using Models;

namespace Api.Clients
{
    public class UserClient : ApiClient
    {
        public UserClient() : base(Resources.AccountController)
        {
        }

        public async Task Register(RegisterModel registerModel)
        {
            await PostAsync<TokenUpdate>(Resources.RegisterAction, registerModel);
            await LoginAsync(registerModel.Email, registerModel.Password);
        }

        public async Task<UserDetails> GetCurrentUserDetails()
        {
            var res = await GetAsync<UserDetailsResponse>(Resources.DetailsAction, CurrentAuth.UserId);
            return res.MapToModel();
        }
    }
}