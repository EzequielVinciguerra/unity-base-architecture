using Core.Events;

namespace Core.UI
{
    public struct ShowView : IEvent
    {
        public ScreenId Screen { get; }

        public ShowView(ScreenId screen)
        {
            Screen = screen;
        }
    }
    public struct HideView  : IEvent { 
        public ScreenId Screen { get; }

        public HideView(ScreenId screen)
        {
            Screen = screen;
        }}
    public struct ToggleView : IEvent { 
        public ScreenId Screen { get; }

        public ToggleView(ScreenId screen)
        {
            Screen = screen;
        }}
}
