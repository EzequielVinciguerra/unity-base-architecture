using Core.Events;
using Core.Scenes;
using Core.Services;
using Core.UI;
using Core.UI.Views;
using UnityEngine;

namespace Core.UI.Presenters
{
    public class MainMenuScreenPresenter : BasePresenter
    {
        private MainMenuScreenView _menuView;
        public MainMenuScreenPresenter(IView view) : base(view) { }

        public override void Initialize()
        {
            _menuView = (MainMenuScreenView)view;
            SubscribeEvents();
        }

        public override void SubscribeEvents()
        {
            _menuView.OnSettingsClicked += HandleSettingsClicked;
            _menuView.OnGoIngameClicked += HandleGoIngameClicked;
        }


        public override void UnSubscribeEvents()
        {
            _menuView.OnSettingsClicked -= HandleSettingsClicked;
            _menuView.OnGoIngameClicked -= HandleGoIngameClicked;
        }

        public override void Dispose()
        {
            UnSubscribeEvents();
        }

        private void HandleSettingsClicked()
        {
            eventBus.Publish(new ShowView( ScreenId.Settings));
        }
        private void HandleGoIngameClicked()
        {
            eventBus.Publish(new LoadSceneRequest("Game"));
        }
    }
}
