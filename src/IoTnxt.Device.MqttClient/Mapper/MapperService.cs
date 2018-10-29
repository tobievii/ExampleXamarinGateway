using System;
using System.Collections.Generic;
using IoTnxt.Device;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Message;
using MQTTnet;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace IotNxt.Device.MqttClient.Mapper
{
    public class MapperService
    {
        private readonly ICryptoServiceProvider _cryptoServiceProvider;
        private readonly ICryptoService _cryptoService;

        public MapperService()
        {
            _cryptoServiceProvider = DependencyService.Resolve<ICryptoServiceProvider>();
            _cryptoService = DependencyService.Resolve<ICryptoService>();
        }

        public ApplicationMessageResult Create(IPayload message)
        {
            if (message is AuthNotifyRequest request1)
            {
                return Create(request1);
            }

            if (message is RegisterGatewayFromGatewayRequest request2)
            {
                return Create(request2);
            }

            if (message is StateRequest request3)
            {
                return Create(request3);
            }

            return null;
        }

        public TResult Create<TResult>(string message) where TResult : class
        {
            var envelope = Envelope.Create(message);
            var payload = envelope.GetPayload(_cryptoService.Key, _cryptoService.Iv);
            return JsonConvert.DeserializeObject<TResult>(payload);
        }

        public ApplicationMessageResult Create(StateRequest message)
        {
            var envelope = Envelope.Create(message);
            envelope.ReplyKey = "DAPI.1.DAPI.REPLY.1." + IoTnxtClient.Secret.ClientId + "." + Guid.NewGuid();
            var payload = JsonConvert.SerializeObject(envelope);
            return new ApplicationMessageResult
            {
                Envelope = envelope,
                Message = new MqttApplicationMessageBuilder()
                    .WithTopic($"{IoTnxtClient.Secret.RoutingKeyBase}.NFY".ToUpperInvariant())
                    .WithPayload(payload)
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                TopicFilter = new TopicFilterBuilder().WithTopic(envelope.ReplyKey)
                .WithAtMostOnceQoS()
                .Build()
            };
        }

        public ApplicationMessageResult Create(AuthNotifyRequest message)
        {
            var envelope = Envelope.Create(message, _cryptoService.Key, _cryptoService.Iv, _cryptoServiceProvider);
            envelope.ReplyKey = "MessageAuthNotify." + Guid.NewGuid();
            var payload = JsonConvert.SerializeObject(envelope);
            return new ApplicationMessageResult
            {
                Envelope = envelope,
                Message = new MqttApplicationMessageBuilder()
                    .WithTopic("MESSAGEAUTHREQUEST")
                    .WithPayload(payload)
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                TopicFilter = new TopicFilterBuilder().WithTopic(envelope.ReplyKey)
                    .WithAtMostOnceQoS()
                    .Build()
            };
        }

        public ApplicationMessageResult Create(RegisterGatewayFromGatewayRequest message)
        {
            var envelope = Envelope.Create(message);
            envelope.ReplyKey = "DAPI.1.DAPI.REPLY.1." + IoTnxtClient.Secret.ClientId + "." + Guid.NewGuid();
            var payload = JsonConvert.SerializeObject(envelope);
            return new ApplicationMessageResult
            {
                Envelope = envelope,
                Message = new MqttApplicationMessageBuilder()
                    .WithTopic(("DAPI.1.Gateway.RegisterGatewayFromGateway.1." + IoTnxtClient.Secret.ClientId + ".DEFAULT").ToUpperInvariant())
                    .WithPayload(payload)
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                TopicFilter = new TopicFilterBuilder().WithTopic(envelope.ReplyKey)
                    .WithAtMostOnceQoS()
                    .Build()
            };
        }



    }
    public class ApplicationMessageResult
    {
        public MqttApplicationMessage Message { get; set; }
        public Envelope Envelope { get; set; }
        public TopicFilter TopicFilter { get; set; }
    }

}



