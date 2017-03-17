using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using EvotoClient.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient
{
    public class CustomUriHandler
    {
        public static void HandleArgs(IList<string> args)
        {
            // Ensure only handling one argument (aside from the exe path)
            if (args.Count != 2)
                return;

            var arg = args[1];
            // Ensure there's nothing funny going on with the custom uri
            if (arg.Substring(0, 8) != "evoto://")
                return;

            var uriParams = arg.Substring(8).Split('/');
            // Expect at least 2 params (command + param(s))
            if (uriParams.Length < 2)
                return;

            switch (uriParams[0].ToLower())
            {
                case "resetpassword":
                    HandleResetPassword(uriParams[1]);
                    break;
                case "confirmemail":
                    HandleConfirmEmail(uriParams);
                    break;
            }
        }

        private static void HandleResetPassword(string arg)
        {
            // Token should only contain letters and numbers
            if (InvalidToken(arg))
                return;

            // Check we're not already logged in
            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (mainVm.LoggedIn)
                return;

            // Switch to password reset view
            mainVm.ChangeView(EvotoView.ResetPassword);

            var resetPassVm = ServiceLocator.Current.GetInstance<ResetPasswordViewModel>();
            resetPassVm.SetToken(arg);
        }

        private static void HandleConfirmEmail(string[] args)
        {
            // Expect 3 args
            if (args.Length != 3)
                return;

            // Get email and validate
            var email = args[1];

            var foo = new EmailAddressAttribute();
            if (!foo.IsValid(email))
                return;

            // Get token and validate
            var token = args[2];

            if (InvalidToken(token))
                return;

            // Check we're not already logged in
            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (mainVm.LoggedIn)
                return;

            // Switch to login view
            mainVm.ChangeView(EvotoView.Login);

            var loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();
            loginVm.SetToken(email, token);
        }

        private static bool InvalidToken(string token)
        {
            return Regex.IsMatch(token, "![a-z0-9]", RegexOptions.IgnoreCase);
        }
    }
}