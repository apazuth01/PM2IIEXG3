using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;

namespace PM2IIPExamen
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class mapa : ContentPage
    {
        Double lat;
        Double lon;
        String des;
        public mapa(String lat_, String lon_, String des_)
        {
            InitializeComponent();

            lat = Convert.ToDouble(lat_);
            lon = Convert.ToDouble(lon_);
            des = des_;
        }
        protected override async void OnAppearing()
        {

            base.OnAppearing();
            Pin ubicacion = new Pin();
            ubicacion.Label = "Ubicación Seleccionada";
            ubicacion.Address = des;
            ubicacion.Position = new Position(Convert.ToDouble(lat), Convert.ToDouble(lon));
            Mapa.Pins.Add(ubicacion);


            Mapa.MoveToRegion(new MapSpan(new Position(Convert.ToDouble(lat), Convert.ToDouble(lon)), 1, 1));



            var localizacion = CrossGeolocator.Current;
            if (localizacion != null)
            {
                localizacion.PositionChanged += localizacion_positionChanged;



                if (!localizacion.IsListening)
                {
                    await localizacion.StartListeningAsync(TimeSpan.FromSeconds(10), 100);
                }

            }
        }

        private void localizacion_positionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var posicion_mapa = new Position(e.Position.Latitude, e.Position.Longitude);
            Mapa.MoveToRegion(new MapSpan(posicion_mapa, 1, 1));
        }


       

        //private void loadConfiguration()
        //{

        //    var pin = new Pin()
        //    {
        //        Type = PinType.SavedPin,
        //        Position = new Position(PaisSeleccionado.latlng[0], PaisSeleccionado.latlng[1]),
        //        Label = PaisSeleccionado.NameCountry.official,
        //        Address = "Capital: " + PaisSeleccionado.capital


        //    };

        //    mapa.Pins.Add(pin);
        //    mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(PaisSeleccionado.latlng[0], PaisSeleccionado.latlng[1]), Distance.FromKilometers(41)));
        //}

    }
}