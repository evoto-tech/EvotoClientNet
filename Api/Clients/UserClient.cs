using System;
using System.Threading.Tasks;
using Api.Exceptions;
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
        }

        public async Task<UserDetails> GetCurrentUserDetails()
        {
            var res = await GetAsync<UserDetailsResponse>(Resources.DetailsAction, CurrentAuth.UserId);
            return res.MapToModel();
        }

        public async Task ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var requestModel = new ForgotPasswordRequestModel(forgotPasswordModel);
            try
            {
                await PostAsync(Resources.ForgotPasswordAction, requestModel);
            }
            catch (BadRequestException e)
            {
                ThrowNewIfDelayException(e);

                if (e.Message.StartsWith("Unconfirmed Email"))
                {
                    throw new UnconfirmedEmailException();
                }

                throw;
            }
            
        }

        public async Task ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var requestModel = new ResetPasswordRequestModel(resetPasswordModel);
            await PostAsync(Resources.ResetPasswordAction, requestModel);
            await LoginAsync(resetPasswordModel.Email, resetPasswordModel.Password);
        }

        public async Task VerifyEmail(VerifyEmailModel verifyEmailModel)
        {
            var requestModel = new VerifyEmailRequestModel(verifyEmailModel);
            await PostAsync(Resources.VerifyEmailAction, requestModel);
        }

        public async Task ResendVerificationEmail(string email)
        {
            var requestModel = new ResendVerificationRequestModel(email);
            try
            {
                await PostAsync(Resources.ResendVerificationEmailAction, requestModel);
            }
            catch (BadRequestException e)
            {
                ThrowNewIfDelayException(e);
                throw;
            }
        }

        private static void ThrowNewIfDelayException(Exception e)
        {
            if (e.Message.StartsWith("Please wait"))
            {
                var delay = e.Message.Substring(12);
                throw new TokenDelayException(delay);
            }
        }
    }
}