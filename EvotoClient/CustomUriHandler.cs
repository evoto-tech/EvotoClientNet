using System.Collections.Generic;
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

            var uriParams = arg.Substring(9).Split('/');
            // Right now we're only handling one action and one parameter
            if (uriParams.Length != 2)
                return;

            switch (uriParams[0])
            {
                case "resetPassword":
                    HandleResetPassword(uriParams[1]);
                    break;
            }
        }

        private static void HandleResetPassword(string arg)
        {
            // Token should only contain letters and numbers
            if (Regex.IsMatch(arg, "![a-z0-9]", RegexOptions.IgnoreCase))
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
    }
}