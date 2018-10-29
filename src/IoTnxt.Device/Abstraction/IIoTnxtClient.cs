using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTnxt.Device.Message;
using Xamarin.Essentials;

namespace IoTnxt.Device.Abstraction
{
    public interface IIoTnxtClient
    {
        bool IsConnected { get; }
        string GatewayId { get; }
        string IoTnxtGateway { get; }
        string IoTnxtGatewayPort { get; }

        Task ConnectAsync();
        Task SendAsync(IPayload payload);
        Task<TResult> SendAsync<TResult>(IPayload payload) where TResult : class;
        Task DisconnectAsync();

        IIoTnxtClient ConfigureDevices(Action<DeviceSetup> devices);
    }

    public class DeviceSetup
    {
        public List<Device> Devices { get;  } =  new List<Device>();

        public void AddDevice(string name, int index, Action<DeviceProperties> propertiesSetup)
        {
            if (!Devices.Any(x => string.Equals(x.DeviceName, name, StringComparison.CurrentCultureIgnoreCase) && x.Index != index))
            {
                Devices.Add(new Device(propertiesSetup)
                {
                    DeviceName = name,
                    Index = index
                });
            }
        }

        public Dictionary<string, GatewayDevice> ToGatewayDevices(string gatewayId)
        {
            var gatewayDevices = new Dictionary<string, GatewayDevice>();
            foreach (var device in Devices)
            {
                gatewayDevices.Add($"{gatewayId}:{DeviceInfo.Manufacturer}|{gatewayId}:{device.DeviceName}|{device.Index}", device.ToGatewayDevice(gatewayId));
            }


            return gatewayDevices;
        }
    }

    public class Device
    {
        public string DeviceName { get; set; }
        public int Index { get; set; }

        public Device(Action<DeviceProperties> propertiesSetup)
        {
            propertiesSetup.Invoke(Properties);
        }

        public DeviceProperties Properties { get; } = new DeviceProperties();

        public GatewayDevice ToGatewayDevice(string gatewayId)
        {
            var gatewayDevice =  new GatewayDevice
            {
                DeviceName = $"{gatewayId}:{DeviceInfo.Manufacturer}|{gatewayId}:{DeviceName}|{Index}",
                DeviceType = DeviceName,
                Properties = Properties.ToGatewayDeviceProperties()
            };

            return gatewayDevice;
        }
    }

    public class DeviceProperties
    {
        public List<string> Properties { get; set; } = new List<string>();

        public void AddProperty(string name)
        {
            if (!Properties.Any(x=>string.Equals(x,name,StringComparison.CurrentCultureIgnoreCase)))
            {
                Properties.Add(name);
            }
        }

        public Dictionary<string, Property> ToGatewayDeviceProperties()
        {
            var result = new Dictionary<string, Property>();
            foreach (var property in Properties)
            {
                result.Add(property, new Property
                {
                    PropertyName = property
                });
            }

            return result;
        }
    }
}