using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Message;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace IoTnxt.Device.Poc
{
    public partial class MainPage : ContentPage
    {
        private IIoTnxtClient _client;

        public MainPage()
        {
            InitializeComponent();
            _client = DependencyService.Resolve<IIoTnxtClient>();
            _client.ConfigureDevices(x =>
                {
                    x.AddDevice("GPS", 1, p =>
                    {
                        p.AddProperty("Latitude");
                        p.AddProperty("Longitude");
                        p.AddProperty("Altitude");
                    });
                });


            lblGatewayId.Text = _client.GatewayId;
            CollectMetrics();
        }


        public async Task CollectMetrics()
        {
            try
            {
                await _client.ConnectAsync();

                var counter = 0;
                do
                {

                    await Heartbeat(counter);
                    await Gps();
                    ++counter;
                    await Task.Delay(new TimeSpan(0, 0, 10));
                } while (true);
            }
            catch (Exception e)
            {
                await _client.DisconnectAsync();
                Console.WriteLine(e);
                throw;
            }

        }

        private async Task Heartbeat(int beat)
        {

            await _client.SendAsync(new StateRequest
            {
                deviceGroups = new Dictionary<string, Dictionary<string, object>>
                        {
                            {
                                $"{_client.GatewayId}:{DeviceInfo.Manufacturer}|{_client.GatewayId}:HEARTBEAT|1", new Dictionary<string, object>()
                                {
                                    { "value", beat}
                                }
                            }
                        }
            });

        }

        private async Task Gps()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    await _client.SendAsync(new StateRequest
                    {
                        deviceGroups = new Dictionary<string, Dictionary<string, object>>
                        {
                            {
                                $"{_client.GatewayId}:{DeviceInfo.Manufacturer}|{_client.GatewayId}:GPS|1", new Dictionary<string, object>()
                                {
                                    { "Latitude", location.Latitude},
                                    { "Longitude", location.Longitude},
                                    { "Altitude", location.Altitude}
                                }
                            }
                        }
                    });
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
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
    }
}
