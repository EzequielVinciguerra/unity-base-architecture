using TMPro;
using UnityEngine;

namespace Core.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextTMP : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI target;
        [SerializeField] private string key;

        private void Reset()
        {
            if (!target) target = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()  => LocalizationRegistry.Add(this);
        private void OnDisable() => LocalizationRegistry.Remove(this);

        internal void Refresh(ILocalizationService loc)
        {
            if (target && loc != null) target.text = loc.Get(key);
        }
    }
}
