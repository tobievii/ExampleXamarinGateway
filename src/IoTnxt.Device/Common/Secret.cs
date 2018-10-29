using System;

namespace IoTnxt.Device.Common
{
    public class Secret 
    {
        public string Uid { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string vHost { get; set; }
        public object Host { get; set; }
        public object IoTHubCredentials { get; set; }
        public string RoutingKeyBase { get; set; }
        public string[] Hosts { get; set; }
        public object RsaPrivate { get; set; }
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
        public string ClientId { get; set; }
        public DateTime PostUtc { get; set; }
        //public Headers Headers { get; set; }
        public string MessageId { get; set; }
        public object MessageSourceId { get; set; }
    }

}