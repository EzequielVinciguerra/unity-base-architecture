using Core.Events;
using Core.Localization;
using Core.Services;
using UnityEngine;

namespace Core.Installers
{
    [DefaultExecutionOrder(-70)]
    public class LocalizationInstaller : Installer
    {
        [Header("Localization")]
        [SerializeField] private string resourcesFolder = "Localization";
        [SerializeField] private LanguageId defaultLanguage = LanguageId.English;

        private GameObject _notifierGO;

        public override void Install(ServiceLocator locator)
        {
            var eventBus = locator.Get<IEventBus>();
            var service = new LocalizationService(eventBus, resourcesFolder, defaultLanguage);
            locator.Register<ILocalizationService>(service);

            _notifierGO = new GameObject("[Localization]");
            var notifier = _notifierGO.AddComponent<LocalizationNotifier>();
            notifier.Init(service, eventBus);
        }

        public override void Uninstall(ServiceLocator locator)
        {
            locator.Unregister<ILocalizationService>();

            if (_notifierGO) Object.Destroy(_notifierGO);
            _notifierGO = null;
        }
    }
}
