using System;
using System.Linq;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Common;
using MQTTnet.Client;

namespace IotNxt.Device.MqttClient
{
    public class MqttClientFactory : IChannelFactory
    {

        public virtual IChannel CreateGreenChannel(string gatewayId)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("greenqueue.prod.iotnxt.io", 8883)
                .WithClientId( $"{gatewayId}.GREEN.{DateTime.Now.Ticks}")
                .WithCredentials("green1:public1", "publicpassword1")
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    UseTls = true,

                })
                .WithCleanSession()
                .Build();
            return new MqttClientChannel(options);
        }

        public virtual IChannel CreateRedChannel(Secret secret)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(secret.Hosts?.FirstOrDefault(), 8883)
                .WithClientId(secret.ClientId + ".RED." + DateTime.Now.Ticks)
                .WithCredentials(secret.vHost + ":" + secret.Uid, secret.Password)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    UseTls = true,

                })
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(5))
                .WithCleanSession()
                .Build();
            return new MqttClientChannel(options);
        }
    }
}
