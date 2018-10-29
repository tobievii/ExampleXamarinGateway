using System;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Common;

namespace IoTnxt.Device.Message
{
    public class AuthNotifyRequest : IMessage
    {
        public string Uid { get; set; }
        public string SecretKey { get; set; }
        public string PostUtc { get; set; } = DateTime.UtcNow.ToString();
    }

    public class AuthNotifyResponse : Secret
    {
    }
}
