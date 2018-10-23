using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Extension;
using Newtonsoft.Json;

namespace IoTnxt.Device.Message
{
    public class Envelope
    {
        private string _replyKey;
        public string Payload { get;  set; }
        public bool IsEncrypted { get;  set; }
        public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        public string ReplyKey
        {
            get => _replyKey;
            set => _replyKey = value?.ToUpperInvariant();
        }

        public DateTime PostUtc { get; set; } = DateTime.UtcNow;

        public void AddHeader(string key, object value)
        {
            Headers.Add(key, value);
        }

        public void SetPayload(IPayload value)
        {
            var innerMessage = JsonConvert.SerializeObject(value);
            Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(innerMessage));
        }

        public void SetPayload(IPayload value, byte[] key, byte[] iv, ICryptoServiceProvider cryptoServiceProvider)
        {
            IsEncrypted = true;
            var innerMessage = JsonConvert.SerializeObject(value);
            Payload = Convert.ToBase64String(innerMessage.EncryptStringToBytes(key, iv));
            AddHeader("SymIv", Convert.ToBase64String(cryptoServiceProvider.Encrypt(iv)));
            AddHeader("SymKey", Convert.ToBase64String(cryptoServiceProvider.Encrypt(key)));
        }

        public string GetPayload()
        {
            return Payload;
        }

        public string GetPayload(byte[] key, byte[] iv)
        {
            return IsEncrypted ? Convert.FromBase64String(Payload).DecryptStringFromBytes(key, iv) : Payload;
        }

        public static Envelope Create(string value)
        {
            try
            {
                var envelope = JsonConvert.DeserializeObject<Envelope>(value);
                return envelope;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
      
  
        }

        public static Envelope Create(IPayload value)
        {
            var envelope = new Envelope();
            envelope.SetPayload(value);
            return envelope;
        }

        public static Envelope Create(IPayload value, byte[] key, byte[] iv, ICryptoServiceProvider cryptoServiceProvider)
        {
            var envelope = new Envelope();
            envelope.SetPayload(value, key, iv, cryptoServiceProvider);
            return envelope;
        }


    }
}


