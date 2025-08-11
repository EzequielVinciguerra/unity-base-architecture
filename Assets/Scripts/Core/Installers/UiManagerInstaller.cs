using Core.Services;
using Core.UI;
using UnityEngine;

namespace Core.Installers
{
    [DefaultExecutionOrder(-20)]
    public class UiManagerInstaller : Installer
    {
        [SerializeField] private UiManager existing;

        private IUiManager _registeredInstance;
        private bool _spawnedByInstaller;

        public override void Install(ServiceLocator locator)
        {
            UiManager instance = existing;
            
            locator.Register<IUiManager>(instance);
            _registeredInstance = instance;
        }

        public override void Uninstall(ServiceLocator locator)
        {
            locator.Unregister<IUiManager>();

            if (_spawnedByInstaller && _registeredInstance is UiManager ui)
            {
                Destroy(ui.gameObject);
            }

            _registeredInstance = null;
            _spawnedByInstaller = false;
        }
    }
}
