using System.Security.Cryptography;
using IoTnxt.Device.Abstraction;

namespace IoTnxt.Device.Crypto
{
    public class CryptoServiceProvider:  ICryptoServiceProvider
    {
        private RSACryptoServiceProvider _provider;

        public CryptoServiceProvider()
        {
            Init();
        }

        public byte[] Decrypt(byte[] value)
        {
            return _provider.Decrypt(value, RSAEncryptionPadding.Pkcs1);
        }

        public byte[]  Encrypt(byte[] value)
        {
            return _provider.Encrypt(value, RSAEncryptionPadding.Pkcs1);
        }

        private void Init()
        {
            _provider = new RSACryptoServiceProvider();
            _provider.FromXmlString("<RSAKeyValue><Exponent>AQAB</Exponent><Modulus>rbltknM3wO5/TAEigft0RDlI6R9yPttweDXtmXjmpxwcuVtqJgNbIQ3VduGVlG6sOg20iEbBWMCdwJ3HZTrtn7qpXRdJBqDUNye4Xbwp1Dp+zMFpgEsCklM7c6/iIq14nymhNo9Cn3eBBM3yZzUKJuPn9CTZSOtCenSbae9X9bnHSW2qB1qRSQ2M03VppBYAyMjZvP1wSDVNuvCtjU2Lg/8o/t231E/U+s1Jk0IvdD6rLdoi91c3Bmp00rVMPxOjvKmOjgPfE5LESRPMlUli4kJFWxBwbXuhFY+TK2I+BUpiYYKX+4YL3OFrn/EpO4bNcI0NHelbWGqZg57x7rNe9Q==</Modulus></RSAKeyValue>");
        }

        public void Dispose()
        {
            _provider?.Dispose();
        }
    }
}
