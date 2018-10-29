using System.Security.Cryptography;
using IoTnxt.Device.Abstraction;

namespace IoTnxt.Device.Crypto
{
    public class CryptoService : ICryptoService
    {
        public CryptoService()
        {
            Init();
        }

        public byte[] Iv { get; private set; }

        public byte[] Key { get; private set; }

        public void Init()
        {
            using (var myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();

                Key = myRijndael.Key;
                Iv = myRijndael.IV;
            }
        }

        public void Dispose()
        {
            Iv = null;
            Key = null;
        }
    }
}