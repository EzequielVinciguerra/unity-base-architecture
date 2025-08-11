using Core.UI.Views;
using Core.UI.Presenters;

namespace Core.UI
{
    public class PresenterFactory : IPresenterFactory
    {
        public IPresenter Create(ScreenId id, IView view)
        {
            switch (id)
            {
                case ScreenId.MainMenu:  return new MainMenuScreenPresenter(view);
                case ScreenId.Settings:  return new SettingsScreenPresenter(view);
                default:
                    throw new System.NotImplementedException($"No presenter for {id}");
            }
        }
    }
}