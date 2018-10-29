using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Common;
using IoTnxt.Device.Message;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IoTnxt.Device
{
    public class IoTnxtClient : IIoTnxtClient, IDisposable
    {
        private IChannelFactory _channelFactory;
        private IChannel _channel;
        private DeviceSetup _deviceSetup { get; } = new DeviceSetup();

        public IoTnxtClient()
        {
            GatewayId = DependencyService.Resolve<IDevice>()?.GetIdentifier()?.ToUpperInvariant() ?? throw new NoNullAllowedException(nameof(IDevice));
            _channelFactory = DependencyService.Resolve<IChannelFactory>() ?? throw new ArgumentNullException(nameof(IChannelFactory));
        }


        public string GatewayId { get; }
        public string IoTnxtGateway { get; }
        public string IoTnxtGatewayPort { get; }
        public bool IsConnected { get; private set; } = false;

        public static Secret Secret { get; private set; }

        public async Task ConnectAsync()
        {
            if (!IsConnected)
            {
                //var value = System.Text.Encoding.Default.GetString(Convert.FromBase64String("ewogICJyZXN1bHQiOiBudWxsLAogICJtZXNzYWdlVHlwZSI6ICJPSyIsCiAgInN0YXRlIjogbnVsbCwKICAic291cmNlTWVzc2FnZUlEIjogbnVsbAp9"));
                using (var channel = _channelFactory.CreateGreenChannel(GatewayId))
                {
                    await channel.ConnectAsync();
                    var message = new AuthNotifyRequest
                    {
                        SecretKey = "EASYmsaBOA",
                        Uid = GatewayId
                    };

                    var result = await channel.SendAsync<AuthNotifyResponse>(message);
                    if (!result.Success)
                    {
                        throw new Exception(result.ErrorMsg);
                    }

                    Secret = result;
                }

                _channel = _channelFactory.CreateRedChannel(Secret);
                await _channel.ConnectAsync();
                ConfigureDevices(x => x.AddDevice("HEARTBEAT", 1, p => p.AddProperty("VALUE")));
                try
                {
                    await _channel.SendAsync(new RegisterGatewayFromGatewayRequest
                    {
                        args = new GatewayArgs
                        {
                            gateway = new Gateway
                            {
                                Secret = "EASYmsaBOA",
                                GatewayId = GatewayId,
                                ClientId = Secret.ClientId,
                                Devices = _deviceSetup.ToGatewayDevices(GatewayId)
                            //        new Dictionary<string, GatewayDevice>()
                            //{
                            //    {
                            //        $"{GatewayId}:{DeviceInfo.Manufacturer}|{GatewayId}:HEARTBEAT|1", new GatewayDevice
                            //        {
                            //            DeviceName = $"{GatewayId}:{DeviceInfo.Manufacturer}|{GatewayId}:HEARTBEAT|1",
                            //            DeviceType = "HEARTBEAT",
                            //            Properties = new Dictionary<string, Property>()
                            //            {
                            //                {
                            //                    "VALUE", new Property
                            //                    {
                            //                        PropertyName = "value"
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    },
                            //    {
                            //        $"{GatewayId}:{DeviceInfo.Manufacturer}|{GatewayId}:GPS|1", new GatewayDevice
                            //        {
                            //            DeviceName = $"{GatewayId}:{DeviceInfo.Manufacturer}|{GatewayId}:GPS|1",
                            //            DeviceType = "GPS",
                            //            Properties = new Dictionary<string, Property>()
                            //            {
                            //                {
                            //                    "Latitude", new Property
                            //                    {
                            //                        PropertyName = "Latitude"
                            //                    }
                            //                },
                            //                {
                            //                    "Longitude", new Property
                            //                    {
                            //                        PropertyName = "Longitude"
                            //                    }
                            //                },
                            //                {
                            //                    "Altitude", new Property
                            //                    {
                            //                        PropertyName = "Altitude"
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    },
                            //}

                            }
                        }
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                IsConnected = true;
            }
        }

        public async Task DisconnectAsync()
        {
            if (IsConnected)
            {
                await _channel.DisconnectAsync();
            }
            IsConnected = false;
        }

        public IIoTnxtClient ConfigureDevices(Action<DeviceSetup> devices)
        {
            devices.Invoke(_deviceSetup);
            return this;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _channel = null;
        }

        public Task SendAsync(IPayload payload)
        {
            if (!IsConnected)
            {
                throw new Exception("Not connected");
            }

            return _channel.SendAsync(payload);
        }

        public Task<TResult> SendAsync<TResult>(IPayload payload) where TResult : class
        {
            return _channel.SendAsync<TResult>(payload);
        }
    }
}
