using System;
using System.Threading.Tasks;

namespace IoTnxt.Device.Abstraction
{
    public interface IChannel : IDisposable
    {
        Task ConnectAsync();
        Task SendAsync(IPayload payload);
        Task<TResult> SendAsync<TResult>(IPayload payload) where TResult : class;
        Task DisconnectAsync();
    }
}