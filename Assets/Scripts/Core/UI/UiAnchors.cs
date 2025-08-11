using UnityEngine;

namespace Core.UI
{
    public interface IUiAnchors
    {
        Transform ScreenRoot { get; }
        Transform OverlayRoot { get; }
        Transform PopupRoot  { get; }
    }
}
