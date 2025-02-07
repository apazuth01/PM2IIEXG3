﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Plugin.Media;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.Geolocator;
using Plugin.AudioRecorder;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Net.Http;
using Plugin.Media.Abstractions;

namespace PM2IIPExamen
{
    public partial class MainPage : ContentPage
    {   // variables
        string ruta = "", StringBase64Foto = "", StringBase64Audio="", StringBase64Video = ""; //ruta de la imagen
        int    aud = 0; // validaciones
        AudioRecorderService recorder = new AudioRecorderService();// grabadora


        public MainPage()
        {
            InitializeComponent();

            InizializatePlugins();
        }
        private async void InizializatePlugins()
        {

            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    // Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    lblLatitud.Text = location.Latitude.ToString();
                    lblLongitud.Text = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private async void tomar()
        {
            var takepic = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "PhotoApp",
                Name = "TEST.jpg"
            });


            ruta = takepic.Path;


            if (takepic != null)
            {
                imgFoto.Source = ImageSource.FromStream(() => { return takepic.GetStream(); });

            }
            byte[] ImageBytes = null;

            using (var stream = new MemoryStream())
            {
                takepic.GetStream().CopyTo(stream);
                takepic.Dispose();
                ImageBytes = stream.ToArray();
                StringBase64Foto = Convert.ToBase64String(ImageBytes);
            }
        }

        private async void guardar()
        {
            if (String.IsNullOrWhiteSpace(txtDescripcion.Text) || ruta == "" || lblLatitud.Text == "Latitud" || lblLongitud.Text == "Longitud" || aud==0)
            {
                
                await DisplayAlert("Error", "No completó todos los campos", "OK");
            }
            else
            {
            
                String direccion = "https://cr10.000webhostapp.com/examen/insertarSitios.php";


                // cliente.UploadValues(direccion, "POST", parametros);
                MultipartFormDataContent parametros = new MultipartFormDataContent();
                StringContent des = new StringContent(txtDescripcion.Text);
                StringContent lon = new StringContent(lblLongitud.Text);
                StringContent lat = new StringContent(lblLatitud.Text);
                StringContent img = new StringContent(StringBase64Video);
                StringContent aud = new StringContent(StringBase64Audio);
                parametros.Add( des, "descripcion");
                parametros.Add(lon,  "longitud");
                parametros.Add(lat,  "latitud");
                parametros.Add(img,  "imgT");
                parametros.Add(aud,  "audioT");

                using (HttpClient client = new HttpClient())
                {
                    var respuesta = await client.PostAsync(direccion, parametros);

                    Debug.WriteLine(respuesta.Content.ReadAsStringAsync().Result);
                    await DisplayAlert("Proceso Terminado", "Datos Guardados", "OK");

                    reset();
                }                
            }






        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            //tomar();
            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await DisplayAlert("Alerta", "Antes de Tomar el Video Debe Llenar los Campos Vacios", "Ok");
            }
            else
            {

                var cameraOptions = new StoreVideoOptions();
                cameraOptions.PhotoSize = PhotoSize.Full;
                cameraOptions.Name = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "mp4";
                cameraOptions.Directory = "DefaultVideos";

                var mediaFile = await CrossMedia.Current.TakeVideoAsync(cameraOptions);

                ruta = mediaFile.Path;
                if (mediaFile != null)
                {
                    byte[] arreglo = null;
                    //{
                    //    ruta = mediaFile.Path;

                    //    arreglo = File.ReadAllBytes(ruta);
                    //    StringBase64Video = Convert.ToBase64String(arreglo);
                    //};
                    
                    using (var stream = new MemoryStream())
                    {
                       
                        mediaFile.GetStream().CopyTo(stream);
                        mediaFile.Dispose();
                        arreglo = stream.ToArray();
                        StringBase64Video = Convert.ToBase64String(arreglo);
                    }


                }



            }
        }

        private void reset()
        {
            txtDescripcion.Text = "";
            ruta = "";
            imgFoto.Source = "paisajes.gif";
            lblAudio.Text = "Estatus Audio";
            lblAudio.TextColor = Color.Blue;
            aud = 0;
            recorder= new AudioRecorderService();
        }

        private async void btnGrabar_Clicked(object sender, EventArgs e)
        {
            if (!recorder.IsRecording)
            {
                //recorder = new AudioRecorderService();

                lblAudio.Text = "Grabando";
                await recorder.StartRecording();

            }

        }

        private async void btnDetener_Clicked(object sender, EventArgs e)
        {
            if (recorder.IsRecording)
            {
                lblAudio.Text = "Audio Detenido";
                await recorder.StopRecording();
                aud = 1;


                byte[] AudioBytes = null;

                using (var stream = new MemoryStream())
                {
                    recorder.GetAudioFileStream().CopyTo(stream);
                    AudioBytes = stream.ToArray();
                    StringBase64Audio = Convert.ToBase64String(AudioBytes);                    

                }

            }

        }

        private void btnReproducir_Clicked(object sender, EventArgs e)
        {
            AudioPlayer player = new AudioPlayer();
            var filePath = recorder.GetAudioFilePath();
            lblAudio.Text = "Reproduciendo";
            player.Play(filePath);

            lblAudio.Text = "Sin acción";
        }

        private void btnGuardar_Clicked(object sender, EventArgs e)
        {
            guardar();
        }

        private async void btnListar_Clicked(object sender, EventArgs e)
        {
            var am = new lista();
            await Navigation.PushAsync(am);
        }
    }
}
