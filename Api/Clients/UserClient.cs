using System.Threading.Tasks;
using Api.Properties;
using Api.Requests;
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
            var requestModel = new RegisterRequestModel(registerModel);
            await PostAsync<TokenUpdate>(Resources.RegisterAction, requestModel);
            await LoginAsync(registerModel.Email, registerModel.Password);
        }

        public async Task<UserDetails> GetCurrentUserDetails()
        {
            var res = await GetAsync<UserDetailsResponse>(Resources.DetailsAction, CurrentAuth.UserId);
            return res.MapToModel();
        }
    }
}