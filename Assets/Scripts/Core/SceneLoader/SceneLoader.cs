using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using Core.Events;
using Core.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    public class SceneLoader : MonoBehaviour,ISceneLoader
    {
        private IEventBus _eventBus;
        private CancellationTokenSource _cts;
        private string _currentLoadingScene;
        
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SubscribeEvents();
        }

        void OnDestroy()
        {
            Dispose(); 
        }
        private void SubscribeEvents()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _eventBus.Subscribe<LoadSceneRequest>(OnLoadRequested);
            _eventBus.Subscribe<UnloadSceneRequest>(OnUnloadRequested);
            _eventBus.Subscribe<ReloadActiveRequest>(OnReloadRequested);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<LoadSceneRequest>(OnLoadRequested);
            _eventBus.Unsubscribe<UnloadSceneRequest>(OnUnloadRequested);
            _eventBus.Unsubscribe<ReloadActiveRequest>(OnReloadRequested);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async void OnLoadRequested(LoadSceneRequest req)
        {
            try
            {
                if (req.CancelPrevious && _cts != null)
                {
                    _cts.Cancel();
                    _eventBus.Publish(new SceneLoadCanceled( _currentLoadingScene ));
                }

                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                _currentLoadingScene = req.Scene;

                await Load(req.Scene, req.Additive, req.ActivateOnLoad, _cts.Token);
            }
            catch (System.OperationCanceledException)
            {
                _eventBus.Publish(new SceneLoadCanceled(req.Scene));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneLoader] Error loading '{req.Scene}': {ex}");
            }
        }

        private async void OnUnloadRequested(UnloadSceneRequest req)
        {
            try
            {
                await Unload(req.Scene);
                _eventBus.Publish(new SceneUnloadCompleted(req.Scene ));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneLoader] Error unloading '{req.Scene}': {ex}");
            }
        }

        private async void OnReloadRequested(ReloadActiveRequest _)
        {
            var active = SceneManager.GetActiveScene().name;
            await Load(active, additive: false, activateOnLoad: true);
        }

        public async Task Load(string sceneName, bool additive = false, CancellationToken ct = default)
            => await Load(sceneName, additive, activateOnLoad: true, ct);

        public async Task Unload(string sceneName, CancellationToken ct = default)
        {
            var op = SceneManager.UnloadSceneAsync(sceneName);
            if (op == null) return;

            while (!op.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        public async Task ReloadActive(CancellationToken ct = default)
        {
            var active = SceneManager.GetActiveScene().name;
            await Load(active, additive: false, activateOnLoad: true, ct);
        }

        private async Task Load(string sceneName, bool additive, bool activateOnLoad, CancellationToken ct = default)
        {
            _eventBus?.Publish(new SceneLoadStarted { Scene = sceneName, Additive = additive });

            var mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (op == null) return;

            op.allowSceneActivation = activateOnLoad;

            while (!op.isDone)
            {
                ct.ThrowIfCancellationRequested();
                _eventBus?.Publish(new SceneLoadProgress { Scene = sceneName, Progress = op.progress });

                await Task.Yield();
            }

            _eventBus?.Publish(new SceneLoadCompleted { Scene = sceneName, Additive = additive });
        }
    }
}