/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
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
using sdkMapControlWP8CS.ViewModels;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;


namespace sdkMapControlWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {

        private IMobileServiceTable<Location> LocationTable = App.MobileService.GetTable<Location>();

        private Location location = new Location();
        public static double myLatitude;
        public static double myLongitude;
       
        const int MIN_ZOOM_LEVEL = 1;
        const int MAX_ZOOM_LEVEL = 20;
        const int MIN_ZOOMLEVEL_FOR_LANDMARKS = 14;
        bool globito = true;


        ToggleStatus locationToggleStatus = ToggleStatus.ToggledOff;
        //ToggleStatus landmarksToggleStatus = ToggleStatus.ToggledOff;

        GeoCoordinate currentLocation = null;
        MapLayer locationLayer = null;
        MapLayer locationAreaLayer = null;

        // Constructor.
        public MainPage()
        {
            DataContext = App.ViewModel;
            InitializeComponent();

            // Create the localized ApplicationBar.
            BuildLocalizedApplicationBar();

            // Get current location.
            GetLocation();
            obtenerLugares();
        }

        public void obtenerLugares()
        {
            this.Loaded += async (sender, args) =>
                {
                    var lugares = await LocationTable.ReadAsync();
                    App.ViewModel.CustomSounds.Items.Clear();
                    var layer = new MapLayer();
                    foreach (var lugar in lugares)
                    {
                        Canvas myCanvas = new Canvas();

                        if (globito == true)
                        {


                            Image image = new Image();
                            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/globoFondo.png", UriKind.RelativeOrAbsolute));
                            image.Opacity = 0.8;
                            image.Stretch = System.Windows.Media.Stretch.None;
                            


                            myCanvas.Children.Add(image);

                            TextBox titulo = new TextBox();
                            titulo.FontSize = 24;
                            //TextBlock1.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Black);
                            titulo.Text = "HearMe";
                            Canvas.SetTop(titulo, 20);
                            Canvas.SetLeft(titulo, 30);

                            myCanvas.Children.Add(titulo);

                            Image image2 = new Image();
                            image2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/pin.png", UriKind.RelativeOrAbsolute));
                            image2.Opacity = 0.8;
                            image2.Stretch = System.Windows.Media.Stretch.None;
                            Canvas.SetTop(image2, 40);
                            Canvas.SetLeft(image2, 30);

                            myCanvas.Children.Add(image2);
                        }

                        else
                        {
                            Image image = new Image();
                            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/pin.png", UriKind.RelativeOrAbsolute));
                            image.Opacity = 0.8;
                            image.Stretch = System.Windows.Media.Stretch.None;


                            myCanvas.Children.Add(image);
                        }

                        myCanvas.Tap += cambiarImagen;

                        locationToSoundData(lugar);
                        // Create a MapOverlay and add marker.
                        MapOverlay overlay = new MapOverlay();
                        overlay.Content = myCanvas;
                        overlay.GeoCoordinate = new GeoCoordinate(lugar.Latitude,lugar.Longitude);
                        overlay.PositionOrigin = new Point(0.0, 0.0);
                        layer.Add(overlay);

                         
                        /*
                        MapOverlay overlay = new MapOverlay();
                        overlay.Content = globo_8;
                        globo_8.Visibility = Visibility.Visible;
                        overlay.GeoCoordinate = new GeoCoordinate(lugar.Latitude, lugar.Longitude);
                        overlay.PositionOrigin = new Point(0.5, 1.0);
                        layer.Add(overlay);
                         * */
                    }
                    sampleMap.Layers.Add(layer);

                };
        }

        private void cambiarImagen(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (globito == true)
                globito = false;
            else globito = true;
            sampleMap.Layers.Clear();
            obtenerLugares();
        //    //Crear Globo
         //   Image fondo = new Image();
         //   fondo = (Image)sender;
         //   fondo.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/pin", UriKind.RelativeOrAbsolute));
        //    Canvas.SetTop(fondo, 100);
        //    Canvas.SetLeft(fondo, 10);
        //    elGlobo.Children.Add(fondo);
        //   Image imagen = ((Canvas)sender).Children.
        //        imagen.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/globoFondo.png", UriKind.RelativeOrAbsolute));
        //        imagen.Tap += cambiaDenuevo;

       }

