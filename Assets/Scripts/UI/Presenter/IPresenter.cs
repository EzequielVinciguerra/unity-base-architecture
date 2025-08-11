namespace Core.UI.Presenters
{
    public interface IPresenter
    {
        void Initialize();
        void SubscribeEvents();
        void UnSubscribeEvents();
    }
}