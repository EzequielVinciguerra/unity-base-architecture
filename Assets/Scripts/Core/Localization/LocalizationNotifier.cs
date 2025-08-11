using Core.Events;
using UnityEngine;

namespace Core.Localization
{
    [DefaultExecutionOrder(-60)]
    public class LocalizationNotifier : MonoBehaviour
    {
        private IEventBus _eventBus;
        private ILocalizationService _localization;

        public void Init(ILocalizationService loc, IEventBus bus)
        {
            _localization = loc;
            _eventBus = bus;

            LocalizationRegistry.SetService(_localization);
            LocalizationRegistry.RefreshAll(_localization);

            _eventBus?.Subscribe<LanguageChanged>(OnLanguageChanged);

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<LanguageChanged>(OnLanguageChanged);
        }

        private void OnLanguageChanged(LanguageChanged _)
        {
            LocalizationRegistry.RefreshAll(_localization);
        }
    }
}