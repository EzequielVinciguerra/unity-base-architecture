namespace Core.UI
{
    public interface IUiManager
    {
        void Show(ScreenId id);
        void Hide(ScreenId id);
        void Toggle(ScreenId id);
    }
}