using System;
using System.Collections.Generic;
using IoTnxt.Device.Abstraction;
using Xamarin.Essentials;

namespace IoTnxt.Device.Message
{
    public class RegisterGatewayFromGatewayRequest : ICommand
    {
        public string messageType { get; set; } = "Gateway.RegisterGatewayFromGateway.1";
        public IArgs args { get; set; }
        public DateTime expiresAt { get; set; } = DateTime.UtcNow.AddDays(1);

    }

    public class GatewayArgs : IArgs
    {
        public Gateway gateway { get; set; }
    }

    public class Gateway 
    {
        public string GatewayId { get; set; }
        public string Make { get; set; } = "MICRORAPTOR";
        public string Model { get; set; } = DeviceInfo.Model;
        public string FirmwareVersion { get; set; } = AppInfo.VersionString;
        public string Secret { get; set; }
        public string ClientId { get; set; }
        public Dictionary<string, GatewayDevice> Devices { get; set; } =  new Dictionary<string, GatewayDevice>();
        
    }

    public class GatewayDevice
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public Dictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();
    }

    public class Property
    {
        public string PropertyName { get; set; }
        public string DataType { get; set; }
    }

    public interface IArgs
    {

    }
}
