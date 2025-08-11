/// <summary>
/// This component should be present in every scene that requires UI.
/// It provides scene-specific UI parent transforms (anchors) for different UI layers.
/// The UiManager will use these references to correctly parent instantiated views
/// to the appropriate Canvas roots of the current scene.
/// </summary>

using Core.Services;
using UnityEngine;

namespace Core.UI
{
    public class CanvasHub : MonoBehaviour, IUiAnchors
    {
        [SerializeField] private Transform screenRoot;
        [SerializeField] private Transform overlayRoot;
        [SerializeField] private Transform popupRoot;

        public Transform ScreenRoot => screenRoot != null ? screenRoot : transform;
        public Transform OverlayRoot => overlayRoot != null ? overlayRoot : transform;
        public Transform PopupRoot  => popupRoot  != null ? popupRoot  : transform;
        
        private void OnEnable()
        {
            var ui = ServiceLocator.Instance.Get<IUiManager>() as UiManager;
            if (ui != null) ui.SetAnchors(this);
        }
    }
}
