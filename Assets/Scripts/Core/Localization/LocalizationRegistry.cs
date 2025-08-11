using System.Collections.Generic;

namespace Core.Localization
{
    internal static class LocalizationRegistry
    {
        private static readonly HashSet<LocalizedTextTMP> _items = new();
        private static ILocalizationService _localization;

        public static void SetService(ILocalizationService localization)
        {
            _localization = localization;
        }

        public static void Add(LocalizedTextTMP item)
        {
            if (_items.Add(item))
            {
                item.Refresh(_localization);
            }
        }

        public static void Remove(LocalizedTextTMP item) => _items.Remove(item);

        public static void RefreshAll(ILocalizationService localization)
        {
            _localization = localization;
            foreach (var it in _items) it.Refresh(localization);
        }
    }
}
