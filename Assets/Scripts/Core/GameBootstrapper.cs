using Core.Services;
using Core.Events;              
using Core.Scenes;      
using Core.UI;               
using UnityEngine;

namespace Core.Boot
{
    [DefaultExecutionOrder(0)]
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Start Flow")]
        [SerializeField] private string mainMenuScene = "MainMenu"; 
        [SerializeField] private ScreenId mainMenuScreen = ScreenId.MainMenu;
        [SerializeField] private bool dontDestroyOnLoad = true;

        private IEventBus _eventBus;

        private void Awake()
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            if (_eventBus == null)
            {
                Debug.LogError("[GameBootstrapper] IEventBus not found.");
                enabled = false;
                return;
            }

            _eventBus.Subscribe<SceneLoadCompleted>(OnSceneLoaded);
        }

        private void Start()
        {
            _eventBus.Publish(new LoadSceneRequest(
                scene: mainMenuScene,
                additive: false,
                activateOnLoad: true,
                cancelPrevious: true
            ));
        }

        private void OnDisable()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe<SceneLoadCompleted>(OnSceneLoaded);
        }

        private void OnSceneLoaded(SceneLoadCompleted evt)
        {
            if (!string.Equals(evt.Scene, mainMenuScene))
                return;

            _eventBus.Publish(new ShowView(ScreenId.MainMenu));
        }
    }
}
