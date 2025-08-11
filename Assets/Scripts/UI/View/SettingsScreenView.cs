using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Views
{
    public class SettingsScreenView : BaseView
    {

        [SerializeField] private Button _closeSettingsButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _vfxVolumeSlider;

        public event Action OnCloseClicked;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnVFXVolumeChanged;

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

        public override void ActiveView(bool active)
        {
        }
    }
}
