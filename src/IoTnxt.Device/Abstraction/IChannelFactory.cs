using IoTnxt.Device.Common;
using IoTnxt.Device.Message;

namespace IoTnxt.Device.Abstraction
{
    public interface IChannelFactory
    {
        IChannel CreateGreenChannel(string gatewayId);

        IChannel CreateRedChannel(Secret secret);
    }
}
