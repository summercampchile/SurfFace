using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkMapControlWP8CS.ViewModels;
using sdkMapControlWP8CS.Resources;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Audio;
using System.IO.IsolatedStorage;
using System.IO;

namespace sdkMapControlWP8CS
{
    public partial class SoundBoard : PhoneApplicationPage
    {
        public SoundBoard()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            BuildLozalizedApplicationBar();
        }

        private void BuildLozalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton recordAudioAppBar =
                new ApplicationBarIconButton();
            recordAudioAppBar.IconUri =
                new Uri("/Assets/AppBar/microphone.png", UriKind.Relative);

            recordAudioAppBar.Text = AppResources.AppBarRecord;

            recordAudioAppBar.Click += RecordAudioClick;

            ApplicationBarMenuItem aboutAppBar = new ApplicationBarMenuItem();
            aboutAppBar.Text = AppResources.AppBarAbout;

            aboutAppBar.Click += AboutClick;

            ApplicationBar.Buttons.Add(recordAudioAppBar);
            ApplicationBar.MenuItems.Add(aboutAppBar);

        }

        private void AboutClick(object sender, EventArgs e)
        {
            //AboutPromt aboutMe = new AboutPromt();
        }

        private void RecordAudioClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void LongListerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            if (selector == null)
                return;

            SoundData data = selector.SelectedItem as SoundData;

            if (data == null)
                return;

            if (File.Exists(data.FilePath))
            {
                AudioPlayer.Source = new Uri(data.FilePath, UriKind.RelativeOrAbsolute);
            }
            else
            {
                using (var storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = new IsolatedStorageFileStream(data.FilePath, FileMode.Open, FileAccess.Read, storageFolder))
                    {
                        AudioPlayer.SetSource(stream);
                    }
                }
            }

            //AudioPlayer.Source = new Uri(data.FilePath, UriKind.RelativeOrAbsolute);

            selector.SelectedItem = null;

        }
    }
}