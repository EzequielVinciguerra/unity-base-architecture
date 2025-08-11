using Core.UI.Views;
using Core.UI.Presenters;

namespace Core.UI
{
    public interface IPresenterFactory
    {
        IPresenter Create(ScreenId id, IView view);
    }
}
