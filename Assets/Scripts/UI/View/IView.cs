namespace Core.UI.Views
{
    public interface IView
    {
        void Initialize();
        void DestroyFeature();

        void ActiveView(bool active);
    }
}
