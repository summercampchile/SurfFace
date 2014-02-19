using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkMapControlWP8CS.Resources;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Coding4Fun.Toolkit.Audio;
using Coding4Fun.Toolkit.Audio.Helpers;
using Coding4Fun.Toolkit.Controls;
using sdkMapControlWP8CS.ViewModels;
using Microsoft.Phone.Maps;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;





namespace sdkMapControlWP8CS
{

    public class Location
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; } 

        /*[JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        [JsonProperty(PropertyName = "resourceName")]
        public string ResourceName { get; set; }

        [JsonProperty(PropertyName = "sasQueryString")]
        public string SasQueryString { get; set; }

        [JsonProperty(PropertyName = "Audio")]
        public string Audio { get; set; }*/
    }

    public partial class Record : PhoneApplicationPage
    {

        private SoundData soundData;

        private IMobileServiceTable<Location> LocationTable = App.MobileService.GetTable<Location>();
        private MobileServiceCollection<Location, Location> locations;


        //private IMobileServiceTable<SoundData> SoundData = App.MobileService.GetTable<SoundData>();


        //actual audio grabado
        private MicrophoneRecorder _recorder = new MicrophoneRecorder();

        //revisa el buffer de audio
        private IsolatedStorageFileStream _audioStream;
        private string _tempFileName = "tempWav.wav";

        //sacar audioName!!!
        private string audioName;
        private Geoposition myGeoposition;

        public Record()
        {

            InitializeComponent();

            BuildLocalizedApplicationBar();

            Loaded += MainPage_Loaded;

        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Geolocator myGeolocator = new Geolocator();
            myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;

            Lati.Text = myGeoposition.Coordinate.Latitude.ToString();
            Long.Text = myGeoposition.Coordinate.Longitude.ToString();
        }

        private async void RefreshLocations()
        {
            try
            {
                locations = await LocationTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Error loading locations", MessageBoxButton.OK);
            }

        }


        //Barra + boton guardar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton recordAudioAppBar = 
                new ApplicationBarIconButton();
            recordAudioAppBar.IconUri = 
                new Uri("/Assets/AppBar/save.png", UriKind.Relative);
            recordAudioAppBar.Text = AppResources.AppBarSave;

            recordAudioAppBar.Click += SaveRecordingClick;

            ApplicationBar.Buttons.Add(recordAudioAppBar);
            ApplicationBar.IsVisible = false;
        }

        //trigger boton guardar
        private void SaveRecordingClick(object sender, EventArgs e)
        {
            InputPrompt fileName = new InputPrompt();

            fileName.Title = "Sound Name";
            fileName.Message = "What should we call the sound?";

           

            fileName.Completed += FileNameCompleted;

            fileName.Show();
        }


        // listener de boton guardar, no implementado
      
        private void LocationSave(object sender, RoutedEventArgs e)
        {
            Location location = new Location();
            location.Latitude = myGeoposition.Coordinate.Latitude;
            location.Longitude = myGeoposition.Coordinate.Longitude;
            location.Text = Name.Text;
            location.description = "descripcion";

            PrompToSubmit(location);

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void FileNameCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                
                // Create a SoundData object
                soundData = new SoundData();
                soundData.FilePath = string.Format("/customAudio/{0}.wav", DateTime.Now.ToFileTime());
                soundData.Title = e.Result;
                soundData.Latitude = myGeoposition.Coordinate.Latitude;
                soundData.Longitude = myGeoposition.Coordinate.Longitude;


                audioName = e.Result;
                Name.Text = e.Result;

                Location location = new Location();
                location.Latitude = myGeoposition.Coordinate.Latitude;
                location.Longitude = myGeoposition.Coordinate.Longitude;
                location.Text = audioName;
                
            
                
                PrompToSubmit(location);
                

                // Save wav file into directory /customAudio/
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication()) 
                {
                    if (!isoStore.DirectoryExists("/customAudio/"))
                        isoStore.CreateDirectory("/customAudio/");

                    isoStore.MoveFile(_tempFileName, soundData.FilePath);
                }

                // Add the SoundData to App.ViewModel.CustomSounds
                App.ViewModel.CustomSounds.Items.Add(soundData);

                // Save the list of CustomSounds to IsolatedStorage.ApplicationSettings
                var data = JsonConvert.SerializeObject(App.ViewModel.CustomSounds);

                IsolatedStorageSettings.ApplicationSettings[SoundModel.CustomSoundKey] = data;
                IsolatedStorageSettings.ApplicationSettings.Save();

                // We'll need to modify our SoundModel to retrieve CustomSounds
                // from IsolatedStorage.ApplicationSettings

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void PrompToSubmit(Location location)
        {
             InsertLocation(location);
        }

        private async void InsertLocation(Location location)
        {

            await LocationTable.InsertAsync(location);

        }



        private void RecordAudioChecked(object sender, RoutedEventArgs e)
        {
            PlayAudio.IsEnabled = false;
            _recorder.Start();
        }

        private void RecordAudioUnchecked(object sender, RoutedEventArgs e)
        {
            _recorder.Stop();

            SaveTempAudio(_recorder.Buffer);

            PlayAudio.IsEnabled = true;
            ApplicationBar.IsVisible = true;
        }

        private void SaveTempAudio(MemoryStream buffer)
        {
            // Be defensive ... trust no one & nothing
            if (buffer == null)
                throw new ArgumentNullException("Attempting to save an empty sound buffer.");

            // Clean out the AudioPlayer's hold on our audioStream
             if (_audioStream != null)
            {
                AudioPlayer.Stop();
                AudioPlayer.Source = null;

                _audioStream.Dispose();
            }
           
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication()) 
            {

                if (isoStore.FileExists(_tempFileName))
                    isoStore.DeleteFile(_tempFileName);

                _tempFileName = string.Format("{0}.wav", DateTime.Now.ToFileTime());

                //transforma el buffer en wav
                var bytes = buffer.GetWavAsByteArray(_recorder.SampleRate);

                _audioStream = isoStore.CreateFile(_tempFileName);

                //escribe con bytes de 0 el audiostream, lo libera
                _audioStream.Write(bytes, 0, bytes.Length);

                // le pasa al audio player la grabacion reciente
                AudioPlayer.SetSource(_audioStream);


            }
        }

        private void PlayAudioClick(object sender, RoutedEventArgs e)
        {
            AudioPlayer.Play();
        }

        

       

    }
}
