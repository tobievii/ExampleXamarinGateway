using System;

namespace IoTnxt.Device.Abstraction
{
    public interface ICryptoService : IDisposable
    {
        byte[] Iv { get;  }
        byte[] Key { get;  }
    }
}