        private void cambiaDenuevo(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SoundBoard.xaml", UriKind.RelativeOrAbsolute));
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        // Placeholder code to contain the ApplicationID and AuthenticationToken
        // that must be obtained online from the Windows Phone Dev Center
        // before publishing an app that uses the Map control.
        private void sampleMap_Loaded(object sender, RoutedEventArgs e)
        {
            MapsSettings.ApplicationContext.ApplicationId = "<applicationid>";
            MapsSettings.ApplicationContext.AuthenticationToken = "<authenticationtoken>";
        }

        #region Event handlers for App Bar buttons and menu items

        void ToggleLocation(object sender, EventArgs e)
        {
            switch (locationToggleStatus)
            {
                case ToggleStatus.ToggledOff:
                    ShowLocation();
                    CenterMapOnLocation();
                    locationToggleStatus = ToggleStatus.ToggledOn;
                    break;
                case ToggleStatus.ToggledOn:
                    sampleMap.Layers.Remove(locationLayer);
                    locationLayer = null;
                    locationToggleStatus = ToggleStatus.ToggledOff;
                    break;
            }
        }

       /* void ToggleLandmarks(object sender, EventArgs e)
        {
            switch (landmarksToggleStatus)
            {
                case ToggleStatus.ToggledOff:
                    sampleMap.LandmarksEnabled = true;
                    if (sampleMap.ZoomLevel < MIN_ZOOMLEVEL_FOR_LANDMARKS)
                    {
                        sampleMap.ZoomLevel = MIN_ZOOMLEVEL_FOR_LANDMARKS;
                    }
                    landmarksToggleStatus = ToggleStatus.ToggledOn;
                    break;
                case ToggleStatus.ToggledOn:
                    sampleMap.LandmarksEnabled = false;
                    landmarksToggleStatus = ToggleStatus.ToggledOff;
                    break;
            }

        }*/

        void ZoomIn(object sender, EventArgs e)
        {
            if (sampleMap.ZoomLevel < MAX_ZOOM_LEVEL)
            {
                sampleMap.ZoomLevel++;
            }
        }

        void ZoomOut(object sender, EventArgs e)
        {
            if (sampleMap.ZoomLevel > MIN_ZOOM_LEVEL)
            {
                sampleMap.ZoomLevel--;
            }
        }

        #endregion

        #region Helper functions for App Bar button and menu item event handlers

        private void ShowLocation()
        {
            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            Ellipse areaCircle = new Ellipse();
            areaCircle.Fill = new SolidColorBrush(Colors.Yellow);
            areaCircle.Height = 100;
            areaCircle.Width = 100;
            areaCircle.Opacity = 0.9;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = currentLocation;

            MapOverlay areaLocationOverlay = new MapOverlay();
            areaLocationOverlay.Content = areaCircle;
            areaLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            areaLocationOverlay.GeoCoordinate = currentLocation;

            // Create a MapLayer to contain the MapOverlay.
            locationLayer = new MapLayer();
            locationLayer.Add(myLocationOverlay);

            locationAreaLayer = new MapLayer();
            locationAreaLayer.Add(areaLocationOverlay);

            // Add the MapLayer to the Map.
            //sampleMap.Layers.Add(locationAreaLayer);
            sampleMap.Layers.Add(locationLayer);

            sampleMap.ZoomLevel = 16;
            sampleMap.Center = currentLocation;

        }

        private async void GetLocation()
        {
            // Get current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            currentLocation = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
            sampleMap.ZoomLevel = 16;
            sampleMap.Center = currentLocation;

        }

        public async static void getLocation()
        {
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;

            myLatitude = myGeoposition.Coordinate.Latitude;
            myLongitude = myGeoposition.Coordinate.Longitude;
        }
        private void CenterMapOnLocation()
        {
            sampleMap.Center = currentLocation;
        }

        #endregion

        // Create the localized ApplicationBar.
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;

            // Create buttons with localized strings from AppResources.
            // Toggle Location button.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/location.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarToggleLocationButtonText;
            appBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(appBarButton);
            // Toggle Landmarks button.
           /* appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/landmarks.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarToggleLandmarksButtonText;
            appBarButton.Click += ToggleLandmarks;
            ApplicationBar.Buttons.Add(appBarButton);*/
            // Zoom In button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomin.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarZoomInButtonText;
            appBarButton.Click += ZoomIn;
            ApplicationBar.Buttons.Add(appBarButton);
            // Zoom Out button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomout.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarZoomOutButtonText;
            appBarButton.Click += ZoomOut;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create menu items with localized strings from AppResources.
            // Toggle Location menu item.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLocationMenuItemText);
            appBarMenuItem.Click += ToggleLocation;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
            // Toggle Landmarks menu item.
           /* appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLandmarksMenuItemText);
            appBarMenuItem.Click += ToggleLandmarks;
            ApplicationBar.MenuItems.Add(appBarMenuItem);*/
            // Zoom In menu item.
            appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarZoomInMenuItemText);
            appBarMenuItem.Click += ZoomIn;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
            // Zoom Out menu item.
            appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarZoomOutMenuItemText);
            appBarMenuItem.Click += ZoomOut;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }


        private void locationToSoundData(Location location)
        {
            var soundData = new SoundData();

            soundData.Title = location.Text;
            soundData.Description = location.description;
            soundData.Latitude = location.Latitude;
            soundData.Longitude = location.Longitude;
            soundData.ResourceName = location.ResourceName;
            soundData.ContainerName = location.ContainerName;
            soundData.SasQueryString = location.SasQueryString;
            soundData.ImageUri = location.ImageUri;
            soundData.Id = location.Id;

            if (App.ViewModel.CustomSounds.Items.Exists(e => e.Equals(soundData)) == false)
            App.ViewModel.CustomSounds.Items.Add(soundData);

        }


        private enum ToggleStatus
        {
            ToggledOff,
            ToggledOn
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Record.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SoundBoard.xaml", UriKind.RelativeOrAbsolute));
        }

        private void abrirMenu(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (men__principal.Visibility != Visibility.Visible)
                men__principal.Visibility = Visibility.Visible;
            else men__principal.Visibility = Visibility.Collapsed;// TODO: Add event handler implementation here.
        }

        private void goToGrabar(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	NavigationService.Navigate(new Uri("/Record.xaml", UriKind.RelativeOrAbsolute));
        }

        private void goToEscuchar(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	NavigationService.Navigate(new Uri("/SoundBoard.xaml", UriKind.RelativeOrAbsolute));
        }

        
    }
}
