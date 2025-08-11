using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Views
{
    public class MainMenuScreenView :BaseView
    {
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _goIngameButton;

        public event Action OnSettingsClicked;
        public event Action OnGoIngameClicked;
        public override void Initialize() 
        { 
            _openSettingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());
            _goIngameButton.onClick.AddListener(() => OnGoIngameClicked?.Invoke());
        }

        public override void DestroyFeature()
        {
            _openSettingsButton.onClick.RemoveAllListeners();
            _goIngameButton.onClick.RemoveAllListeners();
        }

        public override void ActiveView(bool active) => (root ? root : gameObject).SetActive(active);
    }
}
