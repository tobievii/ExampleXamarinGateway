using Android.Provider;
using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Poc.Droid;

//http://codeworks.it/blog/?p=260
[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace IoTnxt.Device.Poc.Droid
{
    public class AndroidDevice : IDevice
    {
        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}