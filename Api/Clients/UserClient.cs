using System.Threading.Tasks;
using Api.Properties;
using Api.Requests;
using Api.Responses;
using Models;
using Models.Forms;

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

        public async Task ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var requestModel = new ForgotPasswordRequestModel(forgotPasswordModel);
            await PostAsync(Resources.ForgotPasswordAction, requestModel);
        }

        public async Task ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var requestModel = new ResetPasswordRequestModel(resetPasswordModel);
            await PostAsync(Resources.ResetPasswordAction, requestModel);
        }
    }
}