using System;
using System.Collections.Generic;
using Core.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Views
{
    public class SettingsScreenView : BaseView
    {
        [SerializeField] private Button _closeSettingsButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _vfxVolumeSlider;
        [SerializeField] private TMP_Dropdown _dropdownLanguague;

        private List<LanguageId> _langIds = new();
        public event Action OnCloseClicked;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnVFXVolumeChanged;
        public event Action<LanguageId> OnLanguageSelected;

        public override void Initialize()
        {
            _closeSettingsButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
            _musicVolumeSlider.onValueChanged.AddListener((f) => OnMusicVolumeChanged?.Invoke(f));
            _vfxVolumeSlider.onValueChanged.AddListener((f) => OnVFXVolumeChanged?.Invoke(f));
        }

        public override void DestroyFeature()
        {
            _closeSettingsButton.onClick.RemoveAllListeners();
            _musicVolumeSlider.onValueChanged.RemoveAllListeners();
            _vfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }
        
        public void SetUp(float musicVolume, float vfxVolume)
        {
            _musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            _vfxVolumeSlider.SetValueWithoutNotify(vfxVolume);
        }
        public void PopulateLanguages(IReadOnlyList<(LanguageId id, string label)> options, LanguageId current)
        {
            if (_dropdownLanguague == null) return;

            _dropdownLanguague.onValueChanged.RemoveAllListeners();
            _dropdownLanguague.ClearOptions();

            _langIds.Clear();
            var labels = new List<string>(options.Count);

            for (int i = 0; i < options.Count; i++)
            {
                _langIds.Add(options[i].id);
                labels.Add(options[i].label);
            }

            _dropdownLanguague.AddOptions(labels);

            var currentIndex = _langIds.IndexOf(current);
            _dropdownLanguague.SetValueWithoutNotify(Mathf.Max(0, currentIndex));

            _dropdownLanguague.onValueChanged.AddListener(idx =>
            {
                if (idx >= 0 && idx < _langIds.Count)
                    OnLanguageSelected?.Invoke(_langIds[idx]);
            });
        }
        public override void ActiveView(bool active) { }
    }
}
