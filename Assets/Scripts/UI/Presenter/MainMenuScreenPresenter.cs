using Core.Events;
using Core.UI;
using Core.UI.Views;

namespace Core.UI.Presenters
{
    public class MainMenuScreenPresenter : BasePresenter
    {
        private MainMenuScreenBaseView _menuBaseView;
        public MainMenuScreenPresenter(IView view) : base(view) { }

        public override void Initialize()
        {
            _menuBaseView = (MainMenuScreenBaseView)view;
            SubscribeEvents();
        }

        public override void SubscribeEvents()
        {
            _menuBaseView.OnSettingsClicked += HandleSettingsClicked;
        }

        public override void UnSubscribeEvents()
        {
            _menuBaseView.OnSettingsClicked -= HandleSettingsClicked;
        }

        public override void Dispose()
        {
            UnSubscribeEvents();
        }

        private void HandleSettingsClicked()
        {
            eventBus.Publish(new ShowView( ScreenId.Settings));
        }
    }
}
