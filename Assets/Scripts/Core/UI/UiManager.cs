/// <summary>
/// Central UI management service that listens to UI-related events from the IEventBus
/// and controls the lifecycle of all views and presenters in the application.
/// 
/// The UiManager is responsible for:
/// - Listening to ShowView / HideView / ToggleView events and displaying/hiding views accordingly.
/// - Instantiating the correct prefab for a given ScreenId and assigning it to the appropriate parent
///   transform (anchors) for the current scene, provided by a CanvasHub.
/// - Creating and initializing the matching presenter for each view, subscribing and unsubscribing from events.
/// - Cleaning up presenters, views, and instantiated objects when hidden or when the manager is disabled.
/// 
/// This component is typically kept alive for the duration of the application (or scene),
/// and must have a valid IUiAnchors set (usually by a CanvasHub) to correctly parent instantiated views.
/// </summary>

using System;
using System.Collections.Generic;
using Core.Services;
using Core.Events;
using Core.UI.Views;
using Core.UI.Presenters;
using UnityEngine;

namespace Core.UI
{
    public class UiManager : MonoBehaviour, IUiManager
    {
        [Serializable]
        public class ViewEntry
        {
            public ScreenId id;
            public GameObject prefab;
            public UiLayer layer = UiLayer.Screen;
            public Transform parent;               // opcional override
        }

        [Header("View Catalog")]
        [SerializeField] private List<ViewEntry> _views = new();

        private readonly Dictionary<ScreenId, (IView view, IPresenter presenter, GameObject go)> _active =
            new Dictionary<ScreenId, (IView, IPresenter, GameObject)>();

        private IEventBus _eventBus;
        private IPresenterFactory _factory;
        private IUiAnchors _anchors;

        private void Awake()
        {
            _factory = new PresenterFactory();
        }

        private void OnEnable()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            if (_eventBus == null)
            {
                Debug.LogError("[UiManager] IEventBus not found.");
                enabled = false;
                return;
            }

            _eventBus.Subscribe<ShowView>(OnShowView);
            _eventBus.Subscribe<HideView>(OnHideView);
            _eventBus.Subscribe<ToggleView>(OnToggleView);
        }

        private void OnDisable()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ShowView>(OnShowView);
                _eventBus.Unsubscribe<HideView>(OnHideView);
                _eventBus.Unsubscribe<ToggleView>(OnToggleView);
            }

            var current = ServiceLocator.Instance.Get<IUiManager>();
            if (ReferenceEquals(current, this))
                ServiceLocator.Instance.Unregister<IUiManager>();

            // clean views
            foreach (var kv in _active.Values)
            {
                kv.presenter?.UnSubscribeEvents();
                (kv.presenter as BasePresenter)?.Dispose();
                kv.view?.DestroyFeature();
                if (kv.go) Destroy(kv.go);
            }
            _active.Clear();
        }
        
        public void Show(ScreenId id)
        {
            if (_active.ContainsKey(id))
            {
                var tup = _active[id];
                tup.view.ActiveView(true);
                return;
            }

            var entry = FindEntry(id);
            if (entry.prefab == null)
            {
                Debug.LogError($"[UiManager] Prefab not found: {id}");
                return;
            }

            var parent = ResolveParent(entry);
            if (parent == null)
            {
                Debug.LogError("[UiManager] No anchors found");
                return;
            }
            var go = Instantiate(entry.prefab, parent);
            var view = go.GetComponent<IView>();
            if (view == null)
            {
                Destroy(go);
                return;
            }

            view.Initialize();
            view.ActiveView(true);

            var presenter = _factory.Create(id, view);
            presenter.Initialize();
            presenter.SubscribeEvents();

            _active[id] = (view, presenter, go);
        }

        public void Hide(ScreenId id)
        {
            if (!_active.TryGetValue(id, out var tup)) return;

            tup.presenter.UnSubscribeEvents();
            (tup.presenter as BasePresenter)?.Dispose();

            tup.view.ActiveView(false);
            tup.view.DestroyFeature();

            if (tup.go) Destroy(tup.go);
            _active.Remove(id);
        }

        public void Toggle(ScreenId id)
        {
            if (_active.ContainsKey(id)) Hide(id);
            else Show(id);
        }

        private void OnShowView(ShowView evt)   => Show(evt.Screen);
        private void OnHideView(HideView evt)   => Hide(evt.Screen);
        private void OnToggleView(ToggleView e) => Toggle(e.Screen);

       
        
        public void SetAnchors(IUiAnchors anchors) => _anchors = anchors;

        private Transform ResolveParent(ViewEntry entry)
        {
            if (entry.parent != null) return entry.parent;
            if (_anchors == null)     return null;

            return entry.layer switch
            {
                UiLayer.Screen  => _anchors.ScreenRoot,
                UiLayer.Overlay => _anchors.OverlayRoot,
                UiLayer.Popup   => _anchors.PopupRoot,
                _               => transform
            };
        }

        private ViewEntry FindEntry(ScreenId id)
        {
            foreach (var v in _views)
                if (v.id.Equals(id)) return v;

            return new ViewEntry { id = id, prefab = null, parent = null, layer = UiLayer.Screen };
        }
    }
}