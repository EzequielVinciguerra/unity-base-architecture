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
        [SerializeField] private string mainMenuScene = "MainMenu"; // nombre de la escena a cargar
        [SerializeField] private ScreenId mainMenuScreen = ScreenId.MainMenu;
        [SerializeField] private bool dontDestroyOnLoad = true;

        private IEventBus _bus;

        private void Awake()
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _bus = ServiceLocator.Instance.Get<IEventBus>();
            if (_bus == null)
            {
                Debug.LogError("[GameBootstrapper] IEventBus no encontrado. Asegúrate de instalar EventBus antes.");
                enabled = false;
                return;
            }

            _bus.Subscribe<SceneLoadCompleted>(OnSceneLoaded);
        }

        private void Start()
        {
            // Pedimos cargar la escena del Main Menu (Single)
            _bus.Publish(new LoadSceneRequest(
                scene: mainMenuScene,
                additive: false,
                activateOnLoad: true,
                cancelPrevious: true
            ));
        }

        private void OnDisable()
        {
            if (_bus != null)
                _bus.Unsubscribe<SceneLoadCompleted>(OnSceneLoaded);
        }

        private void OnSceneLoaded(SceneLoadCompleted evt)
        {
            if (!string.Equals(evt.Scene, mainMenuScene))
                return;

            _bus.Publish(new ShowView(ScreenId.MainMenu));
        }
    }
}
