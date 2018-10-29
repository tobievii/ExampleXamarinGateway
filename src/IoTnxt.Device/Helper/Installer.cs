using IoTnxt.Device.Abstraction;
using IoTnxt.Device.Crypto;
using Xamarin.Forms;

namespace IoTnxt.Device.Helper
{
    public  class Installer
    {
        public static void Install()
        {
            DependencyService.Register<ICryptoService, CryptoService >();
            DependencyService.Register<ICryptoServiceProvider, CryptoServiceProvider>();
            DependencyService.Register<IIoTnxtClient,IoTnxtClient>();
        }
    }
}
