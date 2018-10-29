using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotNxt.Device.MqttClient.Mapper;
using IoTnxt.Device.Abstraction;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace IotNxt.Device.MqttClient
{
    public class MqttClientChannel : IChannel
    {
        private readonly IMqttClientOptions _mqttClientOptions;
        private readonly MapperService _mapperService = new MapperService();
        private readonly ConcurrentDictionary<string, string> _message =  new ConcurrentDictionary<string, string>();
        private IMqttClient _mqttClient;


        public MqttClientChannel(IMqttClientOptions mqttClientOptions)
        {
            _mqttClientOptions = mqttClientOptions;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            _mqttClient.Connected += async (s, e) =>
            {
                Console.WriteLine($"### CONNECTED TO CLIENT {_mqttClientOptions.ClientId} ###");
            };
            _mqttClient.ApplicationMessageReceived += async (s, e) =>
            {
                Console.WriteLine($"### MESSAGE RECEIVED FROM {_mqttClientOptions.ClientId} ###");

                var topic = e.ApplicationMessage.Topic.Replace('/','.').ToUpperInvariant();
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (!_message.ContainsKey(topic))
                {
                    _message.TryAdd(topic, payload);
                }
                else
                {
                    _message[topic] = payload;
                }
            };
            _mqttClient.Disconnected += (sender, args) =>
            {
                Console.WriteLine($"### DISCONNECTED FROM CLIENT {_mqttClientOptions.ClientId} ###");
            };

        }

        public List<string> Subscribes { get; private set; } = new List<string>();

        public async Task ConnectAsync()
        {
 
                await _mqttClient.ConnectAsync(_mqttClientOptions);
       
         
        }

        public async Task DisconnectAsync()
        {
            await _mqttClient.DisconnectAsync();
        }

        public async void Dispose()
        {
            await _mqttClient.UnsubscribeAsync(Subscribes);
            await _mqttClient.DisconnectAsync();
            _mqttClient.Dispose();
            _mqttClient = null;
        }

        public async Task SendAsync(IPayload payload) 
        {
            if (!_mqttClient.IsConnected)
            {
                throw new Exception("Not connected");
            }
            var message = _mapperService.Create(payload);
            if (message.TopicFilter != null && !Subscribes.Contains(message.TopicFilter.Topic))
            {
                Subscribes.Add(message.TopicFilter.Topic);
                await _mqttClient.SubscribeAsync(message.TopicFilter);
            }

            await _mqttClient.PublishAsync(message.Message);
        }

        public async Task<TResult> SendAsync<TResult>(IPayload payload) where TResult : class
        {
            if (!_mqttClient.IsConnected)
            {
                throw new Exception("Not connected");
            }

            var isMessage = payload as IMessage;
            if (isMessage == null )
            {
                throw new Exception("Not a message");
            }
            var message = _mapperService.Create(payload);
            if (message.TopicFilter != null && !Subscribes.Contains(message.TopicFilter.Topic))
            {
                Subscribes.Add(message.TopicFilter.Topic);
                await _mqttClient.SubscribeAsync(message.TopicFilter);
            }

            await _mqttClient.PublishAsync(message.Message);

            var i = 0;
            do
            {
                await Task.Delay(new TimeSpan(0, 0, 0, 1));
                ++i;
                if(i > 10)
                    throw new TimeoutException();

                if (!_message.ContainsKey(message.Envelope.ReplyKey))
                    continue;

                var value = _mapperService.Create<TResult>(_message[message.Envelope.ReplyKey]);
                _message.TryRemove(message.Envelope.ReplyKey, out _);
                return value;
            } while (true);

        }
    }
}
