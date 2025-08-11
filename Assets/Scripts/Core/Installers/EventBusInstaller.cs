using Core.Events;
using Core.Services;

namespace Core.Installers
{
    public class EventBusInstaller : Installer
    {
        private IEventBus _instance;

        public override void Install(ServiceLocator locator)
        {
            _instance = new EventBus();
            locator.Register<IEventBus>(_instance);
        }

        public override void Uninstall(ServiceLocator locator)
        {
            locator.Unregister<IEventBus>();
            _instance = null;
        }
    }
}
