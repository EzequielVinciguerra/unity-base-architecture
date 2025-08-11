namespace Core.Localization
{
    public interface ILocalizationService
    {
        LanguageId CurrentLanguageId { get; }
        string CurrentLanguageCode { get; } // ISO (ej: "en", "es")

        void SetLanguage(LanguageId id);
        void SetLanguage(UnityEngine.SystemLanguage sysLang);

        string Get(string key);
        bool TryGet(string key, out string value);
        string Format(string key, params object[] args);
    }
}
