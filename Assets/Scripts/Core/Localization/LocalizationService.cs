using System.Collections.Generic;
using Core.Events;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Localization
{
    /// <summary>
    /// Loads JSON from Resources/Localization/{code}.json (code = ISO like "en","es"...)
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private const string PREF_KEY = "Localization.LanguageCode";

        private readonly IEventBus _eventBus;
        private readonly string _resourcesFolder;
        private readonly LanguageId _defaultLanguageId;

        private readonly Dictionary<string, string> _entries = new();
        private readonly Dictionary<string, string> _fallbackEntries = new();

        public LanguageId CurrentLanguageId { get; private set; }
        public string CurrentLanguageCode { get; private set; } 

        public LocalizationService(IEventBus eventBus, string resourcesFolder = "Localization", LanguageId defaultLanguage = LanguageId.English)
        {
            _eventBus = eventBus;
            _resourcesFolder = string.IsNullOrWhiteSpace(resourcesFolder) ? "Localization" : resourcesFolder;
            _defaultLanguageId = defaultLanguage;

            var defaultCode = ToIsoCode(_defaultLanguageId);
            LoadFallback(defaultCode);

            var savedCode = PlayerPrefs.GetString(PREF_KEY, defaultCode);
            var savedId = FromIsoCode(savedCode) ?? _defaultLanguageId;
            SetLanguage(savedId);
        }

        public void SetLanguage(LanguageId id)
        {
            var code = ToIsoCode(id);
            if (string.IsNullOrEmpty(code))
            {
                Debug.LogWarning($"[Localization] No ISO code for {id}, falling back to default.");
                id = _defaultLanguageId;
                code = ToIsoCode(id);
            }

            if (CurrentLanguageId.Equals(id) && _entries.Count > 0)
                return;

            if (!LoadLanguage(code))
            {
                var defCode = ToIsoCode(_defaultLanguageId);
                Debug.LogWarning($"[Localization] Language '{code}' not found, using default '{defCode}'.");
                LoadLanguage(defCode);
                id = _defaultLanguageId;
                code = defCode;
            }

            CurrentLanguageId = id;
            CurrentLanguageCode = code;

            PlayerPrefs.SetString(PREF_KEY, CurrentLanguageCode);

            _eventBus?.Publish(new LanguageChanged(CurrentLanguageId, CurrentLanguageCode));
        }

        public void SetLanguage(SystemLanguage sysLang)
        {
            var id = sysLang switch
            {
                SystemLanguage.English => LanguageId.English,
                SystemLanguage.Spanish => LanguageId.Spanish,
                SystemLanguage.Italian =>LanguageId.Italian,
                SystemLanguage.French => LanguageId.French,
                SystemLanguage.Russian => LanguageId.Russian,
                _ => LanguageId.English
            };
            SetLanguage(id);
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (_entries.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v))
                return v;

            if (_fallbackEntries.TryGetValue(key, out var fb) && !string.IsNullOrEmpty(fb))
                return fb;

            return $"#{key}";
        }

        public bool TryGet(string key, out string value)
        {
            if (_entries.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                return true;

            if (_fallbackEntries.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                return true;

            value = null;
            return false;
        }

        public string Format(string key, params object[] args)
        {
            var raw = Get(key);
            return string.Format(raw, args);
        }

        private bool LoadLanguage(string code)
        {
            _entries.Clear();
            var json = LoadJsonText(code);
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                foreach (var kv in dict)
                    _entries[kv.Key] = kv.Value;

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Localization] Failed to parse '{code}': {ex}");
                return false;
            }
        }

        private void LoadFallback(string defaultCode)
        {
            _fallbackEntries.Clear();
            var json = LoadJsonText(defaultCode);
            if (string.IsNullOrEmpty(json)) return;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                foreach (var kv in dict)
                    _fallbackEntries[kv.Key] = kv.Value;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Localization] Failed to parse fallback '{defaultCode}': {ex}");
            }
        }

        private string LoadJsonText(string code)
        {
            var ta = Resources.Load<TextAsset>($"{_resourcesFolder}/{code}");
            if (ta == null)
            {
                Debug.LogWarning($"[Localization] JSON not found at Resources/{_resourcesFolder}/{code}.json");
                return null;
            }
            return ta.text;
        }

        public static string ToIsoCode(LanguageId id) => id switch
        {
            LanguageId.English => "en",
            LanguageId.Spanish => "es",
            LanguageId.Italian => "it",
            LanguageId.French => "fr",
            LanguageId.Russian => "ru",
            _ => "en"
        };

        public static LanguageId? FromIsoCode(string code)
        {
            switch ((code ?? "").ToLowerInvariant())
            {
                case "en": return LanguageId.English;
                case "es": return LanguageId.Spanish;
                case "it": return LanguageId.Italian;
                case "fr": return LanguageId.French;
                case "ru": return LanguageId.Russian;
                default: return null;
            }
        }
    }
}