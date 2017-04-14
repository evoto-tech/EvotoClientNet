/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:EvotoClient.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Diagnostics.CodeAnalysis;
using Blockchain;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MultiChainHandler>();
            SimpleIoc.Default.Register<MultiChainViewModel>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<VoteViewModel>();
            SimpleIoc.Default.Register<PostVoteViewModel>();
            SimpleIoc.Default.Register<FindVoteViewModel>();
            SimpleIoc.Default.Register<ResultsViewModel>();
            SimpleIoc.Default.Register<ForgotPasswordViewModel>();
            SimpleIoc.Default.Register<ResetPasswordViewModel>();
            SimpleIoc.Default.Register<UserBarViewModel>();
        }

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
             "CA1822:MarkMembersAsStatic",
             Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public LoginViewModel Login
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }

        public HomeViewModel Home
        {
            get { return ServiceLocator.Current.GetInstance<HomeViewModel>(); }
        }

        public RegisterViewModel Register
        {
            get { return ServiceLocator.Current.GetInstance<RegisterViewModel>(); }
        }

        public VoteViewModel Vote
        {
            get { return ServiceLocator.Current.GetInstance<VoteViewModel>(); }
        }

        public PostVoteViewModel PostVote
        {
            get { return ServiceLocator.Current.GetInstance<PostVoteViewModel>(); }
        }

        public FindVoteViewModel FindVote
        {
            get { return ServiceLocator.Current.GetInstance<FindVoteViewModel>(); }
        }

        public ResultsViewModel Results
        {
            get { return ServiceLocator.Current.GetInstance<ResultsViewModel>(); }
        }

        public ForgotPasswordViewModel ForgotPassword
        {
            get { return ServiceLocator.Current.GetInstance<ForgotPasswordViewModel>(); }
        }

        public ResetPasswordViewModel ResetPassword
        {
            get { return ServiceLocator.Current.GetInstance<ResetPasswordViewModel>(); }
        }

        public UserBarViewModel UserBar
        {
            get { return ServiceLocator.Current.GetInstance<UserBarViewModel>(); }
        }

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            var main = SimpleIoc.Default.GetInstance<MainViewModel>();
            main.Cleanup();
        }
    }
}