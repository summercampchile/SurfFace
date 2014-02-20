using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;


namespace sdkMapControlWP8CS
{
    public partial class LogIn : PhoneApplicationPage
    {
        public LogIn()
        {
            InitializeComponent();
        }


        private async void OnBtnFacebookAccountClick(object sender, RoutedEventArgs e)
        {
            try
            {
                
                ////TODO: Call LoginAsync: 
                await App.MobileServiceFacebook.LoginAsync(MobileServiceAuthenticationProvider.Facebook); 
               // if(App.mobileService.CurrentUser != null)
                //txtStatus.Text = string.Format("Logged in with: {0}", App.mobileService.CurrentUser.UserId);
                NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.RelativeOrAbsolute));

            }
            catch (InvalidOperationException iopEx)
            {
                MessageBox.Show(iopEx.Message);
            }
        }
		
		private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
		{
    	if(e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened)
           NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            else NavigationService.Navigate(new Uri("/LogIn.xaml", UriKind.RelativeOrAbsolute));
		}

       
        private void OnBtnLogoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: add logout
                if (App.MobileServiceFacebook.CurrentUser != null && App.MobileServiceFacebook.CurrentUser.UserId != null)
                     App.MobileServiceFacebook.Logout();

                MessageBox.Show("Logged out");
            }
            catch (InvalidOperationException iopEx)
            {
                MessageBox.Show(iopEx.Message);
            }
        }

        private void justGetMeIn(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	 NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
    }
