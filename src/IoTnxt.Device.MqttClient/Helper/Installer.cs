using System;
using System.Collections.Generic;
using System.Text;
using IotNxt.Device.MqttClient;
using IotNxt.Device.MqttClient.Mapper;
using IoTnxt.Device.Abstraction;
using Xamarin.Forms;

namespace IoTnxt.Device.MqttClient.Helper
{
    public class Installer
    {
        public static void Install()
        {
            DependencyService.Register<IChannelFactory, MqttClientFactory>();
            DependencyService.Register<MapperService>();

            IoTnxt.Device.Helper.Installer.Install();
        }
    }
}
