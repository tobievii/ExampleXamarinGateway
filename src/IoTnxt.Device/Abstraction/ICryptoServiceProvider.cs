using System;

namespace IoTnxt.Device.Abstraction
{
    public interface ICryptoServiceProvider: IDisposable
    {
        byte[] Decrypt(byte[] value);
        byte[] Encrypt(byte[] value);
    }
}